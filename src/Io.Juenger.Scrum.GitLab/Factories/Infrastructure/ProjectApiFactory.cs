using Io.Juenger.GitLabClient.Api;
using Io.Juenger.GitLabClient.Client;
using Io.Juenger.Scrum.GitLab.Config;
using Microsoft.Extensions.Configuration;

namespace Io.Juenger.Scrum.GitLab.Factories.Infrastructure
{
    internal class ProjectApiFactory : IProjectApiFactory
    {
        private readonly IConfiguration _configuration;
        private readonly IGitLabClientConfig _gitLabClientConfig;
        public IProjectApi ProjectApi => new ProjectApi(GetGitLabApiConfiguration());

        public ProjectApiFactory(IConfiguration configuration, IGitLabClientConfig gitLabClientConfig)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _gitLabClientConfig = gitLabClientConfig ?? throw new ArgumentNullException(nameof(gitLabClientConfig));
        }
        
        private Configuration GetGitLabApiConfiguration()
        {
            var configurationSection = _configuration.GetSection("GitLab");

            var basePath = _gitLabClientConfig.BaseUrl; 
            if (string.IsNullOrWhiteSpace(basePath))
            {
                basePath = configurationSection.GetValue("BasePath", "https://gitlab.com");
            }
            basePath += "/api";

            var apiToken = _gitLabClientConfig.AccessToken;
            if (string.IsNullOrWhiteSpace(apiToken))
            {
                apiToken = configurationSection.GetValue<string>("AccessToken");
            }
            
            var config = new Configuration { BasePath = basePath, AccessToken = apiToken };
            
            return config;
        }
    }
}