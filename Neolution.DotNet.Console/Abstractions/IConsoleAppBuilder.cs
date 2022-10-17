namespace Neolution.DotNet.Console.Abstractions
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Neolution.DotNet.Console.Internal;

    /// <summary>
    /// A console app initialization abstraction.
    /// </summary>
    /// <remarks>
    /// Customized (for console apps) variant of Microsoft's IHostBuilder
    /// Source: https://github.com/dotnet/runtime/blob/main/src/libraries/Microsoft.Extensions.Hosting.Abstractions/src/IHostBuilder.cs
    /// </remarks>
    public interface IConsoleAppBuilder
    {
        /// <summary>
        /// Gets a central location for sharing state between components during the host building process.
        /// </summary>
        IDictionary<object, object> Properties { get; }

        /// <summary>
        /// Set up the configuration for the builder itself. This will be used to initialize the <see cref="ConsoleAppEnvironment"/>
        /// for use later in the build process. This can be called multiple times and the results will be additive.
        /// </summary>
        /// <param name="configureDelegate">The delegate for configuring the <see cref="IConfigurationBuilder"/> that will be used
        /// to construct the <see cref="IConfiguration"/> for the host.</param>
        /// <returns>The same instance of the <see cref="IConsoleAppBuilder"/> for chaining.</returns>
        IConsoleAppBuilder ConfigureConsoleConfiguration(Action<IConfigurationBuilder> configureDelegate);

        /// <summary>
        /// Sets up the configuration for the remainder of the build process and application. This can be called multiple times and
        /// the results will be additive. The results will be available at <see cref="ConsoleAppBuilderContext.Configuration"/> for
        /// subsequent operations.
        /// </summary>
        /// <param name="configureDelegate">The delegate for configuring the <see cref="IConfigurationBuilder"/> that will be used
        /// to construct the <see cref="IConfiguration"/> for the application.</param>
        /// <returns>The same instance of the <see cref="IConsoleAppBuilder"/> for chaining.</returns>
        IConsoleAppBuilder ConfigureAppConfiguration(Action<ConsoleAppBuilderContext, IConfigurationBuilder> configureDelegate);

        /// <summary>
        /// Adds services to the container. This can be called multiple times and the results will be additive.
        /// </summary>
        /// <param name="configureDelegate">The delegate for configuring the <see cref="IServiceCollection"/> that will be used
        /// to construct the <see cref="IServiceProvider"/>.</param>
        /// <returns>The same instance of the <see cref="IConsoleAppBuilder"/> for chaining.</returns>
        IConsoleAppBuilder ConfigureServices(Action<ConsoleAppBuilderContext, IServiceCollection> configureDelegate);

        /// <summary>
        /// Run the given actions to initialize the host. This can only be called once.
        /// </summary>
        /// <returns>The entry point wired up with dependency injection and the resolved configuration instance.</returns>
        IConsoleApp Build();

        /// <summary>
        /// Run the given actions to initialize the async host. This can only be called once.
        /// </summary>
        /// <returns>The entry point wired up with dependency injection and the resolved configuration instance.</returns>
        IAsyncConsoleApp AsyncBuild();

        /// <summary>
        /// Instructs the console app builder to use the specified composition root.
        /// </summary>
        /// <typeparam name="TCompositionRoot">The type of the composition root.</typeparam>
        /// <returns>The console app builder.</returns>
        IConsoleAppBuilder UseCompositionRoot<TCompositionRoot>()
            where TCompositionRoot : ICompositionRoot;
    }
}
