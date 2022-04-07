using Microsoft.Extensions.DependencyInjection;
using Neolution.DotNet.Console.Abstractions;
using Neolution.DotNet.Console.UnitTests.Common.Stubs;

namespace Neolution.DotNet.Console.UnitTests.ConsoleAppStartup.Stubs
{
    public class CompositionRootWithDisallowedInjectedServicesStub : ICompositionRoot
    {
        private readonly ITransientServiceStub disallowedInjection;

        public CompositionRootWithDisallowedInjectedServicesStub(ITransientServiceStub disallowedInjection)
        {
            this.disallowedInjection = disallowedInjection;
        }

        public void ConfigureServices(IServiceCollection services)
        {
        }
    }
}
