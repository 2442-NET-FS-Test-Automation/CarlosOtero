using HospitalApi.Data;
using HospitalApi.DTOs.Pharmacy;
using HospitalApi.Exceptions; 
using System.Collections.Concurrent;
using HospitalApi.Models.Pharmacy;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace HospitalApi.Services.Pharmacy;

public class FulfillmentService : IFulfillmentService
{
        private readonly IDbContextFactory<HospitalDbContext> _factory;
    private readonly IBurstPlanner _planner;
    
    // Target Requirement: Thread-safe high-speed ConcurrentDictionary lookup cache
    private readonly ConcurrentDictionary<string, int> _batchToInventoryIdMap = new();

    public FulfillmentService(IDbContextFactory<HospitalDbContext> factory, IBurstPlanner planner)
    {
        _factory = factory;
        _planner = planner;

        // Populate the lookup cache immediately upon application startup
        using var db = _factory.CreateDbContext();
        foreach (var item in db.Inventory.AsNoTracking())
        {
            _batchToInventoryIdMap[item.BatchNumber] = item.InventoryID;
        }
    }

    // High-speed O(1) memory lookup method
    public int ResolveInventoryId(string batchNumber)
    {
        if (_batchToInventoryIdMap.TryGetValue(batchNumber, out int id))
        {
            return id;
        }
        throw new KeyNotFoundException($"Batch number {batchNumber} could not be found in cache.");
    }

