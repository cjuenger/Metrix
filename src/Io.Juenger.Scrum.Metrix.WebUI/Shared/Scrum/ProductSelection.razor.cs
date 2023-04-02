using Io.Juenger.Scrum.GitLab.Contracts.Aggregates;
using Io.Juenger.Scrum.GitLab.Contracts.Repositories;
using Io.Juenger.Scrum.Metrix.WebUI.Configs;
using Microsoft.AspNetCore.Components;

namespace Io.Juenger.Scrum.Metrix.WebUI.Shared.Scrum;

public partial class ProductSelection
{
    private IEnumerable<IProductAggregate>? _products;
    private IProductAggregate? _selectedProduct;
    
    [Inject] 
    private IProductRepository ProductRepository { get; set; } = default!;
    
    [Inject]
    private ProductConfig ProductConfig { get; set; } = default!;
    
    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync().ConfigureAwait(false);
        _products = await LoadProductsAsync().ConfigureAwait(false);
    }
    
    private async Task<IEnumerable<IProductAggregate>> LoadProductsAsync()
    {
        var unorderedProducts = await ProductRepository
            .LoadProductsAsync()
            .ConfigureAwait(false);
        
        var orderedProducts = unorderedProducts.OrderBy(p => p.Name);
        return orderedProducts;
    }
    
    private async void OnSelectedProductChanged(object value)
    {
        await InvokeAsync(StateHasChanged).ConfigureAwait(false);
    }
}