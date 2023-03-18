using System;
using System.Collections.Generic;
using Io.Juenger.Scrum.GitLab.Contracts.Values;

namespace Io.Juenger.Scrum.GitLab.Contracts.Entities;

public class ItemEntity
{
    public int Id { get; set; }
    public string Title { get; set; }
    public WorkflowState State { get; set; }
    public string Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime StartedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime? ClosedAt { get; set; }
    public IEnumerable<string> Tasks { get; set; }
    public string Link { get; set; }
}