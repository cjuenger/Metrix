using Metrix.Core.Values;

namespace Metrix.Core.Entities;

public class ItemEntity
{
    public int Id { get; set; }
    public string Title { get; set; }
    public WorkflowStateValue WorkflowState { get; set; }
    public string Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime StartedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime? ClosedAt { get; set; }
    public IEnumerable<string> Tasks { get; set; }
    public string Link { get; set; }
}