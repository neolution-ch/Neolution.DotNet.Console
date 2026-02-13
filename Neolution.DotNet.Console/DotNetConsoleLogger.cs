namespace Neolution.DotNet.Console
{
    using System;
    using Microsoft.Extensions.Configuration;
    using NLog;
    using NLog.Extensions.Logging;

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
                    throw new InvalidOperationException("Logger has not yet been initialized.");
                }

                return logger;
            }
        }

        /// <summary>
        /// Ensures the logger flushes messages and shuts down internal timers.
        /// </summary>
        public static void Shutdown()
        {
            LogManager.Shutdown();
        }

        /// <summary>
        /// Initializes the logger based on the provided configuration.
        /// </summary>
        /// <param name="configuration">The configuration used to initialize the logger.</param>
        public static void Initialize(IConfiguration configuration)
        {
            try
            {
                LogManager.Setup().LoadConfigurationFromSection(configuration);
                logger = LogManager.GetCurrentClassLogger();
            }
            catch (Exception ex)
            {
                // Fallback: minimal console logger setup using NLog fluent API
                LogManager.Setup().LoadConfiguration(builder => builder.ForLogger().WriteToConsole());
                logger = LogManager.GetCurrentClassLogger();
                logger.Error(ex, "Logger initialization failed.");
            }
        }
    }
}
