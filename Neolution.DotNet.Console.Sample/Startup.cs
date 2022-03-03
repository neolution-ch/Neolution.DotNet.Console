namespace Neolution.DotNet.Console.Sample
{
    using Microsoft.Extensions.DependencyInjection;
    using Neolution.DotNet.Console.Abstractions;

    /// <inheritdoc />
    public class Startup : ICompositionRoot
    {
        /// <inheritdoc />
        public void ConfigureServices(IServiceCollection services)
        {
            // Configure services...
        }
    }
}
