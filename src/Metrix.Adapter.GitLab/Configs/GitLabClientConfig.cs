namespace Io.Juenger.Scrum.GitLab.Configs
{
    internal class GitLabClientConfig : IGitLabClientConfig
    {
        public string AccessToken { get; set; }
        
        public string BaseUrl { get; set; }
    }
}