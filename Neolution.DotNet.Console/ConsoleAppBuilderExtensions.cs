namespace Neolution.DotNet.Console
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using Neolution.DotNet.Console.Abstractions;

    /// <summary>
    /// Console app builder extensions
    /// </summary>
    /// <remarks>
    /// Customized (for console apps) variant of Microsoft's HostingHostBuilderExtensions
    /// Source: https://github.com/dotnet/runtime/blob/main/src/libraries/Microsoft.Extensions.Hosting/src/HostingHostBuilderExtensions.cs
    /// </remarks>
    public static class ConsoleAppBuilderExtensions
    {
        /// <summary>
        /// Adds a delegate for configuring the provided <see cref="ILoggingBuilder"/>. This may be called multiple times.
        /// </summary>
        /// <param name="consoleAppBuilder">The <see cref="IConsoleAppBuilder" /> to configure.</param>
        /// <param name="configureDelegate">The delegate that configures the <see cref="ILoggingBuilder"/>.</param>
        /// <returns>The same instance of the <see cref="IConsoleAppBuilder"/> for chaining.</returns>
        public static IConsoleAppBuilder ConfigureLogging(this IConsoleAppBuilder consoleAppBuilder, Action<ConsoleAppBuilderContext, ILoggingBuilder> configureDelegate)
        {
            if (consoleAppBuilder == null)
            {
                throw new ArgumentNullException(nameof(consoleAppBuilder));
            }

            return consoleAppBuilder.ConfigureServices((context, collection) => collection.AddLogging(builder => configureDelegate(context, builder)));
        }

        /// <summary>
        /// Specify the content root directory to be used by the host.
        /// </summary>
        /// <param name="consoleAppBuilder">The <see cref="IConsoleAppBuilder"/> to configure.</param>
        /// <param name="contentRoot">Path to root directory of the application.</param>
        /// <returns>The <see cref="IConsoleAppBuilder"/>.</returns>
        public static IConsoleAppBuilder UseContentRoot(this IConsoleAppBuilder consoleAppBuilder, string contentRoot)
        {
            if (consoleAppBuilder == null)
            {
                throw new ArgumentNullException(nameof(consoleAppBuilder));
            }

            if (contentRoot == null)
            {
                throw new ArgumentNullException(nameof(contentRoot));
            }

            return consoleAppBuilder.ConfigureConsoleConfiguration(configBuilder =>
               {
                   configBuilder.AddInMemoryCollection(new[]
                   {
                        new KeyValuePair<string, string>(HostDefaults.ContentRootKey, contentRoot),
                   });
               });
        }
    }
}
