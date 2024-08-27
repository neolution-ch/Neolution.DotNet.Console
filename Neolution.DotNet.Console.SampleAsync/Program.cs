namespace Neolution.DotNet.Console.SampleAsync
{
    using System;
    using System.Globalization;
    using Microsoft.Extensions.DependencyInjection;
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
            var builder = DotNetConsole.CreateDefaultBuilder(args);

            var logger = LogManager.Setup().LoadConfigurationFromSection(builder.Configuration).GetCurrentClassLogger();

            try
            {
                // Check if IHostEnvironment and IConfiguration are available before building the app
                logger.Debug(CultureInfo.InvariantCulture, message: $"Environment: {builder.Environment.EnvironmentName}");
                logger.Debug(CultureInfo.InvariantCulture, message: $"Setting Value: {builder.Configuration["NLog:throwConfigExceptions"]}");

                // Add services
                builder.Services.AddHttpClient();

                var app = builder.Build();
                await app.RunAsync();
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
    }
}
