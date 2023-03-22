using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Io.Juenger.GitLabClient.Model;
using Io.Juenger.Scrum.GitLab.Aggregates;
using Io.Juenger.Scrum.GitLab.Contracts.Aggregates;
using Io.Juenger.Scrum.GitLab.Contracts.Repositories;
using Io.Juenger.Scrum.GitLab.Factories.Infrastructure;
using Io.Juenger.Scrum.GitLab.Services.Infrastructure;

namespace Io.Juenger.Scrum.GitLab.Repositories
{
    internal class ReleaseRepository : IReleaseRepository
    {
        private readonly IProjectApiFactory _projectApiFactory;
        private readonly IPaginationService _paginationService;

        public ReleaseRepository(
            IProjectApiFactory projectApiFactory, 
            IPaginationService paginationService)
        {
            _projectApiFactory = projectApiFactory ?? throw new ArgumentNullException(nameof(projectApiFactory));
            _paginationService = paginationService ?? throw new ArgumentNullException(nameof(paginationService));
        }

        public async Task<IReadOnlyList<IReleaseAggregate>> LoadReleasesAsync(
            string projectId, 
            CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(projectId))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(projectId));
            }

            var milestones = await LoadMilestonesAsync(projectId, ct);

            return milestones
                .Select(milestone => 
                    new ReleaseAggregate(
                        projectId, 
                        milestone.Id, 
                        milestone.Title, 
                        milestone.StartDate, 
                        milestone.DueDate))
                .ToList();
        }

        public async Task<IReleaseAggregate> LoadReleaseByIdAsync(
            string projectId, 
            int releaseId,
            CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(projectId))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(projectId));
            }

            var milestone = await LoadMilestoneAsync(projectId, releaseId, ct);

            var releaseAggregate = new ReleaseAggregate(
                projectId,
                milestone.Iid, 
                milestone.Title, 
                milestone.StartDate, 
                milestone.DueDate);

            return releaseAggregate;
        }

        public async Task<IReleaseAggregate> LoadLatestReleaseAsync(
            string projectId,
            CancellationToken ct = default)
        {
            // TODO: I should make this more performant!
            var releases = await LoadReleasesAsync(projectId, ct);

            // TODO: I should either catch the exception here or use a 'TryLoad...' variant 
            return releases
                .Where(r => r.DueDate.HasValue)
                .OrderBy(r => r.DueDate)
                .Last();
        }

        private async Task<IEnumerable<Milestone>> LoadMilestonesAsync(string projectId, CancellationToken ct)
        {
            var projectApi = _projectApiFactory.ProjectApi;

            var milestones = await _paginationService
                .BrowseAllAsync(page => 
                    projectApi
                        .GetProjectMilestonesAsync(projectId, page: page, cancellationToken: ct))
                .ConfigureAwait(false);

            return milestones;
        }
        
        private async Task<Milestone> LoadMilestoneAsync(string projectId, int id, CancellationToken ct)
        {
            var projectApi = _projectApiFactory.ProjectApi;

            var milestone = await projectApi
                .GetProjectMilestoneAsync(projectId, iid: id, cancellationToken: ct)
                .ConfigureAwait(false);

            return milestone;
        }
    }
}