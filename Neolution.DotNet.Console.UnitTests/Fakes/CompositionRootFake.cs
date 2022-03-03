namespace Neolution.DotNet.Console.UnitTests.Fakes
{
    using Microsoft.Extensions.DependencyInjection;
    using Neolution.DotNet.Console.Abstractions;

    /// <summary>
    /// The fake composition root.
    /// </summary>
    /// <seealso cref="Neolution.DotNet.Console.Abstractions.ICompositionRoot" />
    public class CompositionRootFake : ICompositionRoot
    {
        /// <inheritdoc />
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<IService, FakeService>();
        }
    }
}
