namespace Metrix.Core.ProductCalendar;

public record Workday
{
    public Workday(TimeOnly regularStart, TimeSpan pauseLength, int workHours)
    {
        RegularStart = regularStart;
        RegularEnd = regularStart.Add(pauseLength + TimeSpan.FromHours(workHours));
        WorkHours = workHours;
    }
    
    public TimeOnly RegularStart { get; }
    public TimeOnly RegularEnd { get; }
    public int WorkHours { get; }
}