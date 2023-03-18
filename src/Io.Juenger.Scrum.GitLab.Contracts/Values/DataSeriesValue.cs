namespace Io.Juenger.Scrum.GitLab.Contracts.Values
{
    public class DataSeriesValue<TX, TY>
    {
        public string Title { get; set; }

        public IList<XyValue<TX, TY>> Series { get; set; }
    }
}