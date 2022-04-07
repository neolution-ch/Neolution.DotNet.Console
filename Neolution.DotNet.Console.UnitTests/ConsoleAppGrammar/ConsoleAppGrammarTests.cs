using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Neolution.DotNet.Console.Abstractions;
using Neolution.DotNet.Console.UnitTests.Common.Spies;
using Neolution.DotNet.Console.UnitTests.Common.Stubs;
using Neolution.DotNet.Console.UnitTests.ConsoleAppGrammar.Fakes;
using Shouldly;
using Xunit;

namespace Neolution.DotNet.Console.UnitTests.ConsoleAppGrammar
{
    /// <summary>
    /// Command line arguments grammar tests
    /// </summary>
    public class ConsoleAppGrammarTests
    {
        [Fact]
        public void GivenBuiltConsoleApp_WhenCallingWithoutVerb_ThenShouldRunDefaultVerb()
        {
            // Arrange
            const string args = "";
            var logger = new UnitTestLogger();
            var console = CreateConsoleAppWithLogger(args, logger);

            // Act
            console.Run();

            // Assert
            logger.LoggedObjects.First().ShouldBeOfType<DefaultVerbOptions>();
        }

        [Fact]
        public void GivenBuiltConsoleApp_WhenCallingVerbWithoutValue_ThenShouldReturnExpectedResult()
        {
            // Arrange
            const string args = $"{EchoOptions.CommandName}";
            var logger = new UnitTestLogger();
            var console = CreateConsoleAppWithLogger(args, logger);

            // Act
            console.Run();

            // Assert
            var options = (EchoOptions)logger.LoggedObjects.First();
            options.Message.ShouldBeNull();
        }

        /// <summary>
        /// Given the built console application, when calling with the verb only then should return expected result.
        /// </summary>
        [Fact]
        public void GivenBuiltConsoleApp_WhenCallingVerbWithValue_ThenShouldReturnExpectedResult()
        {
            // Arrange
            const string args = $"{EchoOptions.CommandName} hello";
            var logger = new UnitTestLogger();
            var console = CreateConsoleAppWithLogger(args, logger);

            // Act
            console.Run();

            // Assert
            var options = (EchoOptions)logger.LoggedObjects.First();
            options.Message.ShouldBe("hello");
        }

        /// <summary>
        /// Given the built console application, when calling the verb with option then should return expected result.
        /// </summary>
        [Fact]
        public void GivenBuiltConsoleApp_WhenCallingVerbWithSwitchOption_ThenShouldReturnExpectedResult()
        {
            // Arrange
            const string args = $"{EchoOptions.CommandName} hello --upper";
            var logger = new UnitTestLogger();
            var console = CreateConsoleAppWithLogger(args, logger);

            // Act
            console.Run();

            // Assert
            var options = (EchoOptions)logger.LoggedObjects.First();
            options.Message.ShouldBe("hello");
            options.Upper.ShouldBe(true);
        }

        /// <summary>
        /// Given the built console application, when calling with the verb and an option that takes value then should return expected result3.
        /// </summary>
        [Fact]
        public void GivenBuiltConsoleApp_WhenCallingVerbWithScalarOption_ThenShouldReturnExpectedResult()
        {
            // Arrange
            const string args = $"{EchoOptions.CommandName} hello --upper --repeat 2";
            var logger = new UnitTestLogger();
            var console = CreateConsoleAppWithLogger(args, logger);

            // Act
            console.Run();

            // Assert
            var options = (EchoOptions)logger.LoggedObjects.First();
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
        private static IConsoleApp CreateConsoleAppWithLogger(string args, IUnitTestLogger tracker)
        {
            var builder = DotNetConsole.CreateDefaultBuilder(args.Split(" "))
                .ConfigureServices((context, services) =>
                {
                    services.Replace(new ServiceDescriptor(typeof(IUnitTestLogger), tracker));
                })
                .UseCompositionRoot<CompositionRootStub>();

            return builder.Build();
        }
    }
}
