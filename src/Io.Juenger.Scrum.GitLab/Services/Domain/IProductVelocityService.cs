using System.Threading;
using System.Threading.Tasks;
using Io.Juenger.Scrum.GitLab.Contracts.Values;

namespace Io.Juenger.Scrum.GitLab.Services.Domain;

internal interface IProductVelocityService
{
    Task<VelocityValue> CalculateVelocityAsync(string productId, CancellationToken cancellationToken = default);
}