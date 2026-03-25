namespace Neolution.DotNet.Console.IntegrationTests
{
    using System.IO;

    /// <summary>
    /// Provides the solution directory and Demo project path for integration tests.
    /// </summary>
    public class SolutionDirectoryFixture
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SolutionDirectoryFixture"/> class.
        /// </summary>
        public SolutionDirectoryFixture()
        {
            var dir = Directory.GetCurrentDirectory();
            while (dir != null && !File.Exists(Path.Combine(dir, "Neolution.DotNet.Console.sln")))
            {
                dir = Path.GetDirectoryName(dir);
            }

            // Set the solution directory to the one containing the solution file
            this.SolutionDirectory = dir ?? throw new DirectoryNotFoundException("Could not find solution directory.");

            // Set the Demo project path relative to the solution directory
            this.DemoProjectPath = Path.Combine(this.SolutionDirectory, "Neolution.DotNet.Console.Demo", "Neolution.DotNet.Console.Demo.csproj");
            if (!File.Exists(this.DemoProjectPath))
            {
                throw new FileNotFoundException($"Demo project file not found: {this.DemoProjectPath}");
            }
        }

        /// <summary>
        /// Gets the solution directory path.
        /// </summary>
        public string SolutionDirectory { get; }

        /// <summary>
        /// Gets the Demo project file path.
        /// </summary>
        public string DemoProjectPath { get; }
    }
}
