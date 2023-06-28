namespace Neolution.DotNet.Console.UnitTests.ConsoleAppStartup.Stubs
{
    using Microsoft.Extensions.DependencyInjection;
    using Neolution.DotNet.Console.Abstractions;
    using Neolution.DotNet.Console.UnitTests.Common.Stubs;

    /// <summary>
    /// Composition root stub to test if the console app builder reports when services are injected that are not allowed.
    /// </summary>
    /// <remarks>
    /// In composition roots, only <see cref="Microsoft.Extensions.Configuration.IConfiguration"/> and <see cref="Microsoft.Extensions.Hosting.IHostEnvironment"/> injections are allowed.
    /// </remarks>
    /// <seealso cref="ICompositionRoot" />
    public class CompositionRootWithDisallowedInjectedServicesStub : ICompositionRoot
    {
        /// <summary>
        /// The injected service that is not allowed.
        /// </summary>
        private readonly ITransientServiceStub notAllowedService;

        /// <summary>
        /// Initializes a new instance of the <see cref="CompositionRootWithDisallowedInjectedServicesStub"/> class.
        /// </summary>
        /// <param name="notAllowedService">The disallowed injection.</param>
        public CompositionRootWithDisallowedInjectedServicesStub(ITransientServiceStub notAllowedService)
        {
            this.notAllowedService = notAllowedService;
        }

        /// <inheritdoc />
        public void ConfigureServices(IServiceCollection services)
        {
            this.notAllowedService.DoSomething();
        }
    }
}
