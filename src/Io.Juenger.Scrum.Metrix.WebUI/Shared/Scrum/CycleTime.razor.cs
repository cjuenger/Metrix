using System.Threading.Tasks;
using Io.Juenger.GitShake.UI.Clients;
using Io.Juenger.GitShake.UI.Configs;
using Microsoft.AspNetCore.Components;
using TimeSpan = System.TimeSpan;

namespace Io.Juenger.GitShake.UI.Shared
{
    public partial class CycleTime
    {
        [Inject]
        private IDataAccessConfig DataAccessConfig { get; set; }
        
        [Inject]
        private IClientsFactory ClientsFactory { get; set; }
        
        private TimeSpan AverageStoryCycleTime { get; set; }
        private TimeSpan BestStoryCycleTime { get; set; }
        private TimeSpan WorstStoryCycleTime { get; set; }
        
        private TimeSpan AverageBugCycleTime { get; set; }
        private TimeSpan BestBugCycleTime { get; set; }
        private TimeSpan WorstBugCycleTime { get; set; }
        
        private TimeSpan AverageOtherCycleTime { get; set; }
        private TimeSpan BestOtherCycleTime { get; set; }
        private TimeSpan WorstOtherCycleTime { get; set; }
        
        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync().ConfigureAwait(false);

            var projectsClient = ClientsFactory.GetProjectsClient();
            
            var cycleTime = await projectsClient.GetCycleTimeAsync(DataAccessConfig.ProjectId);

            AverageStoryCycleTime = cycleTime.AverageStoryCycleTime ?? TimeSpan.Zero;
            BestStoryCycleTime = cycleTime.BestStoryCycleTime ?? TimeSpan.Zero;
            WorstStoryCycleTime = cycleTime.WorstStoryCycleTime ?? TimeSpan.Zero;

            AverageBugCycleTime = cycleTime.AverageBugCycleTime ?? TimeSpan.Zero;
            BestBugCycleTime = cycleTime.BestBugCycleTime ?? TimeSpan.Zero;
            WorstBugCycleTime = cycleTime.WorstBugCycleTime ?? TimeSpan.Zero;

            AverageOtherCycleTime = cycleTime.AverageOtherCycleTime ?? TimeSpan.Zero;
            BestOtherCycleTime = cycleTime.BestOtherCycleTime ?? TimeSpan.Zero;
            WorstOtherCycleTime = cycleTime.WorstOtherCycleTime ?? TimeSpan.Zero;
        }
    }
}