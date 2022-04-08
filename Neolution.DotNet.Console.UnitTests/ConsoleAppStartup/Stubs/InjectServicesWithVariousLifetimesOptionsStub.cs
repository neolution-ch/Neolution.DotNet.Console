namespace Neolution.DotNet.Console.UnitTests.ConsoleAppStartup.Stubs
{
    using CommandLine;

    /// <summary>
    /// The options stub for the <see cref="InjectServicesWithVariousLifetimesCommandStub"/>
    /// </summary>
    [Verb(CommandName)]
    public class InjectServicesWithVariousLifetimesOptionsStub
    {
        /// <summary>
        /// The command name
        /// </summary>
        public const string CommandName = "InjectServicesWithVariousLifetimes";
    }
}
