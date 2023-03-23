using Io.Juenger.Scrum.GitLab.Contracts.Aggregates;

namespace Io.Juenger.Scrum.GitLab.Contracts.Repositories;

public interface IProductRepository
{
    public Task<IEnumerable<IProductAggregate>> LoadProductsAsync(CancellationToken cancellationToken = default);

    Task<IProductAggregate> LoadProductAsync(
        string productId,
        CancellationToken cancellationToken = default);
}