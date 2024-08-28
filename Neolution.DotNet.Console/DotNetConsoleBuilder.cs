namespace Neolution.DotNet.Console
{
    using CommandLine;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;

    /// <summary>
    /// The console application builder.
    /// </summary>
    public class DotNetConsoleBuilder
    {
        /// <summary>
        /// The host builder
        /// </summary>
        private readonly IHostBuilder hostBuilder;

        /// <summary>
        /// The command line parser result
        /// </summary>
        private readonly ParserResult<object> commandLineParserResult;

        /// <summary>
        /// The service collection
        /// </summary>
        private readonly ServiceCollection serviceCollection = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="DotNetConsoleBuilder"/> class.
        /// </summary>
        /// <param name="hostBuilder">The host builder.</param>
        /// <param name="commandLineParserResult">The command line parser result.</param>
        /// <param name="environment">The environment.</param>
        /// <param name="configuration">The configuration.</param>
        public DotNetConsoleBuilder(IHostBuilder hostBuilder, ParserResult<object> commandLineParserResult, IHostEnvironment environment, IConfiguration configuration)
        {
            this.hostBuilder = hostBuilder;
            this.commandLineParserResult = commandLineParserResult;
            this.Environment = environment;
            this.Configuration = configuration;
        }

        /// <summary>
        /// Gets information about the host environment an application is running in.
        /// </summary>
        public IHostEnvironment Environment { get; }

        /// <summary>
        /// Gets a collection of configuration providers for the application to compose. This is useful for adding new configuration sources and providers.
        /// </summary>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// Gets the collection of services for the application to compose. This is useful for adding user provided or framework provided services.
        /// </summary>
        public IServiceCollection Services => this.serviceCollection;

        /// <summary>
        /// Builds this instance.
        /// </summary>
        /// <returns>The <see cref="DotNetConsole"/>.</returns>
        public DotNetConsole Build()
        {
            // Copy over the services to host builder before it gets built
            this.hostBuilder.ConfigureServices(services =>
            {
                foreach (var service in this.serviceCollection)
                {
                    services.Add(service);
                }
            });

            var host = this.hostBuilder.Build();
            return new DotNetConsole(host, this.commandLineParserResult);
        }
    }
}
