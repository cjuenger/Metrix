namespace Metrix.Core.Values
{
    public class DataSeriesValue<TX, TY>
    {
        public string Title { get; set; }

        public IList<XyValue<TX, TY>> Series { get; set; }
    }
}