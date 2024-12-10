using System.Diagnostics.CodeAnalysis;
using Metrix.Core.BacklogItem;
using Metrix.Core.Metrics.Values;
using Metrix.Core.ProductCalendar;
using Metrix.Core.Sprint;
using Microsoft.Extensions.Logging;
using NMolecules.DDD;

namespace Metrix.Core.Metrics;

[Service]
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

    public Velocity CalculateVelocity(IReadOnlyCollection<SprintVelocity> sprintVelocityValues)
    {
        var countOfSprints = sprintVelocityValues.Count;

        var allSprints = sprintVelocityValues
            .OrderBy(tsp => tsp.TotalStoryPoints)
            .ToList();
        
        var averageSprintLength = CalculateAverageSprintLength(sprintVelocityValues);
        var averageVelocity = CalculateAverageVelocity(allSprints);

        var last5Sprints = sprintVelocityValues.TakeLast(5).ToList();
        var last5SprintsAverageVelocity = CalculateAverageVelocity(last5Sprints);
        var last5SprintsDayAverageVelocity = CalculateDayAverageVelocity(last5Sprints);

        var best3Sprints = allSprints.TakeLast(3).ToList();
        var best3SprintsAverageVelocity = CalculateAverageVelocity(best3Sprints);
        var best3SprintsDayAverageVelocity = CalculateDayAverageVelocity(best3Sprints);
        
        var worst3Sprints = allSprints.Take(3).ToList();
        var worst3SprintsAverageVelocity = CalculateAverageVelocity(worst3Sprints);
        var worst3SprintsDayAverageVelocity = CalculateDayAverageVelocity(worst3Sprints);
        
        return new Velocity(
            averageVelocity,
            last5SprintsAverageVelocity,
            last5SprintsDayAverageVelocity,
            best3SprintsAverageVelocity,
            best3SprintsDayAverageVelocity,
            worst3SprintsAverageVelocity,
            worst3SprintsDayAverageVelocity,
            countOfSprints,
            averageSprintLength);
    }
    
    public VelocityTrend CalculateVelocityTrend(IReadOnlyCollection<SprintVelocity> sprintVelocityValues)
    {
        var velocitySeries = CalculateVelocitySeries(sprintVelocityValues);
        return new VelocityTrend(velocitySeries);
    }
    
    public Composition CalculateComposition(IReadOnlyCollection<BacklogItem.BacklogItem> itemEntities)
    {
        return new Composition(itemEntities);
    }

    public CompositionTrend CalculateCompositionTrend(
        IEnumerable<(string Name, IEnumerable<BacklogItem.BacklogItem> ItemEntities)> itemEntityGroups)
    {
        var compositionTrendSeries = new List<XyValue<string, Composition>>();
        
        // ReSharper disable once LoopCanBeConvertedToQuery
        foreach (var itemEntityGroup in itemEntityGroups)
        {
            var compositionValue = new Composition(itemEntityGroup.ItemEntities);
            compositionTrendSeries.Add(new XyValue<string, Composition>
            {
                X = $"{itemEntityGroup.Name}", 
                Y = compositionValue
            });
        }

        return new CompositionTrend(compositionTrendSeries);
    }

    public BurnDown CalculateBurnDown(
        IEnumerable<BacklogItem.BacklogItem> itemEntities,
        Velocity velocity,
        ProductCalendar.ProductCalendar productCalendar)
    {
        var stories = itemEntities.OfType<Story>().ToList();
            
        var burnDownSeries = CalculateBurnDownChartSeries(stories);
            
        var estimateSeries = CalculateBurnDownEstimationChartSeries(
            burnDownSeries, 
            velocity.Last5SprintsDayAverageVelocity,
            productCalendar);
            
        var bestEstimateSeries = CalculateBurnDownEstimationChartSeries(
            burnDownSeries, 
            velocity.Best3SprintsDayAverageVelocity,
            productCalendar);
            
        var worstEstimateSeries = CalculateBurnDownEstimationChartSeries(
            burnDownSeries, 
            velocity.Worst3SprintsDayAverageVelocity,
            productCalendar);
        
        var burnDownValue = new BurnDown(
            burnDownSeries, 
            estimateSeries, 
            bestEstimateSeries, 
            worstEstimateSeries); 
        
        return burnDownValue;
    }

    public BurnUp CalculateBurnUp(IEnumerable<BacklogItem.BacklogItem> itemEntities)
    {
        var stories = itemEntities.OfType<Story>();
        var storyArray = stories.ToArray();
        
        var totalStoryPointsSeries = _storySeriesService
            .CalculateCumulatedOpenedStoryChart(storyArray);
        
        var completedStoryPointsSeries = _storySeriesService
            .CalculateCumulatedClosedStoryChart(storyArray);
        
        return new BurnUp(totalStoryPointsSeries, completedStoryPointsSeries);
    }

    public CycleTimesValue CalculateCycleTime(IReadOnlyCollection<BacklogItem.BacklogItem> itemEntities)
    {
        var stories = itemEntities.OfType<Story>().ToList();
        var bugs = itemEntities.OfType<Bug>().ToList();
        var others = itemEntities.Except(stories).Except(bugs).ToList();

        var storyThroughputTimeAsTicks = GetThroughputTimesAsTicks(
            stories.Where(i => i.ClosedAt.HasValue));
        var bugThroughputTimeAsTicks = GetThroughputTimesAsTicks(
            bugs.Where(i => i.ClosedAt.HasValue));
        var otherThroughputTimeAsTicks = GetThroughputTimesAsTicks(
            others.Where(i => i.ClosedAt.HasValue));

        var storyCycleTime = new CycleTime(
            typeof(Story),
            GetAverageThroughputTime(storyThroughputTimeAsTicks),
            GetMinThroughputTime(storyThroughputTimeAsTicks),
            GetMaxThroughputTime(storyThroughputTimeAsTicks));

        var bugCycleTime = new CycleTime(
            typeof(Bug),
            GetAverageThroughputTime(bugThroughputTimeAsTicks),
            GetMinThroughputTime(bugThroughputTimeAsTicks),
            GetMaxThroughputTime(bugThroughputTimeAsTicks)
        );

        var otherCycleTime = new CycleTime(
            typeof(BacklogItem.BacklogItem),
            GetAverageThroughputTime(otherThroughputTimeAsTicks),
            GetMinThroughputTime(otherThroughputTimeAsTicks),
            GetMaxThroughputTime(otherThroughputTimeAsTicks));
        
        return new CycleTimesValue(
            new []{storyCycleTime, bugCycleTime, otherCycleTime});
    }
    
    private static IEnumerable<XyValue<string, int>> CalculateVelocitySeries(IEnumerable<SprintVelocity> sprintVelocityValues)
    {
        var storyPointsPerSprint = sprintVelocityValues
            .Aggregate(new List<XyValue<string, int>>(), (aggregate, sprint) =>
            {
                var averageXy = new XyValue<string, int>
                {
                    X = $"{aggregate.Count+1}",
                    Y = sprint.TotalStoryPoints
                };
                
                aggregate.Add(averageXy);
                
                return aggregate;
            });

        return storyPointsPerSprint.Count <= 0 ? 
            Enumerable.Empty<XyValue<string, int>>() : 
            storyPointsPerSprint;
    }
    
    private static float CalculateAverageVelocity(IReadOnlyCollection<SprintVelocity> sprintVelocityValues)
    {
        var totalStoryPoints = sprintVelocityValues.Sum(sp => sp.TotalStoryPoints);
        var averageStoryPoints = (float) totalStoryPoints / Math.Max(1,sprintVelocityValues.Count);
        return averageStoryPoints;
    }
    
    private static float CalculateDayAverageVelocity(IReadOnlyCollection<SprintVelocity> sprintVelocityValues)
    {
        var totalStoryPoints = sprintVelocityValues.Sum(sp => sp.TotalStoryPoints);
        var totalDays = sprintVelocityValues.Sum(sp => sp.SprintLengthInDays);
        var averageStoryPoints = (float) totalStoryPoints / Math.Max(1,totalDays);
        return averageStoryPoints;
    }

    private static float CalculateAverageSprintLength(IReadOnlyCollection<SprintVelocity> sprintVelocityValues)
    {
        var totalLengthOfSprints = sprintVelocityValues.Sum(sp => sp.SprintLengthInDays);
        var averageSprintLength = (float) totalLengthOfSprints / sprintVelocityValues.Count;
        return averageSprintLength;
    }
    
    private ICollection<XyValue<DateTime, int>> CalculateBurnDownChartSeries(
        IEnumerable<Story> storyEntities, 
        bool tillToday = true)
    {
        var storyArray = storyEntities?.ToArray() ?? Array.Empty<Story>();
        
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
        float velocityPerDay,
        ProductCalendar.ProductCalendar productCalendar)
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
        var dueDate = productCalendar.PredictDueDate(new BusinessDay(lastBurnDown.X), TimeSpan.FromDays(daysToGo));
        
        var estimatedXy = new XyValue<DateTime, int>
        {
            X = dueDate.Date,
            Y = 0
        };

        return new[]
        {
            lastBurnDown,
            estimatedXy
        };
    }
    
    private static ICollection<long> GetThroughputTimesAsTicks(IEnumerable<BacklogItem.BacklogItem> items)
    {
        var throughputTimesAsTicks = items
            .Select(i => (i.ClosedAt!.Value - i.CreatedAt).Ticks);

        return throughputTimesAsTicks.ToList();
    }

    private static TimeSpan GetAverageThroughputTime(ICollection<long> ticks)
    {
        var totalThroughputTimeAsTicks = ticks.Sum();
        var averageThroughputTimeAsTicks = ticks.Count > 0 ? totalThroughputTimeAsTicks / ticks.Count : 0;
        return TimeSpan.FromTicks(averageThroughputTimeAsTicks);
    }

    [SuppressMessage("ReSharper", "PossibleMultipleEnumeration")]
    private static TimeSpan GetMaxThroughputTime(IEnumerable<long> ticks)
    {
        var throughputTimesAsTicks = ticks.Any() ? ticks.Max() : 0;
        return TimeSpan.FromTicks(throughputTimesAsTicks);
    }
    
    [SuppressMessage("ReSharper", "PossibleMultipleEnumeration")]
    private static TimeSpan GetMinThroughputTime(IEnumerable<long> ticks)
    {
        var throughputTimesAsTicks = ticks.Any() ? ticks.Min() : 0;
        return TimeSpan.FromTicks(throughputTimesAsTicks);
    }
}