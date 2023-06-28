namespace Neolution.DotNet.Console.UnitTests.ConsoleAppStartup.Stubs
{
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Neolution.DotNet.Console.Abstractions;
    using Neolution.DotNet.Console.UnitTests.Common.Stubs;

    /// <summary>
    /// A composition root stub with injected services.
    /// </summary>
    /// <seealso cref="ICompositionRoot" />
    public class CompositionRootWithInjectedServicesStub : ICompositionRoot
    {
        /// <summary>
        /// The configuration
        /// </summary>
        private readonly IConfiguration configuration;

        /// <summary>
        /// The host environment
        /// </summary>
        private readonly IHostEnvironment hostEnvironment;

        /// <summary>
        /// Initializes a new instance of the <see cref="CompositionRootWithInjectedServicesStub"/> class.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="hostEnvironment">The host environment.</param>
        public CompositionRootWithInjectedServicesStub(IConfiguration configuration, IHostEnvironment hostEnvironment)
        {
            this.configuration = configuration;
            this.hostEnvironment = hostEnvironment;
        }

        /// <inheritdoc />
        public void ConfigureServices(IServiceCollection services)
        {
            // Do something
            if (this.hostEnvironment.IsDevelopment())
            {
                services?.Add(new ServiceDescriptor(typeof(ITransientServiceStub), this.configuration["LoremIpsum"]));
            }
        }
    }
}
