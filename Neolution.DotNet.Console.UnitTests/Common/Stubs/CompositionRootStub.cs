using Microsoft.Extensions.DependencyInjection;
using Neolution.DotNet.Console.Abstractions;

namespace Neolution.DotNet.Console.UnitTests.Common.Stubs
{
    public class CompositionRootStub : ICompositionRoot
    {
        /// <inheritdoc />
        public void ConfigureServices(IServiceCollection services)
        {
        }
    }
}
