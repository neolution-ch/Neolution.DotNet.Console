namespace Neolution.DotNet.Console.Internal
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.Extensions.DependencyInjection;
    using Neolution.DotNet.Console.Abstractions;

    /// <summary>
    /// The no operation console application.
    /// </summary>
    /// <seealso cref="IDotNetConsole" />
    public class NoOperationConsole : IDotNetConsole
    {
        /// <inheritdoc />
        public IServiceProvider Services => new ServiceCollection().BuildServiceProvider();

        /// <inheritdoc />
        public Task RunAsync()
        {
            return Task.CompletedTask;
        }
    }
}
