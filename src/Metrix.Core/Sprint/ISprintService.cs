using Metrix.Core.Values;

namespace Metrix.Core.Services;

public interface ISprintService
{
    Task<CompositionValue> CalculateCompositionAsync(string productId, int sprintId, CancellationToken cancellationToken = default);
    Task<BurnDownValue> CalculateBurnDownAsync(string productId, int sprintId, CancellationToken cancellationToken = default);
    Task<BurnUpValue> CalculateBurnUpAsync(string productId, int sprintId, CancellationToken cancellationToken = default);
    Task<CycleTimesValue> CalculateCycleTimesAsync(string productId, int sprintId, CancellationToken cancellationToken = default);

    Task<SprintStatusValue> CalculateSprintStatusAsync(
        string productId, 
        int sprintId,
        CancellationToken cancellationToken = default);
}