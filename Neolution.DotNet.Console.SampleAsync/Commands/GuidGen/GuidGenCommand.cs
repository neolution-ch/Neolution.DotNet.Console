﻿namespace Neolution.DotNet.Console.SampleAsync.Commands.GuidGen
{
    using System;
    using System.Globalization;
    using Microsoft.Extensions.Logging;
    using Neolution.DotNet.Console.Abstractions;

    /// <summary>
    /// Prints a randomly generated GUID in the console window
    /// </summary>
    /// <seealso cref="IDotNetConsoleCommand{TOptions}" />
    public class GuidGenCommand : IDotNetConsoleCommand<GuidGenOptions>
    {
        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger<GuidGenCommand> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="GuidGenCommand"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        public GuidGenCommand(ILogger<GuidGenCommand> logger)
        {
            this.logger = logger;
        }

        /// <inheritdoc />
        public Task RunAsync(GuidGenOptions options, CancellationToken cancellationToken)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            var format = options.Format ?? "B";
            var result = Guid.NewGuid().ToString(format, CultureInfo.InvariantCulture);

            if (options.Upper)
            {
                result = result.ToUpperInvariant();
            }

            System.Console.WriteLine(result);

            this.logger.LogTrace("Wrote result to console");
            return Task.CompletedTask;
        }
    }
}
