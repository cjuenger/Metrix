using Io.Juenger.Scrum.GitLab.Contracts.Aggregates;

namespace Io.Juenger.Scrum.GitLab.Factories.Domain;

internal interface IProductFactory
{
    IProductAggregate Create(string productId);
}