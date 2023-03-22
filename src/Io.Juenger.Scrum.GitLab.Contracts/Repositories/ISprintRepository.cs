using Io.Juenger.Scrum.GitLab.Contracts.Aggregates;

namespace Io.Juenger.Scrum.GitLab.Contracts.Repositories;

public interface ISprintRepository
{
    Task<ISprintAggregate> LoadSprintByIdAsync(
        string productId, 
        int sprintId,
        CancellationToken ct = default);

    Task<IReadOnlyList<ISprintAggregate>> LoadSprintsAsync(
        string productId, 
        CancellationToken ct = default);

    Task<ISprintAggregate> LoadLatestSprintAsync(
        string productId, 
        CancellationToken ct = default);
}