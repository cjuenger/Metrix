namespace Metrix.Core.ProductCalendar;

public record ProductCalendarId
{
    public int Id { get; }

    public ProductCalendarId(int id)
    {
        Id = id;
    }
}