using AutoMapper;
using Io.Juenger.Scrum.GitLab.Contracts.Aggregates;
using Io.Juenger.Scrum.GitLab.Contracts.Repositories;
using Io.Juenger.Scrum.GitLab.Factories.Infrastructure;
using Io.Juenger.Scrum.GitLab.Services.Infrastructure;
using Microsoft.Extensions.Logging;

namespace Io.Juenger.Scrum.GitLab.Repositories;

internal class ProductRepository : IProductRepository
{
    private readonly IProjectApiFactory _projectApiFactory;
    private readonly IPaginationService _paginationService;
    private readonly IMapper _mapper;
    private readonly ILogger<ProductRepository> _logger;

    public ProductRepository(
        IProjectApiFactory projectApiFactory, 
        IPaginationService paginationService,
        IMapper mapper,
        ILogger<ProductRepository> logger)
    {
        _projectApiFactory = projectApiFactory ?? throw new ArgumentNullException(nameof(projectApiFactory));
        _paginationService = paginationService ?? throw new ArgumentNullException(nameof(paginationService));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<IProductAggregate> LoadProductAsync(
        string productId,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Loading product {ProductId}", productId);
        
        var projectApi = _projectApiFactory.ProjectApi;
        var project = await projectApi
            .GetProjectAsync(productId, cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        var product = _mapper.Map<IProductAggregate>(project);
        return product;
    }
    
    public async Task<IEnumerable<IProductAggregate>> LoadProductsAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Loading all products...");
        
        // TODO: What is the rule for a repository to be a product?
        var projectApi = _projectApiFactory.ProjectApi;
        var projects = await _paginationService
            .BrowseAllAsync(page => projectApi.GetProjectsAsync(page:page, cancellationToken: cancellationToken))
            .ConfigureAwait(false);

        var products = _mapper.Map<IEnumerable<IProductAggregate>>(projects);

        return products;
    }
}