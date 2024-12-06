using Metrix.Core.Metrics;

namespace Metrix.Core.BacklogItem
{
    internal interface IStorySeriesService
    {
        IEnumerable<XyValue<DateTime, int>> CalculateOpenedStoryChart(IEnumerable<Story> stories);
        
        IEnumerable<XyValue<DateTime, int>> CalculateCumulatedOpenedStoryChart(IEnumerable<Story> stories, bool tillToday = true);
        
        IEnumerable<XyValue<DateTime, int>> CalculateClosedStoryChart(IEnumerable<Story> stories);
        
        IEnumerable<XyValue<DateTime, int>> CalculateCumulatedClosedStoryChart(IEnumerable<Story> stories, bool tillToday = true);
    }
}