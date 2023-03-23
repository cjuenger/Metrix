using System.Diagnostics;
using Io.Juenger.Scrum.GitLab.Contracts.Aggregates;
using Io.Juenger.Scrum.GitLab.Contracts.Entities;
using Io.Juenger.Scrum.GitLab.Contracts.Repositories;
using Io.Juenger.Scrum.GitLab.Contracts.Values;
using Io.Juenger.Scrum.Metrix.WebUI.Configs;
using Microsoft.AspNetCore.Components;

namespace Io.Juenger.Scrum.Metrix.WebUI.Pages.Scrum;

public partial class ProductReport
{
    private IEnumerable<IProductAggregate>? _products;
    private IProductAggregate? _selectedProduct;
    private IEnumerable<ItemEntity>? _backlogItems;
    private string _productId = "";

    private IEnumerable<ItemEntity> OpenItems => 
        _backlogItems?.Where(i => i.State == WorkflowState.Opened) ?? Enumerable.Empty<ItemEntity>();
    
    private IEnumerable<ItemEntity> ClosedItems => 
        _backlogItems?.Where(i => i.State == WorkflowState.Closed) ?? Enumerable.Empty<ItemEntity>();

    private IEnumerable<ItemEntity>? TotalItems => _backlogItems;
    
    private IEnumerable<StoryEntity> Stories => 
        _backlogItems?.OfType<StoryEntity>() ?? Enumerable.Empty<StoryEntity>();

    [Inject] 
    private ILogger<SprintReport> Logger { get; set; } = default!;

    [Inject]
    private ProductConfig ProductConfig { get; set; } = default!;
    
    [Inject] 
    private IProductRepository ProductRepository { get; set; } = default!;
    
    [Inject]
    private IItemsRepository ItemsRepository { get; set; } = default!;
    
    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync().ConfigureAwait(false);
        _products = await LoadProductsAsync().ConfigureAwait(false);
    }
    
    private async void OnSelectedProductChanged(object value)
    {
        if (value is IProductAggregate product)
        {
            _selectedProduct = product;
            _backlogItems = await LoadBacklogItemsAsync(_selectedProduct.Id).ConfigureAwait(false);
        }

        await InvokeAsync(StateHasChanged).ConfigureAwait(false);

        Debug.WriteLine($"Value of {value} changed");
    }

    private async void OnProductIdChanged(object value)
    {
        if (value is not string productId) return;
        _selectedProduct = await LoadProductAsync(productId).ConfigureAwait(false);
        _backlogItems = await LoadBacklogItemsAsync(productId).ConfigureAwait(false);
    }

    private async Task<IEnumerable<IProductAggregate>> LoadProductsAsync()
    {
        var unorderedProducts = await ProductRepository
            .LoadProductsAsync()
            .ConfigureAwait(false);
        
        var orderedProducts = unorderedProducts.OrderBy(p => p.Id);
        return orderedProducts;
    }

    private Task<IProductAggregate> LoadProductAsync(string productId) => ProductRepository.LoadProductAsync(productId);

    private Task<IReadOnlyCollection<ItemEntity>> LoadBacklogItemsAsync(string productId)
    {
        return ItemsRepository.LoadProductItemsAsync(productId);
    }
}