    public async Task<FulfillmentResult> FulfillOneAsync(int recordId, int inventoryId, int quantity, CancellationToken ct)
    {
        // Milestone M3 Requirement: Open a clean, fully isolated DbContext instance per task thread
        await using var db = await _factory.CreateDbContextAsync(ct);

        try
        {
            // Fetch target tracking inventory row containing the RowVersion Concurrency Token
            // Milestone M5 Requirement: Cancellation tokens are passed down into every single async DB operation
            var inv = await db.Inventory.FirstOrDefaultAsync(i => i.InventoryID == inventoryId, ct);

            // ========================================================
            // MILESTONE M5: CUSTOM DOMAIN EXCEPTION TRIGGERS
            // ========================================================
            if (inv == null)
            {
                throw new InventoryNotFoundException(inventoryId);
            }

            if (inv.StockQuantity < quantity)
            {
                throw new BackorderException(recordId, inventoryId, quantity);
            }

            // Apply deduction atomically inside context tracking states
            inv.StockQuantity -= quantity;

            var fulfillmentLog = new PrescriptionDetail
            {
                RecordId = recordId,
                MedicationId = inv.MedicationID,
                DosageInstructions = "Fulfilled successfully via asynchronous core workflow processor.",
                DurationDays = 1,
                QuantityDispensed = quantity
            };
            await db.PrescriptionDetails.AddAsync(fulfillmentLog, ct);

            // Milestone M3 Core: Call the recursive save loop passing the requested dictionary map bounds
            var requestedMap = new Dictionary<int, int> { { inventoryId, quantity } };
            
            if (!await SaveWithRetryAsync(db, requestedMap, ct))
            {
                db.ChangeTracker.Clear();
                Log.Warning("Backordered transaction tracking entry {RecordId} after exhausting concurrency retry loops", recordId);
                return FulfillmentResult.Backordered;
            }

            Log.Information("Fulfilled prescription allocation entry: {RecordId}, allocated quantity volume: {Quantity}", recordId, quantity);
            return FulfillmentResult.Fulfilled;
        }
        // ========================================================
        // MILESTONE M5 HIERARCHY: CATCH SPECIFIC TYPED EXCEPTIONS FIRST
        // ========================================================
        catch (InventoryNotFoundException unfoundEx)
        {
            Log.Error(unfoundEx, "Fulfillment canceled completely. Targeting invalid Inventory Token {InvId}", unfoundEx.InventoryId);
            
            // Audit-log the failure to track missing lookup attempts gracefully
            var errorLog = new PrescriptionDetail
            {
                RecordId = recordId,
                MedicationId = 0,
                DosageInstructions = $"CRITICAL ERROR: Targeted Inventory ID {unfoundEx.InventoryId} could not be resolved.",
                DurationDays = 0,
                QuantityDispensed = 0
            };
            await db.PrescriptionDetails.AddAsync(errorLog, ct);
            await db.SaveChangesAsync(ct);

            return FulfillmentResult.Backordered;
        }
        catch (BackorderException boEx)
        {
            Log.Warning("[Serilog] Trimming execution path. Appointment {AppId} marked BACKORDERED due to missing capacity bounds.", boEx.RecordId);
            
            // Milestone M2/M5 Audit Logging: Record backorders instantly into PrescriptionDetails track logs
            var backorderLog = new PrescriptionDetail
            {
                RecordId = boEx.RecordId,
                MedicationId = boEx.InventoryId, // Associated inventory row metadata tracking
                DosageInstructions = $"BACKORDERED: Requested {boEx.RequestedQuantity} units but inventory was insufficient.",
                DurationDays = 0,
                QuantityDispensed = 0
            };
            await db.PrescriptionDetails.AddAsync(backorderLog, ct);
            await db.SaveChangesAsync(ct);

            return FulfillmentResult.Backordered;
        }
        catch (OperationCanceledException)
        {
            Log.Warning("Fulfillment operation for Record {RecordId} was canceled by a graceful shutdown or client request token timeout.", recordId);
            throw; // Re-throw to allow worker pipelines to stop gracefully without masking state
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "An unhandled critical database framework error occurred inside the system pipeline.");
            throw;
        }
    }

    private static async Task<bool> SaveWithRetryAsync(HospitalDbContext db, IReadOnlyDictionary<int, int> requestedByInventoryId, CancellationToken ct)
    {
        int attempts = 0;
        const int maxAttempts = 3; // Milestone M3: Bounded retries restriction guard

        while (attempts < maxAttempts)
        {
            try
            {
                await db.SaveChangesAsync(ct);
                return true; // Save succeeded, race condition won successfully!
            }
            catch (DbUpdateConcurrencyException ex) // Milestone M3: Catch specific concurrency collisions first
            {
                attempts++;
                Log.Warning("Concurrency race collision caught. Initiating lookup reload attempt {Attempt}/{Max}", attempts, maxAttempts);

                foreach (var entry in ex.Entries)
                {
                    // Reload the fresh database state directly from SQL Server disk
                    var currentDatabaseValues = await entry.GetDatabaseValuesAsync(ct);
                    if (currentDatabaseValues == null) return false; 

                    // Update the entry's original values bucket so EF knows we are checking fresh baselines
                    entry.OriginalValues.SetValues(currentDatabaseValues);

                    if (entry.Entity is InventoryItem inv)
                    {
                        int freshStockValue = currentDatabaseValues.GetValue<int>(nameof(InventoryItem.StockQuantity));
                        int desiredAmount = requestedByInventoryId[inv.InventoryID];

                        // Milestone M3 Re-check Validation: Fail fast if background threads dropped volumes too low
                        if (freshStockValue < desiredAmount)
                        {
                            Log.Warning("Concurrency re-check failed. Fresh database stock level ({FreshValue}) is lower than desired quantity ({Desired})", freshStockValue, desiredAmount);
                            return false; 
                        }

                        // Re-apply deduction over the newly reloaded baseline stock volume state values
                        inv.StockQuantity = freshStockValue - desiredAmount;
                    }
                }
            }
        }
        return false; 
    }

    public async Task<BurstResult> FulfillBurstAsync(IEnumerable<BurstRequestPayload> orders, CancellationToken ct)
    {
        int fulfilled = 0;
        int backordered = 0;

        // 1. Milestone M4 Priority Optimization: Sort the incoming batch using your PriorityQueue engine
        var prioritizedOrders = _planner.OrderByPriority(orders);

        // Convert payloads seamlessly into tuples for downstream loop executions
        var executionTuples = prioritizedOrders.Select(o => (o.AppointmentId, o.InventoryId, o.QuantityRequested));

        // 2. Parallel Target execution tracking over your multi-threaded core processing lanes
        await Parallel.ForEachAsync(executionTuples, new ParallelOptions 
        { 
            MaxDegreeOfParallelism = Environment.ProcessorCount, 
            CancellationToken = ct 
        }, async (order, token) =>
        {
            // Pass the cancellation token through to uphold safe cascading cancellation checks
            var result = await FulfillOneAsync(order.AppointmentId, order.InventoryId, order.QuantityRequested, token);
            
            if (result == FulfillmentResult.Fulfilled)
                Interlocked.Increment(ref fulfilled);
            else
                Interlocked.Increment(ref backordered);
        });

        return new BurstResult(fulfilled, backordered);
    }
}