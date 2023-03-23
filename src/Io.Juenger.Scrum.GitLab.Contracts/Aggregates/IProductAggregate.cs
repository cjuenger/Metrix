namespace Io.Juenger.Scrum.GitLab.Contracts.Aggregates;

public interface IProductAggregate
{
    public string Id { get; }
    public string Name { get; set; }
}