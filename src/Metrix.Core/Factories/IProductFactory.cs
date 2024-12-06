using Metrix.Core.Aggregates;

namespace Metrix.Core.Factories;

internal interface IProductFactory
{
    Product Create(string productId);
}