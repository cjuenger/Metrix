using Io.Juenger.Scrum.GitLab.Configs;
using Io.Juenger.Scrum.GitLab.Contracts.Values;
using Io.Juenger.Scrum.GitLab.Values;

namespace Io.Juenger.Scrum.GitLab.Factories.Application;

internal class WorkflowFactory : IWorkflowFactory
{
    private readonly IWorkflowConfig _workflowConfig;

    public WorkflowValue Workflow => CreateWorkflow();
    
    public WorkflowFactory(IWorkflowConfig workflowConfig)
    {
        _workflowConfig = workflowConfig ?? throw new ArgumentNullException(nameof(workflowConfig));
    }

    private WorkflowValue CreateWorkflow()
    {
        var workflowStates = _workflowConfig.Workflow
            .Select(workflowItem => new WorkflowStateValue(workflowItem))
            .ToList();

        var workflow = new WorkflowValue(workflowStates);
        return workflow;
    }
}