namespace Neolution.DotNet.Console.SampleAsync.Commands.Start
{
    using System;
    using Microsoft.Extensions.Logging;
    using Neolution.DotNet.Console.Abstractions;

    /// <summary>
    /// The default command of the application.
    /// </summary>
    /// <seealso cref="IConsoleAppCommand{TOptions}" />
    public class StartCommand : IConsoleAppCommand<StartOptions>
    {
        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger<StartCommand> logger;

        /// <summary>
        /// The HTTP client
        /// </summary>
        private readonly HttpClient httpClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="StartCommand" /> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="httpClientFactory">The HTTP client factory.</param>
        public StartCommand(ILogger<StartCommand> logger, IHttpClientFactory httpClientFactory)
        {
            this.logger = logger;
            this.httpClient = httpClientFactory.CreateClient();
        }

        /// <inheritdoc />
        public async Task RunAsync(StartOptions options)
        {
            this.logger.LogDebug("Check if Google is online");
            var response = await this.httpClient.GetAsync(new Uri("https://www.google.com"));
            if (response.IsSuccessStatusCode)
            {
                this.logger.LogTrace("Internet connection is available");
            }
            else
            {
                this.logger.LogError("No internet connection detected!");
            }

            this.logger.LogTrace("Trace: Message");
            this.logger.LogDebug("Debug: Message");
            this.logger.LogInformation("Information: Message");
            this.logger.LogWarning("Warning: Message");
            this.logger.LogError("Error: Message");
            this.logger.LogCritical("Critical: Message");

            try
            {
                throw new NotSupportedException("Test exception");
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Exception: Message");
            }

            this.logger.LogInformation("Run ended");
        }
    }
}
