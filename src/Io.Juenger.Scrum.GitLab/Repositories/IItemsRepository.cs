using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Io.Juenger.Scrum.GitLab.Contracts.Entities;

namespace Io.Juenger.Scrum.GitLab.Repositories
{
    internal interface IItemsRepository
    {
        Task<IReadOnlyCollection<ItemEntity>> LoadProductItemsAsync(
            string projectId, 
            string? ofSprint = null,
            int? ofReleaseId = null,
            CancellationToken ct = default);
    }
}