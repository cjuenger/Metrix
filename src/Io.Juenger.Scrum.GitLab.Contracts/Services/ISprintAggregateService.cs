using Io.Juenger.Scrum.GitLab.Contracts.Values;

namespace Io.Juenger.Scrum.GitLab.Contracts.Services;

public interface ISprintAggregateService
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