using System;
using System.Collections.Generic;

namespace Io.Juenger.Scrum.GitLab.Contracts.Values
{
    public class BurnUpValue
    {
        public IEnumerable<XyValue<DateTime, int>> TotalSeries { get; }
        public IEnumerable<XyValue<DateTime, int>> CompleteSeries { get; }

        public BurnUpValue(
            IEnumerable<XyValue<DateTime, int>> totalSeries,
            IEnumerable<XyValue<DateTime, int>> completeSeries)
        {
            TotalSeries = totalSeries ?? throw new ArgumentNullException(nameof(totalSeries));
            CompleteSeries = completeSeries ?? throw new ArgumentNullException(nameof(completeSeries));
        }
    }
}