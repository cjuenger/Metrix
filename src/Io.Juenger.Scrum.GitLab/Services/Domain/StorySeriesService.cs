using Io.Juenger.Scrum.GitLab.Contracts.Entities;
using Io.Juenger.Scrum.GitLab.Contracts.Values;

namespace Io.Juenger.Scrum.GitLab.Services.Domain
{
    internal class StorySeriesService : IStorySeriesService
    {
        public IEnumerable<XyValue<DateTime, int>> CalculateOpenedStoryChart(IEnumerable<StoryEntity> stories)
        {
            var graph = stories?
                .OrderBy(s => s.CreatedAt)
                .Select(s => new XyValue<DateTime, int>
                {
                    X = s.CreatedAt,
                    Y = s.StoryPoints ?? 0
                }) ?? new List<XyValue<DateTime, int>>();

            return graph;
        }

        public IEnumerable<XyValue<DateTime, int>> CalculateCumulatedOpenedStoryChart(IEnumerable<StoryEntity> stories, bool tillToday = true)
        {
            var graph = stories?
                .OrderBy(s => s.CreatedAt)
                .Aggregate(
                    new List<XyValue<DateTime, int>>(),
                    (coordinates, s) =>
                    {
                        var previousCoordinate = coordinates.LastOrDefault();

                        var xy = new XyValue<DateTime, int>
                        {
                            X = s.CreatedAt,
                            Y = (s.StoryPoints ?? 0) + (previousCoordinate?.Y ?? 0)
                        };
                        
                        coordinates.Add(xy);
                        
                        return coordinates;
                    }) ?? new List<XyValue<DateTime, int>>();
            
            if(!graph.Any()) return Enumerable.Empty<XyValue<DateTime, int>>();

            if (tillToday)
            {
                graph.Add(new XyValue<DateTime, int> {X = DateTime.Now, Y = graph.Last().Y});    
            }
            
            return graph;
        }

        public IEnumerable<XyValue<DateTime, int>> CalculateClosedStoryChart(IEnumerable<StoryEntity> stories)
        {
            var graph = stories?
                .Where(s => s.ClosedAt != null)
                .OrderBy(s => s.ClosedAt)
                .Select(s => new XyValue<DateTime, int>
                {
                    X = s.ClosedAt!.Value,
                    Y = -(s.StoryPoints ?? 0)
                }) ?? Enumerable.Empty<XyValue<DateTime, int>>();

            return graph;
        }

        public IEnumerable<XyValue<DateTime, int>> CalculateCumulatedClosedStoryChart(IEnumerable<StoryEntity> stories, bool tillToday = true)
        {
            var graph = stories?
                .Where(s => s.ClosedAt != null)
                .OrderBy(s => s.ClosedAt)
                .Aggregate(
                    new List<XyValue<DateTime, int>>(),
                    (coordinates, s) =>
                    {
                        var previousCoordinate = coordinates.LastOrDefault();

                        var xy = new XyValue<DateTime, int>
                        {
                            X = s.ClosedAt!.Value,
                            Y = (s.StoryPoints ?? 0) + (previousCoordinate?.Y ?? 0)
                        };
                        
                        coordinates.Add(xy);
                        
                        return coordinates;
                    }).ToList() ?? new List<XyValue<DateTime, int>>();
            
            if(!graph.Any()) return Enumerable.Empty<XyValue<DateTime, int>>();

            if (tillToday)
            {
                graph.Add(new XyValue<DateTime, int> {X = DateTime.Now, Y = graph.Last().Y});    
            }

            return graph;
        }
    }
}