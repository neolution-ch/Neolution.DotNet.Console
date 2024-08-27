namespace Neolution.DotNet.Console.UnitTests.ConsoleAppGrammar.Fakes
{
    using System.Threading.Tasks;
    using Neolution.DotNet.Console.Abstractions;
    using Neolution.DotNet.Console.UnitTests.Common.Spies;

    /// <summary>
    /// The command for the default verb.
    /// </summary>
    /// <seealso cref="IConsoleAppCommand{DefaultVerbOptions}" />
    public class DefaultCommand : IConsoleAppCommand<DefaultOptions>
    {
        /// <summary>
        /// The logger
        /// </summary>
        private readonly IUnitTestLogger logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultCommand"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        public DefaultCommand(IUnitTestLogger logger)
        {
            this.logger = logger;
        }

        /// <inheritdoc />
        public async Task RunAsync(DefaultOptions options)
        {
            this.logger.Log(options);
            await Task.CompletedTask;
        }
    }
}
