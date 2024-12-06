using Metrix.Core.Aggregates;

namespace Metrix.Core.Factories;

internal class ProductFactory : IProductFactory
{
    private readonly Func<string, Product> _productAggregateFactory;

    public ProductFactory(Func<string, Product> productAggregateFactory)
    {
        _productAggregateFactory = productAggregateFactory ?? throw new ArgumentNullException(nameof(productAggregateFactory));
    }
    
    public Product Create(string productId)
    {
        return _productAggregateFactory.Invoke(productId);
    }
}