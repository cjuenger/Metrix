using System;

namespace Io.Juenger.Scrum.GitLab.Contracts.Values
{
    public class ReleaseInfoValue
    {
        /// <summary>
        ///     Gets or Sets Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        ///     Gets or Sets Title
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        ///     Gets or Sets Description
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        ///     Gets or Sets State
        /// </summary>
        public string State { get; set; }

        /// <summary>
        ///     Gets or Sets CreatedAt
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        ///     Gets or Sets UpdatedAt
        /// </summary>
        public DateTime UpdatedAt { get; set; }

        /// <summary>
        ///     Gets or Sets DueDate
        /// </summary>
        public DateTime? DueDate { get; set; }

        /// <summary>
        ///     Gets or Sets StartDate
        /// </summary>
        public DateTime? StartDate { get; set; }

        /// <summary>
        ///     Gets or Sets Expired
        /// </summary>
        public bool? Expired { get; set; }

        /// <summary>
        ///     Gets or Sets WebUrl
        /// </summary>
        public string WebUrl { get; set; }
    }
}