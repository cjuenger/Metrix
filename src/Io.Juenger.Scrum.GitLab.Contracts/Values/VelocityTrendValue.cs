namespace Io.Juenger.Scrum.GitLab.Contracts.Values
{
    public class VelocityTrendValue
    {
        public IEnumerable<XyValue<string, int>> VelocitySeries { get; }

        public VelocityTrendValue(IEnumerable<XyValue<string, int>> velocitySeries)
        {
            VelocitySeries = velocitySeries;
        }
    }
}