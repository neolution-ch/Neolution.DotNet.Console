namespace Neolution.DotNet.Console.Sample.Commands.Echo
{
    using System;
    using Microsoft.Extensions.Logging;
    using Neolution.DotNet.Console.Abstractions;

    /// <summary>
    /// Prints the command line parameter into the console window
    /// </summary>
    /// <seealso cref="IConsoleAppCommand{TOptions}" />
    public class EchoCommand : IConsoleAppCommand<EchoOptions>
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

        /// <summary>
        /// Runs the command with the specified options.
        /// </summary>
        /// <param name="options">The options.</param>
        /// <exception cref="System.ArgumentNullException">options</exception>
        public void Run(EchoOptions options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            this.logger.LogInformation("Called with message: {Message}", options.Message);
        }
    }
}
