namespace Neolution.DotNet.Console.Internal
{
    using System;
    using System.Reflection;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using Neolution.DotNet.Console.Abstractions;
    using NLog.Extensions.Logging;

    /// <summary>
    /// Configures the host builder for console applications.
    /// </summary>
    internal static class ConsoleHostBuilderConfigurator
    {
        /// <summary>
        /// Creates a configured host builder for console applications.
        /// </summary>
        /// <param name="assembly">The assembly to register commands from.</param>
        /// <param name="args">The command line arguments.</param>
        /// <param name="environment">The host environment.</param>
        /// <returns>A configured host builder.</returns>
        public static IHostBuilder CreateConfiguredHostBuilder(Assembly assembly, string[] args, IHostEnvironment environment)
        {
            return Host.CreateDefaultBuilder(args)
                .UseContentRoot(environment.ContentRootPath)
                .ConfigureLogging((context, logging) =>
                {
                    AdjustDefaultBuilderLoggingProviders(logging);
                    logging.AddNLog(context.Configuration);
                })
                .ConfigureServices((_, services) =>
                {
                    RegisterCommands(services, assembly);
                });
        }

        /// <summary>
        /// Registers all commands found in the specified assembly.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <param name="assembly">The assembly to scan for commands.</param>
        private static void RegisterCommands(IServiceCollection services, Assembly assembly)
        {
            services.Scan(selector => selector.FromAssemblies(assembly)
                .AddClasses(classes => classes.AssignableTo(typeof(IDotNetConsoleCommand<>)))
                .AsImplementedInterfaces());
        }

        /// <summary>
        /// Adjusts the default builder logging providers.
        /// </summary>
        /// <param name="logging">The logging builder.</param>
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
    }
}
