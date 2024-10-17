namespace Neolution.DotNet.Console
{
    using System;
    using System.Reflection;
    using System.Threading.Tasks;
    using CommandLine;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Neolution.DotNet.Console.Abstractions;

    /// <summary>
    /// The console application.
    /// </summary>
    public class DotNetConsole : IDotNetConsole
    {
        /// <summary>
        /// The host
        /// </summary>
        private readonly IHost host;

        /// <summary>
        /// The command line parser result
        /// </summary>
        private readonly ParserResult<object> commandLineParserResult;

        /// <summary>
        /// Initializes a new instance of the <see cref="DotNetConsole"/> class.
        /// </summary>
        /// <param name="host">The host.</param>
        /// <param name="commandLineParserResult">The command line parser result.</param>
        public DotNetConsole(IHost host, ParserResult<object> commandLineParserResult)
        {
            this.host = host;
            this.commandLineParserResult = commandLineParserResult;
        }

        /// <inheritdoc />
        public IServiceProvider Services => this.host.Services;

        /// <summary>
        /// Creates the default builder.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns>The <see cref="DotNetConsoleBuilder"/>.</returns>
        public static DotNetConsoleBuilder CreateDefaultBuilder(string[] args)
        {
            // Get the entry assembly of the console application. It will later be scanned to find commands and verbs and their options.
            var assembly = Assembly.GetEntryAssembly() ?? throw new DotNetConsoleException("Could not determine entry assembly");

            return DotNetConsoleBuilder.CreateBuilderInternal(assembly, null, args);
        }

        /// <summary>
        /// Creates the builder with explicitly specified assembly.
        /// This is useful when the assembly that contains the commands and verbs is not the same as the entry assembly.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <param name="args">The arguments.</param>
        /// <returns> The <see cref="DotNetConsoleBuilder" />. </returns>
        public static DotNetConsoleBuilder CreateBuilderWithReference(Assembly assembly, string[] args)
        {
            ArgumentNullException.ThrowIfNull(assembly);

            return DotNetConsoleBuilder.CreateBuilderInternal(assembly, null, args);
        }

        /// <summary>
        /// Creates the builder with explicitly specified assembly and verb types.
        /// This is useful when commands and verbs are defined in different assemblies.
        /// </summary>
        /// <param name="servicesAssembly">The assembly.</param>
        /// <param name="verbTypes">The verb types.</param>
        /// <param name="args">The arguments.</param>
        /// <returns> The <see cref="DotNetConsoleBuilder" />. </returns>
        public static DotNetConsoleBuilder CreateBuilderWithReference(Assembly servicesAssembly, Type[] verbTypes, string[] args)
        {
            ArgumentNullException.ThrowIfNull(servicesAssembly);

            return DotNetConsoleBuilder.CreateBuilderInternal(servicesAssembly, verbTypes, args);
        }

        /// <inheritdoc />
        public async Task RunAsync()
        {
            await this.commandLineParserResult.WithParsedAsync(this.RunWithOptionsAsync);
        }

        /// <summary>
        /// Runs the command with options asynchronously.
        /// </summary>
        /// <param name="options">The options.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        private async Task RunWithOptionsAsync(object options)
        {
            // To support scoped services, create a scope for each command call/run.
            var scopeFactory = this.Services.GetRequiredService<IServiceScopeFactory>();
            await using var scope = scopeFactory.CreateAsyncScope();

            // Determine the type of the options object to find the corresponding command type
            var dataType = new[] { options.GetType() };
            var genericBase = typeof(IDotNetConsoleCommand<>);
            var commandType = genericBase.MakeGenericType(dataType);
            var commandEntryPointMethodInfo = commandType.GetMethod(nameof(IDotNetConsoleCommand<object>.RunAsync));

            // Resolve the command from the service provider by the determined type and invoke the entry point method (RunAsync)
            var command = scope.ServiceProvider.GetRequiredService(commandType);
            if (commandEntryPointMethodInfo?.Invoke(command, new[] { options }) is Task commandRunTask)
            {
                await commandRunTask;
            }
        }
    }
}
