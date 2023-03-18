using Io.Juenger.Scrum.GitLab.Contracts.Services;
using Io.Juenger.Scrum.GitLab.Contracts.Values;

namespace io.juenger.Scrum.GitLab.Demo;

public class ProductDemo
{
    private readonly IDemoConfig _demoConfig;
    private readonly IProductAggregateService _productAggregateService;

    public ProductDemo(
        IDemoConfig demoConfig,
        IProductAggregateService productAggregateService)
    {
        _demoConfig = demoConfig ?? throw new ArgumentNullException(nameof(demoConfig));
        _productAggregateService = productAggregateService ?? throw new ArgumentNullException(nameof(productAggregateService));
    }
    
    public async Task ShowProductMetricsAsync()
    {
        var composition = await _productAggregateService.CalculateCompositionAsync(_demoConfig.ProductId);
        Console.WriteLine(
            "Composition: " +
            $"Stories: {composition.CountOfStories}, " +
            $"Bugs: {composition.CountOfBugs}, " +
            $"Others: {composition.CountOfOthers}");
        Console.WriteLine();

        var compositionTrend = await _productAggregateService.CalculateCompositionTrendAsync(_demoConfig.ProductId);
        Console.WriteLine("Composition Trend:");
        WriteCompositionTrendSeries(compositionTrend);
        
        var velocity = await _productAggregateService.CalculateVelocityAsync(_demoConfig.ProductId);
        Console.WriteLine(
            "Velocity: \n" +
            $"Average: {velocity.AverageVelocity}\n" +
            $"Best3SprintsAverageVelocity: {velocity.Best3SprintsAverageVelocity}\n" +
            $"Worst3SprintsAverageVelocity: {velocity.Worst3SprintsAverageVelocity}\n" +
            $"Count of Sprints: {velocity.CountOfSprints}\n");
        Console.WriteLine();

        var velocityTrend = await _productAggregateService.CalculateVelocityTrendAsync(_demoConfig.ProductId);
        Console.WriteLine("Velocity Trend:");
        WriteSeries(velocityTrend.VelocitySeries);

        var burnDown = await _productAggregateService.CalculateBurnDownAsync(_demoConfig.ProductId);
        Console.WriteLine("Burn Down: ");
        WriteSeries(burnDown.BurnDownSeries);
        Console.WriteLine();
        
        Console.WriteLine("Burn Down Estimate: ");
        WriteSeries(burnDown.EstimateSeries);
        Console.WriteLine();
        
        Console.WriteLine("Burn Down Best Estimate: ");
        WriteSeries(burnDown.BestEstimateSeries);
        Console.WriteLine();
        
        Console.WriteLine("Burn Down Worst Estimate: ");
        WriteSeries(burnDown.WorstEstimateSeries);
        Console.WriteLine();
            
        var burnUp = await _productAggregateService.CalculateBurnUpAsync(_demoConfig.ProductId);
        Console.WriteLine("Burn Up: ");
        WriteSeries(burnUp.TotalSeries);
        Console.WriteLine();
            
        var cycleTimeValue = await _productAggregateService.CalculateCycleTimeAsync(_demoConfig.ProductId);
        Console.WriteLine("Cycle Time:");
        foreach (var cycleTime in cycleTimeValue.CycleTimes)
        {
            Console.WriteLine(
                $"{cycleTime.Type.Name} "+
                $"Average: {cycleTime.AverageCycleTime} " +
                $"Best: {cycleTime.BestCycleTime}, " +
                $"Worst: {cycleTime.WorstCycleTime}");
        }
    }

    private static void WriteSeries<TX, TY>(IEnumerable<XyValue<TX, TY>> series)
    {
        foreach (var xyValue in series)
        {
            Console.WriteLine($"{xyValue.X} | {xyValue.Y}");
        }
    }
    
    private static void WriteCompositionTrendSeries(CompositionTrendValue compositionTrendValue)
    {
        foreach (var xyValue in compositionTrendValue.CompositionSeries)
        {
            var totalItems = xyValue.Y.CountOfStories + xyValue.Y.CountOfBugs + xyValue.Y.CountOfOthers;
            var percentageOfStories = totalItems > 0 ? 100 * (float) xyValue.Y.CountOfStories / totalItems : 0;
            var percentageOfBugs = totalItems > 0 ? 100 * (float) xyValue.Y.CountOfBugs / totalItems : 0;
            var percentageOfOthers = totalItems > 0  ?100 * (float) xyValue.Y.CountOfOthers / totalItems : 0;
            
            Console.WriteLine($"{xyValue.X, -30} |" +
                              $" {xyValue.Y.CountOfStories, -3} Stories {percentageOfStories,7:0.00} %, " +
                              $"{xyValue.Y.CountOfBugs, -3} Bugs {percentageOfBugs,7:0.00} %, " +
                              $"{xyValue.Y.CountOfOthers, -3} Others {percentageOfOthers,7:0.00} %");
        }
    }
}