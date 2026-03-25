namespace Neolution.DotNet.Console.UnitTests.Fakes
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Hosting;
    using Neolution.DotNet.Console.Abstractions;
    using Neolution.DotNet.Console.UnitTests.Spies;

    /// <summary>
    /// The command for the mixed verb - has properties but also custom parsing.
    /// </summary>
    /// <seealso cref="IDotNetConsoleCommand{TOptions}" />
    public class MixedCommand : IDotNetConsoleCommand<MixedOptions>
    {
        /// <summary>
        /// The logger
        /// </summary>
        private readonly IUnitTestLogger logger;

        /// <summary>
        /// The environment
        /// </summary>
        private readonly IHostEnvironment environment;

        /// <summary>
        /// Initializes a new instance of the <see cref="MixedCommand" /> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="environment">The environment.</param>
        public MixedCommand(IUnitTestLogger logger, IHostEnvironment environment)
        {
            this.logger = logger;
            this.environment = environment;
        }

        /// <inheritdoc />
        public async Task RunAsync(MixedOptions options, CancellationToken cancellationToken)
        {
            this.logger.Log("options", options);
            this.logger.Log("environment", this.environment);

            // In real scenario, customers would manually parse additional args from Environment.GetCommandLineArgs()
            await Task.CompletedTask;
        }
    }
}
