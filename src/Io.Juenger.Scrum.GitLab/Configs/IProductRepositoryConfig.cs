namespace Io.Juenger.Scrum.GitLab.Configs;

internal interface IProductRepositoryConfig
{
    string ProductVisionPattern { get; set; }
    string RepositoryTypePattern { get; set; }
    string ProductPattern { get; set; }
    string ProductKickoffPattern { get; set; }
    string ProductEndPattern { get; set; }
}