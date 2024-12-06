using Metrix.Core.BacklogItem;
using NMolecules.DDD;

namespace Metrix.Core.Metrics.Values
{
    [ValueObject]
    public class Composition
    {
        public int CountOfStories { get; private set; }

        public int CountOfBugs { get; private set; }

        public int CountOfOthers { get; private set; }

        public Composition(IEnumerable<BacklogItem.BacklogItem> items)
        {
            if (items == null) throw new ArgumentNullException(nameof(items));
            CalculateComposition(items.ToList());
        }
        
        private void CalculateComposition(IReadOnlyCollection<BacklogItem.BacklogItem> items)
        {
            var countOfTotalItems = items.Count;
            var countOfStories = items.OfType<Story>().Count();
            var countOfBugs = items.OfType<Bug>().Count();
            var countOfOthers = countOfTotalItems - countOfStories - countOfBugs;

            CountOfStories = countOfStories;
            CountOfBugs = countOfBugs;
            CountOfOthers = countOfOthers;
        }
    }
}