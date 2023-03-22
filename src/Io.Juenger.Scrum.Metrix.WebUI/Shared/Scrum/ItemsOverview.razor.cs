using Io.Juenger.Scrum.GitLab.Contracts.Entities;
using Microsoft.AspNetCore.Components;

// ReSharper disable once IdentifierTypo
namespace Io.Juenger.Scrum.Metrix.WebUI.Shared.Scrum
{
    public partial class ItemsOverview
    {
        [Parameter]
        public string Title { get; set; }

        private int _countOfStories;

        private int _countOfBugs;

        private int _countOfOthers;

        private int _totalCount;
        
        [Parameter]
        public IEnumerable<ItemEntity> Items { get; set; }
        
        protected override void OnParametersSet()
        {
            base.OnParametersSet();

            if(Items == null) return;
            
            _countOfStories = Items.OfType<StoryEntity>().Count();
            _countOfBugs = Items.OfType<BugEntity>().Count();
            _countOfOthers = Items.Count() - _countOfStories - _countOfBugs;
            _totalCount = _countOfStories + _countOfBugs + _countOfOthers;
        }
    }
}