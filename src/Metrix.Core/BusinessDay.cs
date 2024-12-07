using NMolecules.DDD;

namespace Metrix.Core;

[ValueObject]
public class BusinessDay
{
    public DateTime Date => DateTime.Date;
    public DateTime DateTime { get; }

    public BusinessDay(DateTime dateTime)
    {
        if (IsDayAtWeekend(dateTime))
        {
            throw new ArgumentException("The date is a weekend.");
        }
        
        DateTime = dateTime;
    }

    private static bool IsDayAtWeekend(DateTime dateTime)
    {
        return dateTime.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday;
    }
}