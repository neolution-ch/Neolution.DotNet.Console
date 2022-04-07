namespace Neolution.DotNet.Console.UnitTests.Fakes
{
    using Microsoft.Extensions.DependencyInjection;
    using Neolution.DotNet.Console.Abstractions;

    public class CompositionRootStub : ICompositionRoot
    {
        /// <inheritdoc />
        public void ConfigureServices(IServiceCollection services)
        {
        }
    }
}
