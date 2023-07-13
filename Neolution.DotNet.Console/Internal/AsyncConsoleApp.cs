namespace Neolution.DotNet.Console.Internal
{
    using System;
    using System.Threading.Tasks;
    using CommandLine;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Neolution.DotNet.Console.Abstractions;

    /// <inheritdoc cref="IAsyncConsoleApp" />
    internal sealed class AsyncConsoleApp : ConsoleAppBase, IAsyncConsoleApp
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncConsoleApp"/> class.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <param name="compositionRootType">Type of the composition root.</param>
        /// <param name="services">The services.</param>
        /// <param name="configuration">The configuration.</param>
        /// <param name="environment">The environment.</param>
        public AsyncConsoleApp(string[] args, Type compositionRootType, IServiceProvider services, IConfiguration configuration, IHostEnvironment environment)
            : base(args, compositionRootType, services, configuration, environment)
        {
        }

        /// <inheritdoc />
        public async Task RunAsync()
        {
            await Parser.Default.ParseArguments(this.Args, this.Verbs)
                .WithParsedAsync(this.RunWithOptionsAsync);
        }

        /// <summary>
        /// Runs the command with options asynchronously.
        /// </summary>
        /// <param name="options">The options.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        private async Task RunWithOptionsAsync(object options)
        {
            var dataType = new[] { options.GetType() };
            var genericBase = typeof(IAsyncConsoleAppCommand<>);
            var combinedType = genericBase.MakeGenericType(dataType);
            var method = combinedType.GetMethod(nameof(IAsyncConsoleAppCommand<object>.RunAsync));

            // To support scoped services, create a scope for each command call/run.
            var scopeFactory = this.Services.GetRequiredService<IServiceScopeFactory>();
            await using var scope = scopeFactory.CreateAsyncScope();

            var command = scope.ServiceProvider.GetRequiredService(combinedType);
            var result = (Task)method?.Invoke(command, new[] { options });
            if (result is null)
            {
                await Task.CompletedTask;
                return;
            }

            await result;
        }
    }
}
