namespace Neolution.DotNet.Console
{
    using System;
    using System.Reflection;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.FileProviders;
    using Microsoft.Extensions.Hosting;
    using Neolution.DotNet.Console.Internal;

    /// <summary>
    /// Provides default configurations and environment setup for the console application.
    /// </summary>
    internal static class DotNetConsoleDefaults
    {
        /// <summary>
        /// The command argument used to trigger dependency validation.
        /// </summary>
        internal const string CheckDependenciesCommand = "check-deps";

        /// <summary>
        /// Determines if the given arguments represent a check-deps run.
        /// </summary>
        /// <param name="args">The command line arguments.</param>
        /// <returns>True if this is a check-deps run, false otherwise.</returns>
        public static bool IsCheckDependenciesRun(string[] args)
        {
            return args.Length == 1 && string.Equals(args[0], CheckDependenciesCommand, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Creates the console environment.
        /// </summary>
        /// <param name="args">The command line arguments.</param>
        /// <returns>The <see cref="IHostEnvironment"/>.</returns>
        internal static DotNetConsoleEnvironment CreateConsoleEnvironment(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .AddEnvironmentVariables(prefix: "DOTNET_")
                .AddCommandLine(args)
                .Build();

            // The apps root directory is where the appsettings.json are located
            var appRootDirectory = AppContext.BaseDirectory;

            // Check if this is a check-deps run - if so, always use Development environment
            var isCheckDepsRun = IsCheckDependenciesRun(args);

            // Default to Production for normal runs, matching ASP.NET Core behavior
            // For check-deps, always use Development to ensure appsettings.Development.json is loaded
            // Environment can be overridden via DOTNET_ENVIRONMENT or command line arguments
            var defaultEnvironment = isCheckDepsRun ? Environments.Development : Environments.Production;

            return new DotNetConsoleEnvironment
            {
                EnvironmentName = isCheckDepsRun ? Environments.Development : (configuration[HostDefaults.EnvironmentKey] ?? defaultEnvironment),
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
        internal static IConfiguration CreateConsoleConfiguration(Assembly assembly, string[] args, IHostEnvironment environment)
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
