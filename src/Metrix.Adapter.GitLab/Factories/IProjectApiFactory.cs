using Io.Juenger.GitLabClient.Api;

namespace Io.Juenger.Scrum.GitLab.Factories
{
    internal interface IProjectApiFactory
    {
        IProjectApi ProjectApi { get; }
    }
}