using ArrangeContext.NSubstitute;
using FluentAssertions;
using Io.Juenger.Common.Util;
using Io.Juenger.Scrum.GitLab.Contracts.Entities;
using Io.Juenger.Scrum.GitLab.Contracts.Values;
using Io.Juenger.Scrum.GitLab.Services.Application;
using Io.Juenger.Scrum.GitLab.Services.Domain;
using Io.Juenger.Scrum.GitLab.Values;

namespace Io.Juenger.Scrum.GitLab.Tests.Services;

[TestFixture]
public class MetricServiceTests
{
    private readonly SprintVelocityValue[] _sprintVelocityValues = {
        new(20, 10),
        new(15, 10),
        new(5, 5),
        new(30, 10),
        new(40, 15),
        new(25, 20),
    };
    
    [Test]
    public void Should_CalculateVelocity()
    {
        var context = new ArrangeContext<MetricsService>();
        var sut = context.Build();
        
        var velocityValue = sut.CalculateVelocity(_sprintVelocityValues);

        velocityValue
            .AverageVelocity
            .Should()
            .Be(22.5f);

        velocityValue
            .DayAverageVelocity
            .Should()
            .BeApproximately(1.92f, 0.1f);

        velocityValue
            .Best3SprintsAverageVelocity
            .Should()
            .BeApproximately(31.66f, 0.01f);

        velocityValue
            .Best3SprintsDayAverageVelocity
            .Should()
            .BeApproximately(2.11f,0.01f);
        
        velocityValue
            .Worst3SprintsAverageVelocity
            .Should()
            .BeApproximately(13.33f, 0.01f);

        velocityValue
            .Worst3SprintsDayAverageVelocity
            .Should()
            .Be(1.6f);
        
        velocityValue
            .CountOfSprints
            .Should()
            .Be(_sprintVelocityValues.Length);
        
        velocityValue
            .AverageSprintLength
            .Should()
            .BeApproximately(11.66f, 0.01f);
    }

    [Test]
    public void Should_CalculateVelocityTrend()
    {
        var context = new ArrangeContext<MetricsService>();
        var sut = context.Build();

        var velocityTrendValue = sut.CalculateVelocityTrend(_sprintVelocityValues);

        var velocityTrendSeries = new[]
        {
            new XyValue<string, int> { X="1", Y=20 },
            new XyValue<string, int> { X="2", Y=15 },
            new XyValue<string, int> { X="3", Y=5 },
            new XyValue<string, int> { X="4", Y=30 },
            new XyValue<string, int> { X="5", Y=40 },
            new XyValue<string, int> { X="6", Y=25 }
        };

        velocityTrendValue.VelocitySeries.Should().BeEquivalentTo(velocityTrendSeries);
    }

    [TestCase(15,10,5)]
    public void Should_CalculateComposition(int countOfStories, int countOfBugs, int countOfOthers)
    {
        var context = new ArrangeContext<MetricsService>();
        var sut = context.Build();

        var itemEntities = CreateEmptyItemEntities(countOfStories, countOfBugs, countOfOthers);
        
        var compositionValue = sut.CalculateComposition(itemEntities.ToList());

        compositionValue.CountOfStories.Should().Be(countOfStories);
        compositionValue.CountOfBugs.Should().Be(countOfBugs);
        compositionValue.CountOfOthers.Should().Be(countOfOthers);
    }

