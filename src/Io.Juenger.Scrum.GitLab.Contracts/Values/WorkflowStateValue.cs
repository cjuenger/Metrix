namespace Io.Juenger.Scrum.GitLab.Contracts.Values;

public class WorkflowStateValue : IEquatable<WorkflowStateValue>
{
    public string Name { get; }

    public WorkflowStateValue(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(name));
        
        Name = name;
    }

    public bool Equals(WorkflowStateValue? other)
    {
        if (ReferenceEquals(this, other)) return true;
        return Name == other?.Name;
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(this, obj)) return true;
        if (obj?.GetType() != GetType()) return false;
        return Equals((WorkflowStateValue)obj);
    }

    public override int GetHashCode()
    {
        return Name.GetHashCode();
    }
}