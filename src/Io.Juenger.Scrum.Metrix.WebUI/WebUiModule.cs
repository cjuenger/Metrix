using Autofac;
using Io.Juenger.Autoconf;
using Io.Juenger.Scrum.GitLab;
using Io.Juenger.Scrum.Metrix.WebUI.Configs;

namespace Io.Juenger.Scrum.Metrix.WebUI;

public class WebUiModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        RegisterConfigurations(builder);
        RegisterModules(builder);

        builder
            .RegisterType<Context>()
            .As<IContext>()
            .SingleInstance();
    }

    private static void RegisterConfigurations(ContainerBuilder builder)
    {
        builder
            .RegisterConfiguration<ProductConfig>()
            .AsSelf()
            .SingleInstance();
    }

    private static void RegisterModules(ContainerBuilder builder)
    {
        builder.RegisterModule<ScrumModule>();
    }
}