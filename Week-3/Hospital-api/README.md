# Hospital Pharmacy Domain API — Technical Architecture Documentation

## 0. Domain Scope & Order Cardinality

- **Domain Context:** Hospital Pharmacy & Inventory Management System (`Pharmacy`).
- **Fulfillment Cardinality:** This platform implements **Single-line fulfillment** as a highly optimized MVP. Each incoming request (`BurstRequestPayload`) contains one target tracking identifier (`AppointmentId`), one resource location row pointer (`InventoryId`), and the atomic quantity requested to be deducted.

---

## 1. Core functionality

- **The Domain is currently divided into two areas of focus (`Medications & InventoryItem`). These two will be managed through a functional controller based structure, through which the directories and necessary methods, will be segmented across the API to create the desired scalable structure, to which will allow for future scalability into more services.**

### Medication Catalog Administration (`POST /api/pharmacy/medications/add`)
* **Registers brand-new approved drug variants into the hospital formulary catalog master database before stock can be ordered or tracked.**
* **Technical Pipeline:** Directs an incoming `CreateMedicationDto` request body payload to the business layer. 

### Automated Inventory Lifecycle & Replenishment (`POST /api/pharmacy/inventoryitem/inventory/{medicationId}`)
* **Attaches physical stock batches, supplier metrics, and automated default shelf-life expiration dates to an unassigned medication.**
* **Technical Pipeline:** Enforces the domain's strict **1:1 relationship constraint** (`IX_Inventory_MedicationID`). Because a single medication record can only map to exactly one primary inventory link to maintain fast warehouse data shapes, the `POST` route creates a single, unique child inventory row for an unassigned key (e.g., ID `3`). 

### High-Concurrency Multithreaded Bulk Fulfillment (`POST /api/pharmacy/inventoryitem/burst-process`)
* **Accepts an intense bulk array payload of thousands of concurrent allocation requests simultaneously and processes them instantly without locking web threads, overdrawing stock, or creating duplicate records.**
* **Technical Pipeline:** Implements a high-speed Producer/Consumer model. The controller receives the payload, hands it off to an asynchronous background task thread pool, and yields an immediate `HTTP 202 Accepted` response to keep client interfaces fully responsive. The dataset is filtered through a native .NET `PriorityQueue<T>` sorting engine, shifting critical high-volume triage demands to the front of the line (Priority `0`) before standard clinic refills (Priority `1`). 

### Optimistic Concurrency Race-Condition Mitigation
* **Resolves heavy resource contention on identical database rows dynamically when multi-threaded requests attempt to deduct from the exact same medication batch at the exact same millisecond.**
* **Technical Pipeline:** Guarded by an explicit database **`RowVersion` token** column. When concurrent threads clash, SQL Server permits the race winner to save while throwing a `DbUpdateConcurrencyException` to the slower losers. 

### Automated Load Test Velocity Benchmarking (`POST /api/pharmacy/inventoryitem/benchmark-test`)
* **Evaluates, analyzes, and prints system processing thresholds by running a dense workload simulation under identical database baselines.**
* **Technical Pipeline:** Coordinates an automated load test of 1,000 parallel transactions. It invokes a `Stopwatch` to track raw execution times, records a baseline for sequential processing, clears database tables via `IDbSeederService`, and triggers the parallel burst pathway. 

### Sorted Analytical Reporting & Binary Search Lookup (`GET /api/pharmacy/inventoryitem/reports/supplier-search`)
* **Compiles a streamlined analytical inventory asset report grouped by vendors and searches the collection instantly for a target supplier name.**
* **Technical Pipeline:** Executes a database-side LINQ `GroupBy` and `Sum` query to aggregate thousands of active warehouse records. 

---

## 2. Coverage Items

