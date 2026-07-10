using HospitalApi.DTOs.Pharmacy;

namespace HospitalApi.Services.Pharmacy;

public interface IBurstPlanner
{
    IReadOnlyList<BurstRequestPayload> OrderByPriority(IEnumerable<BurstRequestPayload> payloads);
}

public class BurstPlanner : IBurstPlanner
{
    public IReadOnlyList<BurstRequestPayload> OrderByPriority(IEnumerable<BurstRequestPayload> payloads)
    {

        var pq = new PriorityQueue<BurstRequestPayload, int>();

        foreach (var payload in payloads)
        {

            int priorityValue = (payload.QuantityRequested > 50) ? 0 : 1;

            pq.Enqueue(payload, priorityValue);
        }

        var orderedByPriority = new List<BurstRequestPayload>();

        while (pq.TryDequeue(out var payload, out _))
        {
            orderedByPriority.Add(payload);
        }

        return orderedByPriority; 
    }
}