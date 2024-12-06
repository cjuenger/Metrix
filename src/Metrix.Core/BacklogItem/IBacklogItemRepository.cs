namespace Metrix.Core.BacklogItem
{
    public interface IBacklogItemRepository
    {
        Task<IReadOnlyCollection<BacklogItem>> LoadProductItemsAsync(
            string projectId, 
            string? ofSprint = null,
            int? ofReleaseId = null,
            CancellationToken ct = default);
    }
}