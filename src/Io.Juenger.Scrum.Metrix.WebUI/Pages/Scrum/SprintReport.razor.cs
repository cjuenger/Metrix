using System.Diagnostics;
using Io.Juenger.Scrum.GitLab.Contracts.Aggregates;
using Io.Juenger.Scrum.GitLab.Contracts.Entities;
using Io.Juenger.Scrum.GitLab.Contracts.Repositories;
using Io.Juenger.Scrum.GitLab.Contracts.Values;
using Io.Juenger.Scrum.Metrix.WebUI.Configs;
using Microsoft.AspNetCore.Components;

namespace Io.Juenger.Scrum.Metrix.WebUI.Pages.Scrum
{
    public partial class SprintReport
    {
        private IEnumerable<ISprintAggregate>? _sprints;
        private ISprintAggregate? _selectedSprint;
        private IEnumerable<ItemEntity>? _sprintBacklogItems;

        private IEnumerable<ItemEntity> OpenItems => 
            _sprintBacklogItems?.Where(i => i.State == WorkflowState.Opened) ?? Enumerable.Empty<ItemEntity>();
        
        private IEnumerable<ItemEntity> ClosedItems => 
            _sprintBacklogItems?.Where(i => i.State == WorkflowState.Closed) ?? Enumerable.Empty<ItemEntity>();

        private IEnumerable<ItemEntity>? TotalItems => _sprintBacklogItems;
        
        private IEnumerable<StoryEntity> Stories => 
            _sprintBacklogItems?.OfType<StoryEntity>() ?? Enumerable.Empty<StoryEntity>();

        [Inject] 
        private ILogger<SprintReport> Logger { get; set; } = default!;

        [Inject]
        private ProductConfig ProductConfig { get; set; } = default!;
        
        [Inject] 
        private ISprintRepository SprintRepository { get; set; } = default!;
        
        [Inject]
        private IItemsRepository ItemsRepository { get; set; } = default!;
        
        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync().ConfigureAwait(false);

            var productId = ProductConfig.ProductId;
            _sprints = await LoadSprintsAsync(productId).ConfigureAwait(false);
            
            _selectedSprint = _sprints.Last();

            var sprintId = _selectedSprint.Name;
            _sprintBacklogItems = await LoadSprintItemsAsync(productId, sprintId).ConfigureAwait(false);
        }
        
        private async void OnChange(object value)
        {
            if (value is ISprintAggregate sprint)
            {
                _selectedSprint = sprint;
            }
            
            await InvokeAsync(StateHasChanged).ConfigureAwait(false);

            Debug.WriteLine($"Value of {value} changed");
        }

        private async Task<IEnumerable<ISprintAggregate>> LoadSprintsAsync(string productId)
        {
            var unorderedSprints = await SprintRepository
                .LoadSprintsAsync(productId)
                .ConfigureAwait(false);
            
            var orderedSprints = unorderedSprints.OrderBy(sp => sp.StartTime);
            return orderedSprints;
        }

        private Task<IReadOnlyCollection<ItemEntity>> LoadSprintItemsAsync(string productId, string sprintId)
        {
            return ItemsRepository.LoadProductItemsAsync(productId, ofSprint: sprintId);
        }
    }
}