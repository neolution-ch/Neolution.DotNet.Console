namespace Neolution.DotNet.Console
{
    using System;
    using System.IO;
    using System.Reflection;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using Neolution.DotNet.Console.Abstractions;
    using NLog.Extensions.Logging;

    /// <summary>
    /// Provides convenience methods for creating instances of <see cref="IConsoleAppBuilder"/> with pre-configured defaults.
    /// </summary>
    /// <remarks>
    /// Customized (for console apps) variant of Microsoft's Host helper
    /// Source: https://github.com/dotnet/runtime/blob/main/src/libraries/Microsoft.Extensions.Hosting/src/Host.cs
    /// </remarks>
    public static class DotNetConsole
    {
        /// <summary>
        /// Creates the default builder.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns>The console app builder.</returns>
        public static IConsoleAppBuilder CreateDefaultBuilder(string[] args)
        {
            var builder = new ConsoleAppBuilder(args);

            var assemblyLocation = typeof(DotNetConsole).Assembly.Location;
            var assemblyPath = Path.GetDirectoryName(assemblyLocation);

            builder.UseContentRoot(assemblyPath);
            builder.ConfigureConsoleConfiguration(config =>
            {
                config.AddEnvironmentVariables(prefix: "DOTNET_");

                if (args != null)
                {
                    config.AddCommandLine(args);
                }
            });

            builder.ConfigureAppConfiguration((context, config) =>
                {
                    var env = context.ConsoleAppEnvironment;

                    config.AddJsonFile("appsettings.json", optional: true)
                        .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

                    if (env.IsDevelopment() && !string.IsNullOrEmpty(env.ApplicationName))
                    {
                        var appAssembly = Assembly.Load(new AssemblyName(env.ApplicationName));
                        if (appAssembly != null)
                        {
                            config.AddUserSecrets(appAssembly, optional: true);
                        }
                    }

                    config.AddEnvironmentVariables();

                    if (args != null)
                    {
                        config.AddCommandLine(args);
                    }
                })
                .ConfigureLogging((context, logging) =>
                {
                    logging.ClearProviders();
                    logging.AddNLog(context.Configuration);
                });

            return builder;
        }
    }
}
