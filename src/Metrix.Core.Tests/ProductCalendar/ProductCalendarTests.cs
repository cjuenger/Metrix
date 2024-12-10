using FluentAssertions;
using Metrix.Core.ProductCalendar;

namespace Metrix.Core.Tests.ProductCalendar;

public class ProductCalendarTests
{
    [TestCase(4, 0)]
    [TestCase(8, 0)]
    [TestCase(9, 1)]
    [TestCase(32, 3)]
    [TestCase(40, 4)]
    public void PredictDueDate_At_The_Very_Beginning_Of_Workday(int remainingWorkHours, int expectedDays)
    {
        const int calendarId = 1;
        
        var kickOffDate = new BusinessDay(new DateTime(2024, 12, 16)); // Monday
        var dueDate = new BusinessDay(new DateTime(2024,12,20)); // Friday

        var workday = new Workday(new TimeOnly(8, 0), TimeSpan.FromMinutes(45), 8);
        var workweek = new Workweek(5);
        
        var sut = new Core.ProductCalendar.ProductCalendar(
            calendarId, 
            kickOffDate, 
            dueDate, 
            workday,
            workweek);

        var predictedDueDate = sut.PredictDueDate(kickOffDate, TimeSpan.FromHours(remainingWorkHours));
        predictedDueDate.Should().Be(new BusinessDay(kickOffDate.DateTime + TimeSpan.FromDays(expectedDays)));
    }
}