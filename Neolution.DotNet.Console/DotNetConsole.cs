namespace Neolution.DotNet.Console
{
    using System;
    using System.Globalization;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;
    using CommandLine;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Neolution.DotNet.Console.Abstractions;
    using NLog;
    using NLog.Extensions.Logging;

    /// <summary>
    /// The console application.
    /// </summary>
    public class DotNetConsole
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

        /// <summary>
        /// Gets the services.
        /// </summary>
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

        /// <summary>
        /// Runs the application.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task RunAsync()
        {
            using var cancellationTokenSource = new CancellationTokenSource();

            Console.CancelKeyPress += (_, e) =>
            {
                // Prevent the console from terminating immediately and instead cancel the cancellation token source.
                e.Cancel = true;
                cancellationTokenSource.Cancel();
            };

            await this.RunAsync(cancellationTokenSource.Token);
        }

        /// <summary>
        /// Runs the application.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task RunAsync(CancellationToken cancellationToken)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                await this.commandLineParserResult.WithParsedAsync(async options => await this.RunWithOptionsAsync(options, cancellationToken));
            }
            catch (OperationCanceledException)
            {
                try
                {
                    var logger = LogManager.Setup().LoadConfigurationFromSection(this.Services.GetRequiredService<IConfiguration>()).GetCurrentClassLogger();
                    logger.Log(LogLevel.Info, CultureInfo.InvariantCulture, message: "Operation was canceled by the user.");
                }
                catch (Exception)
                {
                    // Ignore any exceptions that might occur while trying to log the cancellation
                }
            }
        }

        /// <summary>
        /// Runs the command with options asynchronously.
        /// </summary>
        /// <param name="options">The options.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns> The <see cref="Task" />. </returns>
        private async Task RunWithOptionsAsync(object options, CancellationToken cancellationToken)
        {
            // Check for cancellation before proceeding
            cancellationToken.ThrowIfCancellationRequested();

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
            if (commandEntryPointMethodInfo != null)
            {
                // Prepare the parameters, including the cancellation token
                var parameters = new[] { options, cancellationToken };

                // Invoke the command's RunAsync method with the cancellation token
                if (commandEntryPointMethodInfo.Invoke(command, parameters) is Task commandRunTask)
                {
                    await commandRunTask;
                }
            }
        }
    }
}
