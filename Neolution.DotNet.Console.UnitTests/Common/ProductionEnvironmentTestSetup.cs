namespace Neolution.DotNet.Console.UnitTests.Common
{
    using System;

    /// <summary>
    /// Fixture that sets the environment variable to Production.
    /// </summary>
    /// <seealso cref="System.IDisposable" />
    public sealed class ProductionEnvironmentTestSetup : IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProductionEnvironmentTestSetup"/> class.
        /// </summary>
        public ProductionEnvironmentTestSetup()
        {
            // Set the environment variable before any tests run
            Environment.SetEnvironmentVariable("DOTNET_ENVIRONMENT", "Production");
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            // Reset the environment variable after all tests have run
            Environment.SetEnvironmentVariable("DOTNET_ENVIRONMENT", null);
        }
    }
}