    [Test]
    public void Should_CalculateCompositionTrend()
    {
        var context = new ArrangeContext<MetricsService>();
        var sut = context.Build();

        var expectedCompositions = new (int CountOfStories, int CountOfBugs, int CountOfOthers)[]
        {
            (10, 0, 2),
            (5, 5, 0),
            (10, 5, 3),
            (8, 7, 2),
            (6, 9, 1)
        };

        var itemsPerSprint = new List<(string, IEnumerable<ItemEntity>)>();
        var index = 1;
        foreach (var expectedComposition in expectedCompositions)
        {
            itemsPerSprint.Add(
                ($"Sprint {index}", 
                CreateEmptyItemEntities(
                    expectedComposition.CountOfStories,
                    expectedComposition.CountOfBugs,
                    expectedComposition.CountOfOthers)));
            index++;
        }

        var compositionTrendValue = sut.CalculateCompositionTrend(itemsPerSprint);

        compositionTrendValue.CompositionSeries.Should().HaveCount(expectedCompositions.Length);
        
        for(index = 0; index < expectedCompositions.Length; index++)
        {
            compositionTrendValue.CompositionSeries
                .ElementAt(index).Y.CountOfStories
                .Should()
                .Be(expectedCompositions[index].CountOfStories);
            
            compositionTrendValue.CompositionSeries
                .ElementAt(index).Y.CountOfBugs
                .Should()
                .Be(expectedCompositions[index].CountOfBugs);
            
            compositionTrendValue.CompositionSeries
                .ElementAt(index).Y.CountOfOthers
                .Should()
                .Be(expectedCompositions[index].CountOfOthers);            
        }
    }
    
    [Test]
    public void Should_CalculateBurnDown()
    {
        var context = new ArrangeContext<MetricsService>();
        context.Use<IStorySeriesService>(new StorySeriesService());
        var sut = context.Build();

        var start = DateTime.UtcNow - TimeSpan.FromDays(10);
        var story1 = new StoryEntity { CreatedAt = start, ClosedAt = start + TimeSpan.FromDays(2), StoryPoints = 5 };
        var story2 = new StoryEntity { CreatedAt = start + TimeSpan.FromDays(1), StoryPoints = 3 };
        var story3 = new StoryEntity { CreatedAt = start + TimeSpan.FromDays(3), StoryPoints = 20 };
        var story4 = new StoryEntity { CreatedAt = start + TimeSpan.FromDays(4), StoryPoints = 1 };
        var story5 = new StoryEntity { CreatedAt = start + TimeSpan.FromDays(5), ClosedAt = start + TimeSpan.FromDays(6), StoryPoints = 13 };
        var story6 = new StoryEntity { CreatedAt = start + TimeSpan.FromDays(7), StoryPoints = 8 };
        var stories = new[] {story1, story2, story3, story4, story5, story6};
        
        var velocity = new VelocityValue(
            15f,
            1f,
            1f,
            15f,
            1.5f,
            5f,
            0.5f,
            3,
            10f);

        var now = DateTime.UtcNow;
        var burnDownValue = sut.CalculateBurnDown(stories, velocity);

        const int expectedBurnDownSeriesItemsCount = 9; // 6 created stories + 2 closed stories + 1 for today
        burnDownValue
            .BurnDownSeries
            .Should()
            .HaveCount(expectedBurnDownSeriesItemsCount);

        var expectedBurnDownSeries = new[]
        {
            new XyValue<DateTime, int> { X = story1.CreatedAt, Y = story1.StoryPoints.Value },
            new XyValue<DateTime, int> { X = story2.CreatedAt, Y = story1.StoryPoints.Value + story2.StoryPoints.Value },
            new XyValue<DateTime, int> { X = story1.ClosedAt.Value, Y = story2.StoryPoints.Value },
            new XyValue<DateTime, int> { X = story3.CreatedAt, Y = story2.StoryPoints.Value + story3.StoryPoints.Value },
            new XyValue<DateTime, int> { X = story4.CreatedAt, Y = story2.StoryPoints.Value + story3.StoryPoints.Value + story4.StoryPoints.Value },
            new XyValue<DateTime, int> { X = story5.CreatedAt, Y = story2.StoryPoints.Value + story3.StoryPoints.Value + story4.StoryPoints.Value + story5.StoryPoints.Value },
            new XyValue<DateTime, int> { X = story5.ClosedAt.Value, Y = story2.StoryPoints.Value + story3.StoryPoints.Value + story4.StoryPoints.Value },
            new XyValue<DateTime, int> { X = story6.CreatedAt, Y = story2.StoryPoints.Value + story3.StoryPoints.Value + story4.StoryPoints.Value + story6.StoryPoints.Value },
        };
        burnDownValue
            .BurnDownSeries
            .Take(expectedBurnDownSeries.Length)
            .Should()
            .BeEquivalentTo(expectedBurnDownSeries);

        burnDownValue
            .BurnDownSeries
            .Last()
            .X
            .Should()
            .BeCloseTo(now, TimeSpan.FromMilliseconds(50));
        burnDownValue
            .BurnDownSeries
            .Last()
            .Y
            .Should()
            .Be(expectedBurnDownSeries[^1].Y);
        
        var last = burnDownValue
            .BurnDownSeries
            .Last();
        
        burnDownValue.EstimateSeries.Should().HaveCount(2);
        burnDownValue.EstimateSeries.First().Y.Should().Be(last.Y);
        burnDownValue.EstimateSeries.First().X.Should().Be(last.X);
        burnDownValue.EstimateSeries.Last().Y.Should().Be(0);
        var daysRemaining = burnDownValue.EstimateSeries.First().Y / velocity.Last5SprintsAverageVelocity;
        var estimatedDay = burnDownValue.EstimateSeries.First().X.GetBusinessDueDate(daysRemaining);
        burnDownValue.EstimateSeries.Last().X.Should().Be(estimatedDay);
        
        burnDownValue.BestEstimateSeries.Should().HaveCount(2);
        burnDownValue.BestEstimateSeries.First().Y.Should().Be(last.Y);
        burnDownValue.BestEstimateSeries.First().X.Should().Be(last.X);
        burnDownValue.BestEstimateSeries.Last().Y.Should().Be(0);
        daysRemaining = burnDownValue.BestEstimateSeries.First().Y / velocity.Best3SprintsDayAverageVelocity;
        estimatedDay = burnDownValue.BestEstimateSeries.First().X.GetBusinessDueDate(daysRemaining);
        burnDownValue.BestEstimateSeries.Last().X.Should().Be(estimatedDay);
        
        burnDownValue.WorstEstimateSeries.Should().HaveCount(2);
        burnDownValue.WorstEstimateSeries.First().Y.Should().Be(last.Y);
        burnDownValue.WorstEstimateSeries.First().X.Should().Be(last.X);
        burnDownValue.WorstEstimateSeries.Last().Y.Should().Be(0);
        daysRemaining = burnDownValue.WorstEstimateSeries.First().Y / velocity.Worst3SprintsDayAverageVelocity;
        estimatedDay = burnDownValue.WorstEstimateSeries.First().X.GetBusinessDueDate(daysRemaining);
        burnDownValue.WorstEstimateSeries.Last().X.Should().Be(estimatedDay);
    }

