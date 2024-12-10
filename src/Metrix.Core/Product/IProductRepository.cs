namespace Metrix.Core.Product;

public interface IProductRepository
{
    public Task<IEnumerable<Product>> LoadProductsAsync(CancellationToken cancellationToken = default);

    Task<Product> LoadProductAsync(
        string productId,
        CancellationToken cancellationToken = default);
}