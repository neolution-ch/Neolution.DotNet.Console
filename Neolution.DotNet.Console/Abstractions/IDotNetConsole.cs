using Neolution;
using Neolution.DotNet;
using Neolution.DotNet.Console;
using Neolution.DotNet.Console.Abstractions;

namespace Neolution.DotNet.Console.Abstractions
{
    using System;
    using System.Threading.Tasks;

    /// <summary>
    /// The console application.
    /// </summary>
    public interface IDotNetConsole
    {
        /// <summary>
        /// Gets the services.
        /// </summary>
        IServiceProvider Services { get; }

        /// <summary>
        /// Runs the application.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        Task RunAsync();
    }
}
