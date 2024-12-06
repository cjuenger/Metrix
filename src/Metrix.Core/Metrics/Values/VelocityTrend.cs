using NMolecules.DDD;

namespace Metrix.Core.Metrics.Values
{
    [ValueObject]
    public class VelocityTrend
    {
        public IEnumerable<XyValue<string, int>> VelocitySeries { get; }

        public VelocityTrend(IEnumerable<XyValue<string, int>> velocitySeries)
        {
            VelocitySeries = velocitySeries;
        }
    }
}