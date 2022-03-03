namespace Neolution.DotNet.Console.Abstractions
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Hosting;

    /// <summary>
    /// Context containing the common services on the console app. Some properties may be null until set by the console app.
    /// </summary>
    /// <remarks>
    /// Customized (for console apps) variant of Microsoft's HostBuilderContext
    /// Source: https://github.com/dotnet/extensions/blob/master/src/Hosting/Abstractions/src/HostBuilderContext.cs
    /// </remarks>
    public class ConsoleAppBuilderContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConsoleAppBuilderContext"/> class.
        /// </summary>
        /// <param name="properties">The properties.</param>
        /// <exception cref="System.ArgumentNullException">properties</exception>
        public ConsoleAppBuilderContext(IDictionary<object, object> properties)
        {
            this.Properties = properties ?? throw new ArgumentNullException(nameof(properties));
        }

        /// <summary>
        /// Gets or sets the <see cref="ConsoleAppEnvironment" /> initialized by the console app.
        /// </summary>
        public IHostEnvironment ConsoleAppEnvironment { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IConfiguration" /> containing the merged configuration of the application and the host.
        /// </summary>
        public IConfiguration Configuration { get; set; }

        /// <summary>
        /// Gets a central location for sharing state between components during the host building process.
        /// </summary>
        public IDictionary<object, object> Properties { get; }
    }
}
