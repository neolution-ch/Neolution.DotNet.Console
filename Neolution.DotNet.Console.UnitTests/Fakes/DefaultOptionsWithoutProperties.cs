namespace Neolution.DotNet.Console.UnitTests.Fakes
{
    using CommandLine;
    using Neolution.DotNet.Console.Attributes;

    /// <summary>
    /// The options stub for the <see cref="DefaultCommandWithoutProperties"/> - simulates scenario where options have none or are ignoring properties
    /// </summary>
    [Verb("default-no-props", isDefault: true)]
    [CustomArgumentParsing]
    public class DefaultOptionsWithoutProperties
    {
        // No properties defined, options may be parsed manually in the command
    }
}