    [Test]
    public void Should_CalculateBurnUp()
    {
        var context = new ArrangeContext<MetricsService>();
        context.Use<IStorySeriesService>(new StorySeriesService());
        var sut = context.Build();
        
        var start = DateTime.UtcNow - TimeSpan.FromDays(10);
        var story1 = new StoryEntity { CreatedAt = start, ClosedAt = start + TimeSpan.FromDays(2), StoryPoints = 5 };
        var story2 = new StoryEntity { CreatedAt = start + TimeSpan.FromDays(1), StoryPoints = 3 };
        var story3 = new StoryEntity { CreatedAt = start + TimeSpan.FromDays(3), StoryPoints = 20 };
        var story4 = new StoryEntity { CreatedAt = start + TimeSpan.FromDays(4), StoryPoints = 1 };
        var story5 = new StoryEntity { CreatedAt = start + TimeSpan.FromDays(5), ClosedAt = start + TimeSpan.FromDays(6), StoryPoints = 13 };
        var story6 = new StoryEntity { CreatedAt = start + TimeSpan.FromDays(7), StoryPoints = 8 };
        var stories = new[] {story1, story2, story3, story4, story5, story6};

        var burnUp = sut.CalculateBurnUp(stories);

        burnUp.TotalSeries.Should().HaveCount(stories.Length + 1);
        for (var index = 0; index < stories.Length; index++)
        {
            var xyValue = burnUp.TotalSeries.ElementAt(index);
            xyValue.X.Should().Be(stories[index].CreatedAt);
            var cumulatedStoryPoints = stories.Take(index + 1).Sum(s => s.StoryPoints);
            xyValue.Y.Should().Be(cumulatedStoryPoints);
        }

        var closedStories = stories.Where(s => s.ClosedAt.HasValue).ToList();
        burnUp.CompleteSeries.Should().HaveCount(closedStories.Count + 1);
        for (var index = 0; index < closedStories.Count; index++)
        {
            var xyValue = burnUp.CompleteSeries.ElementAt(index);
            xyValue.X.Should().Be(closedStories[index].ClosedAt);
            var cumulatedStoryPoints = closedStories.Take(index + 1).Sum(s => s.StoryPoints);
            xyValue.Y.Should().Be(cumulatedStoryPoints);
        }
    }

