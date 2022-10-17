﻿namespace Neolution.DotNet.Console.SampleAsync
{
    using System;
    using Neolution.DotNet.Console.Abstractions;
    using NLog;
    using NLog.Extensions.Logging;

    /// <summary>
    /// The program
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// Defines the entry point of the application.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public static async Task Main(string[] args)
        {
            var console = CreateConsoleAppBuilder(args).AsyncBuild();
            var logger = LogManager.Setup().LoadConfigurationFromSection(console.Configuration).GetCurrentClassLogger();

            try
            {
                await console.RunAsync().ConfigureAwait(true);
            }
            catch (Exception ex)
            {
                // NLog: catch any exception and log it.
                logger.Error(ex, "Stopped program because of an unexpected exception");
                throw;
            }
            finally
            {
                // Ensure to flush and stop internal timers/threads before application-exit (Avoid segmentation fault on Linux)
                LogManager.Shutdown();
            }
        }

        /// <summary>
        /// Creates the console application builder.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns>The console app builder.</returns>
        private static IConsoleAppBuilder CreateConsoleAppBuilder(string[] args)
        {
            return DotNetConsole.CreateDefaultBuilder(args)
                .UseCompositionRoot<Startup>();
        }
    }
}
