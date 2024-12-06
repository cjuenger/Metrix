using Metrix.Core.Entities;
using Metrix.Core.Values;

namespace Metrix.Core.Services
{
    internal interface IStorySeriesService
    {
        IEnumerable<XyValue<DateTime, int>> CalculateOpenedStoryChart(IEnumerable<StoryEntity> stories);
        
        IEnumerable<XyValue<DateTime, int>> CalculateCumulatedOpenedStoryChart(IEnumerable<StoryEntity> stories, bool tillToday = true);
        
        IEnumerable<XyValue<DateTime, int>> CalculateClosedStoryChart(IEnumerable<StoryEntity> stories);
        
        IEnumerable<XyValue<DateTime, int>> CalculateCumulatedClosedStoryChart(IEnumerable<StoryEntity> stories, bool tillToday = true);
    }
}