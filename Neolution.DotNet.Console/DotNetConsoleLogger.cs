namespace Neolution.DotNet.Console
{
    using System;
    using Microsoft.Extensions.Configuration;
    using NLog;
    using NLog.Extensions.Logging;
    using NLog.Targets;

    /// <summary>
    /// Provides static methods to initialize and manage a logger instance.
    /// </summary>
    public static class DotNetConsoleLogger
    {
        /// <summary>
        /// The internal logger instance used for logging.
        /// </summary>
        private static Logger? logger;

        /// <summary>
        /// Gets the currently initialized logger.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when the logger has not been initialized.</exception>
        public static Logger Log
        {
            get
            {
                if (logger == null)
                {
                    throw new InvalidOperationException("Logger has not been initialized. Call Initialize(configuration) first.");
                }

                return logger;
            }
        }

        /// <summary>
        /// Initializes the logger based on the provided configuration.
        /// </summary>
        /// <param name="configuration">The configuration used to initialize the logger.</param>
        public static void Initialize(IConfiguration configuration)
        {
            ConsoleTarget? consoleTarget = null;
            try
            {
                logger = LogManager.Setup().LoadConfigurationFromSection(configuration).GetCurrentClassLogger();
            }
            catch (Exception ex)
            {
                // Create a simple NLog configuration that logs to the console
                var config = new NLog.Config.LoggingConfiguration();
                consoleTarget = new ConsoleTarget("console");
                config.AddRule(LogLevel.Info, LogLevel.Fatal, consoleTarget);

                LogManager.Configuration = config;
                logger = LogManager.GetCurrentClassLogger();
                logger.Error(ex, "Logger initialization failed");
            }
            finally
            {
                consoleTarget?.Dispose();
            }
        }

        /// <summary>
        /// Ensures the logger flushes messages and shuts down internal timers.
        /// </summary>
        public static void Shutdown()
        {
            LogManager.Shutdown();
        }
    }
}
