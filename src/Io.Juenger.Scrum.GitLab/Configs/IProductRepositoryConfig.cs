namespace Io.Juenger.Scrum.GitLab.Configs;

internal interface IProductRepositoryConfig
{
    string ProductVisionPattern { get; set; }
    string ProductTypePattern { get; set; }
    string ProductNamePattern { get; set; }
    string ProductKickoffPattern { get; set; }
    string ProductDueDatePattern { get; set; }
}