namespace Io.Juenger.Scrum.GitLab.Configs
{
    internal interface IGitLabClientConfig
    {
        public string AccessToken { get; set; }
        
        public string BaseUrl { get; set; }
    }
}