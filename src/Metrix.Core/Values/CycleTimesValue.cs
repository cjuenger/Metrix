namespace Metrix.Core.Values
{
    public class CycleTimesValue
    {
        public List<CycleTimeValue> CycleTimes { get; }
        
        public CycleTimesValue(IEnumerable<CycleTimeValue> cycleTimeValues)
        {
            CycleTimes = cycleTimeValues.ToList();
        }
    }
}