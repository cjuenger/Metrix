using Io.Juenger.GitLabClient.Model;
using Metrix.Core.BacklogItem;

namespace Io.Juenger.Scrum.GitLab.Services.Domain
{
    internal interface IItemParserService
    {
        BacklogItem Parse(Issue issue);
    }
}