namespace Neolution.DotNet.Console.SampleAsync.Commands.Echo
{
    using System;
    using Microsoft.Extensions.Logging;
    using Neolution.DotNet.Console.Abstractions;

    /// <summary>
    /// Prints the command line parameter into the console window
    /// </summary>
    /// <seealso cref="IAsyncConsoleAppCommand{TOptions}" />
    public class EchoCommand : IAsyncConsoleAppCommand<EchoOptions>
    {
        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger<EchoCommand> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="EchoCommand"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        public EchoCommand(ILogger<EchoCommand> logger)
        {
            this.logger = logger;
        }

        /// <inheritdoc />
        public Task RunAsync(EchoOptions options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            this.logger.LogInformation("Called with message: {Message}", options.Message);
            return Task.CompletedTask;
        }
    }
}
