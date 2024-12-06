using NMolecules.DDD;

namespace Metrix.Core.Workflow;

[ValueObject]
public class WorkflowState : IEquatable<WorkflowState>
{
    public string Name { get; }

    public WorkflowState(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(name));
        
        Name = name;
    }

    public bool Equals(WorkflowState? other)
    {
        if (ReferenceEquals(this, other)) return true;
        return Name == other?.Name;
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(this, obj)) return true;
        if (obj?.GetType() != GetType()) return false;
        return Equals((WorkflowState)obj);
    }

    public override int GetHashCode()
    {
        return Name.GetHashCode();
    }
}