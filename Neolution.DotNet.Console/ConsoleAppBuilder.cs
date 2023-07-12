namespace Neolution.DotNet.Console
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.FileProviders;
    using Microsoft.Extensions.Hosting;
    using Neolution.DotNet.Console.Abstractions;
    using Neolution.DotNet.Console.Internal;

    /// <remarks>
    /// Customized (for console apps) variant of Microsoft's HostBuilder
    /// Source: https://github.com/dotnet/runtime/blob/master/src/libraries/Microsoft.Extensions.Hosting/src/HostBuilder.cs
    /// </remarks>
    /// <inheritdoc cref="IConsoleAppBuilder" />
    [SuppressMessage("Major Code Smell", "S1200:Classes should not be coupled to too many other classes (Single Responsibility Principle)", Justification = "Follow the code of the Microsoft HostBuilder as close as possible.")]
    public sealed class ConsoleAppBuilder : IConsoleAppBuilder
    {
        /// <summary>
        /// The command line arguments
        /// </summary>
        private readonly string[] args;

        /// <summary>
        /// The configure console configuration actions
        /// </summary>
        private readonly List<Action<IConfigurationBuilder>> configureConsoleConfigActions = new();

        /// <summary>
        /// The configure application configuration actions
        /// </summary>
        private readonly List<Action<ConsoleAppBuilderContext, IConfigurationBuilder>> configureAppConfigActions = new();

        /// <summary>
        /// The configure services actions
        /// </summary>
        private readonly List<Action<ConsoleAppBuilderContext, IServiceCollection>> configureServicesActions = new();

        /// <summary>
        /// The composition root type.
        /// </summary>
        private Type compositionRootType;

        /// <summary>
        /// The built flag for the container. Make sure to build the container only once.
        /// https://stackoverflow.com/a/56058498/879553
        /// </summary>
        private bool containerBuilt;

        /// <summary>
        /// The console configuration
        /// </summary>
        private IConfiguration consoleConfiguration;

        /// <summary>
        /// The application configuration
        /// </summary>
        private IConfiguration appConfiguration;

        /// <summary>
        /// The console application builder context
        /// </summary>
        private ConsoleAppBuilderContext consoleAppBuilderContext;

        /// <summary>
        /// The console application environment
        /// </summary>
        private IHostEnvironment consoleAppEnvironment;

        /// <summary>
        /// The application services
        /// </summary>
        private IServiceProvider appServices;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConsoleAppBuilder"/> class.
        /// </summary>
        /// <param name="args">The arguments.</param>
        public ConsoleAppBuilder(string[] args)
        {
            this.args = args;
        }

        /// <inheritdoc />
        public IDictionary<object, object> Properties { get; } = new Dictionary<object, object>();

        /// <inheritdoc />
        public IConsoleAppBuilder ConfigureConsoleConfiguration(Action<IConfigurationBuilder> configureDelegate)
        {
            this.configureConsoleConfigActions.Add(configureDelegate ?? throw new ArgumentNullException(nameof(configureDelegate)));
            return this;
        }

        /// <inheritdoc />
        public IConsoleAppBuilder ConfigureAppConfiguration(Action<ConsoleAppBuilderContext, IConfigurationBuilder> configureDelegate)
        {
            this.configureAppConfigActions.Add(configureDelegate ?? throw new ArgumentNullException(nameof(configureDelegate)));
            return this;
        }

        /// <inheritdoc />
        public IConsoleAppBuilder ConfigureServices(Action<ConsoleAppBuilderContext, IServiceCollection> configureDelegate)
        {
            this.configureServicesActions.Add(configureDelegate ?? throw new ArgumentNullException(nameof(configureDelegate)));
            return this;
        }

        /// <inheritdoc />
        public IConsoleAppBuilder UseCompositionRoot<TStartup>()
            where TStartup : ICompositionRoot
        {
            this.compositionRootType = typeof(TStartup);
            return this;
        }

        /// <inheritdoc />
        public IConsoleApp Build()
        {
            if (this.containerBuilt)
            {
                throw new InvalidOperationException("Build can only be called once.");
            }

            // Automatically add user secrets for entry point assembly
            this.ConfigureAppConfiguration((context, builder) =>
            {
                if (context.ConsoleAppEnvironment.IsDevelopment())
                {
                    builder.AddUserSecrets(this.compositionRootType?.GetTypeInfo().Assembly);
                }
            });

            this.containerBuilt = true;

            this.BuildConsoleConfiguration();
            this.CreateConsoleAppEnvironment();
            this.CreateConsoleBuilderContext();
            this.BuildAppConfiguration();
            this.CreateServiceProvider(typeof(IConsoleAppCommand<>));

            return new ConsoleApp(this.args, this.compositionRootType, this.appServices, this.appConfiguration, this.consoleAppEnvironment);
        }

        /// <inheritdoc />
        public IAsyncConsoleApp AsyncBuild()
        {
            if (this.containerBuilt)
            {
                throw new InvalidOperationException("Build can only be called once.");
            }

            // Automatically add user secrets for entry point assembly
            this.ConfigureAppConfiguration((context, builder) =>
            {
                if (context.ConsoleAppEnvironment.IsDevelopment())
                {
                    builder.AddUserSecrets(this.compositionRootType?.GetTypeInfo().Assembly);
                }
            });

            this.containerBuilt = true;

            this.BuildConsoleConfiguration();
            this.CreateConsoleAppEnvironment();
            this.CreateConsoleBuilderContext();
            this.BuildAppConfiguration();
            this.CreateServiceProvider(typeof(IAsyncConsoleAppCommand<>));

            return new AsyncConsoleApp(this.args, this.compositionRootType, this.appServices, this.appConfiguration, this.consoleAppEnvironment);
        }

        /// <summary>
        /// Builds the console configuration.
        /// </summary>
        private void BuildConsoleConfiguration()
        {
            var configBuilder = new ConfigurationBuilder()
                .AddInMemoryCollection(); // Make sure there's some default storage since there are no default providers

            foreach (var buildAction in this.configureConsoleConfigActions)
            {
                buildAction(configBuilder);
            }

            this.consoleConfiguration = configBuilder.Build();
        }

        /// <summary>
        /// Creates the console application environment.
        /// </summary>
        private void CreateConsoleAppEnvironment()
        {
            this.consoleAppEnvironment = new ConsoleAppEnvironment
            {
                ApplicationName = this.consoleConfiguration[HostDefaults.ApplicationKey],
                EnvironmentName = this.consoleConfiguration[HostDefaults.EnvironmentKey] ?? Environments.Production,
                ContentRootPath = ResolveContentRootPath(this.consoleConfiguration[HostDefaults.ContentRootKey], AppContext.BaseDirectory),
            };

            if (string.IsNullOrEmpty(this.consoleAppEnvironment.ApplicationName))
            {
                // Note GetEntryAssembly returns null for the net4x console test runner.
                this.consoleAppEnvironment.ApplicationName = Assembly.GetEntryAssembly()?.GetName().Name;
            }

            this.consoleAppEnvironment.ContentRootFileProvider = new PhysicalFileProvider(this.consoleAppEnvironment.ContentRootPath);

            static string ResolveContentRootPath(string contentRootPath, string basePath)
            {
                if (string.IsNullOrEmpty(contentRootPath))
                {
                    return basePath;
                }

                if (Path.IsPathRooted(contentRootPath))
                {
                    return contentRootPath;
                }

                return Path.Combine(Path.GetFullPath(basePath), contentRootPath);
            }
        }

        /// <summary>
        /// Creates the console builder context.
        /// </summary>
        private void CreateConsoleBuilderContext()
        {
            this.consoleAppBuilderContext = new ConsoleAppBuilderContext(this.Properties)
            {
                ConsoleAppEnvironment = this.consoleAppEnvironment,
                Configuration = this.consoleConfiguration,
            };
        }

        /// <summary>
        /// Builds the application configuration.
        /// </summary>
        private void BuildAppConfiguration()
        {
            var configBuilder = new ConfigurationBuilder()
                .SetBasePath(this.consoleAppEnvironment.ContentRootPath)
                .AddConfiguration(this.consoleConfiguration, shouldDisposeConfiguration: true);

            foreach (var buildAction in this.configureAppConfigActions)
            {
                buildAction(this.consoleAppBuilderContext, configBuilder);
            }

            this.appConfiguration = configBuilder.Build();
            this.consoleAppBuilderContext.Configuration = this.appConfiguration;
        }

        /// <summary>
        /// Creates the service provider.
        /// </summary>
        /// <param name="type">The type of the commands used in this application.</param>
        /// <exception cref="ConsoleAppException">No composition root defined.</exception>
        /// <exception cref="InvalidOperationException">The IServiceProviderFactory returned a null IServiceProvider.</exception>
        private void CreateServiceProvider(Type type)
        {
            var services = new ServiceCollection();
            services.AddSingleton(this.consoleAppEnvironment);
            services.AddSingleton(this.consoleAppBuilderContext);

            // register configuration as factory to make it dispose with the service provider
            services.AddSingleton(_ => this.appConfiguration);

            services.AddOptions();
            services.AddLogging();

            foreach (var configureServicesAction in this.configureServicesActions)
            {
                configureServicesAction(this.consoleAppBuilderContext, services);
            }

            if (this.compositionRootType == null)
            {
                throw new ConsoleAppException("No composition root type defined.");
            }

            // Register all commands in service collection.
            services.Scan(selector => selector.FromAssembliesOf(this.compositionRootType)
                    .AddClasses(classes => classes.AssignableTo(type)).AsImplementedInterfaces());

            // Only allow one constructor for ICompositionRoot implementations. DI services should have only one constructor anyways.
            var targetConstructor = this.compositionRootType.GetConstructors().First();

            // Collect the supported constructor injection parameters.
            var parameters = new List<object>();
            foreach (var paramType in targetConstructor.GetParameters().Select(x => x.ParameterType))
            {
                if (typeof(IConfiguration).IsAssignableFrom(paramType))
                {
                    parameters.Add(this.consoleAppBuilderContext.Configuration);
                }
                else if (typeof(IHostEnvironment).IsAssignableFrom(paramType))
                {
                    parameters.Add(this.consoleAppBuilderContext.ConsoleAppEnvironment);
                }
                else
                {
                    throw new ConsoleAppException($"Composition root does not support injection of type '{paramType}'. Only {nameof(IConfiguration)} and {nameof(IHostEnvironment)} are supported");
                }
            }

            // Use Activator to instantiate composition root.
            var compositionRoot = (ICompositionRoot)Activator.CreateInstance(this.compositionRootType, parameters.ToArray());

            // Add Microsoft Dependency Injection services from composition root
            compositionRoot.ConfigureServices(services);

            // Build service provider
            this.appServices = services.BuildServiceProvider(validateScopes: true);
            if (this.appServices == null)
            {
                throw new InvalidOperationException($"The container returned a null {nameof(IServiceProvider)}.");
            }

            // resolve configuration explicitly once to mark it as resolved within the
            // service provider, ensuring it will be properly disposed with the provider
            _ = this.appServices.GetService<IConfiguration>();
        }
    }
}
