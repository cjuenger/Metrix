using Metrix.Core.Metrics.Values;

namespace Metrix.Core.Product;

internal interface IProductVelocityService
{
    Task<Velocity> CalculateVelocityAsync(string productId, CancellationToken cancellationToken = default);
}