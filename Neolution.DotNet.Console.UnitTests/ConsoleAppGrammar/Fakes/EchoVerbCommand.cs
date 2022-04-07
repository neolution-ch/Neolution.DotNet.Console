using Neolution.DotNet.Console.Abstractions;
using Neolution.DotNet.Console.UnitTests.Common.Spies;

namespace Neolution.DotNet.Console.UnitTests.ConsoleAppGrammar.Fakes
{
    /// <summary>
    /// Prints the command line parameter into the console window
    /// </summary>
    /// <seealso cref="IConsoleAppCommand{TOptions}" />
    public class EchoVerbCommand : IConsoleAppCommand<EchoOptions>
    {
        /// <summary>
        /// The logger
        /// </summary>
        private readonly IUnitTestLogger logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="EchoVerbCommand"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        public EchoVerbCommand(IUnitTestLogger logger)
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
