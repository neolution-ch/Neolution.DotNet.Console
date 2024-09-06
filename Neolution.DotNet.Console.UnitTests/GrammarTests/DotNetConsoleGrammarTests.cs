namespace Neolution.DotNet.Console.UnitTests.GrammarTests
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Threading.Tasks;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;
    using Neolution.DotNet.Console.UnitTests.Common.Fakes;
    using Neolution.DotNet.Console.UnitTests.Common.Spies;
    using Neolution.DotNet.Console.UnitTests.GrammarTests.Fakes;
    using Shouldly;
    using Xunit;

    /// <summary>
    /// Command line arguments grammar tests
    /// </summary>
    [Collection("Production Environment Tests")]
    public class DotNetConsoleGrammarTests
    {
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
        /// Given a mistyped verb, when no default verb is defined, then should not throw on console building.
        /// </summary>
        [Fact]
        public void GivenMistypedVerb_WhenNoDefaultVerbIsDefined_ThenShouldNotThrowOnBuilding()
        {
            // Arrange
            const string args = "eho"; // mistyped verb

            var servicesAssembly = Assembly.GetAssembly(typeof(EchoCommand))!;
            var verbTypes = new List<Type> { typeof(EchoOptions) }.ToArray();

            // Act

            // Assert
            Should.NotThrow(() => DotNetConsole.CreateBuilderWithReference(servicesAssembly, verbTypes, args.Split(" ")));
        }

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
        /// Creates the console application with logger.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <param name="tracker">The logger.</param>
        /// <returns>A built console app ready to run.</returns>
        private static DotNetConsole CreateConsoleAppWithLogger(string args, IUnitTestLogger tracker)
        {
            var builder = DotNetConsole.CreateBuilderWithReference(Assembly.GetAssembly(typeof(DefaultCommand))!, args.Split(" "));

            builder.Services.Replace(new ServiceDescriptor(typeof(IUnitTestLogger), tracker));

            return builder.Build();
        }
    }
}
