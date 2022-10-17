namespace Neolution.DotNet.Console.SampleAsync.Commands.Start
{
    using System;
    using Microsoft.Extensions.Logging;
    using Neolution.DotNet.Console.Abstractions;

    /// <summary>
    /// The default command of the application.
    /// </summary>
    /// <seealso cref="IAsyncConsoleAppCommand{TOptions}" />
    public class StartCommand : IAsyncConsoleAppCommand<StartOptions>
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

        /// <inheritdoc />
        public Task RunAsync(StartOptions options)
        {
            if (options is null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            this.logger.LogInformation("Hello World!");
            return Task.CompletedTask;
        }
    }
}
