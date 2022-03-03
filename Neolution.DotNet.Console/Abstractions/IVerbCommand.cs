namespace Neolution.DotNet.Console.Abstractions
{
    /// <summary>
    /// A command that can be started by using a verb from the command line.
    /// </summary>
    /// <typeparam name="TOptions">The type of the verb options.</typeparam>
    public interface IVerbCommand<in TOptions>
    {
        /// <summary>
        /// Runs the command with the specified options.
        /// </summary>
        /// <param name="options">The options.</param>
        void Run(TOptions options);
    }
}
