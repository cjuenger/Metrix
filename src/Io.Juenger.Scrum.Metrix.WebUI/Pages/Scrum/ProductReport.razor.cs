using System.Diagnostics;
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
    private IEnumerable<IProductAggregate>? _products;
    private IProductAggregate? _selectedProduct;
    private IEnumerable<ItemEntity>? _backlogItems;
    private string _productId = "";
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
        await base.OnInitializedAsync().ConfigureAwait(false);
        _products = await LoadProductsAsync().ConfigureAwait(false);
        _workflow = WorkflowFactory.Workflow;
    }
    
    private async void OnSelectedProductChanged(object value)
    {
        _reportCreated = false;
        
        if (value is IProductAggregate product)
        {
            _selectedProduct = product;
            await CreateReportAsync().ConfigureAwait(false);
        }

        await InvokeAsync(StateHasChanged).ConfigureAwait(false);

        Debug.WriteLine($"Value of {value} changed");
    }

    // private async void OnProductIdChanged(object value)
    // {
    //     _reportCreated = false;
    //     if (value is not string productId) return;
    //     _selectedProduct = await LoadProductAsync(productId).ConfigureAwait(false);
    //     await CreateReportAsync().ConfigureAwait(false);
    //     
    //     await InvokeAsync(StateHasChanged).ConfigureAwait(false);
    // }

    private async Task CreateReportAsync()
    {   
        var productId = ProductConfig.ProductId;
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
    
    private async Task<IEnumerable<IProductAggregate>> LoadProductsAsync()
    {
        var unorderedProducts = await ProductRepository
            .LoadProductsAsync()
            .ConfigureAwait(false);
        
        var orderedProducts = unorderedProducts.OrderBy(p => p.Name);
        return orderedProducts;
    }

    private Task<IProductAggregate> LoadProductAsync(string productId) => ProductRepository.LoadProductAsync(productId);

    private Task<IReadOnlyCollection<ItemEntity>> LoadBacklogItemsAsync(string productId)
    {
        return ItemsRepository.LoadProductItemsAsync(productId);
    }
}