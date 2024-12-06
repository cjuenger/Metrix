namespace Io.Juenger.Scrum.GitLab.Configs
{
    internal class SprintRepositoryConfig : ISprintRepositoryConfig
    {
        // You can explore the RegEx here https://regexr.com/6f3jm
        private const string DefaultTimePattern = @"(From:|To:)\s*(\d{4})-(\d{2})-(\d{2})(T(\d{2}):(\d{2})(:(\d{2}))?)?";
        
        // You can explore the RegEx here https://regexr.com/6f3j4
        private const string DefaultSprintLabelPattern = "^(Sprint|sprint)((?!(Backlog|backlog)).)*$";
        public string SprintTimePattern { get; set; } = DefaultTimePattern;
        public string SprintLabelPattern { get; set; } = DefaultSprintLabelPattern;
    }
}