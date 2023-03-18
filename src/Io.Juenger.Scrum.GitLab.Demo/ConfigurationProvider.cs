using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Configuration;

namespace Io.Juenger.Scrum.GitLab.Demo;

[ExcludeFromCodeCoverage]
public static class ConfigurationProvider
{
    public static IConfiguration GetConfiguration()
    {
        var configurationBuilder = new ConfigurationBuilder();
        configurationBuilder
            .AddJsonFile("appsettings.json", true)
            .AddCommandLine(Environment.GetCommandLineArgs());

        var configuration = configurationBuilder.Build();
        return configuration;
    } 
}