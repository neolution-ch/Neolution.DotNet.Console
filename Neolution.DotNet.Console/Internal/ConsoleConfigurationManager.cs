namespace Neolution.DotNet.Console.Internal
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Hosting;

    /// <summary>
    /// Manages configuration building for the console application.
    /// </summary>
    internal class ConsoleConfigurationManager
    {
        /// <summary>
        /// The initial configuration builder used to create dynamic configuration
        /// </summary>
        private readonly IConfigurationBuilder initialConfigurationBuilder;

        /// <summary>
        /// The host builder context used for dynamic configuration building
        /// </summary>
        private readonly HostBuilderContext hostBuilderContext;

        /// <summary>
        /// List of configuration delegates to be applied during host building.
        /// </summary>
        private readonly List<Action<HostBuilderContext, IConfigurationBuilder>> configurationDelegates = new();

        /// <summary>
        /// The built configuration root - built once when first accessed
        /// </summary>
        private IConfigurationRoot? builtConfiguration;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConsoleConfigurationManager"/> class.
        /// </summary>
        /// <param name="configurationBuilder">The initial configuration builder.</param>
        /// <param name="hostBuilderContext">The host builder context.</param>
        public ConsoleConfigurationManager(IConfigurationBuilder configurationBuilder, HostBuilderContext hostBuilderContext)
        {
            this.initialConfigurationBuilder = configurationBuilder;
            this.hostBuilderContext = hostBuilderContext;
        }

        /// <summary>
        /// Gets a collection of configuration providers for the application to compose. This is useful for adding new configuration sources and providers.
        /// </summary>
        public IConfiguration Configuration
        {
            get
            {
                // Build the configuration once when first accessed, similar to Microsoft's approach
                this.builtConfiguration ??= this.BuildConfiguration();

                return this.builtConfiguration;
            }
        }

        /// <summary>
        /// Gets the configuration delegates for use in host building.
        /// </summary>
        public IEnumerable<Action<HostBuilderContext, IConfigurationBuilder>> ConfigurationDelegates => this.configurationDelegates;

        /// <summary>
        /// Adds a delegate for configuring additional configuration sources for the application.
        /// </summary>
        /// <param name="configureDelegate">The delegate for configuring the configuration builder.</param>
        public void AddConfigurationDelegate(Action<HostBuilderContext, IConfigurationBuilder> configureDelegate)
        {
            ArgumentNullException.ThrowIfNull(configureDelegate);

            this.configurationDelegates.Add(configureDelegate);

            // Reset the built configuration so it gets rebuilt on next access
            this.builtConfiguration = null;
        }

        /// <summary>
        /// Builds the configuration from all sources and delegates.
        /// </summary>
        /// <returns>The built configuration root.</returns>
        private IConfigurationRoot BuildConfiguration()
        {
            // Create a new configuration builder based on the initial one
            var configBuilder = new ConfigurationBuilder();

            // Add all sources from the initial configuration builder
            foreach (var source in this.initialConfigurationBuilder.Sources)
            {
                configBuilder.Add(source);
            }

            // Apply all configuration delegates that have been added via ConfigureAppConfiguration
            foreach (var configureDelegate in this.configurationDelegates)
            {
                configureDelegate(this.hostBuilderContext, configBuilder);
            }

            // Build and store the configuration root
            return configBuilder.Build();
        }
    }
}
