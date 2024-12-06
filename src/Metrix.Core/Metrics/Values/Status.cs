using NMolecules.DDD;

namespace Metrix.Core.Metrics.Values;

[ValueObject]
public class Status
{
    public Status(int completedStoryPoints, int openStoryPoints)
    {
        CompletedStoryPoints = completedStoryPoints;
        OpenStoryPoints = openStoryPoints;
    }
    
    public int CompletedStoryPoints { get; }
        
    public int OpenStoryPoints { get; }

    public int TotalStoryPoints => CompletedStoryPoints + OpenStoryPoints;
}