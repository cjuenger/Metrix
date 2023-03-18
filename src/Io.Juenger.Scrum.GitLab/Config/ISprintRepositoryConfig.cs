namespace Io.Juenger.Scrum.GitLab.Config
{
    internal interface ISprintRepositoryConfig
    {
        string SprintTimePattern { get; }
        string SprintLabelPattern { get; }
    }
}