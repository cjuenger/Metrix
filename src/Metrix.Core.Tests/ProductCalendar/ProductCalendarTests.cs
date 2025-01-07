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
        var calendarId = new ProductCalendarId(1);
        
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
    
    [TestCase(4, 0)]
    [TestCase(8, 1)]
    [TestCase(9, 2)]
    [TestCase(32, 4)]
    [TestCase(40, 7)] // The remaining work time extends into the weekend. Thus, we expect 7 the finalization after 7 calendar days!
    public void PredictDueDate_Not_At_The_Very_Beginning_Of_Workday(int remainingWorkHours, int expectedDays)
    {
        var calendarId = new ProductCalendarId(1);
        
        var kickOffDate = new BusinessDay(new DateTime(2024, 12, 16, 9, 0, 0)); // Monday (09:00h, one hour after office start)
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

    [TestCase(8, 0)]
    [TestCase(16, 4)]
    [TestCase(24, 7)]
    public void PredictDueDate_And_Consider_Excluded_Dates(int remainingWorkHours, int expectedDays)
    {
        var calendarId = new ProductCalendarId(1);
        
        var kickOffDate = new BusinessDay(new DateTime(2024, 12, 23, 0, 0, 0)); // Monday
        var dueDate = new BusinessDay(new DateTime(2024,12,27)); // Friday

        var workday = new Workday(new TimeOnly(8, 0), TimeSpan.FromMinutes(45), 8);
        var workweek = new Workweek(5);

        var excludedDates = new[]
        {
            new DateTime(2024,12,24),
            new DateTime(2024,12,25),
            new DateTime(2024,12,26),
        };
        
        var sut = new Core.ProductCalendar.ProductCalendar(
            calendarId, 
            kickOffDate, 
            dueDate, 
            workday,
            workweek,
            excludedDates);
        
        var predictedDueDate = sut.PredictDueDate(kickOffDate, TimeSpan.FromHours(remainingWorkHours));
        predictedDueDate.Should().Be(new BusinessDay(kickOffDate.DateTime + TimeSpan.FromDays(expectedDays)));
    }

    [Test]
    public void PredictDueDate_And_Consider_Weekend()
    {
        const int remainingWorkHours = 10;
        const int expectedDays = 3;
        
        var calendarId = new ProductCalendarId(1);
        
        var kickOffDate = new BusinessDay(new DateTime(2024,12,27)); // Friday
        var dueDate = new BusinessDay(new DateTime(2024,12,30)); // Monday

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