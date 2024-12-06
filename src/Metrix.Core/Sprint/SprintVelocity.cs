using NMolecules.DDD;

namespace Metrix.Core.Sprint;

[ValueObject]
public class SprintVelocity
{
    public SprintVelocity(int totalStoryPoints, int sprintLengthInDays)
    {
        TotalStoryPoints = totalStoryPoints;
        SprintLengthInDays = sprintLengthInDays;
    }
    
    public int TotalStoryPoints { get; }
    public int SprintLengthInDays { get; }
}