using NMolecules.DDD;

namespace Metrix.Core.BacklogItem
{
    [Entity]
    public class Story : BacklogItem
    {
        public string UserStory { get; set; }
        public int? StoryPoints { get; set; }
    }
}