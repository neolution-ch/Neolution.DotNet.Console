namespace Neolution.DotNet.Console.IntegrationTests
{
    using System.Diagnostics;
    using System.Threading.Tasks;
    using Shouldly;
    using Xunit;

    /// <summary>
    /// Integration tests for the CheckDepsConsole scenario.
    /// </summary>
    public class CheckDepsConsoleTests : IClassFixture<SolutionDirectoryFixture>
    {
        /// <summary>
        /// The fixture that provides solution and project paths.
        /// </summary>
        private readonly SolutionDirectoryFixture fixture;

        /// <summary>
        /// Initializes a new instance of the <see cref="CheckDepsConsoleTests"/> class.
        /// </summary>
        /// <param name="fixture">The solution directory fixture.</param>
        public CheckDepsConsoleTests(SolutionDirectoryFixture fixture)
        {
            this.fixture = fixture;
        }

        /// <summary>
        /// Given the Demo app, when run with 'check-deps', then it prints the expected DI validation message.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        [Fact]
        public async Task GivenDemoApp_WhenRunWithCheckDeps_ThenPrintsDependencyInjectionValidationSucceeded()
        {
            // Arrange
            var restorePsi = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = $"restore \"{this.fixture.DemoProjectPath}\"",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            };

            using (var restoreProcess = Process.Start(restorePsi))
            {
                if (restoreProcess is null)
                {
                    throw new System.InvalidOperationException("Failed to start dotnet restore for Demo app.");
                }

                await restoreProcess.StandardOutput.ReadToEndAsync();
                await restoreProcess.StandardError.ReadToEndAsync();
                await restoreProcess.WaitForExitAsync();
                restoreProcess.ExitCode.ShouldBe(0, "dotnet restore failed for Demo app");
            }

            var psi = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = $"run --project \"{this.fixture.DemoProjectPath}\" check-deps",
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
