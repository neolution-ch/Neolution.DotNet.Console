namespace Neolution.DotNet.Console.Abstractions
{
    /// <summary>
    /// An abstraction for console apps.
    /// </summary>
    public interface IConsoleApp : IConsoleAppBase
    {
        /// <summary>
        /// Runs this console app.
        /// </summary>
        void Run();
    }
}
