using System.Collections.Generic;
using System.Linq;

namespace Io.Juenger.Scrum.GitLab.Contracts.Values
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