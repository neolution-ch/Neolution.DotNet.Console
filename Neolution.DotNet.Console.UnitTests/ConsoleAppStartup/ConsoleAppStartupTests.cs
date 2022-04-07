using Microsoft.Extensions.DependencyInjection;
using Neolution.DotNet.Console.UnitTests.Common.Stubs;
using Neolution.DotNet.Console.UnitTests.ConsoleAppStartup.Fakes;
using Neolution.DotNet.Console.UnitTests.ConsoleAppStartup.Stubs;
using Shouldly;
using Xunit;

namespace Neolution.DotNet.Console.UnitTests.ConsoleAppStartup
{
    public class ConsoleAppStartupTests
    {
        [Fact]
        public void GivenServicesWithVariousServiceLifetimes_WhenRunningConsoleApp_ThenShouldNotThrow()
        {
            // Arrange
            var console = DotNetConsole.CreateDefaultBuilder(InjectServicesWithVariousLifetimesOptions.CommandName.Split(" "))
                .ConfigureServices((context, services) =>
                {
                    services.AddTransient<ITransientServiceStub, TransientServiceStub>();
                    services.AddScoped<IScopedServiceStub, ScopedServiceStub>();
                    services.AddSingleton<ISingletonServiceStub, SingletonServiceStub>();
                })
                .UseCompositionRoot<CompositionRootStub>()
                .Build();

            // Act

            // Assert
            Should.NotThrow(() => console.Run());
        }

        [Fact]
        public void GivenNoCompositionRoot_WhenBuildingConsoleApp_ThenShouldThrowConsoleAppException()
        {
            // Arrange
            var builder = DotNetConsole.CreateDefaultBuilder(System.Array.Empty<string>());

            // Act

            // Assert
            Should.Throw<ConsoleAppException>(() => builder.Build());
        }

        [Fact]
        public void GivenCompositionRootWithDisallowedInjections_WhenBuildingConsoleApp_ThenShouldThrowConsoleAppException()
        {
            // Arrange
            var builder = DotNetConsole.CreateDefaultBuilder(System.Array.Empty<string>())
                .UseCompositionRoot<CompositionRootWithDisallowedInjectedServicesStub>();

            // Act

            // Assert
            Should.Throw<ConsoleAppException>(() => builder.Build());
        }

        [Fact]
        public void GivenCompositionRootWithAllowedInjections_WhenBuildingConsoleApp_ThenShouldNotThrow()
        {
            // Arrange
            var builder = DotNetConsole.CreateDefaultBuilder(System.Array.Empty<string>())
                .UseCompositionRoot<CompositionRootWithInjectedServicesStub>();

            // Act

            // Assert
            Should.NotThrow(() => builder.Build());
        }
    }
}
