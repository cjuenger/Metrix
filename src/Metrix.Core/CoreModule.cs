using Autofac;
using Metrix.Core.Metrics;
using Metrix.Core.Product;
using Metrix.Core.Sprint;

namespace Metrix.Core;

public class CoreModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        RegisterProduct(builder);
        RegisterMetrics(builder);
    }

    private static void RegisterProduct(ContainerBuilder builder)
    {
        builder
            .RegisterType<ProductMetricsService>()
            .As<IProductMetricsService>()
            .As<IProductVelocityService>()
            .SingleInstance();
    }

    private static void RegisterSprint(ContainerBuilder builder)
    {
        builder
            .RegisterType<SprintMetricsService>()
            .As<ISprintMetricsService>()
            .SingleInstance();
    }
    
    private static void RegisterMetrics(ContainerBuilder builder)
    {
        builder
            .RegisterType<MetricsService>()
            .As<IMetricsService>()
            .SingleInstance();
        
        
    }
}