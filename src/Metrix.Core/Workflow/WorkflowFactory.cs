namespace Metrix.Core.Workflow;

internal class WorkflowFactory : IWorkflowFactory
{
    private readonly IWorkflowConfig _workflowConfig;

    public Workflow Workflow => CreateWorkflow();
    
    public WorkflowFactory(IWorkflowConfig workflowConfig)
    {
        _workflowConfig = workflowConfig ?? throw new ArgumentNullException(nameof(workflowConfig));
    }

    private Workflow CreateWorkflow()
    {
        var workflowStates = _workflowConfig.Workflow
            .Select(workflowItem => new WorkflowState(workflowItem))
            .ToList();

        var workflow = new Workflow(workflowStates);
        return workflow;
    }
}