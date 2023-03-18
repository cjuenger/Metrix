using Io.Juenger.Scrum.GitLab.Contracts.Entities;
using Io.Juenger.Scrum.GitLab.Contracts.Values;

namespace Io.Juenger.Scrum.GitLab.Contracts.Aggregates;

public interface IProductAggregate
{
    public string ProductId { get; }
}