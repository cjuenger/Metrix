using NMolecules.DDD;

namespace Metrix.Core;

[ValueObject]
public class ProductCalendar
{
    private const int DaysPerWeek = 7;
    
    private readonly BusinessDay _kickOffTime;
    private readonly BusinessDay _dueTime;
    private readonly int _daysPerBusinessWeek;
    private readonly float _dailyWorkHours;
    private readonly DateTime[] _excludeDates;

    public ProductCalendar(
        BusinessDay kickOffTime, 
        BusinessDay dueTime,
        int daysPerBusinessWeek = 5,
        float dailyWorkHours = 8,
        params DateTime[] excludeDates)
    {
        _kickOffTime = kickOffTime;
        _dueTime = dueTime;
        _daysPerBusinessWeek = daysPerBusinessWeek;
        _dailyWorkHours = dailyWorkHours;
        _excludeDates = excludeDates;
    }

    public BusinessDay CreateBusinessDay(DateTime dateTime)
    {
        var isExcludedDate = _excludeDates.Any(ed => ed.Date == dateTime.Date);
        if (isExcludedDate)
        {
            throw new ArgumentException($"Passed date {dateTime} is an excluded date.");
        }
        
        return new BusinessDay(dateTime);
    }
    
    public int CountBusinessDaysUntil(BusinessDay until)
    {
        var kickOffTime = _kickOffTime.Date;
        var untilTime = until.Date;
        
        if (kickOffTime > untilTime)
        {
            throw new ArgumentException("Incorrect last day " + until);
        }

        var span = untilTime - kickOffTime;
        var businessDays = span.Days + 1;
        var fullWeekCount = businessDays / 7;
        
        // find out if there are weekends during the time exceeding the full weeks
        if (businessDays > fullWeekCount*7)
        {
            // we are here to find out if there is a 1-day or 2-days weekend
            // in the time interval remaining after subtracting the complete weeks
            // var firstDayOfWeek = (int) firstDay.DayOfWeek;
            // var lastDayOfWeek = (int) lastDay.DayOfWeek;
            var firstDayOfWeek = kickOffTime.DayOfWeek == DayOfWeek.Sunday 
                ? 7 : (int)kickOffTime.DayOfWeek;
            var lastDayOfWeek = untilTime.DayOfWeek == DayOfWeek.Sunday
                ? 7 : (int)untilTime.DayOfWeek;
            
            if (lastDayOfWeek < firstDayOfWeek)
                lastDayOfWeek += 7;
            switch (firstDayOfWeek)
            {
                // Both Saturday and Sunday are in the remaining time interval
                case <= 6 when lastDayOfWeek >= 7:
                    businessDays -= 2;
                    break;
                case <= 6:
                {
                    if (lastDayOfWeek >= 6)// Only Saturday is in the remaining time interval
                        businessDays -= 1;
                    break;
                }
                // Only Sunday is in the remaining time interval
                case <= 7 when lastDayOfWeek >= 7:
                    businessDays -= 1;
                    break;
            }
        }

        // subtract the weekends during the full weeks in the interval
        businessDays -= fullWeekCount + fullWeekCount;

        // subtract the number of bank holidays during the time interval
        foreach (var bankHoliday in _excludeDates)
        {
            var bh = bankHoliday.Date;
            if (kickOffTime <= bh && bh <= untilTime)
            {
                --businessDays;
            }
        }

        return businessDays;
    }

    public DateTime PredictDueDateFromNow(TimeSpan remainingTotalWorkTime)
    {
        throw new NotImplementedException();
    }
    
