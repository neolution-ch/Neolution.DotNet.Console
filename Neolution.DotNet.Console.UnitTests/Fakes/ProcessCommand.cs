namespace Neolution.DotNet.Console.UnitTests.Fakes
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Hosting;
    using Neolution.DotNet.Console.Abstractions;
    using Neolution.DotNet.Console.UnitTests.Spies;

    /// <summary>
    /// The command for the process verb with custom argument parsing.
    /// </summary>
    /// <seealso cref="IDotNetConsoleCommand{TOptions}" />
    public class ProcessCommand : IDotNetConsoleCommand<ProcessOptions>
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
        /// Initializes a new instance of the <see cref="ProcessCommand" /> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="environment">The environment.</param>
        public ProcessCommand(IUnitTestLogger logger, IHostEnvironment environment)
        {
            this.logger = logger;
            this.environment = environment;
        }

        /// <inheritdoc />
        public async Task RunAsync(ProcessOptions options, CancellationToken cancellationToken)
        {
            this.logger.Log("options", options);
            this.logger.Log("environment", this.environment);

            // In real scenario, customers would manually parse Environment.GetCommandLineArgs() here
            await Task.CompletedTask;
        }
    }
}
