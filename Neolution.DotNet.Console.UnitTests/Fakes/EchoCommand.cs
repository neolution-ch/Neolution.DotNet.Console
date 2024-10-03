namespace Neolution.DotNet.Console.UnitTests.Fakes
{
    using System.Threading.Tasks;
    using Neolution.DotNet.Console.Abstractions;
    using Neolution.DotNet.Console.UnitTests.Spies;

    /// <summary>
    /// Fake command that logs the value passed via command line.
    /// </summary>
    /// <seealso cref="IDotNetConsoleCommand{TOptions}" />
    public class EchoCommand : IDotNetConsoleCommand<EchoOptions>
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
            this.logger.Log("options", options);
            await Task.CompletedTask;
        }
    }
}
