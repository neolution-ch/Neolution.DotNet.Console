namespace Neolution.DotNet.Console.Abstractions
{
    using System.Threading.Tasks;

    /// <summary>
    /// An async console app command that can be started by using a verb from the command line.
    /// </summary>
    /// <typeparam name="TOptions">The type of the options.</typeparam>
    public interface IConsoleAppCommand<in TOptions>
    {
        /// <summary>
        /// Runs the command with the specified options asynchronously.
        /// </summary>
        /// <param name="options">The options.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task RunAsync(TOptions options);
    }
}
