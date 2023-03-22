using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Io.Juenger.GitLabClient.Model;
using Io.Juenger.Scrum.GitLab.Configs;
using Io.Juenger.Scrum.GitLab.Contracts.Entities;
using Io.Juenger.Scrum.GitLab.Contracts.Values;

namespace Io.Juenger.Scrum.GitLab.Services.Domain
{
    internal class ItemParserService : IItemParserService
    {
        private readonly IItemParserConfig _config;
        private readonly IDictionary<string, WorkflowState> _stateMap;

        public ItemParserService(IItemParserConfig config)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));

            _stateMap = _config.LabelToWorkflowMapping;
        }
        
        public ItemEntity Parse(Issue issue)
        {
            ItemEntity item;

            var state = GetItemState(issue);
            
            if (IsStory(issue))
            {
                var hasStoryPoints = TryGetStoryPoints(issue, out var storyPoints);

                item = new StoryEntity
                {   
                    Title = issue.Title,
                    StoryPoints = hasStoryPoints ? storyPoints : null,
                    Description = issue.Description,
                    CreatedAt = issue.CreatedAt,
                    ClosedAt = issue.ClosedAt,
                    UpdatedAt = issue.UpdatedAt,
                    State = state
                };
            }
            else if (IsBug(issue))
            {
                item = new BugEntity
                {
                    Title = issue.Title,
                    Description = issue.Description,
                    CreatedAt = issue.CreatedAt,
                    ClosedAt = issue.ClosedAt,
                    UpdatedAt = issue.UpdatedAt,
                    State = state
                };
            } 
            else
            {
                item = new ItemEntity
                {
                    Title = issue.Title,
                    Description = issue.Description,
                    CreatedAt = issue.CreatedAt,
                    ClosedAt = issue.ClosedAt,
                    UpdatedAt = issue.UpdatedAt,
                    State = state
                };
            }

            return item;
        }

        private bool TryGetStoryPoints(Issue issue, out int storyPoints)
        {
            storyPoints = 0;
            var storyPointLabel = issue.Labels.FirstOrDefault(IsStoryPointLabel);
            if (storyPointLabel == null) return false;
            
            storyPoints = GetStoryPoints(storyPointLabel);
            return true;
        }

        private bool IsStory(Issue issue) => issue.Labels.Contains(_config.StoryLabel);

        private bool IsBug(Issue issue) => issue.Labels.Contains(_config.BugLabel);
        
        private bool IsStoryPointLabel(string label) 
        {
            var rgx = new Regex(_config.StoryPointPattern);
            var match = rgx.Match(label);
            return match.Success;
        }
        
        private int GetStoryPoints(string label)
        {
            var split = label.Split(_config.StoryPointSplitter);
            return Convert.ToInt32(split[0]);
        }

        private WorkflowState GetItemState(Issue issue)
        {
            var state = WorkflowState.Opened;

            if (issue.ClosedAt != null)
            {
                state = WorkflowState.Closed;
                return state;
            } 
            
            var label = issue.Labels
                .Intersect(_stateMap.Keys)
                .FirstOrDefault();

            return label == null ? state : _stateMap[label];
        }
    }
}