using Io.Juenger.GitLabClient.Api;

namespace Io.Juenger.Scrum.GitLab.Factories.Infrastructure
{
    internal interface IProjectApiFactory
    {
        IProjectApi ProjectApi { get; }
    }
}