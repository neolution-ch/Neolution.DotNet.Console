using CommandLine;

namespace Neolution.DotNet.Console.UnitTests.ConsoleAppGrammar.Fakes
{
    [Verb(CommandName, isDefault: true)]
    public class DefaultVerbOptions
    {
        public const string CommandName = "default_verb";
    }
}
