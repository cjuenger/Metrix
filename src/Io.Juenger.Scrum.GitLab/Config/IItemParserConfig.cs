using System.Collections.Generic;
using Io.Juenger.Scrum.GitLab.Contracts.Values;

namespace Io.Juenger.Scrum.GitLab.Config
{
    internal interface IItemParserConfig
    {
        string StoryLabel { get; }
        
        string BugLabel { get; }
        
        string StoryPointPattern { get; }
        
        string StoryPointSplitter { get; }
        
        Dictionary<string, WorkflowState> LabelToWorkflowMapping { get; }
    }
}