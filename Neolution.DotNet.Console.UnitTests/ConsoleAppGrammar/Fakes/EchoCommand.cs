namespace Neolution.DotNet.Console.UnitTests.ConsoleAppGrammar.Fakes
{
    using System.Threading.Tasks;
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

        /// <inheritdoc />
        public async Task RunAsync(EchoOptions options)
        {
            this.logger.Log(options);
            await Task.CompletedTask;
        }
    }
}
