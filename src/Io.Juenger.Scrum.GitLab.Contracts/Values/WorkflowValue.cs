namespace Io.Juenger.Scrum.GitLab.Contracts.Values;

public class WorkflowValue
{
    public IEnumerable<WorkflowStateValue> WorkflowStates { get; }

    public WorkflowValue(IEnumerable<WorkflowStateValue> workflowStateValues)
    {
        WorkflowStates = workflowStateValues ?? throw new ArgumentNullException(nameof(workflowStateValues));
    }
}