using System.Text.RegularExpressions;
using Io.Juenger.GitLabClient.Model;
using Io.Juenger.Scrum.GitLab.Configs;
using Metrix.Core.BacklogItem;
using Metrix.Core.Values;
using Metrix.Core.Workflow;

namespace Io.Juenger.Scrum.GitLab.Services.Domain
{
    internal class ItemParserService : IItemParserService
    {
        private readonly IItemParserConfig _config;

        public ItemParserService(IItemParserConfig config)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
        }
        
        public BacklogItem Parse(Issue issue)
        {
            BacklogItem backlogItem;

            var state = GetItemState(issue);
            
            if (IsStory(issue))
            {
                var hasStoryPoints = TryGetStoryPoints(issue, out var storyPoints);

                backlogItem = new Story
                {   
                    Title = issue.Title,
                    StoryPoints = hasStoryPoints ? storyPoints : null,
                    Description = issue.Description,
                    CreatedAt = issue.CreatedAt,
                    ClosedAt = issue.ClosedAt,
                    UpdatedAt = issue.UpdatedAt,
                    WorkflowState = state
                };
            }
            else if (IsBug(issue))
            {
                backlogItem = new Bug
                {
                    Title = issue.Title,
                    Description = issue.Description,
                    CreatedAt = issue.CreatedAt,
                    ClosedAt = issue.ClosedAt,
                    UpdatedAt = issue.UpdatedAt,
                    WorkflowState = state
                };
            } 
            else
            {
                backlogItem = new BacklogItem
                {
                    Title = issue.Title,
                    Description = issue.Description,
                    CreatedAt = issue.CreatedAt,
                    ClosedAt = issue.ClosedAt,
                    UpdatedAt = issue.UpdatedAt,
                    WorkflowState = state
                };
            }

            return backlogItem;
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
            var state =  _config.WorkflowMapping.Values.First();

            if (issue.ClosedAt != null)
            {
                state = _config.WorkflowMapping.Values.Last();
                return new WorkflowState(state);
            } 
            
            var workflowState = issue.Labels
                .Intersect(_config.WorkflowMapping.Keys)
                .FirstOrDefault();

            return workflowState != null ? new WorkflowState(workflowState) : new WorkflowState(state);
        }
    }
}