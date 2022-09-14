namespace Neolution.DotNet.Console.Internal
{
    using System;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;
    using CommandLine;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Neolution.DotNet.Console.Abstractions;

    /// <summary>
    /// The console app that gets built with the console app builder.
    /// </summary>
    /// <seealso cref="IConsoleApp" />
    internal sealed class ConsoleApp : IConsoleApp
    {
        /// <summary>
        /// The arguments
        /// </summary>
        private readonly string[] args;

        /// <summary>
        /// The verbs
        /// </summary>
        private readonly Type[] verbs;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConsoleApp"/> class.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <param name="compositionRootType">Type of the composition root.</param>
        /// <param name="services">The services.</param>
        /// <param name="configuration">The configuration.</param>
        /// <param name="environment">The environment.</param>
        /// <exception cref="System.ArgumentNullException">services</exception>
        public ConsoleApp(string[] args, Type compositionRootType, IServiceProvider services, IConfiguration configuration, IHostEnvironment environment)
        {
            this.args = args;
            this.verbs = compositionRootType.Assembly.GetTypes()
                .Where(t => t.GetCustomAttribute<VerbAttribute>() != null)
                .ToArray();

            this.Services = services ?? throw new ArgumentNullException(nameof(services));
            this.Configuration = configuration;
            this.Environment = environment;
        }

        /// <summary>
        /// Gets the services.
        /// </summary>
        public IServiceProvider Services { get; }

        /// <summary>
        /// Gets the configuration.
        /// </summary>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// Gets the environment.
        /// </summary>
        public IHostEnvironment Environment { get; }

        /// <summary>
        /// Runs this console app.
        /// </summary>
        public void Run()
        {
            Parser.Default.ParseArguments(this.args, this.verbs)
                .WithParsed(this.RunWithOptions);
        }

        public async Task RunAsync()
        {
            await Parser.Default.ParseArguments(this.args, this.verbs)
                .WithParsedAsync(this.RunWithOptionsAsync);
        }

        /// <summary>
        /// Runs the command with options.
        /// </summary>
        /// <param name="options">The options.</param>
        private void RunWithOptions(object options)
        {
            var dataType = new[] { options.GetType() };
            var genericBase = typeof(IConsoleAppCommand<>);
            var combinedType = genericBase.MakeGenericType(dataType);
            var method = combinedType.GetMethod(nameof(IConsoleAppCommand<object>.Run));

            // To support scoped services, create a scope for each command call/run.
            var scopeFactory = this.Services.GetRequiredService<IServiceScopeFactory>();
            using var scope = scopeFactory.CreateScope();

            var command = scope.ServiceProvider.GetRequiredService(combinedType);
            method?.Invoke(command, new[] { options });
        }

        private async Task RunWithOptionsAsync(object options)
        {
            var dataType = new[] { options.GetType() };
            var genericBase = typeof(IConsoleAppCommand<>);
            var combinedType = genericBase.MakeGenericType(dataType);
            var method = combinedType.GetMethod(nameof(IConsoleAppCommand<object>.Run));

            // To support scoped services, create a scope for each command call/run.
            var scopeFactory = this.Services.GetRequiredService<IServiceScopeFactory>();
            using var scope = scopeFactory.CreateScope();

            var command = scope.ServiceProvider.GetRequiredService(combinedType);
            var result = (Task)method?.Invoke(command, new[] { options });
            if (result is null)
            {
                await Task.CompletedTask;
                return;
            }

            await result.ConfigureAwait(true);
        }
    }
}
