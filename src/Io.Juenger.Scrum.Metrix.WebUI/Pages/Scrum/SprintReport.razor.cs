using System.ComponentModel;
using System.Diagnostics;
using Io.Juenger.Scrum.GitLab.Contracts.Aggregates;
using Io.Juenger.Scrum.GitLab.Contracts.Entities;
using Io.Juenger.Scrum.GitLab.Contracts.Repositories;
using Io.Juenger.Scrum.GitLab.Contracts.Services;
using Io.Juenger.Scrum.GitLab.Contracts.Values;
using Io.Juenger.Scrum.GitLab.Factories.Application;
using Io.Juenger.Scrum.Metrix.WebUI.Configs;
using Microsoft.AspNetCore.Components;

namespace Io.Juenger.Scrum.Metrix.WebUI.Pages.Scrum
{
    public partial class SprintReport
    {
        private IEnumerable<ISprintAggregate>? _sprints;
        private ISprintAggregate? _selectedSprint;
        private IEnumerable<ItemEntity>? _sprintBacklogItems;
        private WorkflowValue _workflow;

        private bool ReportLoaded { get; set; }

        private IEnumerable<ItemEntity> OpenItems => 
            _sprintBacklogItems?.Where(i => !i.WorkflowState.Equals(_workflow.WorkflowStates.Last())) ?? Enumerable.Empty<ItemEntity>();

        private IEnumerable<ItemEntity> InProgressItems =>
            _sprintBacklogItems?.Where(i => 
                !i.WorkflowState.Equals(_workflow.WorkflowStates.First()) && 
                !i.WorkflowState.Equals(_workflow.WorkflowStates.Last())) ?? Enumerable.Empty<ItemEntity>();

        private IEnumerable<ItemEntity> ClosedItems => 
            _sprintBacklogItems?.Where(i => i.WorkflowState.Equals(_workflow.WorkflowStates.Last())) ?? Enumerable.Empty<ItemEntity>();

        private IEnumerable<ItemEntity>? TotalItems => _sprintBacklogItems;
        
        private BurnDownValue BurnDown { get; set; }
        
        private int Progress => (int)((float)ClosedItems.Count() / (_sprintBacklogItems?.Count() ?? 0) * 100);

        [Inject]
        private IContext Context { get; set; }
        
        [Inject] 
        private ILogger<SprintReport> Logger { get; set; } = default!;
        
        [Inject] 
        private ISprintRepository SprintRepository { get; set; } = default!;
        
        [Inject]
        private IItemsRepository ItemsRepository { get; set; } = default!;

        [Inject] 
        private ISprintAggregateService SprintAggregateService { get; set; } = default!;
        
        [Inject] private IWorkflowFactory WorkflowFactory { get; set; } = default!;
        
        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync().ConfigureAwait(false);
            
            _workflow = WorkflowFactory.Workflow;
            Context.PropertyChanged += OnPropertyChanged;

            await LoadSprintsAsync().ConfigureAwait(false);
        }
        
        private async void OnChange(object value)
        {
            ReportLoaded = false;
            
            if (value is ISprintAggregate sprint)
            {
                _selectedSprint = sprint;
                await LoadSprintReportAsync(_selectedSprint);
            }
            
            await InvokeAsync(StateHasChanged).ConfigureAwait(false);
            Debug.WriteLine($"Value of {value} changed");
        }

        private async void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (sender is not IContext) return;
            await LoadSprintsAsync().ConfigureAwait(false);
        }
        
        private async Task LoadSprintReportAsync(ISprintAggregate sprintAggregate)
        {
            if (Context.SelectedProduct == null) return;

            var productId = Context.SelectedProduct.Id;
            
            _sprintBacklogItems = await ItemsRepository
                .LoadProductItemsAsync(productId, ofSprint: sprintAggregate.Name)
                .ConfigureAwait(false);
            
            BurnDown = await SprintAggregateService
                .CalculateBurnDownAsync(productId, sprintAggregate.Id)
                .ConfigureAwait(false);
            
            ReportLoaded = true;
        }

        private async Task LoadSprintsAsync()
        {
            if (Context.SelectedProduct != null)
            {
                _sprints = null;
                _selectedSprint = null;
                _sprints = await LoadSprintsAsync(Context.SelectedProduct.Id).ConfigureAwait(false);
                await InvokeAsync(StateHasChanged).ConfigureAwait(false);
            }
        }
        
        private async Task<IEnumerable<ISprintAggregate>> LoadSprintsAsync(string productId)
        {
            var unorderedSprints = await SprintRepository
                .LoadSprintsAsync(productId)
                .ConfigureAwait(false);
            
            var orderedSprints = unorderedSprints.OrderBy(sp => sp.StartTime);
            return orderedSprints;
        }
    }
}