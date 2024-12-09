using NMolecules.DDD;

namespace Metrix.Core.Workflow;

[ValueObject]
public class Workflow
{
    public IEnumerable<WorkflowState> WorkflowStates { get; }

    public Workflow(IEnumerable<WorkflowState> workflowStateValues)
    {
        WorkflowStates = workflowStateValues ?? throw new ArgumentNullException(nameof(workflowStateValues));
    }
}