using Io.Juenger.Scrum.GitLab.Contracts.Aggregates;

namespace Io.Juenger.Scrum.GitLab.Aggregates;

internal class ProductAggregate : IProductAggregate
{
    public ProductAggregate(string productId, string productName)
    {
        if (string.IsNullOrWhiteSpace(productId))
        {
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(productId));
        }

        if (string.IsNullOrWhiteSpace(productName))
        {
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(productName));
        }


        Id = productId;
        Name = productName;
    }

    public string Id { get; }
    
    public string Name { get; set; }
}