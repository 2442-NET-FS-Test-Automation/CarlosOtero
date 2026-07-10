using HospitalApi.DTOs.Pharmacy;

namespace HospitalApi.Services.Pharmacy;

public interface IBurstPlanner
{
    IReadOnlyList<BurstRequestPayload> OrderByPriority(IEnumerable<BurstRequestPayload> payloads);
}

public class BurstPlanner : IBurstPlanner
{
    /// <summary>
    /// Milestone M4 Core: Sorts an incoming batch array using a PriorityQueue.
    /// Lower numbers represent higher operational priority execution values (Expedited = 0, Normal = 1).
    /// </summary>
    public IReadOnlyList<BurstRequestPayload> OrderByPriority(IEnumerable<BurstRequestPayload> payloads)
    {
        // First type parameter carries the payload card object; second integer parameter tracks priority sorting weight
        var pq = new PriorityQueue<BurstRequestPayload, int>();

        foreach (var payload in payloads)
        {
            // Business Logic Mapping Check:
            // If the transaction requests a high volume of medication (e.g., > 50 units), 
            // classify it as an automated 'Expedited' emergency batch (Priority 0). Otherwise, set it to Routine (Priority 1).
            int priorityValue = (payload.QuantityRequested > 50) ? 0 : 1;

            pq.Enqueue(payload, priorityValue);
        }

        // Initialize the output collection structure that holds the finalized, prioritized order stream
        var orderedByPriority = new List<BurstRequestPayload>();

        // Dequeue items continuously out of the sorting engine using out parameters until empty
        while (pq.TryDequeue(out var payload, out _))
        {
            orderedByPriority.Add(payload);
        }

        return orderedByPriority; // Expedited payload objects are compiled at the front of the list
    }
}