    [Test]
    public void Should_CalculateCycleTime()
    {
        var context = new ArrangeContext<MetricsService>();
        var sut = context.Build();
        
        var start = DateTime.UtcNow - TimeSpan.FromDays(10);
        var story1 = new StoryEntity { CreatedAt = start, ClosedAt = start + TimeSpan.FromDays(2) };
        var story2 = new StoryEntity { CreatedAt = start + TimeSpan.FromDays(1) };
        var story3 = new StoryEntity { CreatedAt = start + TimeSpan.FromDays(1), ClosedAt = start + TimeSpan.FromDays(4) };
         
        var bug1 = new BugEntity { CreatedAt = start, ClosedAt = start + TimeSpan.FromDays(2) };
        var bug2 = new BugEntity { CreatedAt = start + TimeSpan.FromDays(1) };
        var bug3 = new BugEntity { CreatedAt = start + TimeSpan.FromDays(1), ClosedAt = start + TimeSpan.FromDays(4) };
        
        var item1 = new ItemEntity { CreatedAt = start, ClosedAt = start + TimeSpan.FromDays(2) };
        var item2 = new ItemEntity { CreatedAt = start + TimeSpan.FromDays(1) };
        var item3 = new ItemEntity { CreatedAt = start + TimeSpan.FromDays(1), ClosedAt = start + TimeSpan.FromDays(4) };

        var items = new[] { story1, story2, story3, bug1, bug2, bug3, item1, item2, item3 };

        var cycleTimesValue = sut.CalculateCycleTime(items);

        var storyCycleTime = cycleTimesValue.CycleTimes.First(ct => ct.Type == typeof(StoryEntity));
        var bugCycleTime = cycleTimesValue.CycleTimes.First(ct => ct.Type == typeof(BugEntity));
        var otherCycleTime = cycleTimesValue.CycleTimes.First(ct => ct.Type == typeof(ItemEntity));

        storyCycleTime.AverageCycleTime.Should().Be(TimeSpan.FromDays(2.5));
        storyCycleTime.BestCycleTime.Should().Be(TimeSpan.FromDays(2.0));
        storyCycleTime.WorstCycleTime.Should().Be(TimeSpan.FromDays(3.0));
        
        bugCycleTime.AverageCycleTime.Should().Be(TimeSpan.FromDays(2.5));
        bugCycleTime.BestCycleTime.Should().Be(TimeSpan.FromDays(2.0));
        bugCycleTime.WorstCycleTime.Should().Be(TimeSpan.FromDays(3.0));
        
        otherCycleTime.AverageCycleTime.Should().Be(TimeSpan.FromDays(2.5));
        otherCycleTime.BestCycleTime.Should().Be(TimeSpan.FromDays(2.0));
        otherCycleTime.WorstCycleTime.Should().Be(TimeSpan.FromDays(3.0));
    }
    
    private static IEnumerable<ItemEntity> CreateEmptyItemEntities(int countOfStories, int countOfBugs, int countOfOthers)
    {
        var stories = Enumerable.Range(0, countOfStories).Select(_ => new StoryEntity());
        var bugs = Enumerable.Range(0, countOfBugs).Select(_ => new BugEntity());
        var others = Enumerable.Range(0, countOfOthers).Select(_ => new ItemEntity());

        var itemEntities = new List<ItemEntity>();
        itemEntities.AddRange(stories);
        itemEntities.AddRange(bugs);
        itemEntities.AddRange(others);

        return itemEntities;
    }
}