using Metrix.Core.Entities;

namespace Metrix.Core.Repositories
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