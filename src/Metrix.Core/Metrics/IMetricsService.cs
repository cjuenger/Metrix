using Metrix.Core.Metrics.Values;
using Metrix.Core.Sprint;

namespace Metrix.Core.Metrics;

public interface IMetricsService
{
    Velocity CalculateVelocity(IReadOnlyCollection<SprintVelocity> sprintVelocityValues);
    VelocityTrend CalculateVelocityTrend(IReadOnlyCollection<SprintVelocity> sprintVelocityValues);
    Composition CalculateComposition(IReadOnlyCollection<BacklogItem.BacklogItem> itemEntities);
    BurnDown CalculateBurnDown(IEnumerable<BacklogItem.BacklogItem> itemEntities, Velocity velocity, ProductCalendar productCalendar);
    BurnUp CalculateBurnUp(IEnumerable<BacklogItem.BacklogItem> itemEntities);
    CycleTimesValue CalculateCycleTime(IReadOnlyCollection<BacklogItem.BacklogItem> itemEntities);

    CompositionTrend CalculateCompositionTrend(IEnumerable<(string Name, IEnumerable<BacklogItem.BacklogItem> ItemEntities)> itemEntityGroups);
}