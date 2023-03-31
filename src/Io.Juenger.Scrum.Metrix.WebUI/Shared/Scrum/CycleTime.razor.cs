using Io.Juenger.Scrum.GitLab.Contracts.Values;
using Microsoft.AspNetCore.Components;

namespace Io.Juenger.Scrum.Metrix.WebUI.Shared.Scrum
{
    public partial class CycleTime
    {
        [Parameter] public CycleTimesValue CycleTimes { get; set; } = default!;
    }
}