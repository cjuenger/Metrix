using FluentAssertions;
using Metrix.Core.ProductCalendar;

namespace Metrix.Core.Tests.ProductCalendar;

public class ProductCalendarTests
{
    [Test]
    public void PredictDueDate()
    {
        const int calendarId = 1;
        const int daysPerBusinessWeek = 5;
        const int dailyWorkHours = 8;
        
        var kickOffDate = new BusinessDay(new DateTime(2024, 12, 16)); // Monday
        var dueDate = new BusinessDay(new DateTime(2024,12,20)); // Friday

        var workday = new Workday(new TimeOnly(8, 0), TimeSpan.FromMinutes(45), 8);
        var workweek = new Workweek(5);
        
        // var excludedDates = new[] { new DateTime(2024, 12, 24) };
        
        var sut = new Core.ProductCalendar.ProductCalendar(
            calendarId, 
            kickOffDate, 
            dueDate, 
            workday,
            workweek);

        var predictedDueDate = sut.PredictDueDate(kickOffDate, TimeSpan.FromHours(dailyWorkHours));
        predictedDueDate.Should().Be(new BusinessDay(new DateTime(2024, 12, 16)));
        
        predictedDueDate = sut.PredictDueDate(kickOffDate, TimeSpan.FromHours(dailyWorkHours * 4));
        predictedDueDate.Should().Be(new BusinessDay(new DateTime(2024, 12, 19)));
    }
}