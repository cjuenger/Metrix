using System.Diagnostics.CodeAnalysis;
using Autofac;
using AutoMapper;
using Io.Juenger.Autoconf;
using Io.Juenger.Scrum.GitLab.Configs;
using Io.Juenger.Scrum.GitLab.Contracts.Repositories;
using Io.Juenger.Scrum.GitLab.Contracts.Services;
using Io.Juenger.Scrum.GitLab.Factories.Application;
using Io.Juenger.Scrum.GitLab.Factories.Domain;
using Io.Juenger.Scrum.GitLab.Factories.Infrastructure;
using Io.Juenger.Scrum.GitLab.Mappings;
using Io.Juenger.Scrum.GitLab.Repositories;
using Io.Juenger.Scrum.GitLab.Services.Application;
using Io.Juenger.Scrum.GitLab.Services.Domain;
using Io.Juenger.Scrum.GitLab.Services.Infrastructure;

namespace Io.Juenger.Scrum.GitLab
{
    [ExcludeFromCodeCoverage]
    public class ScrumModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            RegisterRepositories(builder);
            RegisterFactories(builder);
            RegisterServices(builder);
            RegisterConfigurations(builder);
            RegisterMappers(builder);
        }

        private static void RegisterRepositories(ContainerBuilder builder)
        {
            builder
                .RegisterType<ItemsRepository>()
                .As<IItemsRepository>()
                .SingleInstance();

            builder
                .RegisterType<SprintRepository>()
                .As<ISprintRepository>()
                .SingleInstance();

            builder
                .RegisterType<ReleaseRepository>()
                .As<IReleaseRepository>()
                .SingleInstance();

            builder
                .RegisterType<ProductRepository>()
                .As<IProductRepository>()
                .SingleInstance();
        }

        private static void RegisterFactories(ContainerBuilder builder)
        {
            builder
                .RegisterType<ProductFactory>()
                .As<IProductFactory>()
                .SingleInstance();

            builder
                .RegisterType<ProjectApiFactory>()
                .As<IProjectApiFactory>()
                .SingleInstance();

            builder
                .RegisterType<WorkflowFactory>()
                .As<IWorkflowFactory>()
                .SingleInstance();
        }

        private static void RegisterServices(ContainerBuilder builder)
        {
            builder
                .RegisterType<ProductAggregateService>()
                .As<IProductAggregateService>()
                .As<IProductVelocityService>()
                .SingleInstance();

            builder
                .RegisterType<ReleaseAggregateService>()
                .As<IReleaseAggregateService>()
                .SingleInstance();

            builder
                .RegisterType<SprintAggregateService>()
                .As<ISprintAggregateService>()
                .SingleInstance();
            
            builder
                .RegisterType<MetricsService>()
                .As<IMetricsService>()
                .SingleInstance();

            builder
                .RegisterType<ItemParserService>()
                .As<IItemParserService>()
                .SingleInstance();

            builder
                .RegisterType<StorySeriesService>()
                .As<IStorySeriesService>()
                .SingleInstance();

            builder
                .RegisterType<PaginationService>()
                .As<IPaginationService>()
                .SingleInstance();
        }

        private static void RegisterConfigurations(ContainerBuilder builder)
        {
            builder
                .RegisterConfiguration<GitLabClientConfig>()
                .As<IGitLabClientConfig>()
                .SingleInstance();

            builder
                .RegisterConfiguration<ItemParserConfig>()
                .As<IItemParserConfig>()
                .SingleInstance();

            builder
                .RegisterConfiguration<SprintRepositoryConfig>()
                .As<ISprintRepositoryConfig>()
                .SingleInstance();

            builder
                .RegisterConfiguration<WorkflowConfig>()
                .As<IWorkflowConfig>()
                .SingleInstance();
        }
        
        private static void RegisterMappers(ContainerBuilder builder)
        {
            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<GitLabMappingProfile>();
            });
            var mapper = new Mapper(mapperConfig);
            builder.Register(_ => mapper).As<IMapper>();
        }
    }
}