using System;

namespace Io.Juenger.Scrum.GitLab.Contracts.Aggregates;

public interface IReleaseAggregate
{
    /// <summary>
    ///     Gets or Sets Id
    /// </summary>
    int Id { get; }

    /// <summary>
    ///     Gets or Sets Title
    /// </summary>
    string Title { get; }

    /// <summary>
    ///     Gets or Sets Description
    /// </summary>
    string Description { get; }

    /// <summary>
    ///     Gets or Sets State
    /// </summary>
    string State { get; }

    /// <summary>
    ///     Gets or Sets CreatedAt
    /// </summary>
    DateTime CreatedAt { get; }

    /// <summary>
    ///     Gets or Sets UpdatedAt
    /// </summary>
    DateTime UpdatedAt { get; }

    /// <summary>
    ///     Gets or Sets DueDate
    /// </summary>
    DateTime? DueDate { get; }

    /// <summary>
    ///     Gets or Sets StartDate
    /// </summary>
    DateTime? StartDate { get; }

    /// <summary>
    ///     Gets or Sets Expired
    /// </summary>
    bool? Expired { get; }

    /// <summary>
    ///     Gets or Sets WebUrl
    /// </summary>
    string WebUrl { get; }

    string ProductId { get; }
}