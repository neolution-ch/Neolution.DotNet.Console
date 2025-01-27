namespace Neolution.DotNet.Console.Abstractions
{
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// An async console app command that can be started by using a verb from the command line.
    /// </summary>
    /// <typeparam name="TOptions">The type of the verb options.</typeparam>
    public interface IDotNetConsoleCommand<in TOptions>
    {
        /// <summary>
        /// Runs the command with the specified options asynchronously.
        /// </summary>
        /// <param name="options">The options.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task RunAsync(TOptions options, CancellationToken cancellationToken);
    }
}
