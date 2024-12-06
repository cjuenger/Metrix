using Metrix.Core.Values;
using Metrix.Core.Workflow;
using NMolecules.DDD;

namespace Metrix.Core.BacklogItem;

[Entity]
public class BacklogItem
{
    public int Id { get; set; }
    
    public BacklogItemType Type { get; set; }
    
    /// <summary>
    /// Story points in Scrum are units of measurement used to estimate the effort required to complete a story.
    /// The effort is also relevant for any backlog item. Thus, I chose a more general name: 'Effort'.
    /// </summary>
    public int? Effort { get; set; }
    
    public string Title { get; set; }
    public WorkflowState WorkflowState { get; set; }
    public string Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime StartedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime? ClosedAt { get; set; }
    public IEnumerable<string> Tasks { get; set; }
    
    public string Link { get; set; }
}