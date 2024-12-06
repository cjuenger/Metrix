using Io.Juenger.GitLabClient.Model;
using Metrix.Core.Entities;

namespace Io.Juenger.Scrum.GitLab.Services.Domain
{
    internal interface IItemParserService
    {
        ItemEntity Parse(Issue issue);
    }
}