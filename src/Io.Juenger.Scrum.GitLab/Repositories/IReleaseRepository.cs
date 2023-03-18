using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Io.Juenger.Scrum.GitLab.Contracts.Aggregates;

namespace Io.Juenger.Scrum.GitLab.Repositories;

internal interface IReleaseRepository
{
    Task<IReadOnlyList<IReleaseAggregate>> LoadReleasesAsync(
        string projectId, 
        CancellationToken ct = default);

    Task<IReleaseAggregate> LoadReleaseByIdAsync(
        string projectId, 
        int releaseId,
        CancellationToken ct = default);

    Task<IReleaseAggregate> LoadLatestReleaseAsync(
        string projectId,
        CancellationToken ct = default);
}