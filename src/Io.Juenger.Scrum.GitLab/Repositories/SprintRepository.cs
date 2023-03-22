using System.Text.RegularExpressions;
using Io.Juenger.GitLabClient.Model;
using Io.Juenger.Scrum.GitLab.Aggregates;
using Io.Juenger.Scrum.GitLab.Configs;
using Io.Juenger.Scrum.GitLab.Contracts.Aggregates;
using Io.Juenger.Scrum.GitLab.Contracts.Repositories;
using Io.Juenger.Scrum.GitLab.Factories.Infrastructure;
using Io.Juenger.Scrum.GitLab.Services.Infrastructure;
using Microsoft.Extensions.Logging;

namespace Io.Juenger.Scrum.GitLab.Repositories
{
    internal class SprintRepository : ISprintRepository
    {
        private readonly ISprintRepositoryConfig _config;
        private readonly ILogger<SprintRepository> _logger;
        private readonly IPaginationService _paginationService;
        private readonly IProjectApiFactory _projectApiFactory;

        public SprintRepository(
            IProjectApiFactory projectApiFactory, 
            IPaginationService paginationService,
            ISprintRepositoryConfig config,
            ILogger<SprintRepository> logger)
        {
            _projectApiFactory = projectApiFactory ?? throw new ArgumentNullException(nameof(projectApiFactory));
            _paginationService = paginationService ?? throw new ArgumentNullException(nameof(paginationService));
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<ISprintAggregate> LoadSprintByIdAsync(
            string projectId, 
            int sprintId,
            CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(projectId))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(projectId));
            }
            
            var sprintLabel = await _projectApiFactory
                .ProjectApi
                .GetProjectLabelAsync(projectId, iid: sprintId, cancellationToken: ct)
                .ConfigureAwait(false);
            
            var (start, end) = GetSprintTimeFromLabelDescription(sprintLabel);
            
            var sprintAggregate = new SprintAggregate(projectId, sprintLabel.Id, sprintLabel.Name, start, end);

            _logger.LogDebug(
                "Retrieved sprint {SprintName}, Start: {SprintStart}, End: {SprintEnd},", 
                sprintAggregate.Name,
                sprintAggregate.StartTime, 
                sprintAggregate.EndTime);

            return sprintAggregate;
        }
        
        public async Task<IReadOnlyList<ISprintAggregate>> LoadSprintsAsync(
            string projectId, 
            CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(projectId))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(projectId));
            }
            
            var labels = await _paginationService
                .BrowseAllAsync(page => 
                    _projectApiFactory
                        .ProjectApi
                        .GetProjectLabelsAsync(projectId, page, cancellationToken: ct))
                .ConfigureAwait(false);

            var sprintLabels = labels.Where(FilterSprintLabel);

            var sprintAggregates = new List<SprintAggregate>();
            foreach (var sprintLabel in sprintLabels)
            {
                var (start, end) = GetSprintTimeFromLabelDescription(sprintLabel);
                
                var sprintAggregate = new SprintAggregate(projectId, sprintLabel.Id, sprintLabel.Name, start, end);
                
                sprintAggregates.Add(sprintAggregate);
            }
            
            sprintAggregates.Sort((x,y) => DateTime.Compare(x.StartTime, y.StartTime));

            return sprintAggregates;
        }
        
        public async Task<ISprintAggregate> LoadLatestSprintAsync(
            string projectId, 
            CancellationToken ct = default)
        {
            // TODO: I should make this more performant!
            var sprints = await LoadSprintsAsync(projectId, ct);
            return sprints[^1];
        }
        
        private bool FilterSprintLabel(Label label)
        {
            var rgx = new Regex(_config.SprintLabelPattern);
            var match = rgx.Match(label.Name);
            return match.Success;
        }

        private (DateTime Start, DateTime End) GetSprintTimeFromLabelDescription(Label label)
        {
            var rgx = new Regex(_config.SprintTimePattern);
            var matches = rgx.Matches(label.Description);

            var from = DateTime.MinValue;
            var to = DateTime.MinValue;
            
            foreach (Match match in matches)
            {
                if (!match.Success) continue;
                
                var split = match.Value.Split(" ");
                var time = DateTime.Parse(split[1]);
                    
                if (match.Value.ToLower().Contains("from"))
                {
                    @from = time;
                }
                else if (match.Value.ToLower().Contains("to"))
                {
                    to = time;
                }
            }

            return (from, to);
        }
        
    }
}