﻿namespace Neolution.DotNet.Console.Internal
{
    using System;
    using CommandLine;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Neolution.DotNet.Console.Abstractions;

    /// <inheritdoc cref="IConsoleApp" />
    internal sealed class ConsoleApp : ConsoleAppBase, IConsoleApp
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConsoleApp"/> class.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <param name="compositionRootType">Type of the composition root.</param>
        /// <param name="services">The services.</param>
        /// <param name="configuration">The configuration.</param>
        /// <param name="environment">The environment.</param>
        public ConsoleApp(string[] args, Type compositionRootType, IServiceProvider services, IConfiguration configuration, IHostEnvironment environment)
            : base(args, compositionRootType, services, configuration, environment)
        {
        }

        /// <inheritdoc />
        public void Run()
        {
            Parser.Default.ParseArguments(this.Args, this.Verbs)
                .WithParsed(this.RunWithOptions);
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
    }
}
