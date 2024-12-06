namespace Metrix.Core.Product;

public interface IProductRepository
{
    public Task<IEnumerable<Core.Product.Product>> LoadProductsAsync(CancellationToken cancellationToken = default);

    Task<Core.Product.Product> LoadProductAsync(
        string productId,
        CancellationToken cancellationToken = default);
}