using System.Collections.Generic;
using Io.Juenger.Scrum.GitLab.Contracts.Entities;
using Io.Juenger.Scrum.GitLab.Contracts.Values;
using Io.Juenger.Scrum.GitLab.Values;

namespace Io.Juenger.Scrum.GitLab.Services.Application;

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