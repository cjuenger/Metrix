using Metrix.Core.Metrics;
using Metrix.Core.Metrics.Values;
using Metrix.Core.Values;

namespace Metrix.Core.Sprint;

public interface ISprintMetricsService
{
    Task<Composition> CalculateCompositionAsync(string productId, int sprintId, CancellationToken cancellationToken = default);
    Task<BurnDown> CalculateBurnDownAsync(string productId, int sprintId, CancellationToken cancellationToken = default);
    Task<BurnUp> CalculateBurnUpAsync(string productId, int sprintId, CancellationToken cancellationToken = default);
    Task<CycleTimesValue> CalculateCycleTimesAsync(string productId, int sprintId, CancellationToken cancellationToken = default);

    Task<Status> CalculateSprintStatusAsync(
        string productId, 
        int sprintId,
        CancellationToken cancellationToken = default);
}