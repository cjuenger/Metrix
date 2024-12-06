using Metrix.Core.Values;

namespace Io.Juenger.Scrum.GitLab.Factories.Application;

public interface IWorkflowFactory
{
    WorkflowValue Workflow { get; }
}