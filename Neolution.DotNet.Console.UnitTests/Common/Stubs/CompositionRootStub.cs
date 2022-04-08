namespace Neolution.DotNet.Console.UnitTests.Common.Stubs
{
    using Microsoft.Extensions.DependencyInjection;
    using Neolution.DotNet.Console.Abstractions;

    /// <summary>
    /// A composition root stub
    /// </summary>
    /// <seealso cref="ICompositionRoot" />
    public class CompositionRootStub : ICompositionRoot
    {
        /// <inheritdoc />
        public void ConfigureServices(IServiceCollection services)
        {
            // just a stub
        }
    }
}
