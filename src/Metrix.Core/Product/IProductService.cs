using Metrix.Core.Values;

namespace Metrix.Core.Services;

public interface IProductService
{
    public Task<VelocityValue> CalculateVelocityAsync(string productId, CancellationToken cancellationToken = default);
    public Task<VelocityTrendValue> CalculateVelocityTrendAsync(string productId, CancellationToken cancellationToken = default);
    Task<CompositionValue> CalculateCompositionAsync(string productId, CancellationToken cancellationToken = default);
    Task<BurnDownValue> CalculateBurnDownAsync(string productId, CancellationToken cancellationToken = default);
    Task<BurnUpValue> CalculateBurnUpAsync(string productId, CancellationToken cancellationToken = default);
    Task<CycleTimesValue> CalculateCycleTimesAsync(string productId, CancellationToken cancellationToken = default);

    Task<ProductStatusValue> CalculateProductStatusAsync(
        string productId,
        CancellationToken cancellationToken = default);

    Task<CompositionTrendValue> CalculateCompositionTrendAsync(
        string productId,
        CancellationToken cancellationToken = default);
}