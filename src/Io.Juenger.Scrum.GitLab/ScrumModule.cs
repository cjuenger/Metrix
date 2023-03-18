using System.Diagnostics.CodeAnalysis;
using Autofac;
using Io.Juenger.Autoconf;
using Io.Juenger.Scrum.GitLab.Config;
using Io.Juenger.Scrum.GitLab.Contracts.Services;
using Io.Juenger.Scrum.GitLab.Factories.Domain;
using Io.Juenger.Scrum.GitLab.Factories.Infrastructure;
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
        }
    }
}