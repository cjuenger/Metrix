using Io.Juenger.GitLabClient.Model;
using Io.Juenger.Scrum.GitLab.Contracts.Entities;

namespace Io.Juenger.Scrum.GitLab.Services.Domain
{
    internal interface IItemParserService
    {
        ItemEntity Parse(Issue issue);
    }
}