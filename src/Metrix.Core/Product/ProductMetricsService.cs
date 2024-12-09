using Metrix.Core.BacklogItem;
using Metrix.Core.Metrics;
using Metrix.Core.Metrics.Values;
using Metrix.Core.Sprint;
using Metrix.Core.Workflow;
using NMolecules.DDD;

namespace Metrix.Core.Product;

[Service]
internal class ProductMetricsService : IProductMetricsService, IProductVelocityService
{
    private readonly IMetricsService _metricsService;
    private readonly ISprintRepository _sprintRepository;
    private readonly IBacklogItemRepository _iBacklogItemRepository;
    private readonly IWorkflowFactory _workflowFactory;

    public ProductMetricsService(
        IMetricsService metricsService,
        ISprintRepository sprintRepository,
        IBacklogItemRepository iBacklogItemRepository,
        IWorkflowFactory workflowFactory)
    {
        _metricsService = metricsService ?? throw new ArgumentNullException(nameof(metricsService));
        _sprintRepository = sprintRepository ?? throw new ArgumentNullException(nameof(sprintRepository));
        _iBacklogItemRepository = iBacklogItemRepository ?? throw new ArgumentNullException(nameof(iBacklogItemRepository));
        _workflowFactory = workflowFactory ?? throw new ArgumentNullException(nameof(workflowFactory));
    }

    public async Task<Velocity> CalculateVelocityAsync(
        string productId, 
        CancellationToken cancellationToken = default)
    {
        var sprints = await _sprintRepository
            .LoadSprintsAsync(productId, ct: cancellationToken);

        var sprintVelocityValues = new List<SprintVelocity>();
        foreach (var sprint in sprints)
        {
            var items = await _iBacklogItemRepository
                .LoadProductItemsAsync(productId, ofSprint: sprint.Name, ct: cancellationToken);

            var totalStoryPoints = items
                .OfType<Story>()
                .Where(st => st.ClosedAt.HasValue)
                .Where(st => st.StoryPoints is > 0)
                .Sum(st => st.StoryPoints ?? 0);
            
            if(totalStoryPoints <= 0) continue;

            var sprintVelocity = new SprintVelocity(totalStoryPoints, sprint.Length);
            
            sprintVelocityValues.Add(sprintVelocity);
        }

        return _metricsService.CalculateVelocity(sprintVelocityValues);
    }

    public async Task<VelocityTrend> CalculateVelocityTrendAsync(
        string productId, 
        CancellationToken cancellationToken = default)
    {
        var sprints = await _sprintRepository
            .LoadSprintsAsync(productId, ct: cancellationToken);
        
        var sprintVelocityValues = new List<SprintVelocity>();
        foreach (var sprint in sprints)
        {
            var items = await _iBacklogItemRepository
                .LoadProductItemsAsync(productId, ofSprint: sprint.Name, ct: cancellationToken);

            var totalStoryPoints = items
                .OfType<Story>()
                .Sum(st => st.StoryPoints ?? 0);

            var sprintVelocity = new SprintVelocity(totalStoryPoints, sprint.Length);
            
            sprintVelocityValues.Add(sprintVelocity);
        }
        
        return _metricsService.CalculateVelocityTrend(sprintVelocityValues);
    }

    public async Task<Composition> CalculateCompositionAsync(
        string productId, 
        CancellationToken cancellationToken = default)
    {
        var items = await _iBacklogItemRepository
            .LoadProductItemsAsync(productId, ct: cancellationToken);
        
        return _metricsService.CalculateComposition(items);
    }

    public async Task<CompositionTrend> CalculateCompositionTrendAsync(
        string productId,
        CancellationToken cancellationToken = default)
    {
        var sprints = await _sprintRepository
            .LoadSprintsAsync(productId, ct: cancellationToken);

        var orderedSprints = sprints.OrderBy(sp => sp.StartTime);

        var compositionValues = new List<XyValue<string, Composition>>();
        foreach (var sprint in orderedSprints)
        {
            var itemsOfSprint = await _iBacklogItemRepository
                .LoadProductItemsAsync(productId, ofSprint: sprint.Name, ct: cancellationToken);

            var compositionValue = _metricsService.CalculateComposition(itemsOfSprint);
            compositionValues.Add(new XyValue<string, Composition>
            {
                X = sprint.Name,
                Y = compositionValue
            });
        }

        return new CompositionTrend(compositionValues);
    }

    public async Task<BurnDown> CalculateBurnDownAsync(
        string productId, 
        CancellationToken cancellationToken = default)
    {
        var sprints = await _sprintRepository
            .LoadSprintsAsync(productId, ct: cancellationToken);

        var orderedSprints = sprints.OrderBy(sp => sp.StartTime);
        
        var sprintVelocityValues = new List<SprintVelocity>();

        foreach (var sprint in orderedSprints)
        {
            var itemsOfSprint = await _iBacklogItemRepository
                .LoadProductItemsAsync(productId, ofSprint: sprint.Name, ct: cancellationToken);

            var totalStoryPoints = itemsOfSprint
                .OfType<Story>()
                .Sum(st => st.StoryPoints ?? 0);

            var sprintVelocity = new SprintVelocity(totalStoryPoints, sprint.Length);
            
            sprintVelocityValues.Add(sprintVelocity);
        }

        var velocityValue = _metricsService.CalculateVelocity(sprintVelocityValues);
        
        var productItems = await _iBacklogItemRepository
            .LoadProductItemsAsync(productId, ct: cancellationToken);
        
        var burnDownValue = _metricsService.CalculateBurnDown(productItems, velocityValue);
        return burnDownValue;
    }

    public async Task<BurnUp> CalculateBurnUpAsync(
        string productId, 
        CancellationToken cancellationToken = default)
    {
        var items = await _iBacklogItemRepository
            .LoadProductItemsAsync(productId, ct: cancellationToken);
        
        return _metricsService.CalculateBurnUp(items);
    }

    public async Task<CycleTimesValue> CalculateCycleTimesAsync(
        string productId, 
        CancellationToken cancellationToken = default)
    {
        var items = await _iBacklogItemRepository
            .LoadProductItemsAsync(productId, ct: cancellationToken);
        
        return _metricsService.CalculateCycleTime(items);
    }
    
    public async Task<Status> CalculateProductStatusAsync(
        string productId,
        CancellationToken cancellationToken = default)
    {
        var workflow = _workflowFactory.Workflow;
        
        var itemEntities = await _iBacklogItemRepository
            .LoadProductItemsAsync(productId, ct: cancellationToken);

        var openStoryPoints = itemEntities
            .OfType<Story>()
            .Where(s => s.WorkflowState.Name != workflow.WorkflowStates.Last().Name)
            .Sum(s => s.StoryPoints ?? 0);

        var completedStoryPoints = itemEntities
            .OfType<Story>()
            .Where(s => s.WorkflowState.Name == workflow.WorkflowStates.Last().Name)
            .Sum(s => s.StoryPoints ?? 0);

        return new Status(completedStoryPoints, openStoryPoints);
    }
}