namespace Neolution.DotNet.Console.Internal
{
    using System;
    using System.Linq;
    using System.Reflection;
    using CommandLine;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Hosting;
    using Neolution.DotNet.Console.Abstractions;

    /// <summary>
    /// The console app that gets built with the console app builder.
    /// </summary>
    /// <seealso cref="IConsoleApp" />
    internal class ConsoleAppBase : IConsoleAppBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConsoleAppBase"/> class.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <param name="compositionRootType">Type of the composition root.</param>
        /// <param name="services">The services.</param>
        /// <param name="configuration">The configuration.</param>
        /// <param name="environment">The environment.</param>
        /// <exception cref="System.ArgumentNullException">services</exception>
        public ConsoleAppBase(string[] args, Type compositionRootType, IServiceProvider services, IConfiguration configuration, IHostEnvironment environment)
        {
            this.Args = args;
            this.Verbs = compositionRootType.Assembly.GetTypes()
                .Where(t => CustomAttributeExtensions.GetCustomAttribute<VerbAttribute>((MemberInfo)t) != null)
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
        /// Gets or sets the arguments
        /// </summary>
        protected string[] Args { get; set; }

        /// <summary>
        /// Gets the verbs
        /// </summary>
        protected Type[] Verbs { get; }
    }
}
