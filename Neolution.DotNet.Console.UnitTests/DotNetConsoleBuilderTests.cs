namespace Neolution.DotNet.Console.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using Neolution.DotNet.Console.UnitTests.Fakes;
    using Shouldly;
    using Xunit;

    /// <summary>
    /// Command line arguments grammar tests
    /// </summary>
    public class DotNetConsoleBuilderTests
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
        /// Given a valid argument, when default verb is defined, then should not throw on building.
        /// </summary>
        /// <param name="args">The arguments.</param>
        [Theory]
        [InlineData("")]
        [InlineData("echo")]
        [InlineData("--silent")]
        [InlineData("-s")]
        public void GivenValidArgument_WhenDefaultVerbIsDefined_ThenShouldNotThrowOnBuilding(string args)
        {
            // Arrange
            var servicesAssembly = Assembly.GetAssembly(typeof(EchoCommand))!;

            // Act

            // Assert
            Should.NotThrow(() => DotNetConsole.CreateBuilderWithReference(servicesAssembly, args.Split(" ")));
        }

        /// <summary>
        /// Given a reserved argument name, when default verb is defined, then should not throw on building.
        /// </summary>
        /// <param name="args">The arguments.</param>
        [Theory]
        [InlineData("help")]
        [InlineData("version")]
        [InlineData("--help")]
        [InlineData("--version")]
        [InlineData("help echo")]
        public void GivenReservedArgumentName_WhenDefaultVerbIsDefined_ThenShouldNotThrowOnBuilding(string args)
        {
            // Arrange
            var servicesAssembly = Assembly.GetAssembly(typeof(EchoCommand))!;

            // Act

            // Assert
            Should.NotThrow(() => DotNetConsole.CreateBuilderWithReference(servicesAssembly, args.Split(" ")));
        }

        /// <summary>
        /// Given invalid arguments, when no default verb is defined, then should not throw on console building.
        /// </summary>
        /// <param name="args">The arguments.</param>
        [Theory]
        [InlineData("")]
        [InlineData("verb-that-does-not-exist")]
        [InlineData("--option-that-does-not-exist")]
        [InlineData("-o")]
        public void GivenInvalidArguments_WhenNoDefaultVerbIsDefined_ThenShouldNotThrowOnBuilding(string args)
        {
            // Arrange
            var servicesAssembly = Assembly.GetAssembly(typeof(EchoCommand))!;
            var verbTypes = new List<Type> { typeof(EchoOptions) }.ToArray();

            // Act

            // Assert
            Should.NotThrow(() => DotNetConsole.CreateBuilderWithReference(servicesAssembly, verbTypes, args.Split(" ")));
        }
    }
}
