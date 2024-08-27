namespace Neolution.DotNet.Console.UnitTests.ConsoleAppStartup
{
    using Microsoft.Extensions.DependencyInjection;
    using Neolution.DotNet.Console.UnitTests.Common;
    using Neolution.DotNet.Console.UnitTests.Common.Stubs;
    using Shouldly;
    using Xunit;

    /// <summary>
    /// Tests for the Console App Startup class.
    /// </summary>
    public class ConsoleAppStartupTests
    {
        /// <summary>
        /// Given configured services with various (all possible) service lifetimes, when running the console application, then should it not throw.
        /// </summary>
        [Fact]
        public void GivenServicesWithVariousServiceLifetimes_WhenRunningConsoleApp_ThenShouldNotThrow()
        {
            // Arrange
            var builder = DotNetConsole.CreateDefaultBuilder(UnitTestsConstants.InjectServicesWithVariousLifetimes.Split(" "));

            builder.Services.AddTransient<ITransientServiceStub, TransientServiceStub>();
            builder.Services.AddScoped<IScopedServiceStub, ScopedServiceStub>();
            builder.Services.AddSingleton<ISingletonServiceStub, SingletonServiceStub>();

            var console = builder.Build();

            // Act

            // Assert
            Should.NotThrow(() => console.RunAsync());
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
        /// Given a composition root with allowed injected services, when building console application, then should not throw.
        /// </summary>
        [Fact]
        public void GivenCompositionRootWithAllowedInjections_WhenBuildingConsoleApp_ThenShouldNotThrow()
        {
            // Arrange
            var builder = DotNetConsole.CreateDefaultBuilder(System.Array.Empty<string>());

            // Act

            // Assert
            Should.NotThrow(() => builder.Build());
        }
    }
}
