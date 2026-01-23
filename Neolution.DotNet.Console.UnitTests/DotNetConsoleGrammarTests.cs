namespace Neolution.DotNet.Console.UnitTests
{
    using System.Reflection;
    using System.Threading.Tasks;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;
    using Neolution.DotNet.Console.Abstractions;
    using Neolution.DotNet.Console.UnitTests.Fakes;
    using Neolution.DotNet.Console.UnitTests.Spies;
    using Shouldly;
    using Xunit;

    /// <summary>
    /// Command line arguments grammar tests
    /// </summary>
    [Collection("Production Environment Tests")]
    public class DotNetConsoleGrammarTests
    {
        /// <summary>
        /// When calling the console app without specifying a verb, it should run the command of the default verb.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task GivenBuiltConsoleApp_WhenCallingWithoutVerb_ThenShouldRunDefaultVerb()
        {
            // Arrange
            const string args = "";
            var logger = new UnitTestLogger();
            var console = CreateConsoleAppWithLogger(args, logger);

            // Act
            await console.RunAsync();

            // Assert
            logger.LoggedObjects["options"].ShouldBeOfType<DefaultOptions>();
        }

        /// <summary>
        /// Given the built console application, when specifying the echo verb without passing a value, then it should still work, but without returning a message .
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task GivenBuiltConsoleApp_WhenCallingVerbWithoutValue_ThenShouldReturnExpectedResult()
        {
            // Arrange
            const string args = "echo";
            var logger = new UnitTestLogger();
            var console = CreateConsoleAppWithLogger(args, logger);

            // Act
            await console.RunAsync();

            // Assert
            var options = (EchoOptions)logger.LoggedObjects["options"];
            options.Message.ShouldBeNull();
        }

        /// <summary>
        /// Given the built console application, when specifying the echo verb and passing a value, then it should return the value as the message.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task GivenBuiltConsoleApp_WhenCallingVerbWithValue_ThenShouldReturnExpectedResult()
        {
            // Arrange
            const string args = "echo hello";
            var logger = new UnitTestLogger();
            var console = CreateConsoleAppWithLogger(args, logger);

            // Act
            await console.RunAsync();

            // Assert
            var options = (EchoOptions)logger.LoggedObjects["options"];
            options.Message.ShouldBe("hello");
        }

        /// <summary>
        /// Given the built console application, when specifying the echo verb with a switch option, then the switch option should be <c>true</c>.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task GivenBuiltConsoleApp_WhenCallingVerbWithSwitchOption_ThenShouldReturnExpectedResult()
        {
            // Arrange
            const string args = "echo hello --upper";
            var logger = new UnitTestLogger();
            var console = CreateConsoleAppWithLogger(args, logger);

            // Act
            await console.RunAsync();

            // Assert
            var options = (EchoOptions)logger.LoggedObjects["options"];
            options.Message.ShouldBe("hello");
            options.Upper.ShouldBe(true);
        }

        /// <summary>
        /// Given the built console application, when specifying the echo verb and a scalar option, then the scalar option should be the scalar value.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task GivenBuiltConsoleApp_WhenCallingVerbWithScalarOption_ThenShouldReturnExpectedResult()
        {
            // Arrange
            const string args = "echo hello --upper --repeat 2";
            var logger = new UnitTestLogger();
            var console = CreateConsoleAppWithLogger(args, logger);

            // Act
            await console.RunAsync();

            // Assert
            var options = (EchoOptions)logger.LoggedObjects["options"];
            options.Message.ShouldBe("hello");
            options.Upper.ShouldBe(true);
            options.Repeat.ShouldBe(2);
        }

        /// <summary>
        /// When calling the console app with parameters but without specifying the default verb, it should run the default command with those parameters.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task GivenBuiltConsoleApp_WhenCallingDefaultVerbWithParametersWithoutVerb_ThenShouldRunDefaultVerbWithParameters()
        {
            // Arrange
            const string args = "--option=Queue --tenantId=1234";
            var logger = new UnitTestLogger();
            var console = CreateConsoleAppWithLogger(args, logger);

            // Act
            await console.RunAsync();

            // Assert
            var options = (DefaultOptions)logger.LoggedObjects["options"];
            options.Option.ShouldBe("Queue");
            options.TenantId.ShouldBe("1234");
        }

        /// <summary>
        /// When calling the console app with parameters and explicitly specifying the default verb, it should run the default command with those parameters.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task GivenBuiltConsoleApp_WhenCallingDefaultVerbWithParametersWithVerb_ThenShouldRunDefaultVerbWithParameters()
        {
            // Arrange
            const string args = "default --option=Queue --tenantId=1234";
            var logger = new UnitTestLogger();
            var console = CreateConsoleAppWithLogger(args, logger);

            // Act
            await console.RunAsync();

            // Assert
            var options = (DefaultOptions)logger.LoggedObjects["options"];
            options.Option.ShouldBe("Queue");
            options.TenantId.ShouldBe("1234");
        }

        /// <summary>
        /// When calling the console app with parameters but without specifying the default verb, and the default options have no properties, it should still run the default command.
        /// This reproduces a customer scenario where they defined isDefault but parse arguments manually in the command.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Fact]
        public async Task GivenBuiltConsoleAppWithDefaultVerbWithoutProperties_WhenCallingWithParametersWithoutVerb_ThenShouldRunDefaultVerb()
        {
            // Arrange
            const string args = "--option=Queue --tenantId=1234";
            var logger = new UnitTestLogger();
            var console = CreateConsoleAppWithLoggerForDefaultWithoutProperties(args, logger);

            // Act
            await console.RunAsync();

            // Assert
            logger.LoggedObjects["options"].ShouldBeOfType<DefaultOptionsWithoutProperties>();
        }

        /// <summary>
        /// Creates the console application with logger.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <param name="tracker">The logger.</param>
        /// <returns>A built console app ready to run.</returns>
        private static IDotNetConsole CreateConsoleAppWithLogger(string args, IUnitTestLogger tracker)
        {
            var servicesAssembly = Assembly.GetAssembly(typeof(DefaultCommand))!;
            var verbTypes = new[] { typeof(DefaultOptions), typeof(EchoOptions) };
            var builder = DotNetConsole.CreateBuilderWithReference(servicesAssembly, verbTypes, args.Split(" "));

            builder.Services.Replace(new ServiceDescriptor(typeof(IUnitTestLogger), tracker));

            return builder.Build();
        }

        /// <summary>
        /// Creates the console application with logger for default command without properties.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <param name="tracker">The logger.</param>
        /// <returns>A built console app ready to run.</returns>
        private static IDotNetConsole CreateConsoleAppWithLoggerForDefaultWithoutProperties(string args, IUnitTestLogger tracker)
        {
            var servicesAssembly = Assembly.GetAssembly(typeof(DefaultCommandWithoutProperties))!;
            var verbTypes = new[] { typeof(DefaultOptionsWithoutProperties) };
            var builder = DotNetConsole.CreateBuilderWithReference(servicesAssembly, verbTypes, args.Split(" "));

            builder.Services.Replace(new ServiceDescriptor(typeof(IUnitTestLogger), tracker));

            return builder.Build();
        }
    }
}
