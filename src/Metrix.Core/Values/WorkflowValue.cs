namespace Metrix.Core.Values;

public class WorkflowValue
{
    public IEnumerable<WorkflowStateValue> WorkflowStates { get; }

    public WorkflowValue(IEnumerable<WorkflowStateValue> workflowStateValues)
    {
        WorkflowStates = workflowStateValues ?? throw new ArgumentNullException(nameof(workflowStateValues));
    }
}