using Io.Juenger.Common.Util;
using Io.Juenger.Scrum.GitLab.Contracts.Aggregates;

namespace Io.Juenger.Scrum.GitLab.Aggregates
{
    internal class SprintAggregate : ISprintAggregate
    {
        /// <summary>
        ///     Id of the sprint
        /// </summary>
        public int Id { get; private set; }
        
        public string ProductId { get; }
        
        /// <summary>
        ///     Name of the sprint
        /// </summary>
        public string Name { get; private set; }
        
        /// <summary>
        ///     Start time of the sprint
        /// </summary>
        public DateTime StartTime { get; private set; }
        
        /// <summary>
        ///     End time of the sprint
        /// </summary>
        public DateTime EndTime { get; private set; }
        
        /// <summary>
        ///     Length of the sprint
        /// </summary>
        public int Length => GetSprintLength();

        public SprintAggregate(
            string productId,
            int id, 
            string name, 
            DateTime start, 
            DateTime end)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(name));
            }
            if (string.IsNullOrWhiteSpace(productId))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(productId));
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(name));

            ProductId = productId;
            Id = id;
            Name = name;
            StartTime = start;
            EndTime = end;
        }
        
        private int GetSprintLength()
        {
            var businessDays = StartTime.GetBusinessDaysUntil(EndTime);
            return businessDays;
        }
    }
}