using System;

namespace Io.Juenger.Scrum.GitLab.Contracts.Aggregates;

public interface ISprintAggregate
{
    /// <summary>
    ///     Id of the sprint
    /// </summary>
    int Id { get; }
    
    /// <summary>
    ///     Name of the sprint
    /// </summary>
    string Name { get; }
    
    /// <summary>
    ///     Start time of the sprint
    /// </summary>
    DateTime StartTime { get; }
    
    /// <summary>
    ///     End time of the sprint
    /// </summary>
    DateTime EndTime { get; }
    
    /// <summary>
    ///     Length of the sprint
    /// </summary>
    int Length { get; }

    string ProductId { get; }
}