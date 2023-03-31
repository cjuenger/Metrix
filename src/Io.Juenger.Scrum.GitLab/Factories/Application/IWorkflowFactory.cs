using Io.Juenger.Scrum.GitLab.Contracts.Values;
using Io.Juenger.Scrum.GitLab.Values;

namespace Io.Juenger.Scrum.GitLab.Factories.Application;

public interface IWorkflowFactory
{
    WorkflowValue Workflow { get; }
}