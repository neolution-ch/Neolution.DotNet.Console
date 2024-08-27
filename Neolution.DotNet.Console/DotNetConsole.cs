namespace Neolution.DotNet.Console
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;
    using CommandLine;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
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
        /// <returns>The <see cref="ConsoleAppBuilder"/>.</returns>
        public static ConsoleAppBuilder CreateDefaultBuilder(string[] args)
        {
            // Get the entry assembly of the console application. It will later be scanned to find commands and verbs and their options.
            var assembly = Assembly.GetEntryAssembly();
            if (assembly is null)
            {
                throw new ConsoleAppException("Could not determine entry assembly");
            }

            return CreateBuilderInternal(assembly, args);
        }

        /// <summary>
        /// Creates the builder with a reference to the specified assembly. This is useful when the assembly that contains the commands is not the same as the entry assembly.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <param name="args">The arguments.</param>
        /// <returns>
        /// The <see cref="ConsoleAppBuilder" />.
        /// </returns>
        /// <exception cref="Neolution.DotNet.Console.ConsoleAppException">Could not determine entry assembly</exception>
        public static ConsoleAppBuilder CreateBuilderWithReference(Assembly assembly, string[] args)
        {
            if (assembly is null)
            {
                throw new ArgumentNullException(nameof(assembly));
            }

            return CreateBuilderInternal(assembly, args);
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
        /// <param name="args">The arguments.</param>
        /// <returns>
        /// The <see cref="ConsoleAppBuilder" />.
        /// </returns>
        private static ConsoleAppBuilder CreateBuilderInternal(Assembly assembly, string[] args)
        {
            // Create a HostBuilder
            var hostBuilder = Host.CreateDefaultBuilder(args)
                .ConfigureLogging((context, logging) =>
                {
                    AdjustDefaultBuilderLoggingProviders(logging);
                    logging.AddNLog(context.Configuration);
                })
                .ConfigureServices((context, services) =>
                {
                    // Register all commands found in the entry assembly.
                    services.Scan(selector => selector.FromAssemblies(assembly)
                        .AddClasses(classes => classes.AssignableTo(typeof(IConsoleAppCommand<>)))
                        .AsImplementedInterfaces());
                });

            // Manually build configuration and environment again, because we unfortunately can't access them from the host builder we just created, but want to provide them in the ConsoleApplicationBuilder.
            var environment = CreateConsoleEnvironment();
            var configuration = ApplyDefaultConfiguration(environment, args);

            // Compile all available verbs for this run by looking for classes with the Verb attribute in the entry assembly
            var availableVerbs = assembly.GetTypes()
                .Where(t => CustomAttributeExtensions.GetCustomAttribute<VerbAttribute>((MemberInfo)t) != null)
                .ToArray();

            return new ConsoleAppBuilder(hostBuilder, Parser.Default.ParseArguments(args, availableVerbs), environment, configuration);
        }

        /// <summary>
        /// Creates the console environment.
        /// </summary>
        /// <returns>The <see cref="IHostEnvironment"/>.</returns>
        private static ConsoleAppEnvironment CreateConsoleEnvironment()
        {
            return new ConsoleAppEnvironment
            {
                EnvironmentName = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? "Production",
                ApplicationName = AppDomain.CurrentDomain.FriendlyName,
                ContentRootPath = Environment.CurrentDirectory,
                ContentRootFileProvider = null!,
            };
        }

        /// <summary>
        /// Applies the default configuration to the console application.
        /// </summary>
        /// <param name="environment">The environment.</param>
        /// <param name="args">The arguments.</param>
        /// <returns>The <see cref="IConfiguration"/>.</returns>
        private static IConfiguration ApplyDefaultConfiguration(ConsoleAppEnvironment environment, string[] args)
        {
            var configurationBuilder = new ConfigurationBuilder()
                .SetBasePath(Environment.CurrentDirectory)
                .AddEnvironmentVariables(prefix: "DOTNET_");

            AddCommandLineConfig(configurationBuilder, args);

            configurationBuilder
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{environment.EnvironmentName}.json", optional: true, reloadOnChange: true);

            if (environment.IsDevelopment() && environment.ApplicationName is { Length: > 0 })
            {
                try
                {
                    var appAssembly = Assembly.Load(new AssemblyName(environment.ApplicationName));
                    configurationBuilder.AddUserSecrets(appAssembly, optional: true, reloadOnChange: true);
                }
                catch (FileNotFoundException)
                {
                    // The assembly cannot be found, so just skip it.
                }
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
            var genericBase = typeof(IConsoleAppCommand<>);
            var commandType = genericBase.MakeGenericType(dataType);
            var commandEntryPointMethodInfo = commandType.GetMethod(nameof(IConsoleAppCommand<object>.RunAsync));

            // Resolve the command from the service provider by the determined type and invoke the entry point method (RunAsync)
            var command = scope.ServiceProvider.GetRequiredService(commandType);
            var result = commandEntryPointMethodInfo?.Invoke(command, new[] { options }) as Task;
            if (result is null)
            {
                await Task.CompletedTask;
                return;
            }

            await result;
        }
    }
}
