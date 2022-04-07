using Neolution.DotNet.Console.Abstractions;
using Neolution.DotNet.Console.UnitTests.Common.Spies;

namespace Neolution.DotNet.Console.UnitTests.ConsoleAppGrammar.Fakes
{
    public class DefaultVerbCommand : IConsoleAppCommand<DefaultVerbOptions>
    {
        private readonly IUnitTestLogger logger;

        public DefaultVerbCommand(IUnitTestLogger logger)
        {
            this.logger = logger;
        }

        public void Run(DefaultVerbOptions options)
        {
            this.logger.Log(options);
        }
    }
}
