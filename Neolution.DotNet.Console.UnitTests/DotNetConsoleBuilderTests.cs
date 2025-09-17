namespace Neolution.DotNet.Console.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Neolution.DotNet.Console.UnitTests.Fakes;
    using Neolution.DotNet.Console.UnitTests.Stubs;
    using Shouldly;
    using Xunit;

    /// <summary>
    /// Command line arguments grammar tests
    /// </summary>
    public class DotNetConsoleBuilderTests
    {
        /// <summary>
        /// The argument string for the internal check-deps command
        /// </summary>
        private const string CheckDependenciesArgumentString = "check-deps";

        /// <summary>
        /// Given a mistyped verb, when a default verb is defined, then should throw on console building.
        /// </summary>
        [Fact]
        public void GivenMistypedVerb_WhenDefaultVerbIsDefined_ThenShouldThrowOnBuilding()
        {
            // Arrange
            const string args = "eho"; // mistyped verb

            // Act

            // Assert
            Should.Throw(() => DotNetConsole.CreateBuilderWithReference(Assembly.GetAssembly(typeof(EchoCommand))!, args.Split(" ")), typeof(DotNetConsoleException));
        }

        /// <summary>
        /// Given a valid argument, when default verb is defined, then should not throw on creating.
        /// </summary>
        /// <param name="args">The arguments.</param>
        [Theory]
        [InlineData("")]
        [InlineData("echo")]
        [InlineData("--silent")]
        [InlineData("-s")]
        public void GivenValidArgument_WhenDefaultVerbIsDefined_ThenShouldNotThrowOnCreating(string args)
        {
            // Arrange
            var servicesAssembly = Assembly.GetAssembly(typeof(EchoCommand))!;

            // Act

            // Assert
            Should.NotThrow(() => DotNetConsole.CreateBuilderWithReference(servicesAssembly, args.Split(" ")));
        }

        /// <summary>
        /// Given a reserved argument name, when default verb is defined, then should not throw on creating.
        /// </summary>
        /// <param name="args">The arguments.</param>
        [Theory]
        [InlineData("help")]
        [InlineData("version")]
        [InlineData("--help")]
        [InlineData("--version")]
        [InlineData("help echo")]
        public void GivenReservedArgumentName_WhenDefaultVerbIsDefined_ThenShouldNotThrowOnCreating(string args)
        {
            // Arrange
            var servicesAssembly = Assembly.GetAssembly(typeof(EchoCommand))!;

            // Act

            // Assert
            Should.NotThrow(() => DotNetConsole.CreateBuilderWithReference(servicesAssembly, args.Split(" ")));
        }

        /// <summary>
        /// Given invalid arguments, when no default verb is defined, then should not throw on console creating.
        /// </summary>
        /// <param name="args">The arguments.</param>
        [Theory]
        [InlineData("")]
        [InlineData("verb-that-does-not-exist")]
        [InlineData("--option-that-does-not-exist")]
        [InlineData("-o")]
        public void GivenInvalidArguments_WhenNoDefaultVerbIsDefined_ThenShouldNotThrowOnCreating(string args)
        {
            // Arrange
            var servicesAssembly = Assembly.GetAssembly(typeof(EchoCommand))!;
            var verbTypes = new List<Type> { typeof(EchoOptions) }.ToArray();

            // Act

            // Assert
            Should.NotThrow(() => DotNetConsole.CreateBuilderWithReference(servicesAssembly, verbTypes, args.Split(" ")));
        }

        /// <summary>
        /// Given the check-deps builder, when registration is missing, then should throw on console building.
        /// </summary>
        [Fact]
        public void GivenCheckDependenciesCommand_WhenRegistrationIsMissing_ThenShouldThrow()
        {
            // Arrange
            var builder = DotNetConsole.CreateBuilderWithReference(Assembly.GetAssembly(typeof(DefaultCommand))!, [CheckDependenciesArgumentString]);

            // Intentionally only registering the transient service and not the scoped and singleton services.
            builder.Services.AddTransient<ITransientServiceStub, TransientServiceStub>();

            // Act

            // Assert
            Should.Throw(() => builder.Build(), typeof(AggregateException));
        }

        /// <summary>
        /// Given a null configuration delegate, when calling ConfigureAppConfiguration, then should throw ArgumentNullException.
        /// </summary>
        [Fact]
        public void GivenNullConfigurationDelegate_WhenCallingConfigureAppConfiguration_ThenShouldThrowArgumentNullException()
        {
            // Arrange
            var servicesAssembly = Assembly.GetAssembly(typeof(EchoCommand))!;
            var builder = DotNetConsole.CreateBuilderWithReference(servicesAssembly, []);

            // Act & Assert
            Should.Throw<ArgumentNullException>(() => builder.ConfigureAppConfiguration(null!));
        }

        /// <summary>
        /// Given a valid configuration delegate, when calling ConfigureAppConfiguration, then should return the builder instance.
        /// </summary>
        [Fact]
        public void GivenValidConfigurationDelegate_WhenCallingConfigureAppConfiguration_ThenShouldReturnBuilderInstance()
        {
            // Arrange
            var servicesAssembly = Assembly.GetAssembly(typeof(EchoCommand))!;
            var builder = DotNetConsole.CreateBuilderWithReference(servicesAssembly, []);

            // Act
            var result = builder.ConfigureAppConfiguration((context, config) => { });

            // Assert
            result.ShouldBeSameAs(builder);
        }

        /// <summary>
        /// Given a configuration delegate that adds a custom configuration source, when building the console, then should be able to access the custom configuration.
        /// </summary>
        [Fact]
        public void GivenCustomConfigurationDelegate_WhenBuildingConsole_ThenShouldApplyCustomConfiguration()
        {
            // Arrange
            const string customKey = "CustomTestKey";
            const string customValue = "CustomTestValue";

            var servicesAssembly = Assembly.GetAssembly(typeof(EchoCommand))!;
            var builder = DotNetConsole.CreateBuilderWithReference(servicesAssembly, [])
                .ConfigureAppConfiguration((context, config) =>
                {
                    config.AddInMemoryCollection(new Dictionary<string, string?>
                    {
                        { customKey, customValue },
                    });
                });

            // Act
            var console = builder.Build();

            // Assert
            var configuration = console.Services.GetRequiredService<IConfiguration>();
            configuration[customKey].ShouldBe(customValue);
        }

        /// <summary>
        /// Given multiple configuration delegates, when building the console, then should apply all delegates in order.
        /// </summary>
        [Fact]
        public void GivenMultipleConfigurationDelegates_WhenBuildingConsole_ThenShouldApplyAllDelegatesInOrder()
        {
            // Arrange
            const string firstKey = "FirstKey";
            const string firstValue = "FirstValue";
            const string secondKey = "SecondKey";
            const string secondValue = "SecondValue";
            const string overrideKey = "OverrideKey";
            const string initialValue = "InitialValue";
            const string finalValue = "FinalValue";

            var servicesAssembly = Assembly.GetAssembly(typeof(EchoCommand))!;
            var builder = DotNetConsole.CreateBuilderWithReference(servicesAssembly, [])
                .ConfigureAppConfiguration((context, config) =>
                {
                    config.AddInMemoryCollection(new Dictionary<string, string?>
                    {
                        { firstKey, firstValue },
                        { overrideKey, initialValue },
                    });
                })
                .ConfigureAppConfiguration((context, config) =>
                {
                    config.AddInMemoryCollection(new Dictionary<string, string?>
                    {
                        { secondKey, secondValue },
                        { overrideKey, finalValue }, // This should override the previous value
                    });
                });

            // Act
            var console = builder.Build();

            // Assert
            var configuration = console.Services.GetRequiredService<IConfiguration>();
            configuration[firstKey].ShouldBe(firstValue);
            configuration[secondKey].ShouldBe(secondValue);
            configuration[overrideKey].ShouldBe(finalValue); // Should be overridden by the second delegate
        }

        /// <summary>
        /// Given a configuration delegate that accesses the HostBuilderContext, when building the console, then should have access to environment information.
        /// </summary>
        [Fact]
        public void GivenConfigurationDelegateThatAccessesContext_WhenBuildingConsole_ThenShouldHaveAccessToEnvironmentInformation()
        {
            // Arrange
            const string environmentKey = "CurrentEnvironment";
            var servicesAssembly = Assembly.GetAssembly(typeof(EchoCommand))!;
            var builder = DotNetConsole.CreateBuilderWithReference(servicesAssembly, [])
                .ConfigureAppConfiguration((context, config) =>
                {
                    config.AddInMemoryCollection(new Dictionary<string, string?>
                    {
                        { environmentKey, context.HostingEnvironment.EnvironmentName },
                    });
                });

            // Act
            var console = builder.Build();

            // Assert
            var configuration = console.Services.GetRequiredService<IConfiguration>();
            configuration[environmentKey].ShouldNotBeNullOrEmpty();
            configuration[environmentKey].ShouldBe("Production"); // Default environment
        }
    }
}
