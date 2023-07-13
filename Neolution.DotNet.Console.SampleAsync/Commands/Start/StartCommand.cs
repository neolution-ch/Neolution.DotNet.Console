namespace Neolution.DotNet.Console.SampleAsync.Commands.Start
{
    using System;
    using Microsoft.Extensions.Logging;
    using Neolution.DotNet.Console.Abstractions;

    /// <summary>
    /// The default command of the application.
    /// </summary>
    /// <seealso cref="IAsyncConsoleAppCommand{TOptions}" />
    public class StartCommand : IAsyncConsoleAppCommand<StartOptions>
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
            var response = await this.httpClient.GetAsync(new Uri("http://www.google.com"));
            var html = await response.Content.ReadAsStringAsync();

            this.logger.LogInformation("Hello World!");
            this.logger.LogInformation("Response: {PartOfHtml}...", html[..100]);
        }
    }
}
