namespace Io.Juenger.Scrum.GitLab.Configs;

internal class WorkflowConfig : IWorkflowConfig
{
    public string[] Workflow { get; set; } =
    {
        "Opened",
        "Doing",
        "CodeReview",
        "Test",
        "Acceptance",
        "Accepted",
        "Closed"
    };
}