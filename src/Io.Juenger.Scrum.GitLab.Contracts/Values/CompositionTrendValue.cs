namespace Io.Juenger.Scrum.GitLab.Contracts.Values;

public class CompositionTrendValue
{
    public IEnumerable<XyValue<string, CompositionValue>> CompositionSeries { get; }

    public CompositionTrendValue(IEnumerable<XyValue<string, CompositionValue>> compositionSeries)
    {
        CompositionSeries = compositionSeries ?? throw new ArgumentNullException(nameof(compositionSeries));
    }
}