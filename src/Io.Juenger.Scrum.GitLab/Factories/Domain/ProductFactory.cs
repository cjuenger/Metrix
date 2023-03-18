using Io.Juenger.Scrum.GitLab.Contracts.Aggregates;

namespace Io.Juenger.Scrum.GitLab.Factories.Domain;

internal class ProductFactory : IProductFactory
{
    private readonly Func<string, IProductAggregate> _productAggregateFactory;

    public ProductFactory(Func<string, IProductAggregate> productAggregateFactory)
    {
        _productAggregateFactory = productAggregateFactory ?? throw new ArgumentNullException(nameof(productAggregateFactory));
    }
    
    public IProductAggregate Create(string productId)
    {
        return _productAggregateFactory.Invoke(productId);
    }
}