namespace Neolution.DotNet.Console.UnitTests.Fakes
{
    using CommandLine;

    /// <summary>
    /// The options stub for the <see cref="DefaultCommand"/>
    /// </summary>
    [Verb("default", isDefault: true)]
    public class DefaultOptions
    {
        /// <summary>
        /// Gets or sets the option.
        /// </summary>
        [Option("option")]
        public string? Option { get; set; }

        /// <summary>
        /// Gets or sets the tenant identifier.
        /// </summary>
        [Option("tenantId")]
        public string? TenantId { get; set; }
    }
}
