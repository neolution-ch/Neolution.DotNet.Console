namespace Neolution.DotNet.Console.SampleAsync.Commands.GuidGen
{
    using CommandLine;
    using Neolution.DotNet.Console.SampleAsync.Commands.Echo;

    /// <summary>
    /// The options for the <see cref="EchoCommand"/>.
    /// </summary>
    [Verb("guidgen", HelpText = "Generate a new GUID and write it into the console.")]
    public class GuidGenOptions
    {
        /// <summary>
        /// Gets or sets the format.
        /// </summary>
        [Option(shortName: 'f', longName: "format", Required = false, HelpText = "How the generated GUID will be formatted.")]
        public string? Format { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the message should printed in upper case.
        /// </summary>
        /// <value>
        ///   <c>true</c> if upper case; otherwise, <c>false</c>.
        /// </value>
        [Option(shortName: 'u', longName: "upper", Required = false, HelpText = "Prints the GUID in upper case.")]
        public bool Upper { get; set; }
    }
}
