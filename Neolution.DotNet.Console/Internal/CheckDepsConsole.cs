namespace Neolution.DotNet.Console.Internal
{
    using System;
    using System.Globalization;
    using System.Threading.Tasks;
    using Microsoft.Extensions.DependencyInjection;
    using Neolution.DotNet.Console.Abstractions;

    /// <summary>
    /// The check dependencies console application.
    /// </summary>
    /// <seealso cref="IDotNetConsole" />
    internal class CheckDepsConsole : IDotNetConsole
    {
        /// <inheritdoc />
        public IServiceProvider Services => new ServiceCollection().BuildServiceProvider();

        /// <inheritdoc />
        public Task RunAsync()
        {
            DotNetConsoleLogger.Log.Info(CultureInfo.InvariantCulture, message: "Dependency injection validation succeeded. All registered services can be constructed and no DI issues were found.");
            return Task.CompletedTask;
        }
    }
}
