using NMolecules.DDD;

namespace Metrix.Core.Sprint
{
    [AggregateRoot]
    public class Sprint
    {
        public Sprint(
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

        public int BusinessDaysOfSprint(ProductCalendar productCalendar)
        {
            var start = new BusinessDay(StartTime);
            var end = new BusinessDay(EndTime);
            return productCalendar.BusinessDaysWithin(start, end);
        }
        
        private int GetSprintLength()
        {
            var businessDays = StartTime.GetBusinessDaysUntil(EndTime);
            return businessDays;
        }
    }
}