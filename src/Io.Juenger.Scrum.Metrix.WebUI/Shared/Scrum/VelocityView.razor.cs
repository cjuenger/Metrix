using Io.Juenger.Scrum.GitLab.Contracts.Values;
using Io.Juenger.Scrum.Metrix.WebUI.Configs;
using Microsoft.AspNetCore.Components;

namespace Io.Juenger.Scrum.Metrix.WebUI.Shared.Scrum
{
    public partial class VelocityView
    {
        [Parameter] public VelocityValue Velocity { get; set; } = default!;
        [Parameter] public VelocityTrendValue VelocityTrend { get; set; } = default!;
        [Inject] private ProductConfig ProductConfig { get; set; } = default!;
        protected override async Task OnParametersSetAsync()
        {
            await base.OnParametersSetAsync();
        }
    }
}