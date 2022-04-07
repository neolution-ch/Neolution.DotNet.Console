using Microsoft.Extensions.Configuration;

namespace Neolution.DotNet.Console.UnitTests.ConsoleAppStartup.Stubs
{
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Neolution.DotNet.Console.Abstractions;

    public class CompositionRootWithInjectedServicesStub : ICompositionRoot
    {
        private readonly IConfiguration configuration;
        private readonly IHostEnvironment hostEnvironment;

        public CompositionRootWithInjectedServicesStub(IConfiguration configuration, IHostEnvironment hostEnvironment)
        {
            this.configuration = configuration;
            this.hostEnvironment = hostEnvironment;
        }

        public void ConfigureServices(IServiceCollection services)
        {
        }
    }
}
