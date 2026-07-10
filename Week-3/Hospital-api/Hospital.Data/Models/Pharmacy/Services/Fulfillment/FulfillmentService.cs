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

    private readonly ConcurrentDictionary<string, int> _batchToInventoryIdMap = new();

    public FulfillmentService(IDbContextFactory<HospitalDbContext> factory, IBurstPlanner planner)
    {
        _factory = factory;
        _planner = planner;

        using var db = _factory.CreateDbContext();
        foreach (var item in db.Inventory.AsNoTracking())
        {
            if (!string.IsNullOrWhiteSpace(item.BatchNumber))
            {
                _batchToInventoryIdMap[item.BatchNumber] = item.InventoryID;
            }
        }
    }

    public int ResolveInventoryId(string batchNumber)
    {
        if (_batchToInventoryIdMap.TryGetValue(batchNumber, out int id))
        {
            return id;
        }
        throw new KeyNotFoundException($"Batch number {batchNumber} could not be found in cache.");
    }

    public async Task<FulfillmentResult> FulfillOneAsync(int recordId, int inventoryID, int quantity, CancellationToken ct)
    {
        await using var db = await _factory.CreateDbContextAsync(ct);

        try
        {

            var inv = await db.Inventory.FirstOrDefaultAsync(i => i.InventoryID == inventoryID, ct);


            if (inv == null)
            {
                throw new InventoryNotFoundException(inventoryID);
            }

            if (inv.StockQuantity < quantity)
            {
                throw new BackorderException(recordId, inventoryID, quantity);
            }

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

            var requestedMap = new Dictionary<int, int> { { inventoryID, quantity } };

            if (!await SaveWithRetryAsync(db, requestedMap, ct))
            {
                db.ChangeTracker.Clear();
                Log.Warning("Backordered transaction tracking entry {RecordId} after exhausting concurrency retry loops", recordId);
                return FulfillmentResult.Backordered;
            }

            Log.Information("Fulfilled prescription allocation entry: {RecordId}, allocated quantity volume: {Quantity}", recordId, quantity);
            return FulfillmentResult.Fulfilled;
        }

        catch (InventoryNotFoundException unfoundEx)
        {
            Log.Error(unfoundEx, "Fulfillment canceled completely. Targeting invalid Inventory Token {InvId}", unfoundEx.InventoryID);

            var errorLog = new PrescriptionDetail
            {
                RecordId = recordId,
                MedicationId = 0,
                DosageInstructions = $"CRITICAL ERROR: Targeted Inventory ID {unfoundEx.InventoryID} could not be resolved.",
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

            var backorderLog = new PrescriptionDetail
            {
                RecordId = boEx.RecordId,
                MedicationId = boEx.InventoryID,
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
            throw; 
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "An unhandled critical database framework error occurred inside the system pipeline.");
            throw;
        }
    }

    private static async Task<bool> SaveWithRetryAsync(HospitalDbContext db, IReadOnlyDictionary<int, int> requestedByInventoryID, CancellationToken ct)
    {
        int attempts = 0;
        const int maxAttempts = 3; 

        while (attempts < maxAttempts)
        {
            try
            {
                await db.SaveChangesAsync(ct);
                return true; 
            }
            catch (DbUpdateConcurrencyException ex) 
            {
                attempts++;
                Log.Warning("Concurrency race collision caught. Initiating lookup reload attempt {Attempt}/{Max}", attempts, maxAttempts);

                foreach (var entry in ex.Entries)
                {
                    var currentDatabaseValues = await entry.GetDatabaseValuesAsync(ct);
                    if (currentDatabaseValues == null) return false;

                    entry.OriginalValues.SetValues(currentDatabaseValues);

                    if (entry.Entity is InventoryItem inv)
                    {
                        int freshStockValue = currentDatabaseValues.GetValue<int>(nameof(InventoryItem.StockQuantity));
                        int desiredAmount = requestedByInventoryID[inv.InventoryID];

                        if (freshStockValue < desiredAmount)
                        {
                            Log.Warning("Concurrency re-check failed. Fresh database stock level ({FreshValue}) is lower than desired quantity ({Desired})", freshStockValue, desiredAmount);
                            return false;
                        }

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

        var prioritizedOrders = _planner.OrderByPriority(orders);

        var executionTuples = prioritizedOrders.Select(o => (o.AppointmentID, o.InventoryID, o.QuantityRequested));

        await Parallel.ForEachAsync(executionTuples, new ParallelOptions
        {
            MaxDegreeOfParallelism = Environment.ProcessorCount,
            CancellationToken = ct
        }, async (order, token) =>
        {
            var result = await FulfillOneAsync(order.AppointmentID, order.InventoryID, order.QuantityRequested, token);

            if (result == FulfillmentResult.Fulfilled)
                Interlocked.Increment(ref fulfilled);
            else
                Interlocked.Increment(ref backordered);
        });

        return new BurstResult(fulfilled, backordered);
    }
}