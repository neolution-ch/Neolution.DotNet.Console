namespace Neolution.DotNet.Console.UnitTests.Fakes
{
    using CommandLine;
    using Neolution.DotNet.Console.Attributes;

    /// <summary>
    /// The options stub for the <see cref="ProcessCommand"/> - non-default verb with custom argument parsing
    /// </summary>
    [Verb("process")]
    [CustomArgumentParsing]
    public class ProcessOptions
    {
        // No properties defined - customers parse manually in the command
    }
}
