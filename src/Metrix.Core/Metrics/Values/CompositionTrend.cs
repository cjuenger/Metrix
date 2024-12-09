using NMolecules.DDD;

namespace Metrix.Core.Metrics.Values;

[ValueObject]
public class CompositionTrend
{
    public IEnumerable<XyValue<string, Composition>> CompositionSeries { get; }

    public CompositionTrend(IEnumerable<XyValue<string, Composition>> compositionSeries)
    {
        CompositionSeries = compositionSeries ?? throw new ArgumentNullException(nameof(compositionSeries));
    }
}