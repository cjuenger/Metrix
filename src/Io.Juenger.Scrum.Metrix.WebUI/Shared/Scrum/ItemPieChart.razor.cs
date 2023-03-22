using Io.Juenger.Scrum.GitLab.Contracts.Entities;
using Microsoft.AspNetCore.Components;

namespace Io.Juenger.Scrum.Metrix.WebUI.Shared.Scrum
{
    public partial class ItemPieChart
    {
        private ItemCategories[] _itemCategories;
        
        [Parameter]
        public string Title { get; set; }
        
        [Parameter] 
        public IEnumerable<ItemEntity> Items { get; set; }
        
        protected override void OnParametersSet(){
            
            base.OnParametersSetAsync();
            
            if(Items == null) return;

            var countOfTotalItems = Items.Count();
            var countOfStories = Items.OfType<StoryEntity>().Count();
            var countOfBugs = Items.OfType<BugEntity>().Count();
            var countOfOthers = countOfTotalItems - countOfStories - countOfBugs;
            
            _itemCategories = new ItemCategories[]
            {
                new()
                {
                    Category = "Stories",
                    Count = countOfStories
                },
                new()
                {
                    Category = "Bugs",
                    Count = countOfBugs
                },
                new()
                {
                    Category = "Others",
                    Count = countOfOthers
                },
            };
        }
        
    }

    internal class ItemCategories
    {
        public string Category { get; set; }
        public int Count { get; set; }
    }
}