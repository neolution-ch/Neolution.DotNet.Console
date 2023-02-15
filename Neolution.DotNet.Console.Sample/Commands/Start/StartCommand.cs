namespace Neolution.DotNet.Console.Sample.Commands.Start
{
    using System;
    using Microsoft.Extensions.Logging;
    using Neolution.DotNet.Console.Abstractions;

    /// <summary>
    /// The default command of the application.
    /// </summary>
    /// <seealso cref="IConsoleAppCommand{TOptions}" />
    public class StartCommand : IConsoleAppCommand<StartOptions>
    {
        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger<StartCommand> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="StartCommand"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        public StartCommand(ILogger<StartCommand> logger)
        {
            this.logger = logger;
        }

        /// <summary>
        /// Runs the command with the specified options.
        /// </summary>
        /// <param name="options">The options.</param>
        public void Run(StartOptions options)
        {
            if (options is null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            const string message = "Hello World!";
            this.logger.LogTrace(message);
            this.logger.LogDebug(message);
            this.logger.LogInformation(message);
            this.logger.LogWarning(message);
            this.logger.LogError(message);
            this.logger.LogCritical(message);
        }
    }
}
