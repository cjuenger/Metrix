using NMolecules.DDD;

namespace Metrix.Core.Metrics.Values;

[ValueObject]
public class CycleTime
{
    public Type Type { get; }
    public TimeSpan AverageCycleTime { get; }
    public TimeSpan BestCycleTime { get; }
    public TimeSpan WorstCycleTime { get; }

    public CycleTime(Type type, TimeSpan averageCycleTime, TimeSpan bestCycleTime, TimeSpan worstCycleTime)
    {
        Type = type;
        AverageCycleTime = averageCycleTime;
        BestCycleTime = bestCycleTime;
        WorstCycleTime = worstCycleTime;
    }
}