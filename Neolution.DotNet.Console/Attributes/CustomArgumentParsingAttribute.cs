namespace Neolution.DotNet.Console.Attributes
{
    using System;

    /// <summary>
    /// Indicates that this verb options class uses custom argument parsing.
    /// When applied, the CommandLineParser will not fail on arguments that are not mapped to Option or Value attributes,
    /// allowing your command implementation to manually parse command-line arguments.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class CustomArgumentParsingAttribute : Attribute
    {
    }
}
