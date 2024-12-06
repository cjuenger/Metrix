namespace Io.Juenger.Scrum.GitLab.Configs
{
    internal interface ISprintRepositoryConfig
    {
        string SprintTimePattern { get; }
        string SprintLabelPattern { get; }
    }
}