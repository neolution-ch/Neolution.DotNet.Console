namespace Neolution.DotNet.Console.IntegrationTests
{
    using System.Diagnostics;
    using System.IO;
    using System.Threading.Tasks;
    using Shouldly;
    using Xunit;

    /// <summary>
    /// Integration tests for the CheckDepsConsole scenario.
    /// </summary>
    public class CheckDepsConsoleTests
    {
        /// <summary>
        /// Given the Demo app, when run with 'check-deps', then it prints the expected DI validation message.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        [Fact]
        public async Task GivenDemoApp_WhenRunWithCheckDeps_ThenPrintsDependencyInjectionValidationSucceeded()
        {
            // Arrange
            // Find the solution directory by traversing up from the current directory
            var dir = Directory.GetCurrentDirectory();
            while (dir != null && !File.Exists(Path.Combine(dir, "Neolution.DotNet.Console.sln")))
            {
                dir = Path.GetDirectoryName(dir);
            }

            if (dir is null)
            {
                throw new DirectoryNotFoundException("Could not find solution directory.");
            }

            var solutionDir = dir;
            var demoProjPath = Path.Combine(solutionDir, "Neolution.DotNet.Console.Demo", "Neolution.DotNet.Console.Demo.csproj");

            File.Exists(demoProjPath).ShouldBeTrue($"Demo project file not found: {demoProjPath}");
            var psi = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = $"run --project \"{demoProjPath}\" check-deps",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            };

            // Act
            using var process = Process.Start(psi);
            if (process is null)
            {
                throw new System.InvalidOperationException("Failed to start process for Demo app.");
            }

            var output = await process.StandardOutput.ReadToEndAsync();
            var error = await process.StandardError.ReadToEndAsync();
            await process.WaitForExitAsync();

            // Assert
            output.ShouldContain("Dependency injection validation succeeded. All registered services can be constructed and no DI issues were found.");
            process.ExitCode.ShouldBe(0, $"Process exited with code {process.ExitCode}. Error: {error}");
        }
    }
}
