using System;

namespace Io.Juenger.Scrum.GitLab.Contracts.Values;

public class CycleTimeValue
{
    public Type Type { get; }
    public TimeSpan AverageCycleTime { get; }
    public TimeSpan BestCycleTime { get; }
    public TimeSpan WorstCycleTime { get; }

    public CycleTimeValue(Type type, TimeSpan averageCycleTime, TimeSpan bestCycleTime, TimeSpan worstCycleTime)
    {
        Type = type;
        AverageCycleTime = averageCycleTime;
        BestCycleTime = bestCycleTime;
        WorstCycleTime = worstCycleTime;
    }
}