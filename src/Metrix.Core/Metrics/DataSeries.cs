using NMolecules.DDD;

namespace Metrix.Core.Metrics
{
    [ValueObject] //TODO: 20241206 CB: Is this really a 'ValueObject'?
    public class DataSeries<TX, TY>
    {
        public string Title { get; set; }

        public IList<XyValue<TX, TY>> Series { get; set; }
    }
}