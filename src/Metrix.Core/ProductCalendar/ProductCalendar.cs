using System.Collections.ObjectModel;
using NMolecules.DDD;

namespace Metrix.Core.ProductCalendar;

[AggregateRoot]
public class ProductCalendar
{
    private readonly BusinessDay _kickOffDate;
    private readonly BusinessDay _dueDate;
    private readonly Workday _workday;
    private readonly Workweek _workweek;
    private readonly DateTime[] _excludeDates;
    
    public ProductCalendar(
        int calendarId,
        BusinessDay kickOffDate, 
        BusinessDay dueDate,
        Workday workday,
        Workweek workweek,
        params DateTime[] excludeDates)
    {
        CalendarId = calendarId;
        _kickOffDate = kickOffDate ?? throw new ArgumentNullException(nameof(kickOffDate));
        _dueDate = dueDate ?? throw new ArgumentNullException(nameof(dueDate));
        _workday = workday ?? throw new ArgumentNullException(nameof(workday));
        _workweek = workweek ?? throw new ArgumentNullException(nameof(workweek));
        _excludeDates = excludeDates;
    }

    public int CalendarId { get; }
    
    public BusinessDay CreateBusinessDay(DateTime dateTime)
    {
        var isExcludedDate = _excludeDates.Any(ed => ed.Date == dateTime.Date);
        if (isExcludedDate)
        {
            throw new ArgumentException($"Passed date {dateTime} is an excluded date.");
        }
        
        return new BusinessDay(dateTime);
    }

    public int BusinessDaysWithin(BusinessDay from, BusinessDay until)
    {
        // TODO: 20241209 CB: Implement!
        throw new NotImplementedException();
    }
    
    public int TotalBusinessDays()
    {
        // TODO: 20241209 CB: Revise this!!!
        throw new NotImplementedException();
        
        var kickOffTime = _kickOffDate.Date;
        var untilTime = _dueDate.Date;
        
        if (kickOffTime > untilTime)
        {
            throw new ArgumentException("Incorrect last day " + _dueDate);
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
    
    public BusinessDay PredictDueDate(BusinessDay from, TimeSpan remainingTotalWorkTime)
    {
        var entireDaysOfWork = (int) Math.Ceiling(remainingTotalWorkTime.TotalHours / _workday.WorkHours);
        
        if (TimeOnly.FromDateTime(from.DateTime) <= _workday.RegularStart || 
            remainingTotalWorkTime < TimeSpan.FromHours(_workday.WorkHours) && TimeOnly.FromDateTime(from.DateTime + remainingTotalWorkTime) <= _workday.RegularEnd )
        {
            entireDaysOfWork = entireDaysOfWork > 0 ? entireDaysOfWork - 1 : 0;
        }

        var daysOfWork = entireDaysOfWork;

        // It is possible of course, that within a "business week" of work time there is one or more excluded dates.
        // If that is so, at least one weekend must be considered!
        var countOfExcludedDates = CountOfExcludedDatesWithin(from.Date, from.Date.AddDays(daysOfWork));
        var isDueDateNotInThisCalendarWeek = daysOfWork + countOfExcludedDates >= _workweek.Workdays;

        // From here on we calculate the calendar days
        var calendarDays = daysOfWork;
        if (isDueDateNotInThisCalendarWeek)
        {
            var weekends = calendarDays / _workweek.Workdays;
            
            // If there was an excluded date, that cause the due date not being in this calendar week,
            // we must consider at least one weekend!
            weekends = weekends <= 0 ? 1 : weekends;
            
            var totalWeekendDays = weekends * _workweek.WeekendDays;
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

        return new BusinessDay(from.DateTime.AddDays(calendarDays));
    }

    private int CountOfExcludedDatesWithin(DateTime from, DateTime until)
    {
        return ExcludedDatesWithin(from, until).Count;
    }

    private bool IsExcludedDate(DateTime date)
    {
        return _excludeDates.Any(ed => ed.Date == date.Date);
    }

    private ReadOnlyCollection<DateTime> ExcludedDatesWithin(DateTime from, DateTime until)
    {
        return _excludeDates
            .Where(ed => ed.Date >= from.Date && ed.Date <= until.Date)
            .ToList()
            .AsReadOnly();
    }
}