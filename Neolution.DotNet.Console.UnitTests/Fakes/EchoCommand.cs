namespace Neolution.DotNet.Console.UnitTests.Fakes
{
    using Neolution.DotNet.Console.Abstractions;

    /// <summary>
    /// Prints the command line parameter into the console window
    /// </summary>
    /// <seealso cref="IVerbCommand{TOptions}" />
    public class EchoCommand : IVerbCommand<EchoOptions>
    {
        /// <summary>
        /// The tracker
        /// </summary>
        private readonly IUnitTestTracker tracker;

        /// <summary>
        /// Initializes a new instance of the <see cref="EchoCommand"/> class.
        /// </summary>
        /// <param name="tracker">The tracker.</param>
        public EchoCommand(IUnitTestTracker tracker)
        {
            this.tracker = tracker;
        }

        /// <summary>
        /// Runs the command with the specified options.
        /// </summary>
        /// <param name="options">The options.</param>
        public void Run(EchoOptions options)
        {
            this.tracker.Track(options);
        }
    }
}
