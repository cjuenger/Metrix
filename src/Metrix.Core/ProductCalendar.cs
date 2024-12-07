using NMolecules.DDD;

namespace Metrix.Core;

[ValueObject]
public class ProductCalendar
{
    private readonly BusinessDay _kickOffTime;
    private readonly BusinessDay _dueTime;
    private readonly int _businessWeekDays;
    private readonly float _dailyWorkHours;
    private readonly DateTime[] _excludeDates;

    public ProductCalendar(
        BusinessDay kickOffTime, 
        BusinessDay dueTime,
        int businessWeekDays = 5,
        float dailyWorkHours = 8,
        params DateTime[] excludeDates)
    {
        _kickOffTime = kickOffTime;
        _dueTime = dueTime;
        _businessWeekDays = businessWeekDays;
        _dailyWorkHours = dailyWorkHours;
        _excludeDates = excludeDates;
    }
    
    public int BusinessDaysUntil(BusinessDay until)
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
                    --businessDays;
            }

            return businessDays;
        }
    
    public DateTime PredictDueDate(BusinessDay from, TimeSpan remainingTotalWorkTime)
    {
        var remainingHours = remainingTotalWorkTime.TotalHours % _dailyWorkHours;
        var totalWorkDays = (int)(remainingTotalWorkTime.TotalHours / _dailyWorkHours);
        
        
        var fullWeekCount = totalWorkDays == _businessWeekDays ? 0 : totalWorkDays/ _businessWeekDays;
        var weekendLength = 7 - _businessWeekDays;
        var totalWeekendDays = fullWeekCount * weekendLength;
        var remainingDays =  totalWorkDays > 7 ? totalWorkDays % _businessWeekDays : 0;

        var correctedDays = totalWorkDays + totalWeekendDays;
        
        var coutOfExcludedDays = CountOfExcludedDaysWithin(from.DateTime, correctedDays + remainingDays);
        correctedDays += coutOfExcludedDays;
        var dueDate = _kickOffTime.DateTime.AddDays(correctedDays - totalWeekendDays - 1);

        return dueDate;
    }
    
    private int CountOfExcludedDaysWithin(DateTime from, DateTime until)
    {
        // var fullWeekCount = (int) totalDays / 7;
        // var remainingDays = (int) totalDays % 7;
        //     
        // var countOfExcludedDates = fullWeekCount * 2;
        //
        // var currentEndDate = startDate;
        //     
        // if (countOfExcludedDates > 0)
        // {
        //     currentEndDate = startDate.AddDays((int) totalDays + countOfExcludedDates - 1);
        //     countOfExcludedDates += excludeDates.Count(d => d >= startDate && d <= currentEndDate);
        // }
        //
        // if (remainingDays == 1)
        // {
        //     if (startDate.DayOfWeek == DayOfWeek.Saturday || startDate.DayOfWeek == DayOfWeek.Sunday )
        //     {
        //         countOfExcludedDates++;
        //     }
        // }
        // else if (remainingDays > 1)
        // {
        //     var endDate = currentEndDate + TimeSpan.FromDays(remainingDays - 1);
        //     var businessDays = currentEndDate.GetBusinessDaysUntil(endDate);
        //     var daysToAdd = remainingDays - businessDays;
        //     countOfExcludedDates += daysToAdd;
        //
        //     return countOfExcludedDates;
        // }
        //
        // return countOfExcludedDates;
    }
    
    // public static int GetWeekendDaysUntil(this DateTime startDate, DateTime endDate)
    // {
    //     var totalTime = endDate - startDate + TimeSpan.FromDays(1);
    //     var totalDays = totalTime.TotalDays;
    //     var daysInWeekend = startDate.GetCountOfExcludedDaysWithinBusinessDays(totalDays);
    //     return daysInWeekend;
    // }
}