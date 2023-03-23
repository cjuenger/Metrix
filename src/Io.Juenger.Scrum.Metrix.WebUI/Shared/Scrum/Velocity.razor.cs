using AutoMapper;
using Microsoft.AspNetCore.Components;

namespace Io.Juenger.GitShake.UI.Shared
{
    public partial class Velocity
    {
        private float _velocity;
        private float _bestVelocity;
        private float _worstVelocity;
        
        private IEnumerable<Xy<string, int>> _completedStoriesSeries;
        private IEnumerable<Xy<string, int>> _smaVelocitySeries;
        
        private readonly bool _smooth = false;
        
        [Inject]
        private IDataAccessConfig DataAccessConfig { get; set; }
        
        [Inject]
        private IClientsFactory ClientsFactory { get; set; }
        
        [Inject]
        private IMapper Mapper { get; set; }

        [Parameter] 
        public int SprintLength { get; set; } = 1;

        [Parameter]
        public DateTime StartDate { get; set; }
        
        protected override async Task OnParametersSetAsync()
        {
            await base.OnParametersSetAsync();

            var projectsClient = ClientsFactory.GetProjectsClient();
            
            var velocity = await projectsClient
                .GetVelocityAsync(DataAccessConfig.ProjectId)
                .ConfigureAwait(false);
            
            _velocity = velocity.AverageVelocity ?? 0;
            _bestVelocity = velocity.Best3SprintsAverageVelocity ?? 0;
            _worstVelocity = velocity.Worst3SprintsAverageVelocity ?? 0;

            var velocityChartData = await projectsClient.GetVelocityChartDataAsync(DataAccessConfig.ProjectId);
            
            _velocity = velocityChartData.Velocity.AverageVelocity ?? 0;
            _bestVelocity = velocityChartData.Velocity.Best3SprintsAverageVelocity ?? 0;
            _worstVelocity = velocityChartData.Velocity.Worst3SprintsAverageVelocity ?? 0;

            _smaVelocitySeries = Mapper.Map<IEnumerable<Xy<string, int>>>(velocityChartData.VelocitySeries);
        }
    }
}