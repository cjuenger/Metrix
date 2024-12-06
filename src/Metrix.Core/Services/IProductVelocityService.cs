using Metrix.Core.Values;

namespace Metrix.Core.Services;

internal interface IProductVelocityService
{
    Task<VelocityValue> CalculateVelocityAsync(string productId, CancellationToken cancellationToken = default);
}