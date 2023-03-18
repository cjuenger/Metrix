namespace Io.Juenger.Scrum.GitLab.Contracts.Entities
{
    public class StoryEntity : ItemEntity
    {
        public string UserStory { get; set; }
        public int? StoryPoints { get; set; }
    }
}