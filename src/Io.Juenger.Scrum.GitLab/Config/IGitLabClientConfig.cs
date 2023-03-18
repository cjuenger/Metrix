namespace Io.Juenger.Scrum.GitLab.Config
{
    internal interface IGitLabClientConfig
    {
        public string AccessToken { get; set; }
        
        public string BaseUrl { get; set; }
    }
}