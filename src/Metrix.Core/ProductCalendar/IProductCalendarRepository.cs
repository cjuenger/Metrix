namespace Metrix.Core.ProductCalendar;

public interface IProductCalendarRepository
{
    Task<Core.ProductCalendar.ProductCalendar> LoadProductCalendarAsync(
        string productId,
        CancellationToken cancellationToken = default);
}