using Io.Juenger.Scrum.GitLab.Contracts.Entities;
using Io.Juenger.Scrum.GitLab.Contracts.Repositories;
using Io.Juenger.Scrum.GitLab.Contracts.Services;
using Io.Juenger.Scrum.GitLab.Contracts.Values;
using Io.Juenger.Scrum.GitLab.Services.Application;

namespace Io.Juenger.Scrum.GitLab.Services.Domain;

internal class ReleaseAggregateService : IReleaseAggregateService
{
    private readonly IMetricsService _metricsService;
    private readonly IItemsRepository _itemsRepository;
    private readonly IProductVelocityService _productVelocityService;

    public ReleaseAggregateService(
        IMetricsService metricsService,
        IItemsRepository itemsRepository,
        IProductVelocityService productVelocityService)
    {
        _metricsService = metricsService ?? throw new ArgumentNullException(nameof(metricsService));
        _itemsRepository = itemsRepository ?? throw new ArgumentNullException(nameof(itemsRepository));
        _productVelocityService = productVelocityService ?? throw new ArgumentNullException(nameof(productVelocityService));
    }
    
    public async Task<CompositionValue> CalculateCompositionAsync(
        string productId, 
        int releaseId,
        CancellationToken cancellationToken = default)
    {
        var itemEntities = await _itemsRepository
            .LoadProductItemsAsync(productId, ofReleaseId: releaseId, ct: cancellationToken);
        
        return _metricsService.CalculateComposition(itemEntities);
    }

    public async Task<BurnDownValue> CalculateBurnDown(
        string productId, 
        int releaseId,
        CancellationToken cancellationToken = default)
    {
        var itemEntities = await _itemsRepository
            .LoadProductItemsAsync(productId, ofReleaseId: releaseId, ct: cancellationToken);

        var velocityValue = await _productVelocityService.CalculateVelocityAsync(productId, cancellationToken);
        
        var burnDownValue = _metricsService.CalculateBurnDown(itemEntities, velocityValue);
        return burnDownValue;
    }

    public async Task<BurnUpValue> CalculateBurnUp(
        string productId, 
        int releaseId,
        CancellationToken cancellationToken = default)
    {
        var itemEntities = await _itemsRepository
            .LoadProductItemsAsync(productId, ofReleaseId: releaseId, ct: cancellationToken);
        
        return _metricsService.CalculateBurnUp(itemEntities);
    }

    public async Task<CycleTimesValue> CalculateCycleTime(
        string productId, 
        int releaseId,
        CancellationToken cancellationToken = default)
    {
        var itemEntities = await _itemsRepository
            .LoadProductItemsAsync(productId, ofReleaseId: releaseId, ct: cancellationToken);
        
        return _metricsService.CalculateCycleTime(itemEntities);
    }
    
    public async Task<ReleaseStatusValue> CalculateReleaseStatusAsync(
        string productId, 
        int releaseId,
        CancellationToken cancellationToken = default)
    {
        var itemEntities = await _itemsRepository
            .LoadProductItemsAsync(productId, ofReleaseId: releaseId, ct: cancellationToken);

        var openStoryPoints = itemEntities
            .OfType<StoryEntity>()
            .Where(s => s.State != WorkflowState.Closed)
            .Sum(s => s.StoryPoints ?? 0);

        var completedStoryPoints = itemEntities
            .OfType<StoryEntity>()
            .Where(s => s.State == WorkflowState.Closed)
            .Sum(s => s.StoryPoints ?? 0);

        return new ReleaseStatusValue(completedStoryPoints, openStoryPoints);
    }
}