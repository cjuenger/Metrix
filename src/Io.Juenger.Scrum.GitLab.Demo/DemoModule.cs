using System.Diagnostics.CodeAnalysis;
using Autofac;
using Io.Juenger.Autoconf;
using Io.Juenger.Scrum.GitLab;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ConfigurationProvider = Io.Juenger.Scrum.GitLab.Demo.ConfigurationProvider;

namespace io.juenger.Scrum.GitLab.Demo;

[ExcludeFromCodeCoverage]
public class DemoModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        RegisterConfigurations(builder);
        RegisterLogger(builder);
        RegisterDemos(builder);
        
        builder.RegisterModule<ScrumModule>();
    }

    private static void RegisterDemos(ContainerBuilder builder)
    {
        builder
            .RegisterType<ProductDemo>()
            .AsSelf()
            .SingleInstance();
    }

    private static void RegisterConfigurations(ContainerBuilder builder)
    {
        builder
            .Register(_ => ConfigurationProvider.GetConfiguration())
            .As<IConfiguration>()
            .SingleInstance();
        
        builder
            .RegisterConfiguration<DemoConfig>()
            .As<IDemoConfig>()
            .SingleInstance();
    }
    
    private static void RegisterLogger(ContainerBuilder builder)
    {
        var loggerFactory = LoggerFactory.Create(Configure);

        // Logging
        builder
            .RegisterInstance(loggerFactory)
            .As<ILoggerFactory>()
            .SingleInstance();
        builder
            .RegisterGeneric(typeof(Logger<>))
            .As(typeof(ILogger<>));
        builder
            .RegisterType<Logger<object>>()
            .As<ILogger<object>>()
            .As<ILogger>();
    }
    
    private static void Configure(ILoggingBuilder loggingBuilder)
    {
        loggingBuilder
            .ClearProviders()
            .AddDebug();
    }
}