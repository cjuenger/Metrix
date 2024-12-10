using Metrix.Core.BacklogItem;
using Metrix.Core.Metrics;
using Metrix.Core.Metrics.Values;
using Metrix.Core.Product;
using Metrix.Core.ProductCalendar;
using Metrix.Core.Workflow;
using NMolecules.DDD;

namespace Metrix.Core.Sprint;

[Service]
internal class SprintMetricsService : ISprintMetricsService
{
    private readonly IMetricsService _metricsService;
    private readonly ISprintRepository _sprintRepository;
    private readonly IBacklogItemRepository _iBacklogItemRepository;
    private readonly IProductVelocityService _productVelocityService;
    private readonly IProductCalendarRepository _productCalendarRepository;
    private readonly IWorkflowFactory _workflowFactory;

    public SprintMetricsService(
        IMetricsService metricsService,
        ISprintRepository sprintRepository,
        IBacklogItemRepository iBacklogItemRepository,
        IProductVelocityService productVelocityService,
        IProductCalendarRepository productCalendarRepository,
        IWorkflowFactory workflowFactory)
    {
        _metricsService = metricsService ?? throw new ArgumentNullException(nameof(metricsService));
        _sprintRepository = sprintRepository ?? throw new ArgumentNullException(nameof(sprintRepository));
        _iBacklogItemRepository = iBacklogItemRepository ?? throw new ArgumentNullException(nameof(iBacklogItemRepository));
        _productVelocityService = productVelocityService ?? throw new ArgumentNullException(nameof(productVelocityService));
        _productCalendarRepository = productCalendarRepository ?? throw new ArgumentNullException(nameof(productCalendarRepository));
        _workflowFactory = workflowFactory ?? throw new ArgumentNullException(nameof(workflowFactory));
    }
    
    public async Task<Composition> CalculateCompositionAsync(
        string productId, 
        int sprintId, 
        CancellationToken cancellationToken = default)
    {
        var sprint = await _sprintRepository
            .LoadSprintByIdAsync(productId, sprintId, ct: cancellationToken);

        var items = await _iBacklogItemRepository
            .LoadProductItemsAsync(productId, ofSprint: sprint.Name, ct: cancellationToken);
        
        return _metricsService.CalculateComposition(items);
    }

    public async Task<BurnDown> CalculateBurnDownAsync(
        string productId, 
        int sprintId, 
        CancellationToken cancellationToken = default)
    {
        var sprint = await _sprintRepository
            .LoadSprintByIdAsync(productId, sprintId, ct: cancellationToken);
        
        var itemsOfSprint = await _iBacklogItemRepository
            .LoadProductItemsAsync(productId, ofSprint: sprint.Name, ct: cancellationToken);

        // NOTE, the burn down of a sprint is only relevant for the scope of the sprint.
        // Thus for this metric the creation time of each item must be changed to the start date of the sprint!
        foreach (var item in itemsOfSprint) item.CreatedAt = sprint.StartTime;
        
        var velocityValue = await _productVelocityService.CalculateVelocityAsync(productId, cancellationToken);

        var productCalendar = await _productCalendarRepository.LoadProductCalendarAsync(productId, cancellationToken);
        
        return _metricsService.CalculateBurnDown(itemsOfSprint, velocityValue, productCalendar);
    }

    public async Task<BurnUp> CalculateBurnUpAsync(
        string productId, 
        int sprintId, 
        CancellationToken cancellationToken = default)
    {
        var sprint = await _sprintRepository
            .LoadSprintByIdAsync(productId, sprintId, ct: cancellationToken);
        
        var itemsOfSprint = await _iBacklogItemRepository
            .LoadProductItemsAsync(productId, ofSprint: sprint.Name, ct: cancellationToken);
        
        return _metricsService.CalculateBurnUp(itemsOfSprint);
    }

    public async Task<CycleTimesValue> CalculateCycleTimesAsync(
        string productId, 
        int sprintId, 
        CancellationToken cancellationToken = default)
    {
        var sprint = await _sprintRepository
            .LoadSprintByIdAsync(productId, sprintId, ct: cancellationToken);
        
        var itemsOfSprint = await _iBacklogItemRepository
            .LoadProductItemsAsync(productId, ofSprint: sprint.Name, ct: cancellationToken);
        
        return _metricsService.CalculateCycleTime(itemsOfSprint);
    }

    public async Task<Status> CalculateSprintStatusAsync(
        string productId, 
        int sprintId,
        CancellationToken cancellationToken = default)
    {
        var sprintAggregate = await _sprintRepository
            .LoadSprintByIdAsync(productId, sprintId, ct: cancellationToken);

        var itemEntities = await _iBacklogItemRepository
            .LoadProductItemsAsync(productId, ofSprint: sprintAggregate.Name, ct: cancellationToken);

        var workflow = _workflowFactory.Workflow;
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