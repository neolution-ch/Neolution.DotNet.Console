namespace Neolution.DotNet.Console.UnitTests.ConsoleAppGrammar.Fakes
{
    using CommandLine;

    /// <summary>
    /// The options stub for the <see cref="DefaultVerbCommand"/>
    /// </summary>
    [Verb(CommandName, isDefault: true)]
    public class DefaultVerbOptions
    {
        /// <summary>
        /// The command name
        /// </summary>
        public const string CommandName = "default_verb";
    }
}
