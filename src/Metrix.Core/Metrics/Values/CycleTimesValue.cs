using NMolecules.DDD;

namespace Metrix.Core.Metrics.Values
{
    [ValueObject]
    public class CycleTimesValue
    {
        public List<CycleTime> CycleTimes { get; }
        
        public CycleTimesValue(IEnumerable<CycleTime> cycleTimeValues)
        {
            CycleTimes = cycleTimeValues.ToList();
        }
    }
}