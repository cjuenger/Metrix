using System.Diagnostics;
using AutoMapper;
using Io.Juenger.Scrum.GitLab.Contracts.Aggregates;
using Io.Juenger.Scrum.GitLab.Contracts.Entities;
using Io.Juenger.Scrum.GitLab.Contracts.Repositories;
using Io.Juenger.Scrum.GitLab.Contracts.Services;
using Io.Juenger.Scrum.GitLab.Contracts.Values;
using Io.Juenger.Scrum.Metrix.WebUI.Configs;
using Microsoft.AspNetCore.Components;

namespace Io.Juenger.Scrum.Metrix.WebUI.Pages.Scrum
{
    public partial class SprintReport
    {
        private IEnumerable<ISprintAggregate> _sprints;
        private ISprintAggregate _selectedSprint;
        
        private IEnumerable<ItemEntity> OpenItems => 
            _selectedSprint
                .Items
                .Where(i => i.State == WorkflowState.Opened) ?? Enumerable.Empty<ItemEntity>();
        
        private IEnumerable<ItemEntity> ClosedItems => 
            _selectedSprint?
                .Items
                .Where(i => i.State == WorkflowState.Closed) ?? Enumerable.Empty<ItemEntity>();

        private IEnumerable<ItemEntity> TotalItems => _selectedSprint?.Items;
        
        private IEnumerable<StoryEntity> Stories =>
            _selectedSprint?
                .Items
                .OfType<StoryEntity>() ?? Enumerable.Empty<StoryEntity>();
        
        [Inject]
        private IMapper Mapper { get; set; }
        
        [Inject]
        private ILogger<SprintReport> Logger { get; set; }

        [Inject]
        private ProductConfig ProductConfig { get; set; }
        
        [Inject]
        private ISprintAggregateService SprintAggregateService { get; set; }
        
        [Inject] 
        private ISprintRepository SprintRepository { get; set; }
        
        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync().ConfigureAwait(false);

            var productId = ProductConfig.ProductId;
            var sprints = await SprintRepository.LoadSprintsAsync(productId);

            sprints = sprints.OrderBy(sp => sp.StartTime).ToList();

            _selectedSprint = sprints[^1];
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
    }
}