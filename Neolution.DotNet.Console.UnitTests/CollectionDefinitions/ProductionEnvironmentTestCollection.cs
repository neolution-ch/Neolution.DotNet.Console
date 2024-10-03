namespace Neolution.DotNet.Console.UnitTests.CollectionDefinitions
{
    using Neolution.DotNet.Console.UnitTests.Setup;
    using Xunit;

    /// <summary>
    /// Collection definition for tests that should run in the production environment.
    /// </summary>
    /// <seealso cref="ICollectionFixture{ProductionEnvironmentTestSetup}" />
    [CollectionDefinition("Production Environment Tests")]
    public class ProductionEnvironmentTestCollection : ICollectionFixture<ProductionEnvironmentTestSetup>
    {
        // This class is empty and serves as a way to attach the fixture to multiple test classes.
    }
}
