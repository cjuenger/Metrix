namespace Metrix.Core.ProductCalendar;

public record Workweek
{
    private const int WeekLength = 7;

    public Workweek(int workdays)
    {
        Workdays = workdays switch
        {
            > 7 => throw new ArgumentException("A workweek cannot be longer than 7 days!", nameof(workdays)),
            < 5 => throw new ArgumentException("A workweek must at least have 5 days!", nameof(workdays)),
            _ => workdays
        };
    }
    
    public int Workdays { get; }
    public int WeekendDays => WeekLength - Workdays;
}