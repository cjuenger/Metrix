using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Io.Juenger.Scrum.GitLab.Contracts.Aggregates;

namespace Io.Juenger.Scrum.GitLab.Repositories;

internal interface ISprintRepository
{
    Task<ISprintAggregate> LoadSprintByIdAsync(
        string projectId, 
        int sprintId,
        CancellationToken ct = default);

    Task<IReadOnlyList<ISprintAggregate>> LoadSprintsAsync(
        string projectId, 
        CancellationToken ct = default);

    Task<ISprintAggregate> LoadLatestSprintAsync(
        string projectId, 
        CancellationToken ct = default);
}