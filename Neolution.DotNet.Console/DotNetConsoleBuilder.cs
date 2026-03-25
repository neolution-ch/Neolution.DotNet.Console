namespace Neolution.DotNet.Console
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using CommandLine;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Neolution.DotNet.Console.Abstractions;
    using Neolution.DotNet.Console.Internal;

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
        /// The configuration manager
        /// </summary>
        private readonly ConsoleConfigurationManager configurationManager;

        /// <summary>
        /// Run only to check dependencies.
        /// </summary>
        private bool checkDependencies;

        /// <summary>
        /// Initializes a new instance of the <see cref="DotNetConsoleBuilder"/> class.
        /// </summary>
        /// <param name="hostBuilder">The host builder.</param>
        /// <param name="commandLineParserResult">The command line parser result.</param>
        /// <param name="environment">The environment.</param>
        /// <param name="configurationBuilder">The initial configuration builder.</param>
        internal DotNetConsoleBuilder(IHostBuilder hostBuilder, ParserResult<object> commandLineParserResult, IHostEnvironment environment, IConfigurationBuilder configurationBuilder)
        {
            ArgumentNullException.ThrowIfNull(configurationBuilder);

            this.hostBuilder = hostBuilder;
            this.commandLineParserResult = commandLineParserResult;
            this.Environment = environment;

            var hostBuilderContext = new HostBuilderContext(new Dictionary<object, object>())
            {
                HostingEnvironment = environment,
                Configuration = configurationBuilder.Build(),
            };

            this.configurationManager = new ConsoleConfigurationManager(configurationBuilder, hostBuilderContext);
        }

        /// <summary>
        /// Gets information about the host environment an application is running in.
        /// </summary>
        public IHostEnvironment Environment { get; }

        /// <summary>
        /// Gets a collection of configuration providers for the application to compose. This is useful for adding new configuration sources and providers.
        /// </summary>
        public IConfiguration Configuration => this.configurationManager.Configuration;

        /// <summary>
        /// Gets the collection of services for the application to compose. This is useful for adding user provided or framework provided services.
        /// </summary>
        public IServiceCollection Services => this.serviceCollection;

        /// <summary>
        /// Adds a delegate for configuring additional configuration sources for the application.
        /// </summary>
        /// <param name="configureDelegate">The delegate for configuring the configuration builder.</param>
        /// <returns>The <see cref="DotNetConsoleBuilder"/>.</returns>
        public DotNetConsoleBuilder ConfigureAppConfiguration(Action<HostBuilderContext, IConfigurationBuilder> configureDelegate)
        {
            this.configurationManager.AddConfigurationDelegate(configureDelegate);
            return this;
        }

        /// <summary>
        /// Builds this instance.
        /// </summary>
        /// <returns>The <see cref="DotNetConsole"/>.</returns>
        public IDotNetConsole Build()
        {
            // Copy over the services to host builder before it gets built
            this.hostBuilder.ConfigureServices(services =>
            {
                foreach (var service in this.serviceCollection)
                {
                    services.Add(service);
                }
            });

            if (this.checkDependencies)
            {
                // Ensure development environment is used for dependency checking
                // Note: The environment should already be set to Development during CreateConsoleEnvironment
                // but we explicitly set it here as well to ensure ValidateScopes and ValidateOnBuild are enabled.
                this.hostBuilder.UseEnvironment("Development");
                this.hostBuilder.Build();

                // If build was successful and did not throw an exception, return a console that logs a success message and then terminates.
                return new CheckDepsConsole();
            }

            var host = this.hostBuilder.Build();
            return new DotNetConsole(host, this.commandLineParserResult);
        }

        /// <summary>
        /// Creates the default builder.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <param name="verbTypes">The verb types.</param>
        /// <param name="args">The arguments.</param>
        /// <returns>
        /// The <see cref="DotNetConsoleBuilder" />.
        /// </returns>
        internal static DotNetConsoleBuilder CreateBuilderInternal(Assembly assembly, Type[]? verbTypes, string[] args)
        {
            // Create configuration and environment instances that are only valid before the host is built.
            // We want to expose these as read-only properties in the DotNetConsoleBuilder.
            var environment = DotNetConsoleDefaults.CreateConsoleEnvironment(args);
            var configBuilder = DotNetConsoleDefaults.CreateConsoleConfigurationBuilder(assembly, args, environment);

            // Initialize NLog logger from configuration with fallback; any config errors are handled in Initialize
            DotNetConsoleLogger.Initialize(configBuilder.Build());

            // Create a HostBuilder
            var builder = ConsoleHostBuilderConfigurator.CreateConfiguredHostBuilder(assembly, args, environment);

            var parsedArguments = CommandLineProcessor.ParseArguments(assembly, verbTypes, args);
            var consoleBuilder = new DotNetConsoleBuilder(builder, parsedArguments, environment, configBuilder);

            // Apply any custom configuration delegates that will be added later via ConfigureAppConfiguration
            builder.ConfigureAppConfiguration((context, configBuilder) =>
            {
                // Apply all stored configuration delegates
                foreach (var configureDelegate in consoleBuilder.configurationManager.ConfigurationDelegates)
                {
                    configureDelegate(context, configBuilder);
                }
            });

            // Determine if this is a check-deps run: only DI validation should run
            if (DotNetConsoleDefaults.IsCheckDependenciesRun(args))
            {
                consoleBuilder.checkDependencies = true;
                return consoleBuilder;
            }

            return consoleBuilder;
        }
    }
}
