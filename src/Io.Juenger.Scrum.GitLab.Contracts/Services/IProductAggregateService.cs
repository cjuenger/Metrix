using System.Threading;
using System.Threading.Tasks;
using Io.Juenger.Scrum.GitLab.Contracts.Values;

namespace Io.Juenger.Scrum.GitLab.Contracts.Services;

public interface IProductAggregateService
{
    public Task<VelocityValue> CalculateVelocityAsync(string productId, CancellationToken cancellationToken = default);
    public Task<VelocityTrendValue> CalculateVelocityTrendAsync(string productId, CancellationToken cancellationToken = default);
    Task<CompositionValue> CalculateCompositionAsync(string productId, CancellationToken cancellationToken = default);
    Task<BurnDownValue> CalculateBurnDownAsync(string productId, CancellationToken cancellationToken = default);
    Task<BurnUpValue> CalculateBurnUpAsync(string productId, CancellationToken cancellationToken = default);
    Task<CycleTimesValue> CalculateCycleTimeAsync(string productId, CancellationToken cancellationToken = default);

    Task<ProductStatusValue> CalculateProductStatusAsync(
        string productId,
        CancellationToken cancellationToken = default);

    Task<CompositionTrendValue> CalculateCompositionTrendAsync(
        string productId,
        CancellationToken cancellationToken = default);
}