namespace Neolution.DotNet.Console.Abstractions
{
    using System;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Hosting;

    /// <summary>
    /// A console app abstraction base.
    /// </summary>
    /// <remarks>
    /// Customized (for console apps) variant of Microsoft's IHost
    /// Source: https://github.com/dotnet/runtime/blob/main/src/libraries/Microsoft.Extensions.Hosting.Abstractions/src/IHost.cs
    /// </remarks>
    public interface IConsoleAppBase
    {
        /// <summary>
        /// Gets the services.
        /// </summary>
        public IServiceProvider Services { get; }

        /// <summary>
        /// Gets the configuration.
        /// </summary>
        IConfiguration Configuration { get; }

        /// <summary>
        /// Gets the environment.
        /// </summary>
        IHostEnvironment Environment { get; }
    }
}
