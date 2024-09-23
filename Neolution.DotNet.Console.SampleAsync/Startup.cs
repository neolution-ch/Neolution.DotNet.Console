namespace Neolution.DotNet.Console.SampleAsync
{
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;

    /// <summary>
    /// The startup class, composition root for the application.
    /// </summary>
    internal class Startup
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Startup" /> class.
        /// </summary>
        /// <param name="environment">The environment.</param>
        /// <param name="configuration">The configuration.</param>
        public Startup(IHostEnvironment environment, IConfiguration configuration)
        {
            this.Environment = environment;
            this.Configuration = configuration;
        }

        /// <summary>
        /// Gets the environment.
        /// </summary>
        public IHostEnvironment Environment { get; }

        /// <summary>
        /// Gets the configuration.
        /// </summary>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// Configures the services.
        /// </summary>
        /// <remarks>This method gets called by the runtime. Use this method to add services to the container.</remarks>
        /// <param name="services">The services.</param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHttpClient();

            // EXAMPLE 1: Configure services based on the environment
            if (this.Environment.IsDevelopment())
            {
                // Register services only used in development
            }

            // EXAMPLE 2: Configure services based on the configurations
            if (this.Configuration.GetValue<bool>("NLog:throwConfigExceptions"))
            {
                // Do something based on a configuration value
            }
        }
    }
}
