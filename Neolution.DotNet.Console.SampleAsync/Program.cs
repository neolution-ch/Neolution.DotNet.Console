namespace Neolution.DotNet.Console.SampleAsync
{
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
            try
            {
                var builder = DotNetConsole.CreateDefaultBuilder(args);
                DotNetConsoleLogger.Initialize(builder.Configuration);

                var startup = new Startup(builder.Environment, builder.Configuration);
                startup.ConfigureServices(builder.Services);
                var console = builder.Build();
                await console.RunAsync();
            }
            catch (Exception ex)
            {
                DotNetConsoleLogger.Log.Error(ex, "Stopped program because of an unexpected exception");
                throw;
            }
            finally
            {
                // Ensure to flush and stop internal timers/threads before application-exit (Avoid segmentation fault on Linux)
                DotNetConsoleLogger.Shutdown();
            }
        }
    }
}
