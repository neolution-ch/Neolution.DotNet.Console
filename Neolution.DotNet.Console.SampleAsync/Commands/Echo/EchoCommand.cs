namespace Neolution.DotNet.Console.SampleAsync.Commands.Echo
{
    using System;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using Neolution.DotNet.Console.Abstractions;

    /// <summary>
    /// Prints the command line parameter into the console window
    /// </summary>
    /// <seealso cref="IDotNetConsoleCommand{TOptions}" />
    public class EchoCommand : IDotNetConsoleCommand<EchoOptions>
    {
        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger<EchoCommand> logger;

        /// <summary>
        /// The environment
        /// </summary>
        private readonly IHostEnvironment environment;

        /// <summary>
        /// The configuration
        /// </summary>
        private readonly IConfiguration configuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="EchoCommand" /> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="environment">The environment.</param>
        /// <param name="configuration">The configuration.</param>
        public EchoCommand(ILogger<EchoCommand> logger, IHostEnvironment environment, IConfiguration configuration)
        {
            this.logger = logger;
            this.environment = environment;
            this.configuration = configuration;
        }

        /// <inheritdoc />
        public Task RunAsync(EchoOptions options, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(options);

            this.logger.LogInformation("App is starting in {EnvironmentName} environment.", this.environment.EnvironmentName);

            var connectionStrings = this.configuration.GetSection("ConnectionStrings").GetChildren();
            this.logger.LogInformation("There are {Amount} connection strings configured", connectionStrings.Count());

            this.logger.LogInformation("ECHO: {Value}", options.Message);
            return Task.CompletedTask;
        }
    }
}
