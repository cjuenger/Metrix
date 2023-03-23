using Io.Juenger.Scrum.GitLab.Contracts.Entities;

namespace Io.Juenger.Scrum.GitLab.Contracts.Repositories
{
    public interface IItemsRepository
    {
        Task<IReadOnlyCollection<ItemEntity>> LoadProductItemsAsync(
            string projectId, 
            string? ofSprint = null,
            int? ofReleaseId = null,
            CancellationToken ct = default);
    }
}