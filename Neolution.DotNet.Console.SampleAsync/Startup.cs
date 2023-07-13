namespace Neolution.DotNet.Console.SampleAsync
{
    using Microsoft.Extensions.DependencyInjection;
    using Neolution.DotNet.Console.Abstractions;

    /// <inheritdoc />
    public class Startup : ICompositionRoot
    {
        /// <inheritdoc />
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHttpClient();
        }
    }
}
