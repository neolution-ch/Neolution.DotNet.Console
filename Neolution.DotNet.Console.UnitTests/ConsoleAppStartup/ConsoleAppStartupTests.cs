namespace Neolution.DotNet.Console.UnitTests.ConsoleAppStartup
{
    using Microsoft.Extensions.DependencyInjection;
    using Neolution.DotNet.Console.UnitTests.Common.Stubs;
    using Neolution.DotNet.Console.UnitTests.ConsoleAppStartup.Stubs;
    using Shouldly;
    using Xunit;

    public class ConsoleAppStartupTests
    {
        /// <summary>
        /// Given configured services with various (all possible) service lifetimes, when running the console application, then should it not throw.
        /// </summary>
        [Fact]
        public void GivenServicesWithVariousServiceLifetimes_WhenRunningConsoleApp_ThenShouldNotThrow()
        {
            // Arrange
            var console = DotNetConsole.CreateDefaultBuilder(InjectServicesWithVariousLifetimesOptionsStub.CommandName.Split(" "))
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

        /// <summary>
        /// Given not specifying a composition root, when building console application, then should throw a <see cref="ConsoleAppException"/>.
        /// </summary>
        [Fact]
        public void GivenNoCompositionRoot_WhenBuildingConsoleApp_ThenShouldThrowConsoleAppException()
        {
            // Arrange
            var builder = DotNetConsole.CreateDefaultBuilder(System.Array.Empty<string>());

            // Act

            // Assert
            Should.Throw<ConsoleAppException>(() => builder.Build());
        }

        /// <summary>
        /// Given a composition root with disallowed injected services, when building the console application, then should throw a <see cref="ConsoleAppException"/>.
        /// </summary>
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

        /// <summary>
        /// Given a composition root with allowed injected services, when building console application, then should not throw.
        /// </summary>
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
