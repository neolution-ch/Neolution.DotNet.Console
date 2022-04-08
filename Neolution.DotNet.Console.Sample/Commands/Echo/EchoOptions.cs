namespace Neolution.DotNet.Console.Sample.Commands.Echo
{
    using CommandLine;

    /// <summary>
    /// The options for the <see cref="EchoCommand"/>.
    /// </summary>
    [Verb("echo", HelpText = "Write a string into the console.")]
    public class EchoOptions
    {
        /// <summary>
        /// Gets or sets the message to echo.
        /// </summary>
        [Value(0)]
        public string Message { get; set; }
    }
}
