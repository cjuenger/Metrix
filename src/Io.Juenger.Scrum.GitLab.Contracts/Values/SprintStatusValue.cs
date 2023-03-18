namespace Io.Juenger.Scrum.GitLab.Contracts.Values;

public class SprintStatusValue
{
    public SprintStatusValue(int completedStoryPoints, int openStoryPoints)
    {
        CompletedStoryPoints = completedStoryPoints;
        OpenStoryPoints = openStoryPoints;
    }
    
    public int CompletedStoryPoints { get; }
        
    public int OpenStoryPoints { get; }

    public int TotalStoryPoints => CompletedStoryPoints + OpenStoryPoints;
}