namespace Io.Juenger.Scrum.GitLab.Contracts.Aggregates;

public interface IProductAggregate
{
    public string Id { get; }
    public string Name { get; }
    public string Vision { get; }
    public DateTime Kickoff { get; }
    public DateTime DueDate { get; }
}