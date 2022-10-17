namespace Neolution.DotNet.Console.Abstractions
{
    using System.Threading.Tasks;

    /// <summary>
    /// An abstraction for async console apps.
    /// </summary>
    public interface IAsyncConsoleApp : IConsoleAppBase
    {
        /// <summary>
        /// Runs this console app asynchronously.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        Task RunAsync();
    }
}
