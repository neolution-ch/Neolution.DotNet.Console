# Neolution.DotNet.Console

[![NuGet](https://img.shields.io/nuget/v/Neolution.DotNet.Console.svg)](https://www.nuget.org/packages/Neolution.DotNet.Console)
[![Build Status](https://github.com/neolution-ch/Neolution.DotNet.Console/actions/workflows/cd-production.yml/badge.svg)](https://github.com/neolution-ch/Neolution.DotNet.Console/actions)
[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](LICENSE)

Neolution.DotNet.Console is a library for building .NET console applications. It uses the Microsoft.Extensions.Hosting model to provide a familiar setup for dependency injection, logging, and command-line parsing.

This project works for simple CLI tools as well as longer-running console services, and supports cancellation tokens, configuration from JSON, environment variables, or command-line arguments.

## Features

- Async commands with cancellation support
- Dependency injection setup matching ASP.NET Core patterns
- Command-line parsing with verb and option support
- Logging integration using NLog
- Configuration from appsettings.json, environment variables, user secrets, and command line
- Strict verb matching to prevent unintended command execution
- A built-in `check-deps` command to verify all services are registered correctly
- Support for multiple environments (Development, Production, etc.)
- Automatic scope creation for each command execution to support scoped services

## Installation

Install the package via NuGet:

### .NET CLI

```sh
dotnet add package Neolution.DotNet.Console
```

### Package Manager

```powershell
Install-Package Neolution.DotNet.Console
```

## Getting Started

To help you kickstart your console application, we've provided a [demo application](/Neolution.DotNet.Console.Demo/Program.cs) that demonstrates the basic usage of this package.

### Example Main Method with Startup Class (Recommended)

Below is the recommended pattern for your `Program.cs` entry point, using a `Startup` class for service registration and configuration:

```csharp
// Program.cs
namespace Neolution.DotNet.Console.Demo
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            try
            {
                var builder = DotNetConsole.CreateDefaultBuilder(args);
                DotNetConsoleLogger.Initialize(builder.Configuration);
                
                var startup = new Startup(builder.Environment, builder.Configuration);
                startup.ConfigureServices(builder.Services);

                var console = builder.Build();
                await console.RunAsync();
            }
            catch (Exception ex)
            {
                DotNetConsoleLogger.Log.Error(ex, "Stopped program because of an unexpected exception");
                throw;
            }
            finally
            {
                // Ensure to flush and stop internal timers/threads before application-exit (Avoid segmentation fault on Linux)
                DotNetConsoleLogger.Shutdown();
            }
        }
    }
}
```

```csharp
// Startup.cs
namespace Neolution.DotNet.Console.Demo
{
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;

    /// <summary>
    /// The startup class, composition root for the application.
    /// </summary>
    internal class Startup
    {
        public Startup(IHostEnvironment environment, IConfiguration configuration)
        {
            this.Environment = environment;
            this.Configuration = configuration;
        }

        public IHostEnvironment Environment { get; }
        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            // Register your services here. This is the main place for service registrations.
            // Example: services.AddHttpClient();

            // You have full access to IHostEnvironment and IConfiguration here.
            // Use 'this.Environment' to register services based on the environment (e.g., Development, Production).
            // Use 'this.Configuration' to register/configure services based on configuration values.
        }
    }
}
```

This pattern keeps your `Program.cs` clean and delegates service registration and configuration to a dedicated `Startup` class, similar to ASP.NET Core applications.

## Usage

### Defining a Command

To add a command, implement the `IDotNetConsoleCommand<TOptions>` interface. For example:

```csharp
using CommandLine;
using Neolution.DotNet.Console.Abstractions;

[Verb("echo", HelpText = "Write a string into the console.")]
public class EchoOptions
{
    [Value(0)]
    public string? Message { get; set; }
}

public class EchoCommand : IDotNetConsoleCommand<EchoOptions>
{
    public async Task RunAsync(EchoOptions options, CancellationToken cancellationToken)
    {
        Console.WriteLine($"Echo: {options.Message}");
        await Task.CompletedTask;
    }
}
```

The library will automatically discover and register all commands (classes implementing `IDotNetConsoleCommand<TOptions>`).

### Running the Application

Run your application and specify the command verb as an argument. The library uses [CommandLineParser](https://github.com/commandlineparser/commandline) verbs and options internally, so you can invoke your command like this:

```sh
dotnet run -- [verb] [options]
```

Replace `[verb]` with the name of your command (e.g., `echo`). Any additional options or parameters defined in your command will be parsed and passed automatically.

## Dependency Validation with check-deps

The framework includes a built-in `check-deps` command that validates your application's dependency injection setup without running any business logic. This is particularly useful for catching DI configuration issues early.

### Why use check-deps?

- **Early Detection**: Catch dependency injection issues during build/CI rather than at runtime
- **Fast Feedback**: Validates your entire DI container setup in seconds without executing business logic
- **Confidence**: Ensures all your services can be properly constructed by the DI container
- **CI Integration**: Perfect for automated validation in your deployment pipeline

The command validates:

- All registered services can be instantiated
- No circular dependencies exist
- Service lifetimes are properly configured
- All required dependencies are registered

If validation fails, the command will exit with a non-zero code and provide detailed error information about what's missing or misconfigured.

### Running check-deps locally

```sh
# Validate your DI setup
dotnet run -- check-deps

# Or if you have a published/built application
myapp.exe check-deps
```

### Using check-deps in CI/CD

The `check-deps` command is especially valuable in CI/CD pipelines to catch dependency injection issues before deployment:

```yaml
# Example GitHub Actions workflow
name: CI
on: [push, pull_request]

jobs:
  validate-dependencies:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
      - name: Setup .NET
        uses: actions/setup-dotnet@v4
        with:
          dotnet-version: '8.x'
      - name: Restore dependencies
        run: dotnet restore
      - name: Build
        run: dotnet build --no-restore
      - name: Validate DI setup
        run: dotnet run --project YourApp -- check-deps
      - name: Run tests
        run: dotnet test --no-build
```

## Migration Guides

### Migrate from V3 to V5

> ⚠️ *Note: V4 was intentionally skipped, there is no V4 release.*

To support cancellation tokens, the `IDotNetConsoleCommand` interface had to be changed: The `RunAsync` method now requires also a `CancellationToken` as a parameter. This change is breaking, so you will need to update your commands to reflect this change.

#### ConfigureAppConfiguration

The [`ConfigureAppConfiguration`](Neolution.DotNet.Console/DotNetConsoleBuilder.cs:81) method is now part of the `DotNetConsoleBuilder` and can be called after the builder has been instantiated.

```csharp
var builder = DotNetConsole.CreateDefaultBuilder(args);
builder.ConfigureAppConfiguration((context, config) =>
{
    config.AddGoogleSecrets(options => { /* configuration */ });
});
```

### Migrate from V2 to V3

In .NET 6 the hosting model for ASP.NET Core applications was changed, we adjusted to that to fulfill the primary goal of this package: to provide a seamless and intuitive user experience. This introduces breaking changes that are explained below.

#### Removed `ICompositionRoot` interface and `UseCompositionRoot` extension method

The builder returned from calling `DotNetConsole.CreateDefaultBuilder(args)` now has a `Services` property that can be used to register services. Like in ASP.NET, this can now be done directly in `Program.cs`.

Refer to the example above for the preferred structure of your Main method.

#### Async by default

All commands are now async by default. Use the new `IDotNetConsoleCommand` interface instead of `IConsoleAppCommand` and `IAsyncConsoleAppCommand`.

#### Service registration validation

The service registrations are now validated when `app.Build()` is called. This means that the application will not start if not all services that are registered (even if not used during runtime) can be created by the DI container or when there are scope/lifetime issues with the services.

#### Strict verb matching when default verb is defined

If one verb is defined as default, but the verb passed as argument is not matching any of the available verbs, the builder will no longer build a console application and instead throw an exception. This is done to avoid running the default verb command if the user accidentally uses a verb that is not available or simply made a typo.

E.g. if the app uses `start` as the default verb and the user intention is to start the command assigned to the verb `echo` but mistypes it as `eccho` or similar, the builder will fail with an exception. Previous versions would have run the `start` command, because it's defined as default verb, but it was not the intention.

### Migrate from V1 to V2

V2 **introduces breaking changes** from V1, primarily because it upgrades NLog to Version 5. For a detailed review of these changes, please refer to the [official NLog release notes](https://nlog-project.org/2021/08/25/nlog-5-0-preview1-ready.html).

#### Removed `Logging` section in appsettings.json

NLog decided to deprecate Microsoft's `Logging` section in the appsettings.json starting with V5. In accordance of that decision we've also transitioned to only using NLog rules for filtering logging output. Although these rules might be a bit more complex, they offer greater flexibility and control.

To get acquainted with these rules and learn how to migrate your current configuration, check out the [NLog documentation on logging rules](https://github.com/NLog/NLog/wiki/Configuration-file#rules). There's also a guide on the [new finalMinLevel attribute](https://github.com/NLog/NLog/wiki/Logging-Rules-FinalMinLevel) that you might find helpful.

#### Removed default logging targets

- **AWS Logger:** The AWS Logger, previously used for logging to CloudWatch, has been removed due to decreased usage. If you still need it, don't worry, you can easily [download it separately](https://www.nuget.org/packages/AWS.Logger.NLog). The configuration remains the same.
- **DatabaseTarget:** The DatabaseTarget is no longer part of the NLog NuGet package. If you use this target, you'll need to [download it separately](https://www.nuget.org/packages/NLog.Database) to ensure its continued functionality.

There are additional targets that have been transitioned into separate packages. Please review [this list](https://nlog-project.org/2021/08/25/nlog-5-0-preview1-ready.html#nlog-targets-extracted-into-their-own-nuget-packages) to determine if any targets you use are affected.
