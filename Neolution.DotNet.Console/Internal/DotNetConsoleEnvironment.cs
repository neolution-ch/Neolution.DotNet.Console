﻿namespace Neolution.DotNet.Console.Internal
{
    using Microsoft.Extensions.FileProviders;
    using Microsoft.Extensions.Hosting;

    /// <summary>
    /// Provides information about the environment a console application is running in.
    /// </summary>
    internal class DotNetConsoleEnvironment : IHostEnvironment
    {
        /// <summary>
        /// Gets or sets the name of the environment. The host automatically sets this property to the value of the "environment" key as specified in configuration.
        /// </summary>
        public string EnvironmentName { get; set; } = null!;

        /// <summary>
        /// Gets or sets the name of the application. This property is automatically set by the host to the assembly containing
        /// the application entry point.
        /// </summary>
        public string ApplicationName { get; set; } = null!;

        /// <summary>
        /// Gets or sets the absolute path to the directory that contains the application content files.
        /// </summary>
        public string ContentRootPath { get; set; } = null!;

        /// <summary>
        /// Gets or sets an <see cref="IFileProvider"/> pointing at <see cref="ContentRootPath"/>.
        /// </summary>
        public IFileProvider ContentRootFileProvider { get; set; } = null!;
    }
}
