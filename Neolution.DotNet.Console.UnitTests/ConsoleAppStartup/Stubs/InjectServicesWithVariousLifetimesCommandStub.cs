namespace Neolution.DotNet.Console.UnitTests.ConsoleAppStartup.Stubs
{
    using Neolution.DotNet.Console.Abstractions;
    using Neolution.DotNet.Console.UnitTests.Common.Stubs;

    /// <summary>
    /// Composition root stub used to check if DI can resolve services with different lifetimes.
    /// </summary>
    /// <seealso cref="IConsoleAppCommand{InjectServicesWithVariousLifetimesOptionsStub}" />
    public class InjectServicesWithVariousLifetimesCommandStub : IConsoleAppCommand<InjectServicesWithVariousLifetimesOptionsStub>
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
        public void Run(InjectServicesWithVariousLifetimesOptionsStub options)
        {
            this.transientService.DoSomething();
            this.scopedService.DoSomething();
            this.singletonService.DoSomething();
        }
    }
}
