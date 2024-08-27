namespace Neolution.DotNet.Console.SampleAsync
{
    using System;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
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
            var builder = ConsoleApplication.CreateDefaultBuilder(args);
            Console.WriteLine($"Environment: {builder.Environment.EnvironmentName}");
            Console.WriteLine($"Setting Value: {builder.Configuration["NLog:throwConfigExceptions"]}");

            var logger = LogManager.Setup().LoadConfigurationFromSection(builder.Configuration).GetCurrentClassLogger();

            try
            {
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
