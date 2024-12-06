using Metrix.Core.Metrics.Values;
using Metrix.Core.Values;

namespace Metrix.Core.Product;

internal interface IProductVelocityService
{
    Task<Velocity> CalculateVelocityAsync(string productId, CancellationToken cancellationToken = default);
}