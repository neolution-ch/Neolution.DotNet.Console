namespace Neolution.DotNet.Console.UnitTests.ConsoleAppGrammar.Fakes
{
    using Neolution.DotNet.Console.Abstractions;
    using Neolution.DotNet.Console.UnitTests.Common.Spies;

    /// <summary>
    /// Fake command that logs the value passed via command line.
    /// </summary>
    /// <seealso cref="IConsoleAppCommand{TOptions}" />
    public class EchoCommand : IConsoleAppCommand<EchoOptions>
    {
        /// <summary>
        /// The logger
        /// </summary>
        private readonly IUnitTestLogger logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="EchoCommand"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        public EchoCommand(IUnitTestLogger logger)
        {
            this.logger = logger;
        }

        /// <summary>
        /// Runs the command with the specified options.
        /// </summary>
        /// <param name="options">The options.</param>
        public void Run(EchoOptions options)
        {
            this.logger.Log(options);
        }
    }
}
