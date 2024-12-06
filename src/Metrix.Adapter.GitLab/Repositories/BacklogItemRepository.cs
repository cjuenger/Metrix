using Io.Juenger.Scrum.GitLab.Factories;
using Io.Juenger.Scrum.GitLab.Services.Domain;
using Io.Juenger.Scrum.GitLab.Services.Infrastructure;
using Metrix.Core.BacklogItem;

namespace Io.Juenger.Scrum.GitLab.Repositories
{
    internal class BacklogItemRepository : IBacklogItemRepository
    {
        private readonly IProjectApiFactory _projectApiFactory;
        private readonly IPaginationService _paginationService;
        private readonly IItemParserService _itemParserService;

        public BacklogItemRepository(
            IProjectApiFactory projectApiFactory,
            IPaginationService paginationService,
            IItemParserService itemParserService)
        {
            _projectApiFactory = projectApiFactory ?? throw new ArgumentNullException(nameof(projectApiFactory));
            _paginationService = paginationService ?? throw new ArgumentNullException(nameof(paginationService));
            _itemParserService = itemParserService ?? throw new ArgumentNullException(nameof(itemParserService));
        }

        public Task<IReadOnlyCollection<BacklogItem>> LoadProductItemsAsync(
            string projectId, 
            string? ofSprint = null, 
            int? ofReleaseId = null, 
            CancellationToken ct = default)
        {
            if (ofSprint != null && ofReleaseId == null)
            {
                return LoadSprintItemsAsync(projectId, ofSprint, ct);
            }

            if (ofReleaseId != null && ofSprint == null)
            {
                return LoadReleaseItemsAsync(projectId, ofReleaseId.Value, ct);
            }
            
            return LoadProductItemsAsync(projectId, ct);
        }

        private async Task<IReadOnlyCollection<BacklogItem>> LoadProductItemsAsync(
            string projectId, 
            CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(projectId))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(projectId));
            }
            
            var issues = await _paginationService
                .BrowseAllAsync(page => 
                    _projectApiFactory
                        .ProjectApi
                        .GetProjectIssuesAsync(projectId, page, cancellationToken: ct))
                .ConfigureAwait(false);
            
            var items = issues
                .Select(i => _itemParserService.Parse(i))
                .ToList();
                
            return items;
        }
        
        private async Task<IReadOnlyCollection<BacklogItem>> LoadSprintItemsAsync(
            string projectId, 
            string ofSprint, 
            CancellationToken ct = default)
        {
            var totalIssues = await _paginationService
                .BrowseAllAsync(page => 
                    _projectApiFactory
                        .ProjectApi
                        .GetProjectIssuesAsync(projectId, page, labels: new List<string> {ofSprint}, cancellationToken: ct))
                .ConfigureAwait(false);
            
            var items = totalIssues.Select(i => _itemParserService.Parse(i));
        
            return items.ToList();
        }
        
        private async Task<IReadOnlyCollection<BacklogItem>> LoadReleaseItemsAsync(
            string projectId, 
            int ofReleaseId, 
            CancellationToken ct = default)
        {
            var totalIssues=  
                await _paginationService
                    .BrowseAllAsync(page => 
                        _projectApiFactory
                            .ProjectApi
                            .GetAllIssuesOfProjectMilestoneAsync(projectId, ofReleaseId, page, cancellationToken: ct))
                    .ConfigureAwait(false);
            
            var items = totalIssues.Select(i => _itemParserService.Parse(i)).ToList();
            return items;
        }
    }
}