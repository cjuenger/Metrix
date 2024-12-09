using NMolecules.DDD;

namespace Metrix.Core.Metrics.Values
{
    [ValueObject]
    public class BurnUp
    {
        public IEnumerable<XyValue<DateTime, int>> TotalSeries { get; }
        public IEnumerable<XyValue<DateTime, int>> CompleteSeries { get; }

        public BurnUp(
            IEnumerable<XyValue<DateTime, int>> totalSeries,
            IEnumerable<XyValue<DateTime, int>> completeSeries)
        {
            TotalSeries = totalSeries ?? throw new ArgumentNullException(nameof(totalSeries));
            CompleteSeries = completeSeries ?? throw new ArgumentNullException(nameof(completeSeries));
        }
    }
}