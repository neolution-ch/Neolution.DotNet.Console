namespace Neolution.DotNet.Console.UnitTests.ConsoleAppStartup
{
    using System.Reflection;
    using Microsoft.Extensions.DependencyInjection;
    using Neolution.DotNet.Console.UnitTests.Common;
    using Neolution.DotNet.Console.UnitTests.Common.Stubs;
    using Neolution.DotNet.Console.UnitTests.ConsoleAppGrammar.Fakes;
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
            var builder = DotNetConsole.CreateBuilderWithReference(Assembly.GetAssembly(typeof(DefaultCommand))!, UnitTestsConstants.InjectServicesWithVariousLifetimes.Split(" "));

            builder.Services.AddTransient<ITransientServiceStub, TransientServiceStub>();
            builder.Services.AddScoped<IScopedServiceStub, ScopedServiceStub>();
            builder.Services.AddSingleton<ISingletonServiceStub, SingletonServiceStub>();

            var console = builder.Build();

            // Act

            // Assert
            Should.NotThrow(() => console.RunAsync());
        }
    }
}
