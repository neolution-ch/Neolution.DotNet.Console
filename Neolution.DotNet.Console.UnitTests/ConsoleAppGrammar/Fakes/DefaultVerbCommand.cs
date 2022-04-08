namespace Neolution.DotNet.Console.UnitTests.ConsoleAppGrammar.Fakes
{
    using Neolution.DotNet.Console.Abstractions;
    using Neolution.DotNet.Console.UnitTests.Common.Spies;

    /// <summary>
    /// The command for the default verb.
    /// </summary>
    /// <seealso cref="IConsoleAppCommand{DefaultVerbOptions}" />
    public class DefaultVerbCommand : IConsoleAppCommand<DefaultVerbOptions>
    {
        /// <summary>
        /// The logger
        /// </summary>
        private readonly IUnitTestLogger logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultVerbCommand"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        public DefaultVerbCommand(IUnitTestLogger logger)
        {
            this.logger = logger;
        }

        /// <summary>
        /// Runs the command with the specified options.
        /// </summary>
        /// <param name="options">The options.</param>
        public void Run(DefaultVerbOptions options)
        {
            this.logger.Log(options);
        }
    }
}
