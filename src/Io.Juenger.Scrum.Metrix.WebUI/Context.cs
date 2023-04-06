using System.ComponentModel;
using System.Runtime.CompilerServices;
using Io.Juenger.Scrum.GitLab.Contracts.Aggregates;

namespace Io.Juenger.Scrum.Metrix.WebUI;

public class Context : IContext
{
    private IProductAggregate? _selectedProduct;
    private IList<IProductAggregate> _products = new List<IProductAggregate>();

    public event PropertyChangedEventHandler? PropertyChanged;

    public IProductAggregate? SelectedProduct
    {
        get => _selectedProduct;
        set
        {
            if (_selectedProduct != null && _selectedProduct.Equals(value)) return;
            _selectedProduct = value;
            OnPropertyChanged();
        }
    }

    public IList<IProductAggregate> Products
    {
        get => _products;
        set
        {
            if (_products.Equals(value)) return;
            _products = value;
            OnPropertyChanged();
        }
    }
    
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}