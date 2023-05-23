using Io.Juenger.Scrum.GitLab.Contracts.Aggregates;
using Io.Juenger.Scrum.GitLab.Contracts.Repositories;
using Io.Juenger.Scrum.Metrix.WebUI.Configs;
using Microsoft.AspNetCore.Components;

namespace Io.Juenger.Scrum.Metrix.WebUI.Shared.Scrum;

public partial class ProductSelection
{
    private IEnumerable<IProductAggregate>? _products;
    private IProductAggregate? _selectedProduct;

    private string Kickoff => _selectedProduct?.Kickoff.ToString("yyyy-MM-dd") ?? string.Empty;
    private string DueDate => _selectedProduct?.DueDate.ToString("yyyy-MM-dd") ?? string.Empty;

    [Inject] 
    private IContext Context { get; set; } = default!;
    
    [Inject] 
    private IProductRepository ProductRepository { get; set; } = default!;
    
    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync().ConfigureAwait(false);
        _products = await LoadProductsAsync().ConfigureAwait(false);
        
        Context.Products = _products.ToList();
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

        Context.SelectedProduct = _selectedProduct;
    }
}