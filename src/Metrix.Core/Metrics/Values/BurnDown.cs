using Metrix.Core.Values;
using NMolecules.DDD;

namespace Metrix.Core.Metrics.Values
{
    [ValueObject]
    public class BurnDown
    {
        public IEnumerable<XyValue<DateTime, int>> BurnDownSeries { get; }
        public IEnumerable<XyValue<DateTime, int>> EstimateSeries { get; }
        public IEnumerable<XyValue<DateTime, int>> BestEstimateSeries { get; }
        public IEnumerable<XyValue<DateTime, int>> WorstEstimateSeries { get; }
        
        public DateTime DueDate { get; set; }
        
        public BurnDown(
            IEnumerable<XyValue<DateTime, int>> burnDownSeries,
            IEnumerable<XyValue<DateTime, int>> estimateSeries,
            IEnumerable<XyValue<DateTime, int>> bestEstimateSeries,
            IEnumerable<XyValue<DateTime, int>> worstEstimateSeries)
        {
            BurnDownSeries = burnDownSeries ?? throw new ArgumentNullException(nameof(burnDownSeries));
            EstimateSeries = estimateSeries ?? throw new ArgumentNullException(nameof(estimateSeries));
            BestEstimateSeries = bestEstimateSeries ?? throw new ArgumentNullException(nameof(bestEstimateSeries));
            WorstEstimateSeries = worstEstimateSeries ?? throw new ArgumentNullException(nameof(worstEstimateSeries));
        }
    }
}