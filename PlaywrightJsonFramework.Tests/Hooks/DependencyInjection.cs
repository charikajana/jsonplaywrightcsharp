using Microsoft.Extensions.DependencyInjection;
using Reqnroll;
using Reqnroll.Microsoft.Extensions.DependencyInjection;

namespace PlaywrightJsonFramework.Tests.Hooks;

/// <summary>
/// Dependency Injection setup for Reqnroll
/// Registers services that will be injected into step definitions
/// </summary>
public static class DependencyInjection
{
    [ScenarioDependencies]
    public static IServiceCollection CreateServices()
    {
        var services = new ServiceCollection();

        // Register any shared services here
        // For now, we're using simple constructor injection via ScenarioContext
        // which Reqnroll provides by default

        // Example: services.AddSingleton<IMyService, MyService>();

        return services;
    }
}
