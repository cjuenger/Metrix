using Metrix.Core.Metrics.Values;

namespace Metrix.Core.Product;

public interface IProductMetricsService
{
    public Task<Velocity> CalculateVelocityAsync(string productId, CancellationToken cancellationToken = default);
    public Task<VelocityTrend> CalculateVelocityTrendAsync(string productId, CancellationToken cancellationToken = default);
    Task<Composition> CalculateCompositionAsync(string productId, CancellationToken cancellationToken = default);
    Task<BurnDown> CalculateBurnDownAsync(string productId, ProductCalendar productCalendar, CancellationToken cancellationToken = default);
    Task<BurnUp> CalculateBurnUpAsync(string productId, CancellationToken cancellationToken = default);
    Task<CycleTimesValue> CalculateCycleTimesAsync(string productId, CancellationToken cancellationToken = default);

    Task<Status> CalculateProductStatusAsync(
        string productId,
        CancellationToken cancellationToken = default);

    Task<CompositionTrend> CalculateCompositionTrendAsync(
        string productId,
        CancellationToken cancellationToken = default);
}