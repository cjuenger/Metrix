using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Io.Juenger.Scrum.GitLab.Contracts.Entities;
using Io.Juenger.Scrum.GitLab.Contracts.Services;
using Io.Juenger.Scrum.GitLab.Contracts.Values;
using Io.Juenger.Scrum.GitLab.Repositories;
using Io.Juenger.Scrum.GitLab.Services.Application;

namespace Io.Juenger.Scrum.GitLab.Services.Domain;

internal class SprintAggregateService : ISprintAggregateService
{
    private readonly IMetricsService _metricsService;
    private readonly ISprintRepository _sprintRepository;
    private readonly IItemsRepository _itemsRepository;
    private readonly IProductVelocityService _productVelocityService;

    public SprintAggregateService(
        IMetricsService metricsService,
        ISprintRepository sprintRepository,
        IItemsRepository itemsRepository,
        IProductVelocityService productVelocityService)
    {
        _metricsService = metricsService ?? throw new ArgumentNullException(nameof(metricsService));
        _sprintRepository = sprintRepository ?? throw new ArgumentNullException(nameof(sprintRepository));
        _itemsRepository = itemsRepository ?? throw new ArgumentNullException(nameof(itemsRepository));
        _productVelocityService = productVelocityService ?? throw new ArgumentNullException(nameof(productVelocityService));
    }
    
    public async Task<CompositionValue> CalculateCompositionAsync(
        string productId, 
        int sprintId, 
        CancellationToken cancellationToken = default)
    {
        var sprint = await _sprintRepository
            .LoadSprintByIdAsync(productId, sprintId, ct: cancellationToken);

        var items = await _itemsRepository
            .LoadProductItemsAsync(productId, ofSprint: sprint.Name, ct: cancellationToken);
        
        return _metricsService.CalculateComposition(items);
    }

    public async Task<BurnDownValue> CalculateBurnDownAsync(
        string productId, 
        int sprintId, 
        CancellationToken cancellationToken = default)
    {
        var sprint = await _sprintRepository
            .LoadSprintByIdAsync(productId, sprintId, ct: cancellationToken);
        
        var itemsOfSprint = await _itemsRepository
            .LoadProductItemsAsync(productId, ofSprint: sprint.Name, ct: cancellationToken);

        var velocityValue = await _productVelocityService.CalculateVelocityAsync(productId, cancellationToken);

        return _metricsService.CalculateBurnDown(itemsOfSprint, velocityValue);
    }

    public async Task<BurnUpValue> CalculateBurnUpAsync(
        string productId, 
        int sprintId, 
        CancellationToken cancellationToken = default)
    {
        var sprint = await _sprintRepository
            .LoadSprintByIdAsync(productId, sprintId, ct: cancellationToken);
        
        var itemsOfSprint = await _itemsRepository
            .LoadProductItemsAsync(productId, ofSprint: sprint.Name, ct: cancellationToken);
        
        return _metricsService.CalculateBurnUp(itemsOfSprint);
    }

    public async Task<CycleTimesValue> CalculateCycleTimeAsync(
        string productId, 
        int sprintId, 
        CancellationToken cancellationToken = default)
    {
        var sprint = await _sprintRepository
            .LoadSprintByIdAsync(productId, sprintId, ct: cancellationToken);
        
        var itemsOfSprint = await _itemsRepository
            .LoadProductItemsAsync(productId, ofSprint: sprint.Name, ct: cancellationToken);
        
        return _metricsService.CalculateCycleTime(itemsOfSprint);
    }

    public async Task<SprintStatusValue> CalculateSprintStatusAsync(
        string productId, 
        int sprintId,
        CancellationToken cancellationToken = default)
    {
        var sprintAggregate = await _sprintRepository
            .LoadSprintByIdAsync(productId, sprintId, ct: cancellationToken);

        var itemEntities = await _itemsRepository
            .LoadProductItemsAsync(productId, ofSprint: sprintAggregate.Name, ct: cancellationToken);

        var openStoryPoints = itemEntities
            .OfType<StoryEntity>()
            .Where(s => s.State != WorkflowState.Closed)
            .Sum(s => s.StoryPoints ?? 0);

        var completedStoryPoints = itemEntities
            .OfType<StoryEntity>()
            .Where(s => s.State == WorkflowState.Closed)
            .Sum(s => s.StoryPoints ?? 0);

        return new SprintStatusValue(completedStoryPoints, openStoryPoints);
    }
}