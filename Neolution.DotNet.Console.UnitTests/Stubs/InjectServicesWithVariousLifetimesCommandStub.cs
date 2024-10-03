namespace Neolution.DotNet.Console.UnitTests.Stubs
{
    using System.Threading.Tasks;
    using Neolution.DotNet.Console.Abstractions;

    /// <summary>
    /// Command stub used to check if
    /// </summary>
    /// <seealso cref="IDotNetConsoleCommand{TOptions}" />
    public class InjectServicesWithVariousLifetimesCommandStub : IDotNetConsoleCommand<InjectServicesWithVariousLifetimesOptionsStub>
    {
        /// <summary>
        /// The transient service
        /// </summary>
        private readonly ITransientServiceStub transientService;

        /// <summary>
        /// The scoped service
        /// </summary>
        private readonly IScopedServiceStub scopedService;

        /// <summary>
        /// The singleton service
        /// </summary>
        private readonly ISingletonServiceStub singletonService;

        /// <summary>
        /// Initializes a new instance of the <see cref="InjectServicesWithVariousLifetimesCommandStub"/> class.
        /// </summary>
        /// <param name="transientService">The transient service.</param>
        /// <param name="scopedService">The scoped service.</param>
        /// <param name="singletonService">The singleton service.</param>
        public InjectServicesWithVariousLifetimesCommandStub(ITransientServiceStub transientService, IScopedServiceStub scopedService, ISingletonServiceStub singletonService)
        {
            this.transientService = transientService;
            this.scopedService = scopedService;
            this.singletonService = singletonService;
        }

        /// <inheritdoc />
        public async Task RunAsync(InjectServicesWithVariousLifetimesOptionsStub options)
        {
            await this.transientService.DoSomethingAsync();
            await this.scopedService.DoSomethingAsync();
            await this.singletonService.DoSomethingAsync();
        }
    }
}
