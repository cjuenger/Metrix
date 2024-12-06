namespace Metrix.Core.Sprint;

public interface ISprintRepository
{
    Task<Sprint> LoadSprintByIdAsync(
        string productId, 
        int sprintId,
        CancellationToken ct = default);

    Task<IReadOnlyList<Sprint>> LoadSprintsAsync(
        string productId, 
        CancellationToken ct = default);

    Task<Sprint> LoadLatestSprintAsync(
        string productId, 
        CancellationToken ct = default);
}