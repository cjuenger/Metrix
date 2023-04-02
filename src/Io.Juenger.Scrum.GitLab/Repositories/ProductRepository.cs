using System.Text.RegularExpressions;
using Io.Juenger.GitLabClient.Model;
using Io.Juenger.Scrum.GitLab.Aggregates;
using Io.Juenger.Scrum.GitLab.Configs;
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
    private readonly IProductRepositoryConfig _config;
    private readonly ILogger<ProductRepository> _logger;

    public ProductRepository(
        IProjectApiFactory projectApiFactory, 
        IPaginationService paginationService,
        IProductRepositoryConfig config,
        ILogger<ProductRepository> logger)
    {
        _projectApiFactory = projectApiFactory ?? throw new ArgumentNullException(nameof(projectApiFactory));
        _paginationService = paginationService ?? throw new ArgumentNullException(nameof(paginationService));
        _config = config ?? throw new ArgumentNullException(nameof(config));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<IProductAggregate> LoadProductAsync(
        string productId,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(productId))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(productId));
        
        _logger.LogInformation("Loading product {ProductId}", productId);
        
        var projectApi = _projectApiFactory.ProjectApi;
        var project = await projectApi
            .GetProjectAsync(productId, cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        // ReSharper disable once InvertIf
        if (IsScrumProduct(project))
        {
            var product = CreateProductAggregate(project);
            return product;    
        }

        throw new ArgumentException(
            $"The product id {productId} does not reference a scrum product!", 
            nameof(productId));
    }
    
    public async Task<IEnumerable<IProductAggregate>> LoadProductsAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Loading all products...");
        
        var projectApi = _projectApiFactory.ProjectApi;
        var projects = await _paginationService
            .BrowseAllAsync(page => projectApi.GetProjectsAsync(page:page, cancellationToken: cancellationToken))
            .ConfigureAwait(false);

        var scrumProjects = projects.Where(IsScrumProduct);
        var products = scrumProjects.Select(CreateProductAggregate).ToList();
        return products;
    }

    private bool IsScrumProduct(Project project)
    {
        var rgx = new Regex(_config.ProductTypePattern);
        var match = rgx.Match(project.Description);
        return match.Success;
    }

    private ProductAggregate CreateProductAggregate(Project project)
    {
        var productName = GetProductName(project);
        var productVision = GetProductVision(project);
        var productKickoff = GetProductKickoff(project);
        var productDueDate = GetProductDueDate(project);

        return new ProductAggregate(
            project.Id,
            productName,
            productVision,
            productKickoff,
            productDueDate);
    }

    private string GetProductName(Project project)
    {
        var rgx = new Regex(_config.ProductNamePattern);
        var match = rgx.Match(project.Description);

        if (TryGetValueFromMatch(match, out var value)) return value;

        _logger.LogWarning("No product name found for GitLab project {GitLabProjectId}", project.Id);
        return $"{project.Name} {project.Id}";
    }
    
    private DateTime GetProductKickoff(Project project)
    {
        var rgx = new Regex(_config.ProductKickoffPattern);
        var match = rgx.Match(project.Description);

        if (TryGetValueFromMatch(match, out var value))
        {
            var time = DateTime.Parse(value);
            return time;
        }
        
        _logger.LogWarning("No kickoff time found for GitLab project {GitLabProjectId}", project.Id);
        return DateTime.MinValue;
    }
    
    private DateTime GetProductDueDate(Project project)
    {
        var rgx = new Regex(_config.ProductDueDatePattern);
        var match = rgx.Match(project.Description);

        if (TryGetValueFromMatch(match, out var value))
        {
            var time = DateTime.Parse(value);
            return time;
        }
        
        _logger.LogWarning("No end time found for GitLab project {GitLabProjectId}", project.Id);
        return DateTime.MaxValue;
    }

    private string GetProductVision(Project project)
    {
        var rgx = new Regex(_config.ProductVisionPattern);
        var match = rgx.Match(project.Description);

        if (TryGetValueFromMatch(match, out var value)) return value;

        _logger.LogWarning("No product vision found for GitLab project {GitLabProjectId}", project.Id);
        return string.Empty;
    }

    // ReSharper disable once SuggestBaseTypeForParameter
    private static bool TryGetValueFromMatch(Match match, out string value)
    {
        if (match.Success)
        {
            var split = match.Value.Split(":");
            value = split[^1].Trim();
            return true;
        }

        value = string.Empty;
        return false;
    }
}