using Io.Juenger.Scrum.GitLab.Contracts.Values;
using Microsoft.AspNetCore.Components;

namespace Io.Juenger.Scrum.Metrix.WebUI.Shared.Scrum
{
    public partial class BurnDownChart
    {
        private int _maxYValue;

        private IEnumerable<XyValue<DateTime, int>> _dueLine = Enumerable.Empty<XyValue<DateTime, int>>();

        private const bool Smooth = false;

        [Parameter] public string Title { get; set; } = "";

        [Parameter] public BurnDownValue? BurnDown { get; set; }
        
        [Parameter]
        public DateTime? DueDate { get; set; }

        protected override async Task OnParametersSetAsync()
        {
            await base.OnParametersSetAsync().ConfigureAwait(false);
            
            _maxYValue = BurnDown?.BurnDownSeries.Any() ?? false
                ? BurnDown?.BurnDownSeries.Select(bd => bd.Y).Max() + 5 ?? 0
                : 5;

            CalculateDueLine();
        }

        private void CalculateDueLine()
        {
            if (DueDate.HasValue)
            {
                _dueLine = new XyValue<DateTime, int>[]
                {
                    new()
                    {
                        X = DueDate.Value, 
                        Y = 0
                    },
                    new()
                    {
                        X = DueDate.Value, 
                        Y = _maxYValue
                    },
                };
            }
        }
    }
}