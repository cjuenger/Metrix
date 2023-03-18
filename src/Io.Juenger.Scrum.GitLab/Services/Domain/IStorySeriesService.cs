using System;
using System.Collections.Generic;
using Io.Juenger.Scrum.GitLab.Contracts.Entities;
using Io.Juenger.Scrum.GitLab.Contracts.Values;

namespace Io.Juenger.Scrum.GitLab.Services.Domain
{
    internal interface IStorySeriesService
    {
        IEnumerable<XyValue<DateTime, int>> CalculateOpenedStoryChart(IEnumerable<StoryEntity> stories);
        
        IEnumerable<XyValue<DateTime, int>> CalculateCumulatedOpenedStoryChart(IEnumerable<StoryEntity> stories, bool tillToday = true);
        
        IEnumerable<XyValue<DateTime, int>> CalculateClosedStoryChart(IEnumerable<StoryEntity> stories);
        
        IEnumerable<XyValue<DateTime, int>> CalculateCumulatedClosedStoryChart(IEnumerable<StoryEntity> stories, bool tillToday = true);
    }
}