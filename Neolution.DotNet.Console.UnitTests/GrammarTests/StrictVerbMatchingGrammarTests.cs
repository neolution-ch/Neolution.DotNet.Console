namespace Neolution.DotNet.Console.UnitTests.GrammarTests
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using Neolution.DotNet.Console.UnitTests.GrammarTests.Fakes;
    using Shouldly;
    using Xunit;

    /// <summary>
    /// Command line arguments grammar tests
    /// </summary>
    [Collection("Production Environment Tests")]
    public class StrictVerbMatchingGrammarTests
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
        /// Givens a reserved argument name, when no default verb is defined, then should not throw on building.
        /// </summary>
        /// <param name="args">The arguments.</param>
        [Theory]
        [InlineData("help")]
        [InlineData("version")]
        [InlineData("--help")]
        [InlineData("--version")]
        [InlineData("help echo")]
        public void GivenReservedArgumentName_WhenNoDefaultVerbIsDefined_ThenShouldNotThrowOnBuilding(string args)
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
