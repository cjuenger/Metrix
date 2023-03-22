using Io.Juenger.Scrum.GitLab.Contracts.Aggregates;

namespace Io.Juenger.Scrum.GitLab.Contracts.Repositories;

public interface IReleaseRepository
{
    Task<IReadOnlyList<IReleaseAggregate>> LoadReleasesAsync(
        string productId, 
        CancellationToken ct = default);

    Task<IReleaseAggregate> LoadReleaseByIdAsync(
        string productId, 
        int releaseId,
        CancellationToken ct = default);

    Task<IReleaseAggregate> LoadLatestReleaseAsync(
        string productId,
        CancellationToken ct = default);
}