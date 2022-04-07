namespace Neolution.DotNet.Console.UnitTests.Fakes
{
    using CommandLine;
    using Neolution.DotNet.Console.Abstractions;
    using Neolution.DotNet.Console.UnitTests.Stubs;

    [Verb(Verb)]
    public class InjectServicesWithVariousLifetimesOptions
    {
        public const string Verb = "InjectServicesWithVariousLifetimes";
    }

    public class InjectServicesWithVariousLifetimesCommand : IConsoleAppCommand<InjectServicesWithVariousLifetimesOptions>
    {
        private readonly ITransientServiceStub transientService;
        private readonly IScopedServiceStub scopedService;
        private readonly ISingletonServiceStub singletonService;

        public InjectServicesWithVariousLifetimesCommand(ITransientServiceStub transientService, IScopedServiceStub scopedService, ISingletonServiceStub singletonService)
        {
            this.transientService = transientService;
            this.scopedService = scopedService;
            this.singletonService = singletonService;
        }

        public void Run(InjectServicesWithVariousLifetimesOptions options)
        {
        }
    }
}
