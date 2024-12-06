using Metrix.Core.Entities;
using Metrix.Core.Values;

namespace Metrix.Core.Services;

internal interface IMetricsService
{
    VelocityValue CalculateVelocity(IReadOnlyCollection<SprintVelocityValue> sprintVelocityValues);
    VelocityTrendValue CalculateVelocityTrend(IReadOnlyCollection<SprintVelocityValue> sprintVelocityValues);
    CompositionValue CalculateComposition(IReadOnlyCollection<ItemEntity> itemEntities);
    BurnDownValue CalculateBurnDown(IEnumerable<ItemEntity> itemEntities, VelocityValue velocityValue);
    BurnUpValue CalculateBurnUp(IEnumerable<ItemEntity> itemEntities);
    CycleTimesValue CalculateCycleTime(IReadOnlyCollection<ItemEntity> itemEntities);

    CompositionTrendValue CalculateCompositionTrend(
        IEnumerable<(string Name, IEnumerable<ItemEntity> ItemEntities)> itemEntityGroups);
}