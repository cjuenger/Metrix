using Io.Juenger.Scrum.GitLab.Contracts.Values;

namespace Io.Juenger.Scrum.GitLab.Configs
{
    internal class ItemParserConfig : IItemParserConfig
    {
        public string StoryLabel { get; set; } = "Story";
        public string BugLabel { get; set; } = "Bug";
        public string StoryPointPattern { get; set; } = @"\d+ SP";
        public string StoryPointSplitter { get; set; } = " ";

        public Dictionary<string, WorkflowState> LabelToWorkflowMapping { get; set; } = new()
        {
            {"Opened", WorkflowState.Opened},
            {"Ready", WorkflowState.Ready},
            {"Planned", WorkflowState.Planned},
            {"Processing", WorkflowState.Processing},
            {"Reviewing", WorkflowState.Reviewing},
            {"Testing", WorkflowState.Testing},
            {"Accepting", WorkflowState.Accepting},
            {"Closed", WorkflowState.Closed},
            {"Cancelled", WorkflowState.Cancelled},
        };
    }
}