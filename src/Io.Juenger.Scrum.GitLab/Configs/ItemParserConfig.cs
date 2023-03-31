namespace Io.Juenger.Scrum.GitLab.Configs
{
    internal class ItemParserConfig : IItemParserConfig
    {
        public string StoryLabel { get; set; } = "Story";
        public string BugLabel { get; set; } = "Bug";
        public string StoryPointPattern { get; set; } = @"\d+ SP";
        public string StoryPointSplitter { get; set; } = " ";

        public Dictionary<string, string> WorkflowMapping { get; set; } = new()
        {
            { "Opened", "Opened" },
            { "Doing", "Doing" },
            { "CodeReview", "CodeReview" },
            { "Test", "Test" },
            { "Acceptance", "Acceptance" },
            { "Accepted", "Accepted" },
            {"Closed", "Closed" }
        };
    }
}