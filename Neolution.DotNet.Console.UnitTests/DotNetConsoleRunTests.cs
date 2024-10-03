namespace Neolution.DotNet.Console.UnitTests
{
    using System;
    using System.Reflection;
    using JetBrains.Annotations;
    using Microsoft.Extensions.DependencyInjection;
    using Neolution.DotNet.Console.UnitTests.Fakes;
    using Neolution.DotNet.Console.UnitTests.Setup;
    using Neolution.DotNet.Console.UnitTests.Spies;
    using Neolution.DotNet.Console.UnitTests.Stubs;
    using Shouldly;
    using Xunit;

    /// <summary>
    /// Run tests for the Console App.
    /// </summary>
    [Collection("Production Environment Tests")]
    public class DotNetConsoleRunTests
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

        /// <summary>
        /// Given valid arguments, when no default verb is defined, then should throw on console building.
        /// </summary>
        /// <param name="args">The arguments.</param>
        [Theory]
        [InlineData("echo")]
        [InlineData("echo --message hello")]
        [InlineData("echo --message hello --upper")]
        [InlineData("echo --message hello -u")]
        [InlineData("echo -u --message hello")]
        [InlineData("help")]
        [InlineData("version")]
        [InlineData("--help")]
        [InlineData("--version")]
        [InlineData("help echo")]
        [InlineData("echo --help")]
        public void GivenValidArguments_WhenNoDefaultVerbIsDefined_ThenShouldNotThrow([NotNull] string args)
        {
            if (args == null)
            {
                throw new ArgumentNullException(nameof(args));
            }

            // Arrange
            var builder = DotNetConsole.CreateBuilderWithReference(Assembly.GetAssembly(typeof(DefaultCommand))!, args.Split(" "));

            var logger = new UnitTestLogger();
            builder.Services.AddSingleton(typeof(IUnitTestLogger), logger);

            var console = builder.Build();

            // Act

            // Assert
            Should.NotThrow(() => console.RunAsync());
        }
    }
}
