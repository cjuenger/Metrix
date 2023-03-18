using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Io.Juenger.Scrum.GitLab.Contracts.Entities;
using Io.Juenger.Scrum.GitLab.Contracts.Services;
using Io.Juenger.Scrum.GitLab.Contracts.Values;
using Io.Juenger.Scrum.GitLab.Repositories;
using Io.Juenger.Scrum.GitLab.Services.Application;
using Io.Juenger.Scrum.GitLab.Values;

namespace Io.Juenger.Scrum.GitLab.Services.Domain;

internal class ProductAggregateService : IProductAggregateService, IProductVelocityService
{
    private readonly IMetricsService _metricsService;
    private readonly ISprintRepository _sprintRepository;
    private readonly IItemsRepository _itemsRepository;

    public ProductAggregateService(
        IMetricsService metricsService,
        ISprintRepository sprintRepository,
        IItemsRepository itemsRepository)
    {
        _metricsService = metricsService ?? throw new ArgumentNullException(nameof(metricsService));
        _sprintRepository = sprintRepository ?? throw new ArgumentNullException(nameof(sprintRepository));
        _itemsRepository = itemsRepository ?? throw new ArgumentNullException(nameof(itemsRepository));
    }
    
    #region Metrics

    public async Task<VelocityValue> CalculateVelocityAsync(
        string productId, 
        CancellationToken cancellationToken = default)
    {
        var sprints = await _sprintRepository
            .LoadSprintsAsync(productId, ct: cancellationToken);

        var sprintVelocityValues = new List<SprintVelocityValue>();
        foreach (var sprint in sprints)
        {
            var items = await _itemsRepository
                .LoadProductItemsAsync(productId, ofSprint: sprint.Name, ct: cancellationToken);

            var totalStoryPoints = items
                .OfType<StoryEntity>()
                .Where(st => st.ClosedAt.HasValue)
                .Where(st => st.StoryPoints is > 0)
                .Sum(st => st.StoryPoints ?? 0);
            
            if(totalStoryPoints <= 0) continue;

            var sprintVelocity = new SprintVelocityValue(totalStoryPoints, sprint.Length);
            
            sprintVelocityValues.Add(sprintVelocity);
        }

        return _metricsService.CalculateVelocity(sprintVelocityValues);
    }

    public async Task<VelocityTrendValue> CalculateVelocityTrendAsync(
        string productId, 
        CancellationToken cancellationToken = default)
    {
        var sprints = await _sprintRepository
            .LoadSprintsAsync(productId, ct: cancellationToken);
        
        var sprintVelocityValues = new List<SprintVelocityValue>();
        foreach (var sprint in sprints)
        {
            var items = await _itemsRepository
                .LoadProductItemsAsync(productId, ofSprint: sprint.Name, ct: cancellationToken);

            var totalStoryPoints = items
                .OfType<StoryEntity>()
                .Sum(st => st.StoryPoints ?? 0);

            var sprintVelocity = new SprintVelocityValue(totalStoryPoints, sprint.Length);
            
            sprintVelocityValues.Add(sprintVelocity);
        }
        
        return _metricsService.CalculateVelocityTrend(sprintVelocityValues);
    }

    public async Task<CompositionValue> CalculateCompositionAsync(
        string productId, 
        CancellationToken cancellationToken = default)
    {
        var items = await _itemsRepository
            .LoadProductItemsAsync(productId, ct: cancellationToken);
        
        return _metricsService.CalculateComposition(items);
    }

    public async Task<CompositionTrendValue> CalculateCompositionTrendAsync(
        string productId,
        CancellationToken cancellationToken = default)
    {
        var sprints = await _sprintRepository
            .LoadSprintsAsync(productId, ct: cancellationToken);

        var orderedSprints = sprints.OrderBy(sp => sp.StartTime);

        var compositionValues = new List<XyValue<string, CompositionValue>>();
        foreach (var sprint in orderedSprints)
        {
            var itemsOfSprint = await _itemsRepository
                .LoadProductItemsAsync(productId, ofSprint: sprint.Name, ct: cancellationToken);

            var compositionValue = _metricsService.CalculateComposition(itemsOfSprint);
            compositionValues.Add(new XyValue<string, CompositionValue>
            {
                X = sprint.Name,
                Y = compositionValue
            });
        }

        return new CompositionTrendValue(compositionValues);
    }

    public async Task<BurnDownValue> CalculateBurnDownAsync(
        string productId, 
        CancellationToken cancellationToken = default)
    {
        var sprints = await _sprintRepository
            .LoadSprintsAsync(productId, ct: cancellationToken);

        var orderedSprints = sprints.OrderBy(sp => sp.StartTime);
        
        var sprintVelocityValues = new List<SprintVelocityValue>();

        foreach (var sprint in orderedSprints)
        {
            var itemsOfSprint = await _itemsRepository
                .LoadProductItemsAsync(productId, ofSprint: sprint.Name, ct: cancellationToken);

            var totalStoryPoints = itemsOfSprint
                .OfType<StoryEntity>()
                .Sum(st => st.StoryPoints ?? 0);

            var sprintVelocity = new SprintVelocityValue(totalStoryPoints, sprint.Length);
            
            sprintVelocityValues.Add(sprintVelocity);
        }

        var velocityValue = _metricsService.CalculateVelocity(sprintVelocityValues);
        
        var productItems = await _itemsRepository
            .LoadProductItemsAsync(productId, ct: cancellationToken);
        
        var burnDownValue = _metricsService.CalculateBurnDown(productItems, velocityValue);
        return burnDownValue;
    }

    public async Task<BurnUpValue> CalculateBurnUpAsync(
        string productId, 
        CancellationToken cancellationToken = default)
    {
        var items = await _itemsRepository
            .LoadProductItemsAsync(productId, ct: cancellationToken);
        
        return _metricsService.CalculateBurnUp(items);
    }

    public async Task<CycleTimesValue> CalculateCycleTimeAsync(
        string productId, 
        CancellationToken cancellationToken = default)
    {
        var items = await _itemsRepository
            .LoadProductItemsAsync(productId, ct: cancellationToken);
        
        return _metricsService.CalculateCycleTime(items);
    }
    
    public async Task<ProductStatusValue> CalculateProductStatusAsync(
        string productId,
        CancellationToken cancellationToken = default)
    {
        var itemEntities = await _itemsRepository
            .LoadProductItemsAsync(productId, ct: cancellationToken);

        var openStoryPoints = itemEntities
            .OfType<StoryEntity>()
            .Where(s => s.State != WorkflowState.Closed)
            .Sum(s => s.StoryPoints ?? 0);

        var completedStoryPoints = itemEntities
            .OfType<StoryEntity>()
            .Where(s => s.State == WorkflowState.Closed)
            .Sum(s => s.StoryPoints ?? 0);

        return new ProductStatusValue(completedStoryPoints, openStoryPoints);
    }

    #endregion
}