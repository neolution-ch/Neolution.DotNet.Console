namespace Neolution.DotNet.Console.UnitTests.Fakes
{
    using CommandLine;

    /// <summary>
    /// The options stub for the <see cref="DefaultCommandWithoutProperties"/> - simulates customer scenario where options have no properties
    /// </summary>
    [Verb("default-no-props", isDefault: true)]
    public class DefaultOptionsWithoutProperties
    {
        // No properties defined - customers parse manually in the command
    }
}
