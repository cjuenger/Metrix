using Io.Juenger.Common.Util;
using Io.Juenger.Scrum.GitLab.Contracts.Entities;
using Io.Juenger.Scrum.GitLab.Contracts.Values;
using Io.Juenger.Scrum.GitLab.Services.Domain;
using Io.Juenger.Scrum.GitLab.Values;
using Microsoft.Extensions.Logging;

namespace Io.Juenger.Scrum.GitLab.Services.Application;

internal class MetricsService : IMetricsService
{
    private readonly IStorySeriesService _storySeriesService;
    private readonly ILogger<MetricsService> _logger;

    public MetricsService( 
        IStorySeriesService storySeriesService,
        ILogger<MetricsService> logger)
    {
        _storySeriesService = storySeriesService ?? throw new ArgumentNullException(nameof(storySeriesService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public VelocityValue CalculateVelocity(IReadOnlyCollection<SprintVelocityValue> sprintVelocityValues)
    {
        var countOfSprints = sprintVelocityValues.Count;

        var allSprints = sprintVelocityValues
            .OrderBy(tsp => tsp.TotalStoryPoints)
            .ToList();
        
        var averageSprintLength = CalculateAverageSprintLength(sprintVelocityValues);
        var averageVelocity = CalculateAverageVelocity(allSprints);

        var best3Sprints = allSprints.TakeLast(3).ToList();
        var best3SprintsAverageVelocity = CalculateAverageVelocity(best3Sprints);
        var best3SprintsDayAverageVelocity = CalculateDayAverageVelocity(best3Sprints);
        
        var worst3Sprints = allSprints.Take(3).ToList();
        var worst3SprintsAverageVelocity = CalculateAverageVelocity(worst3Sprints);
        var worst3SprintsDayAverageVelocity = CalculateDayAverageVelocity(worst3Sprints);

        return new VelocityValue(
            averageVelocity,
            best3SprintsAverageVelocity,
            best3SprintsDayAverageVelocity,
            worst3SprintsAverageVelocity,
            worst3SprintsDayAverageVelocity,
            countOfSprints,
            averageSprintLength);
    }
    
    public VelocityTrendValue CalculateVelocityTrend(IReadOnlyCollection<SprintVelocityValue> sprintVelocityValues)
    {
        var velocitySeries = CalculateVelocitySeries(sprintVelocityValues);
        return new VelocityTrendValue(velocitySeries);
    }
    
    public CompositionValue CalculateComposition(IReadOnlyCollection<ItemEntity> itemEntities)
    {
        return new CompositionValue(itemEntities);
    }

    public CompositionTrendValue CalculateCompositionTrend(
        IEnumerable<(string Name, IEnumerable<ItemEntity> ItemEntities)> itemEntityGroups)
    {
        var compositionTrendSeries = new List<XyValue<string, CompositionValue>>();
        
        // ReSharper disable once LoopCanBeConvertedToQuery
        foreach (var itemEntityGroup in itemEntityGroups)
        {
            var compositionValue = new CompositionValue(itemEntityGroup.ItemEntities);
            compositionTrendSeries.Add(new XyValue<string, CompositionValue>
            {
                X = $"{itemEntityGroup.Name}", 
                Y = compositionValue
            });
        }

        return new CompositionTrendValue(compositionTrendSeries);
    }

    public BurnDownValue CalculateBurnDown(
        IEnumerable<ItemEntity> itemEntities,
        VelocityValue velocityValue)
    {
        var stories = itemEntities.OfType<StoryEntity>().ToList();
            
        var burnDownSeries = CalculateBurnDownChartSeries(stories);
            
        var estimateSeries = CalculateBurnDownEstimationChartSeries(
            burnDownSeries, 
            velocityValue.DayAverageVelocity);
            
        var bestEstimateSeries = CalculateBurnDownEstimationChartSeries(
            burnDownSeries, 
            velocityValue.Best3SprintsDayAverageVelocity);
            
        var worstEstimateSeries = CalculateBurnDownEstimationChartSeries(
            burnDownSeries, 
            velocityValue.Worst3SprintsDayAverageVelocity);
        
        var burnDownValue = new BurnDownValue(
            burnDownSeries, 
            estimateSeries, 
            bestEstimateSeries, 
            worstEstimateSeries); 
        
        return burnDownValue;
    }

    public BurnUpValue CalculateBurnUp(IEnumerable<ItemEntity> itemEntities)
    {
        var stories = itemEntities.OfType<StoryEntity>();
        var storyArray = stories.ToArray();
        
        var totalStoryPointsSeries = _storySeriesService
            .CalculateCumulatedOpenedStoryChart(storyArray);
        
        var completedStoryPointsSeries = _storySeriesService
            .CalculateCumulatedClosedStoryChart(storyArray);
        
        return new BurnUpValue(totalStoryPointsSeries, completedStoryPointsSeries);
    }

    public CycleTimesValue CalculateCycleTime(IReadOnlyCollection<ItemEntity> itemEntities)
    {
        var stories = itemEntities.OfType<StoryEntity>().ToList();
        var bugs = itemEntities.OfType<BugEntity>().ToList();
        var others = itemEntities.Except(stories).Except(bugs).ToList();

        var storyThroughputTimeAsTicks = GetThroughputTimesAsTicks(
            stories.Where(i => i.ClosedAt.HasValue));
        var bugThroughputTimeAsTicks = GetThroughputTimesAsTicks(
            bugs.Where(i => i.ClosedAt.HasValue));
        var otherThroughputTimeAsTicks = GetThroughputTimesAsTicks(
            others.Where(i => i.ClosedAt.HasValue));

        var storyCycleTime = new CycleTimeValue(
            typeof(StoryEntity),
            GetAverageThroughputTime(storyThroughputTimeAsTicks),
            GetMinThroughputTime(storyThroughputTimeAsTicks),
            GetMaxThroughputTime(storyThroughputTimeAsTicks));

        var bugCycleTime = new CycleTimeValue(
            typeof(BugEntity),
            GetAverageThroughputTime(bugThroughputTimeAsTicks),
            GetMinThroughputTime(bugThroughputTimeAsTicks),
            GetMaxThroughputTime(bugThroughputTimeAsTicks)
        );

        var otherCycleTime = new CycleTimeValue(
            typeof(ItemEntity),
            GetAverageThroughputTime(otherThroughputTimeAsTicks),
            GetMinThroughputTime(otherThroughputTimeAsTicks),
            GetMaxThroughputTime(otherThroughputTimeAsTicks));
        
        return new CycleTimesValue(
            new []{storyCycleTime, bugCycleTime, otherCycleTime});
    }
    
    private static IEnumerable<XyValue<string, int>> CalculateVelocitySeries(IEnumerable<SprintVelocityValue> sprintVelocityValues)
    {
        var storyPointsPerSprint = sprintVelocityValues
            .Aggregate(new List<XyValue<string, int>>(), (aggregate, sprint) =>
            {
                var averageXy = new XyValue<string, int>
                {
                    X = $"Sprint {aggregate.Count+1}",
                    Y = sprint.TotalStoryPoints
                };
                
                aggregate.Add(averageXy);
                
                return aggregate;
            });

        return storyPointsPerSprint.Count <= 0 ? 
            Enumerable.Empty<XyValue<string, int>>() : 
            storyPointsPerSprint;
    }
    
    private static float CalculateAverageVelocity(IReadOnlyCollection<SprintVelocityValue> sprintVelocityValues)
    {
        var totalStoryPoints = sprintVelocityValues.Sum(sp => sp.TotalStoryPoints);
        var averageStoryPoints = (float) totalStoryPoints / Math.Max(1,sprintVelocityValues.Count);
        return averageStoryPoints;
    }
    
    private static float CalculateDayAverageVelocity(IReadOnlyCollection<SprintVelocityValue> sprintVelocityValues)
    {
        var totalStoryPoints = sprintVelocityValues.Sum(sp => sp.TotalStoryPoints);
        var totalDays = sprintVelocityValues.Sum(sp => sp.SprintLengthInDays);
        var averageStoryPoints = (float) totalStoryPoints / Math.Max(1,totalDays);
        return averageStoryPoints;
    }

    private static float CalculateAverageSprintLength(IReadOnlyCollection<SprintVelocityValue> sprintVelocityValues)
    {
        var totalLengthOfSprints = sprintVelocityValues.Sum(sp => sp.SprintLengthInDays);
        var averageSprintLength = (float) totalLengthOfSprints / sprintVelocityValues.Count;
        return averageSprintLength;
    }
    
    private ICollection<XyValue<DateTime, int>> CalculateBurnDownChartSeries(
        IEnumerable<StoryEntity> storyEntities, 
        bool tillToday = true)
    {
        var storyArray = storyEntities?.ToArray() ?? Array.Empty<StoryEntity>();
        
        var opened = storyArray.OrderBy(s => s.CreatedAt)
            .Select(s => new XyValue<DateTime, int>
            {
                X = s.CreatedAt,
                Y = s.StoryPoints ?? 0
            });
        
        var closed = _storySeriesService.CalculateClosedStoryChart(storyArray);

        var burnDown = opened
            .Concat(closed)
            .OrderBy(xy => xy.X)
            .Aggregate(
                new List<XyValue<DateTime, int>>(),
                (xys, xy) =>
                {
                    var previousXy = xys.LastOrDefault();

                    var newXy = new XyValue<DateTime, int>
                    {
                        X = xy.X,
                        Y = xy.Y + (previousXy?.Y ?? 0)
                    };
                    
                    xys.Add(newXy);

                    return xys;
                });

        if (burnDown.Count <= 0) return burnDown;

        if (!tillToday) return burnDown;
        
        var lastBurn = burnDown.LastOrDefault();
        var current = new XyValue<DateTime, int>
        {
            X = DateTime.UtcNow,
            Y = lastBurn?.Y ?? 0
        };
        burnDown = burnDown.Append(current).ToList();

        return burnDown;
    }

    private IEnumerable<XyValue<DateTime, int>> CalculateBurnDownEstimationChartSeries(
        IEnumerable<XyValue<DateTime, int>> burnDownSeries, 
        float velocityPerDay)
    {
        var lastBurnDown = burnDownSeries.LastOrDefault();
        
        if(lastBurnDown == null) return Enumerable.Empty<XyValue<DateTime, int>>();

        var remainingStoryPoints = lastBurnDown.Y;
        
        // NOTE, if the velocity is zero a velocity of 0.1 (equals 1 SP per a two weeks sprint)
        // story point per day is assumed.
        velocityPerDay = velocityPerDay <= 0 ? 0.1f : velocityPerDay;

        var daysToGo = remainingStoryPoints / velocityPerDay;

        _logger.LogDebug(
            "{DaysToGo} days to go to complete {StoryPoints} story points by a velocity of {VelocityPerDay} per day", 
            daysToGo, 
            remainingStoryPoints,
            velocityPerDay);

        // TODO: 20220212 CJ: Consider hours of a work day!
        var dueDate = lastBurnDown.X.GetBusinessDueDate(daysToGo);
        
        var estimatedXy = new XyValue<DateTime, int>
        {
            X = dueDate,
            Y = 0
        };

        return new[]
        {
            lastBurnDown,
            estimatedXy
        };
    }
    
    private static ICollection<long> GetThroughputTimesAsTicks(IEnumerable<ItemEntity> items)
    {
        var throughputTimesAsTicks = items
            .Select(i => (i.ClosedAt!.Value - i.CreatedAt).Ticks);

        return throughputTimesAsTicks.ToList();
    }

    private static TimeSpan GetAverageThroughputTime(ICollection<long> ticks)
    {
        var totalThroughputTimeAsTicks = ticks.Sum();
        var averageThroughputTimeAsTicks = totalThroughputTimeAsTicks / ticks.Count;
        return TimeSpan.FromTicks(averageThroughputTimeAsTicks);
    }

    private static TimeSpan GetMaxThroughputTime(IEnumerable<long> ticks)
    {
        var throughputTimesAsTicks = ticks.Max();
        return TimeSpan.FromTicks(throughputTimesAsTicks);
    }
    
    private static TimeSpan GetMinThroughputTime(IEnumerable<long> ticks)
    {
        var throughputTimesAsTicks = ticks.Min();
        return TimeSpan.FromTicks(throughputTimesAsTicks);
    }
}