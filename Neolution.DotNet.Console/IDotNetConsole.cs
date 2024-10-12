namespace Neolution.DotNet.Console
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
