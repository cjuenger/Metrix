using Autofac;
using Io.Juenger.Autoconf;
using Io.Juenger.Scrum.Metrix.WebUI.Configs;

namespace Io.Juenger.Scrum.Metrix.WebUI;

public class WebUiModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        RegisterConfigurations(builder);
    }

    private static void RegisterConfigurations(ContainerBuilder builder)
    {
        builder
            .RegisterConfiguration<ProductConfig>()
            .AsSelf()
            .SingleInstance();
    }
}