using Io.Juenger.Scrum.GitLab.Contracts.Aggregates;

namespace Io.Juenger.Scrum.GitLab.Aggregates;

internal class ProductAggregate : IProductAggregate
{
    public ProductAggregate(string productId)
    {
        if (string.IsNullOrWhiteSpace(productId))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(productId));
        ProductId = productId;
    }

    public string ProductId { get; }
}