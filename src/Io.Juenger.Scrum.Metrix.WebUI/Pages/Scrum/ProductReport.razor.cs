using System.ComponentModel;
using Io.Juenger.Scrum.GitLab.Contracts.Aggregates;
using Io.Juenger.Scrum.GitLab.Contracts.Entities;
using Io.Juenger.Scrum.GitLab.Contracts.Repositories;
using Io.Juenger.Scrum.GitLab.Contracts.Services;
using Io.Juenger.Scrum.GitLab.Contracts.Values;
using Io.Juenger.Scrum.GitLab.Factories.Application;
using Io.Juenger.Scrum.Metrix.WebUI.Configs;
using Microsoft.AspNetCore.Components;

namespace Io.Juenger.Scrum.Metrix.WebUI.Pages.Scrum;

public partial class ProductReport
{
    private IProductAggregate? _selectedProduct;
    private IEnumerable<ItemEntity>? _backlogItems;
    private WorkflowValue _workflow;
    private bool _reportCreated;
    private BurnDownValue _burnDown;
    private VelocityValue _velocity;
    private VelocityTrendValue _velocityTrend;
    private CycleTimesValue _cycleTimes;

    private IEnumerable<ItemEntity> OpenItems => 
        _backlogItems?.Where(i => !i.WorkflowState.Equals(_workflow.WorkflowStates.Last())) ?? Enumerable.Empty<ItemEntity>();
    
    private IEnumerable<ItemEntity> ClosedItems => 
        _backlogItems?.Where(i => i.WorkflowState.Equals(_workflow.WorkflowStates.Last())) ?? Enumerable.Empty<ItemEntity>();

    private IEnumerable<ItemEntity>? TotalItems => _backlogItems;
    
    private int Progress => (int) ((float)ClosedItems.Count() / (TotalItems?.Count() ?? 0) * 100);

    [Inject] 
    private IContext Context { get; set; } = default!;
    
    [Inject] 
    private ILogger<SprintReport> Logger { get; set; } = default!;

    [Inject]
    private ProductConfig ProductConfig { get; set; } = default!;
    
    [Inject] 
    private IProductRepository ProductRepository { get; set; } = default!;
    
    [Inject]
    private IItemsRepository ItemsRepository { get; set; } = default!;

    [Inject] private IProductAggregateService ProductAggregateService { get; set; } = default!;

    [Inject] private IWorkflowFactory WorkflowFactory { get; set; } = default!;
    
    protected override async Task OnInitializedAsync()
    {
        Context.PropertyChanged += OnPropertyChanged;
        
        await base.OnInitializedAsync().ConfigureAwait(false);
        _workflow = WorkflowFactory.Workflow;
    }

    private async void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (sender is not IContext context) return;
        
        _reportCreated = false;
        
        _selectedProduct = context.SelectedProduct;
        
        if(_selectedProduct == null) return;
        await CreateReportAsync().ConfigureAwait(false);
        await InvokeAsync(StateHasChanged).ConfigureAwait(false);
    }

    private async Task CreateReportAsync()
    {   
        // var productId = ProductConfig.ProductId;

        var productId = _selectedProduct!.Id;
        _backlogItems = await ItemsRepository
            .LoadProductItemsAsync(productId)
            .ConfigureAwait(false);
            
        _burnDown = await ProductAggregateService
            .CalculateBurnDownAsync(productId)
            .ConfigureAwait(false);

        _velocity = await ProductAggregateService
            .CalculateVelocityAsync(productId)
            .ConfigureAwait(false);

        _velocityTrend = await ProductAggregateService
            .CalculateVelocityTrendAsync(productId)
            .ConfigureAwait(false);

        _cycleTimes = await ProductAggregateService
            .CalculateCycleTimesAsync(productId)
            .ConfigureAwait(false);
        
        _reportCreated = true;
    }
}