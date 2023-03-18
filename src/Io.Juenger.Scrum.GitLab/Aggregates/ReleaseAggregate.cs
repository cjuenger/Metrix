using Io.Juenger.Scrum.GitLab.Contracts.Aggregates;

namespace Io.Juenger.Scrum.GitLab.Aggregates
{
    internal class ReleaseAggregate : IReleaseAggregate
    {
        /// <summary>
        ///     Gets or Sets Id
        /// </summary>
        public int Id { get; }
        
        public string ProductId { get; }

        /// <summary>
        ///     Gets or Sets Title
        /// </summary>
        public string Title { get; private set; }

        /// <summary>
        ///     Gets or Sets Description
        /// </summary>
        public string Description { get; private set; }

        /// <summary>
        ///     Gets or Sets State
        /// </summary>
        public string State { get; private set; }

        /// <summary>
        ///     Gets or Sets CreatedAt
        /// </summary>
        public DateTime CreatedAt { get; private set; }

        /// <summary>
        ///     Gets or Sets UpdatedAt
        /// </summary>
        public DateTime UpdatedAt { get; private set; }

        /// <summary>
        ///     Gets or Sets DueDate
        /// </summary>
        public DateTime? DueDate { get; private set; }

        /// <summary>
        ///     Gets or Sets StartDate
        /// </summary>
        public DateTime? StartDate { get; private set; }

        /// <summary>
        ///     Gets or Sets Expired
        /// </summary>
        public bool? Expired { get; private set; }

        /// <summary>
        ///     Gets or Sets WebUrl
        /// </summary>
        public string WebUrl { get; private set; }
        
        public ReleaseAggregate(
            string productId,
            int id, 
            string title, 
            DateTime? startDate, 
            DateTime? dueDate)
        {
            if (string.IsNullOrWhiteSpace(productId))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(productId));
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(title));
            ProductId = productId;
            Id = id;
            Title = title ?? throw new ArgumentNullException(nameof(title));
            StartDate = startDate;
            DueDate = dueDate;
        }
    }
}