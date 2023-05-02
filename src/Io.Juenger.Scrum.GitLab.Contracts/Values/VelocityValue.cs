namespace Io.Juenger.Scrum.GitLab.Contracts.Values
{
    public class VelocityValue
    {
        public float AverageVelocity { get; }
        public float DayAverageVelocity { get; private set; }

        public float Last5SprintsAverageVelocity { get; set; }
        
        public float Last5SprintsDayAverageVelocity { get; set; }
        
        public float Best3SprintsAverageVelocity { get; }
        public float Best3SprintsDayAverageVelocity { get; }
        public float Worst3SprintsAverageVelocity { get; }
        public float Worst3SprintsDayAverageVelocity { get; }
        public int CountOfSprints { get; }
        public float AverageSprintLength { get; }

        public VelocityValue(
            float averageVelocity, 
            float last5SprintsAverageVelocity, 
            float last5SprintsDayAverageVelocity,
            float best3SprintsAverageVelocity,
            float best3SprintsDayAverageVelocity,
            float worst3SprintsAverageVelocity,
            float worst3SprintsDayAverageVelocity,
            int countOfSprints,
            float averageSprintLength)
        {
            AverageVelocity = averageVelocity;
            Last5SprintsAverageVelocity = last5SprintsAverageVelocity;
            Last5SprintsDayAverageVelocity = last5SprintsDayAverageVelocity;
            Best3SprintsAverageVelocity = best3SprintsAverageVelocity;
            Best3SprintsDayAverageVelocity = best3SprintsDayAverageVelocity;
            Worst3SprintsAverageVelocity = worst3SprintsAverageVelocity;
            Worst3SprintsDayAverageVelocity = worst3SprintsDayAverageVelocity;
            CountOfSprints = countOfSprints;
            AverageSprintLength = averageSprintLength;

            CalculateDayAverageVelocity();
        }
        
        private void CalculateDayAverageVelocity()
        {
            DayAverageVelocity = AverageVelocity / AverageSprintLength;
        }
    }
}