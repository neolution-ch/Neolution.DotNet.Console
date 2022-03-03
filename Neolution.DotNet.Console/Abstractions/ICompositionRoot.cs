﻿namespace Neolution.DotNet.Console.Abstractions
{
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// The composition root interface.
    /// </summary>
    public interface ICompositionRoot
    {
        /// <summary>
        /// Configures the Microsoft Dependency Injection services.
        /// </summary>
        /// <param name="services">The service collection.</param>
        void ConfigureServices(IServiceCollection services);
    }
}