    public BusinessDay PredictDueDate(BusinessDay from, TimeSpan remainingTotalWorkTime)
    {
        var remainingHours = remainingTotalWorkTime.TotalHours % _dailyWorkHours;
        var entireDaysOfWork = (int)(remainingTotalWorkTime.TotalHours / _dailyWorkHours);
        
        // If there are remaining hours (meaning for example a half day) that day must be
        // added in total, as that day would be the final day.
        var additionalDayOfWork = remainingHours > 0 ? 1 : 0;
        
        var daysOfWork = entireDaysOfWork + additionalDayOfWork;
        var weekendDays = DaysPerWeek - _daysPerBusinessWeek;

        // It is possible of course, that within a "business week" of work time there is one or more excluded dates.
        // If that is so, at least one weekend must be considered!
        var countOfExcludedDates = CountOfExcludedDatesWithin(from.Date, from.Date.AddDays(daysOfWork));
        var isDueDateNotInThisCalendarWeek = daysOfWork + countOfExcludedDates >= _daysPerBusinessWeek;

        // From here on we calculate the calendar days
        var calendarDays = daysOfWork;
        if (isDueDateNotInThisCalendarWeek)
        {
            var weekends = calendarDays / _daysPerBusinessWeek;
            
            // If there was an excluded date, that cause the due date not being in this calendar week,
            // we must consider at least one weekend!
            weekends = weekends <= 0 ? 1 : weekends;
            
            var totalWeekendDays = weekends * weekendDays;
            calendarDays += totalWeekendDays;

            countOfExcludedDates = CountOfExcludedDatesWithin(from.Date, from.Date.AddDays(calendarDays));
            calendarDays += countOfExcludedDates;
        }

        // Finally we must check, if the calculated due date is again an excluded date.
        // In that case we must increase the calendar days incrementally and check each new 
        // date if it is an excluded one.
        while (IsExcludedDate(from.Date.AddDays(calendarDays)))
        {
            calendarDays++;
        }

        return new BusinessDay(from.Date.AddDays(calendarDays));
    }

    private int CountOfExcludedDatesWithin(DateTime from, DateTime until)
    {
        return ExcludedDatesWithin(from, until).Count;
    }

    private bool IsExcludedDate(DateTime date)
    {
        return _excludeDates.Any(ed => ed.Date == date.Date);
    }

    private IReadOnlyCollection<DateTime> ExcludedDatesWithin(DateTime from, DateTime until)
    {
        return _excludeDates
            .Where(ed => ed.Date >= from.Date && ed.Date <= until.Date)
            .ToList()
            .AsReadOnly();
    }
    
    // private int CountOfExcludedDaysWithin(DateTime from, DateTime until)
    // {
    //     // var fullWeekCount = (int) totalDays / 7;
    //     // var remainingDays = (int) totalDays % 7;
    //     //     
    //     // var countOfExcludedDates = fullWeekCount * 2;
    //     //
    //     // var currentEndDate = startDate;
    //     //     
    //     // if (countOfExcludedDates > 0)
    //     // {
    //     //     currentEndDate = startDate.AddDays((int) totalDays + countOfExcludedDates - 1);
    //     //     countOfExcludedDates += excludeDates.Count(d => d >= startDate && d <= currentEndDate);
    //     // }
    //     //
    //     // if (remainingDays == 1)
    //     // {
    //     //     if (startDate.DayOfWeek == DayOfWeek.Saturday || startDate.DayOfWeek == DayOfWeek.Sunday )
    //     //     {
    //     //         countOfExcludedDates++;
    //     //     }
    //     // }
    //     // else if (remainingDays > 1)
    //     // {
    //     //     var endDate = currentEndDate + TimeSpan.FromDays(remainingDays - 1);
    //     //     var businessDays = currentEndDate.GetBusinessDaysUntil(endDate);
    //     //     var daysToAdd = remainingDays - businessDays;
    //     //     countOfExcludedDates += daysToAdd;
    //     //
    //     //     return countOfExcludedDates;
    //     // }
    //     //
    //     // return countOfExcludedDates;
    // }
    
    // public static int GetWeekendDaysUntil(this DateTime startDate, DateTime endDate)
    // {
    //     var totalTime = endDate - startDate + TimeSpan.FromDays(1);
    //     var totalDays = totalTime.TotalDays;
    //     var daysInWeekend = startDate.GetCountOfExcludedDaysWithinBusinessDays(totalDays);
    //     return daysInWeekend;
    // }
}