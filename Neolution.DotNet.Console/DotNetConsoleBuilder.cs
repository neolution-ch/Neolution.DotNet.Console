namespace Neolution.DotNet.Console
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using CommandLine;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.FileProviders;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using Neolution.DotNet.Console.Abstractions;
    using Neolution.DotNet.Console.Internal;
    using NLog.Extensions.Logging;

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
            // Create read-only configuration and environment that are only valid before the host is built, because we want to expose these as properties of our DotNetConsoleBuilder.
            var environment = CreateConsoleEnvironment(args);
            var configuration = CreateConsoleConfiguration(assembly, args, environment);

            // Create a HostBuilder
            var builder = Host.CreateDefaultBuilder(args)
                .UseContentRoot(environment.ContentRootPath)
                .ConfigureLogging((context, logging) =>
                {
                    AdjustDefaultBuilderLoggingProviders(logging);
                    logging.AddNLog(context.Configuration);
                })
                .ConfigureServices((_, services) =>
                {
                    // Register all commands found in the entry assembly.
                    services.Scan(selector => selector.FromAssemblies(assembly)
                        .AddClasses(classes => classes.AssignableTo(typeof(IDotNetConsoleCommand<>)))
                        .AsImplementedInterfaces());
                });

            // If verb types were not specified, compile all available verbs for this run by looking for classes with the Verb attribute in the specified assembly
            verbTypes ??= assembly.GetTypes()
                .Where(t => t.GetCustomAttribute<VerbAttribute>() != null)
                .ToArray();

            EnforceStrictVerbMatching(args, verbTypes);
            var parsedArguments = Parser.Default.ParseArguments(args, verbTypes);

            return new DotNetConsoleBuilder(builder, parsedArguments, environment, configuration);
        }

        /// <summary>
        /// Enforce strict verb matching if one verb is marked as default. Otherwise, the default verb will be executed even if that was not the users intention.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <param name="availableVerbTypes">The available verb types.</param>
        /// <exception cref="Neolution.DotNet.Console.DotNetConsoleException">Cannot create builder, because the specified verb '{firstVerb}' matches no command.</exception>
        private static void EnforceStrictVerbMatching(string[] args, Type[] availableVerbTypes)
        {
            var availableVerbs = availableVerbTypes.Select(t => t.GetCustomAttribute<VerbAttribute>()!).ToList();
            if (!availableVerbs.Any(v => v.IsDefault))
            {
                // If no default verb is defined, we do not enforce strict verb matching
                return;
            }

            var firstVerb = args.FirstOrDefault();
            if (string.IsNullOrWhiteSpace(firstVerb) || firstVerb.StartsWith('-'))
            {
                // If the user passed no verb, but a default verb is defined, the default verb will be executed
                return;
            }

            // Names reserved by CommandLineParser library
            var validFirstArguments = new List<string> { "--help", "--version", "help", "version" };

            // Names of all available verbs
            validFirstArguments.AddRange(availableVerbs.Select(t => t.Name));

            // Check if the first argument can be found in the list of valid arguments
            var verbMatched = validFirstArguments.Any(v => v.Equals(firstVerb, StringComparison.OrdinalIgnoreCase));
            if (!verbMatched)
            {
                throw new DotNetConsoleException($"Cannot create builder, because the specified verb '{firstVerb}' matches no command.");
            }
        }

        /// <summary>
        /// Creates the console environment.
        /// </summary>
        /// <param name="args">The command line arguments.</param>
        /// <returns>The <see cref="IHostEnvironment"/>.</returns>
        private static DotNetConsoleEnvironment CreateConsoleEnvironment(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .AddEnvironmentVariables(prefix: "DOTNET_")
                .AddCommandLine(args)
                .Build();

            // The apps root directory is where the appsettings.json are located
            var appRootDirectory = AppContext.BaseDirectory;

            return new DotNetConsoleEnvironment
            {
                EnvironmentName = configuration[HostDefaults.EnvironmentKey] ?? Environments.Production,
                ApplicationName = AppDomain.CurrentDomain.FriendlyName,
                ContentRootPath = appRootDirectory,
                ContentRootFileProvider = new PhysicalFileProvider(appRootDirectory),
            };
        }

        /// <summary>
        /// Applies the default configuration to the console application.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <param name="args">The arguments.</param>
        /// <param name="environment">The environment.</param>
        /// <returns>The <see cref="IConfiguration" />.</returns>
        private static IConfiguration CreateConsoleConfiguration(Assembly assembly, string[] args, IHostEnvironment environment)
        {
            var configurationBuilder = new ConfigurationBuilder()
                .SetBasePath(environment.ContentRootPath)
                .AddEnvironmentVariables(prefix: "DOTNET_");

            AddCommandLineConfig(configurationBuilder, args);

            configurationBuilder
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{environment.EnvironmentName}.json", optional: true, reloadOnChange: true);

            if (environment.IsDevelopment())
            {
                configurationBuilder.AddUserSecrets(assembly, optional: true, reloadOnChange: true);
            }

            configurationBuilder.AddEnvironmentVariables();

            return configurationBuilder.Build();
        }

        /// <summary>
        /// Adjusts the default builder logging providers.
        /// </summary>
        /// <param name="logging">The logging.</param>
        private static void AdjustDefaultBuilderLoggingProviders(ILoggingBuilder logging)
        {
            // Remove the default logging providers
            logging.ClearProviders();

            // Re-add other logging providers that are assigned in Host.CreateDefaultBuilder
            logging.AddDebug();
            logging.AddEventSourceLogger();

            if (OperatingSystem.IsWindows())
            {
                // Add the EventLogLoggerProvider on windows machines
                logging.AddEventLog();
            }
        }

        /// <summary>
        /// Adds the command line configuration.
        /// </summary>
        /// <param name="configBuilder">The configuration builder.</param>
        /// <param name="args">The arguments.</param>
        private static void AddCommandLineConfig(IConfigurationBuilder configBuilder, string[]? args)
        {
            if (args is { Length: > 0 })
            {
                configBuilder.AddCommandLine(args);
            }
        }
    }
}
