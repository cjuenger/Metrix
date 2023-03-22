using Io.Juenger.Scrum.GitLab.Contracts.Values;

namespace Io.Juenger.Scrum.GitLab.Contracts.Services;

public interface IReleaseAggregateService
{
    Task<CompositionValue> CalculateCompositionAsync(
        string productId, 
        int releaseId,
        CancellationToken cancellationToken = default);

    Task<BurnDownValue> CalculateBurnDown(
        string productId, 
        int releaseId,
        CancellationToken cancellationToken = default);

    Task<BurnUpValue> CalculateBurnUp(
        string productId, 
        int releaseId,
        CancellationToken cancellationToken = default);

    Task<CycleTimesValue> CalculateCycleTime(
        string productId, 
        int releaseId,
        CancellationToken cancellationToken = default);

    Task<ReleaseStatusValue> CalculateReleaseStatusAsync(
        string productId, 
        int releaseId,
        CancellationToken cancellationToken = default);
}