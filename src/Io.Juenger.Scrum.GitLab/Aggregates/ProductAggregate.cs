using Io.Juenger.Scrum.GitLab.Contracts.Aggregates;

namespace Io.Juenger.Scrum.GitLab.Aggregates;

internal class ProductAggregate : IProductAggregate
{
    public string Id { get; }
    public string Name { get; }
    public string Vision { get; }
    public DateTime Kickoff { get; }
    public DateTime DueDate { get; }

    public ProductAggregate(
        string productId, 
        string productName,
        string productVision,
        DateTime kickoff,
        DateTime dueDate)
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
        Vision = productVision ?? throw new ArgumentNullException(nameof(productVision));
        Kickoff = kickoff;
        DueDate = dueDate;
    }
}