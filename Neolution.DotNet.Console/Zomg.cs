namespace Neolution.DotNet.Console
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Threading.Tasks;
    using CommandLine;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using Neolution.DotNet.Console.Abstractions;
    using Neolution.DotNet.Console.Internal;
    using NLog.Extensions.Logging;

    public class ConsoleApplication
    {
        public static SlimApplicationBuilder CreateDefaultBuilder(string[] args)
        {
            // Get the entry assembly of the console application. It will later be scanned to find commands and verbs and their options.
            var entryAssembly = Assembly.GetEntryAssembly();
            if (entryAssembly is null)
            {
                throw new ConsoleAppException("Could not determine entry assembly");
            }

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
                    services.Scan(selector => selector.FromAssemblies(entryAssembly)
                        .AddClasses(classes => classes.AssignableTo(typeof(IAsyncConsoleAppCommand<>)))
                        .AsImplementedInterfaces());
                });

            // Manually build configuration and environment again, because we unfortunately can't access them from the host builder we just created, but want to provide them in the SlimApplicationBuilder.
            var environment = CreateDefaultEnvironment();
            var configuration = ApplyDefaultConfiguration(environment, args);

            // Compile all available verbs for this run by looking for classes with the Verb attribute in the entry assembly
            var availableVerbs = entryAssembly.GetTypes()
                .Where(t => CustomAttributeExtensions.GetCustomAttribute<VerbAttribute>((MemberInfo)t) != null)
                .ToArray();

            return new SlimApplicationBuilder(hostBuilder, configuration, environment, Parser.Default.ParseArguments(args, availableVerbs));
        }

        private static ConsoleAppEnvironment CreateDefaultEnvironment()
        {
            return new ConsoleAppEnvironment
            {
                EnvironmentName = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? "Production",
                ApplicationName = AppDomain.CurrentDomain.FriendlyName,
                ContentRootPath = Environment.CurrentDirectory,
            };
        }

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

        private static void AddCommandLineConfig(IConfigurationBuilder configBuilder, string[]? args)
        {
            if (args is { Length: > 0 })
            {
                configBuilder.AddCommandLine(args);
            }
        }
    }

    public class SlimApplicationBuilder
    {
        private readonly IHostBuilder hostBuilder;
        private readonly ParserResult<object> commandLineParserResult;

        public SlimApplicationBuilder(IHostBuilder hostBuilder, IConfiguration configuration, IHostEnvironment environment, ParserResult<object> commandLineParserResult)
        {
            this.hostBuilder = hostBuilder;
            this.commandLineParserResult = commandLineParserResult;
            this.Configuration = configuration;
            this.Environment = environment;
        }

        /// <summary>
        /// A collection of configuration providers for the application to compose. This is useful for adding new configuration sources and providers.
        /// </summary>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// Provides information about the host environment an application is running in.
        /// </summary>
        public IHostEnvironment Environment { get; }

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

        public SlimApplication Build()
        {
            var host = this.hostBuilder.Build();
            return new SlimApplication(host, this.commandLineParserResult);
        }
    }

    public class SlimApplication
    {
        private readonly IHost host;
        private readonly ParserResult<object> commandLineParserResult;

        public SlimApplication(IHost host, ParserResult<object> commandLineParserResult)
        {
            this.host = host;
            this.commandLineParserResult = commandLineParserResult;
        }

        public IServiceProvider Services => this.host.Services;

        public async Task RunAsync()
        {
            await this.commandLineParserResult.WithParsedAsync(this.RunWithOptionsAsync);
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
            var genericBase = typeof(IAsyncConsoleAppCommand<>);
            var commandType = genericBase.MakeGenericType(dataType);
            var commandEntryPointMethodInfo = commandType.GetMethod(nameof(IAsyncConsoleAppCommand<object>.RunAsync));

            // Resolve the command from the service provider by the determined type and invoke the entry point method (RunAsync)
            var command = scope.ServiceProvider.GetRequiredService(commandType);
            var result = (Task)commandEntryPointMethodInfo?.Invoke(command, new[] { options });
            if (result is null)
            {
                await Task.CompletedTask;
                return;
            }

            await result;
        }
    }
}
