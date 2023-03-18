using Io.Juenger.Scrum.GitLab.Contracts.Entities;

namespace Io.Juenger.Scrum.GitLab.Contracts.Values
{
    public class CompositionValue
    {
        public int CountOfStories { get; private set; }

        public int CountOfBugs { get; private set; }

        public int CountOfOthers { get; private set; }

        public CompositionValue(IEnumerable<ItemEntity> items)
        {
            if (items == null) throw new ArgumentNullException(nameof(items));
            CalculateComposition(items.ToList());
        }
        
        private void CalculateComposition(IReadOnlyCollection<ItemEntity> items)
        {
            var countOfTotalItems = items.Count;
            var countOfStories = items.OfType<StoryEntity>().Count();
            var countOfBugs = items.OfType<BugEntity>().Count();
            var countOfOthers = countOfTotalItems - countOfStories - countOfBugs;

            CountOfStories = countOfStories;
            CountOfBugs = countOfBugs;
            CountOfOthers = countOfOthers;
        }
    }
}