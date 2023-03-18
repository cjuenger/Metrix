// ReSharper disable once IdentifierTypo

using System;
using System.Linq;

// ReSharper disable once IdentifierTypo
namespace Io.Juenger.Common.Util
{
    /// <summary>
    ///     DateTime extensions methods
    /// </summary>
    public static class DateTimeExtensions
    {
        /// <summary>
        ///     Calculates number of business days, taking into account:
        ///         - weekends (Saturdays and Sundays)
        ///         - holidays in the middle of the week
        /// </summary>
        /// <remarks>I found this solution at <see aref="https://stackoverflow.com/a/1619375/6574264"/>.</remarks>
        /// <param name="firstDay">First day in the time interval</param>
        /// <param name="lastDay">Last day in the time interval</param>
        /// <param name="excludeDates">List of bank holidays excluding weekends</param>
        /// <returns>Number of business days during the 'span'</returns>
        public static int GetBusinessDaysUntil(
            this DateTime firstDay, 
            DateTime lastDay, 
            params DateTime[] excludeDates)
        {
            firstDay = firstDay.Date;
            lastDay = lastDay.Date;
            if (firstDay > lastDay)
                throw new ArgumentException("Incorrect last day " + lastDay);

            var span = lastDay - firstDay;
            var businessDays = span.Days + 1;
            var fullWeekCount = businessDays / 7;
            
            // find out if there are weekends during the time exceeding the full weeks
            if (businessDays > fullWeekCount*7)
            {
                // we are here to find out if there is a 1-day or 2-days weekend
                // in the time interval remaining after subtracting the complete weeks
                // var firstDayOfWeek = (int) firstDay.DayOfWeek;
                // var lastDayOfWeek = (int) lastDay.DayOfWeek;
                var firstDayOfWeek = firstDay.DayOfWeek == DayOfWeek.Sunday 
                    ? 7 : (int)firstDay.DayOfWeek;
                var lastDayOfWeek = lastDay.DayOfWeek == DayOfWeek.Sunday
                    ? 7 : (int)lastDay.DayOfWeek;
                
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
            foreach (var bankHoliday in excludeDates)
            {
                var bh = bankHoliday.Date;
                if (firstDay <= bh && bh <= lastDay)
                    --businessDays;
            }

            return businessDays;
        }

        /// <summary>
        ///     Gets the weekend days within the given dates range.
        /// </summary>
        /// <param name="startDate">Start date from where the weekends shall be cumulated.</param>
        /// <param name="endDate">End date till where the weekends shall be cumulated.</param>
        /// <returns></returns>
        public static int GetWeekendDaysUntil(this DateTime startDate, DateTime endDate)
        {
            var totalTime = endDate - startDate + TimeSpan.FromDays(1);
            var totalDays = totalTime.TotalDays;
            var daysInWeekend = startDate.GetCountOfExcludedDaysWithinBusinessDays(totalDays);
            return daysInWeekend;
        }

        /// <summary>
        ///     Gets all excluded days within the given dates range.
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="excludeDates"></param>
        /// <returns></returns>
        public static int GetCountOfExcludedDaysWithinBusinessDaysUntil(
            this DateTime startDate, 
            DateTime endDate, 
            params DateTime[] excludeDates)
        {
            var excludedDays = startDate.GetWeekendDaysUntil(endDate);
            excludedDays += excludeDates.Count(d => d >= startDate && d <= endDate);
            return excludedDays;
        }
        
        /// <summary>
        ///     Gets the due date from the passed required total work time, taking into account:
        ///         - weekends (Saturdays and Sundays)
        ///         - holidays in the middle of the week
        /// </summary>
        /// <remarks>
        ///     NOTE that the passed required total work time is pure time necessary to complete the project.
        /// </remarks>
        /// <param name="startDate">Start date from where the due shall be calculated</param>
        /// <param name="totalWorkTime">Required total work time</param>
        /// <param name="businessWeekDays">Number of days of a business week</param>
        /// <param name="dailyWorkHours">Daily working hours</param>
        /// <param name="excludeDates">Days to exclude</param>
        /// <returns>Returns the due date</returns>
        public static DateTime GetBusinessDueDate(
            this DateTime startDate, 
            TimeSpan totalWorkTime, 
            int businessWeekDays = 5,
            float dailyWorkHours = 8,
            params DateTime[] excludeDates)
        {
            var totalWorkDays = (int)(totalWorkTime.TotalHours / dailyWorkHours);
            var fullWeekCount = totalWorkDays == businessWeekDays ? 0 : totalWorkDays/ businessWeekDays;
            var weekendLength = 7 - businessWeekDays;
            var totalWeekendDays = fullWeekCount * weekendLength;
            var remainingDays =  totalWorkDays > 7 ? totalWorkDays % businessWeekDays : 0;

            var correctedDays = totalWorkDays + totalWeekendDays;
            
            var excludedDays = startDate.GetCountOfExcludedDaysWithinBusinessDays(correctedDays + remainingDays, excludeDates);
            correctedDays += excludedDays;
            var dueDate = startDate.AddDays(correctedDays - totalWeekendDays - 1);

            return dueDate;
        }

        /// <summary>
        ///      Gets the due date from the passed required total work time, taking into account:
        ///         - weekends (Saturdays and Sundays)
        ///         - holidays in the middle of the week    
        /// </summary>
        /// <param name="startDate">Start date from where the due shall be calculated</param>
        /// <param name="totalWorkDays">Required total work days</param>
        /// <param name="businessWeekDays">Number of days of a business week</param>
        /// <param name="dailyWorkHours">Daily working hours</param>
        /// <param name="excludeDates">Days to exclude</param>
        /// <returns>Returns the due date</returns>
        public static DateTime GetBusinessDueDate(
            this DateTime startDate, 
            float totalWorkDays, 
            int businessWeekDays = 5,
            float dailyWorkHours = 8,
            params DateTime[] excludeDates)
        {
            var requiredTotalWorkTime = TimeSpan.FromHours(totalWorkDays * dailyWorkHours);
            return GetBusinessDueDate(startDate, requiredTotalWorkTime, businessWeekDays, dailyWorkHours, excludeDates);
        }

        /// <summary>
        ///     Gets the count of days that had been excluded between a start date and a count of consecutive
        ///     business days
        /// </summary>
        /// <param name="startDate">Start date</param>
        /// <param name="totalDays">Count of consecutive business days</param>
        /// <param name="excludeDates">Special day - other than weekends - to exclude</param>
        /// <returns></returns>
        public static int GetCountOfExcludedDaysWithinBusinessDays(
            this DateTime startDate, 
            double totalDays, 
            params DateTime[] excludeDates)
        {
            var fullWeekCount = (int) totalDays / 7;
            var remainingDays = (int) totalDays % 7;
            
            var countOfExcludedDates = fullWeekCount * 2;

            var currentEndDate = startDate;
            
            if (countOfExcludedDates > 0)
            {
                currentEndDate = startDate.AddDays((int) totalDays + countOfExcludedDates - 1);
                countOfExcludedDates += excludeDates.Count(d => d >= startDate && d <= currentEndDate);
            }

            if (remainingDays == 1)
            {
                if (startDate.DayOfWeek == DayOfWeek.Saturday || startDate.DayOfWeek == DayOfWeek.Sunday )
                {
                    countOfExcludedDates++;
                }
            }
            else if (remainingDays > 1)
            {
                var endDate = currentEndDate + TimeSpan.FromDays(remainingDays - 1);
                var businessDays = currentEndDate.GetBusinessDaysUntil(endDate);
                var daysToAdd = remainingDays - businessDays;
                countOfExcludedDates += daysToAdd;

                return countOfExcludedDates;
            }

            return countOfExcludedDates;
        }
    }
}