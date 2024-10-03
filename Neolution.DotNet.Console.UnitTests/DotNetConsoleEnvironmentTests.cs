namespace Neolution.DotNet.Console.UnitTests
{
    using System;
    using System.Reflection;
    using System.Threading.Tasks;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Neolution.DotNet.Console.UnitTests.Fakes;
    using Neolution.DotNet.Console.UnitTests.Spies;
    using Neolution.DotNet.Console.UnitTests.Stubs;
    using Shouldly;
    using Xunit;

    /// <summary>
    /// Run tests for the <see cref="DotNetConsole"/>.
    /// </summary>
    [Collection("Non-Parallel Tests")]
    public class DotNetConsoleEnvironmentTests
    {
        /// <summary>
        /// Given an environment variable with an environment name, when running the console application, then the environment name of the app should match.
        /// </summary>
        /// <param name="environmentName">Name of the environment.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        [Theory]
        [InlineData("Development")]
        [InlineData("Staging")]
        [InlineData("Production")]
        public async Task GivenEnvironmentNameEnvironmentVariable_WhenRunningConsoleApp_ThenEnvironmentNameShouldMatch(string environmentName)
        {
            // Arrange
            Environment.SetEnvironmentVariable("DOTNET_ENVIRONMENT", environmentName);

            const string commandLineString = "default";
            var builder = DotNetConsole.CreateBuilderWithReference(Assembly.GetAssembly(typeof(DefaultCommand))!, commandLineString.Split(" "));

            if (builder.Environment.IsDevelopment())
            {
                // In Development, all services must be added because validation makes sure that each registration can be constructed by DI.
                builder.Services.AddTransient<ITransientServiceStub, TransientServiceStub>();
                builder.Services.AddScoped<IScopedServiceStub, ScopedServiceStub>();
                builder.Services.AddSingleton<ISingletonServiceStub, SingletonServiceStub>();
            }

            var logger = new UnitTestLogger();
            builder.Services.AddSingleton(typeof(IUnitTestLogger), logger);

            var console = builder.Build();

            // Act
            await console.RunAsync();

            // Assert
            builder.Environment.EnvironmentName.ShouldBe(environmentName);
            var environment = (IHostEnvironment)logger.LoggedObjects["environment"];
            environment.EnvironmentName.ShouldBe(environmentName);

            // Cleanup
            Environment.SetEnvironmentVariable("DOTNET_ENVIRONMENT", null);
        }
    }
}