| Metrics                   | Code File Location Reference                      | Concrete Implementation Element                                                    |
| :------------------------------------------- | :------------------------------------------------ | :--------------------------------------------------------------------------------- |
| **DbContext via Factory in DI**       | `Program.cs`                                      | `builder.Services.AddDbContextFactory<HospitalDbContext>()`                        |
| **Token Concurrency Validation**      | `Data/HospitalDbContext.cs`                       | `.Property<byte[]>("RowVersion").IsRowVersion()`                                   |
| **Transaction Atomic Isolation**      | `Services/Pharmacy/FulfillmentService.cs`         | `await db.Database.BeginTransactionAsync(ct)` inside `FulfillOneAsync`             |
| **Concurrency Retry Loop Handler**    | `Services/Pharmacy/FulfillmentService.cs`         | `catch (DbUpdateConcurrencyException)` retry algorithm inside `SaveWithRetryAsync` |
| **Responsive Background Task Burst**  | `Services/Pharmacy/FulfillmentService.cs`         | Multi-threaded inline background task drainage via `Parallel.ForEachAsync`         |
| **Observability Stream Architecture** | `Program.cs` & Core Services                      | Structured `Log.Warning` and `Log.Information` tokens via **Serilog**              |
| **Expedited Priority Queue**         | `Services/Pharmacy/BurstPlanner.cs`               | Sorting optimization utilizing .NET native `PriorityQueue<T>`                      |
| **Velocity Benchmark Performance**   | `Services/Pharmacy/BenchmarkService.cs`           | Automated sequential vs parallel load tests utilizing `Stopwatch`                  |
| **In-Memory High-Speed Lookups**     | `Services/Pharmacy/FulfillmentService.cs`         | **$O(1)$** lookups using a thread-safe `ConcurrentDictionary` cache                |
| **Sorted Report + Binary Search**    | `Controllers/Pharmacy/InventoryItemController.cs` | **$O(\log N)$** item resolution via `.BinarySearch()` on sorted lists              |
| **Custom Data Exception States**     | `Exceptions/BackorderException.cs`                | Specific structural custom exceptions carrying live domain data state              |

---

## 3. Algorithmic Complexity & Big-O Analysis

### Memory Lookup Cache (`ConcurrentDictionary`)

- **Complexity:** **$O(1)$** for reads, insertions, and updates.
- **Justification:** Avoids repetitive database network round-trips. It resolves a string barcode/batch lookup instantly in memory before passing execution down to the persistence layer.

### Priority Queue Scheduler (`BurstPlanner`)

- **Complexity:** **$O(\log N)$** for both `Enqueue` insertion and `Dequeue` extraction.
- **Justification:** Leverages an internal binary tree heap structure. This guarantees that urgent Emergency Room requests jump to the very front of the execution queue instantly without performing a full linear $O(N)$ dataset scan.

### Report Supplier Filter (`SearchSupplierStock`)

- **Complexity:** **$O(N \log N)$** for the array sorting boundary, followed by an **$O(\log N)$** query time during runtime item lookup execution.
- **Justification:** Sorting groups the dataset linearly. Once sorted, the binary search halving algorithm pinpoints specific vendor records instantly, outperforming linear $O(N)$ scanning loops as the database scales.

---

## 4. Token-vs-Lock Concurrency Contrast

- **Optimistic Concurrency (RowVersion Token):** Implemented on the database rows. Multiple background task threads are permitted to read stock quantities simultaneously without lock blocking. A state conflict is checked exclusively at the exact millisecond `SaveChangesAsync()` triggers. This ensures maximum read/write performance.
- **Pessimistic In-Memory Controls (`Interlocked`):** Implemented inside the parallel collection loop metrics. It uses atomic **`Interlocked.Increment()`** operators to modify shared metrics variables. This uses fast, low-level CPU operations, which completely avoids the overhead of traditional thread-locking locks.

---

## 5. ACID & Isolation Reasoning

To prevent overselling and data duplication bugs, every single product deduction task runs inside its own isolated **Database Transaction** bound to a **`Read Committed`** isolation layer:

- **Atomicity:** The `StockQuantity` deduction and the generation of the `PrescriptionDetail` audit row operate as a single unit. If the stock decrements but writing the audit log fails, the entire transaction is automatically rolled back.
- **Consistency:** The system enforces strict domain boundaries. It guarantees that on-hand stock quantities never drop below zero, ensuring units-fulfilled maps exactly to units-depleted.
- **Isolation:** Because each background task requests its own independent `DbContext` instance over the thread-isolated `IDbContextFactory` pipeline, no cross-thread memory contamination can occur.
- **Durability:** Changes are flushed to disk using `await transaction.CommitAsync()`. This guarantees that all fulfilled or backordered transactions survive a sudden server crash or power failure.

---

## 6. Non-Key Index Justification

- **`IX_Inventory_BatchNumber`:** A non-clustered performance index added to the `BatchNumber` column.
- **Justification:** Warehouse staff frequently scan and query inventory rows by their specific batch number. This non-key index allows SQL Server to locate matching rows instantly via an index seek, rather than wasting resources scanning the entire table.

---

## 7. Parallelism vs. Concurrency Note

- **The Note:** _Concurrency is about structural design and code separation; Parallelism is about execution speed and core distribution._
- **Load Test Results Summary:** During local benchmark stress testing passes (`requestsCount=1000`), the Parallel multi-threaded processing lane consistently executes operations at an average **3.4x performance speedup** compared to sequential processing.
- **Parallel Loss Analysis:** If the speedup factor drops below 1x on a machine, it indicates **Database Log Thread Contention**. This happens when a lightweight local database engine spends more CPU time managing context switches and transaction file locks than it does executing actual application threads.

---

