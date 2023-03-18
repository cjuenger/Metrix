namespace Io.Juenger.Scrum.GitLab.Values;

public class SprintVelocityValue
{
    public SprintVelocityValue(int totalStoryPoints, int sprintLengthInDays)
    {
        TotalStoryPoints = totalStoryPoints;
        SprintLengthInDays = sprintLengthInDays;
    }
    
    public int TotalStoryPoints { get; }
    public int SprintLengthInDays { get; }
}