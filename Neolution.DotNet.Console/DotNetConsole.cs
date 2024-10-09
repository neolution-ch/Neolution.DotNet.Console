namespace Neolution.DotNet.Console
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;
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
    /// The console application.
    /// </summary>
    public class DotNetConsole
    {
        /// <summary>
        /// The host
        /// </summary>
        private readonly IHost host;

        /// <summary>
        /// The command line parser result
        /// </summary>
        private readonly ParserResult<object> commandLineParserResult;

        /// <summary>
        /// Initializes a new instance of the <see cref="DotNetConsole"/> class.
        /// </summary>
        /// <param name="host">The host.</param>
        /// <param name="commandLineParserResult">The command line parser result.</param>
        public DotNetConsole(IHost host, ParserResult<object> commandLineParserResult)
        {
            this.host = host;
            this.commandLineParserResult = commandLineParserResult;
        }

        /// <summary>
        /// Gets the services.
        /// </summary>
        public IServiceProvider Services => this.host.Services;

        /// <summary>
        /// Creates the default builder.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns>The <see cref="DotNetConsoleBuilder"/>.</returns>
        public static DotNetConsoleBuilder CreateDefaultBuilder(string[] args)
        {
            // Get the entry assembly of the console application. It will later be scanned to find commands and verbs and their options.
            var assembly = Assembly.GetEntryAssembly() ?? throw new DotNetConsoleException("Could not determine entry assembly");

            return CreateBuilderInternal(assembly, null, args);
        }

        /// <summary>
        /// Creates the builder with explicitly specified assembly.
        /// This is useful when the assembly that contains the commands and verbs is not the same as the entry assembly.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <param name="args">The arguments.</param>
        /// <returns> The <see cref="DotNetConsoleBuilder" />. </returns>
        public static DotNetConsoleBuilder CreateBuilderWithReference(Assembly assembly, string[] args)
        {
            ArgumentNullException.ThrowIfNull(assembly);

            return CreateBuilderInternal(assembly, null, args);
        }

        /// <summary>
        /// Creates the builder with explicitly specified assembly and verb types.
        /// This is useful when commands and verbs are defined in different assemblies.
        /// </summary>
        /// <param name="servicesAssembly">The assembly.</param>
        /// <param name="verbTypes">The verb types.</param>
        /// <param name="args">The arguments.</param>
        /// <returns> The <see cref="DotNetConsoleBuilder" />. </returns>
        public static DotNetConsoleBuilder CreateBuilderWithReference(Assembly servicesAssembly, Type[] verbTypes, string[] args)
        {
            ArgumentNullException.ThrowIfNull(servicesAssembly);

            return CreateBuilderInternal(servicesAssembly, verbTypes, args);
        }

        /// <summary>
        /// Runs the application.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task RunAsync()
        {
            await this.commandLineParserResult.WithParsedAsync(this.RunWithOptionsAsync);
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
        private static DotNetConsoleBuilder CreateBuilderInternal(Assembly assembly, Type[]? verbTypes, string[] args)
        {
            // Create a HostBuilder
            var hostBuilder = Host.CreateDefaultBuilder(args)
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

            // Manually build configuration and environment again, because we unfortunately can't access them from the host builder we just created, but want to provide them in the ConsoleApplicationBuilder.
            var environment = CreateConsoleEnvironment(args);
            var configuration = ApplyDefaultConfiguration(assembly, args, environment);

            // If verb types were not specified, compile all available verbs for this run by looking for classes with the Verb attribute in the specified assembly
            verbTypes ??= assembly.GetTypes()
                .Where(t => t.GetCustomAttribute<VerbAttribute>() != null)
                .ToArray();

            EnforceStrictVerbMatching(args, verbTypes);
            var parsedArguments = Parser.Default.ParseArguments(args, verbTypes);

            return new DotNetConsoleBuilder(hostBuilder, parsedArguments, environment, configuration);
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

            return new DotNetConsoleEnvironment
            {
                EnvironmentName = configuration[HostDefaults.EnvironmentKey] ?? "Production",
                ApplicationName = AppDomain.CurrentDomain.FriendlyName,
                ContentRootPath = AppContext.BaseDirectory,
                ContentRootFileProvider = new PhysicalFileProvider(AppContext.BaseDirectory),
            };
        }

        /// <summary>
        /// Applies the default configuration to the console application.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <param name="args">The arguments.</param>
        /// <param name="environment">The environment.</param>
        /// <returns>The <see cref="IConfiguration" />.</returns>
        private static IConfiguration ApplyDefaultConfiguration(Assembly assembly, string[] args, IHostEnvironment environment)
        {
            var configurationBuilder = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
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

        /// <summary>
        /// Runs the command with options asynchronously.
        /// </summary>
        /// <param name="options">The options.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        private async Task RunWithOptionsAsync(object options)
        {
            // To support scoped services, create a scope for each command call/run.
            var scopeFactory = this.Services.GetRequiredService<IServiceScopeFactory>();
            await using var scope = scopeFactory.CreateAsyncScope();

            // Determine the type of the options object to find the corresponding command type
            var dataType = new[] { options.GetType() };
            var genericBase = typeof(IDotNetConsoleCommand<>);
            var commandType = genericBase.MakeGenericType(dataType);
            var commandEntryPointMethodInfo = commandType.GetMethod(nameof(IDotNetConsoleCommand<object>.RunAsync));

            // Resolve the command from the service provider by the determined type and invoke the entry point method (RunAsync)
            var command = scope.ServiceProvider.GetRequiredService(commandType);
            if (commandEntryPointMethodInfo?.Invoke(command, new[] { options }) is Task commandRunTask)
            {
                await commandRunTask;
            }
        }
    }
}
