using System.Linq;
using AutoFixture;
using AutoFixture.AutoNSubstitute;

namespace Neolution.DotNet.Console.UnitTests.Common.Fixtures
{
    /// <summary>
    /// Defines NSubstitute fixture
    /// </summary>
    public static class AutoNSubstituteFixture
    {
        /// <summary>
        /// Creates the fixture.
        /// </summary>
        /// <returns>The fixture.</returns>
        public static IFixture CreateFixture()
        {
            var fixture = new Fixture().Customize(new AutoNSubstituteCustomization());

            // Do not throw if mocking recursively dependent classes
            fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList().ForEach(b => fixture.Behaviors.Remove(b));
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            return fixture;
        }
    }
}
