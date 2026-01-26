namespace Neolution.DotNet.Console.UnitTests.Fakes
{
    using CommandLine;
    using Neolution.DotNet.Console.Attributes;

    /// <summary>
    /// The options stub for the <see cref="MixedCommand"/> - has properties but also allows unknown args
    /// </summary>
    [Verb("mixed")]
    [CustomArgumentParsing]
    public class MixedOptions
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        [Option("name")]
        public string? Name { get; set; }

        /// <summary>
        /// Gets or sets the count.
        /// </summary>
        [Option("count")]
        public int Count { get; set; }
    }
}
