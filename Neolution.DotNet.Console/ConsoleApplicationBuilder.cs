﻿namespace Neolution.DotNet.Console
{
    using CommandLine;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;

    /// <summary>
    /// The console application builder.
    /// </summary>
    public class ConsoleApplicationBuilder
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
        /// Initializes a new instance of the <see cref="ConsoleApplicationBuilder"/> class.
        /// </summary>
        /// <param name="hostBuilder">The host builder.</param>
        /// <param name="commandLineParserResult">The command line parser result.</param>
        /// <param name="environment">The environment.</param>
        /// <param name="configuration">The configuration.</param>
        public ConsoleApplicationBuilder(IHostBuilder hostBuilder, ParserResult<object> commandLineParserResult, IHostEnvironment environment, IConfiguration configuration)
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
        /// Gets the services.
        /// </summary>
        public IServiceCollection Services
        {
            get
            {
                // We expose host builders service collection to allow adding additional services
                var services = new ServiceCollection();
                this.hostBuilder.ConfigureServices((_, collection) =>
                {
                    foreach (var service in services)
                    {
                        collection.Add(service);
                    }
                });

                return services;
            }
        }

        /// <summary>
        /// Builds this instance.
        /// </summary>
        /// <returns>The <see cref="ConsoleApplication"/>.</returns>
        public ConsoleApplication Build()
        {
            var host = this.hostBuilder.Build();
            return new ConsoleApplication(host, this.commandLineParserResult);
        }
    }
}