# Introduction 

Neolution.DotNet.Console is a versatile package designed as a launchpad for .NET console applications. It comes equipped with a built-in dependency injection setup, mirroring the functionality found in ASP.NET Core applications, thus providing a seamless and intuitive user experience.

# Getting Started

To help you kickstart your console application, we've provided a a [sample application](/Neolution.DotNet.Console.SampleAsync/Program.cs) that should demonstrate the basic usage of this package.

# Guides

## Migrate from V2 to V3

.NET 6 changed the hosting model for ASP.NET Core applications, we adjusted to that to fulfill the primary goal of this package: to provide a seamless and intuitive user experience. This introduces breaking changes.

### Removed `ICompositionRoot` interface and `UseCompositionRoot` extension method
The builder returned from calling `DotNetConsole.CreateDefaultBuilder(args)` now has a `Services` property that can be used to register services. Like in ASP.NET, this can now be done directly in `Program.cs`.

    public static async Task Main(string[] args)
    {
        var builder = DotNetConsole.CreateDefaultBuilder(args);

        // Register your services here...
        builder.Services.AddHttpClient();
        builder.Services.AddScoped<IScopedService, ServiceA>();
        builder.Services.AddSingleton<ISingletonService, ServiceB>();

        await builder.Build().RunAsync();
    }

### Async by default
All commands are now async by default. The `IAsyncConsoleAppCommand` has been removed and the `IConsoleAppCommand` interface now requires the implementation of the `RunAsync` method.

### Service registration validation
The service registrations are now validated when `app.Build()` is called. This means that the application will not start if not all services that are registered (even if not used during runtime) can be created by the DI container or when there are scope/lifetime issues with the services.

## Migrate from V1 to V2

V2 **introduces breaking changes** from V1, primarily because it upgrades NLog to Version 5. For a detailed review of these changes, please refer to the [official NLog release notes](https://nlog-project.org/2021/08/25/nlog-5-0-preview1-ready.html).

### Removed `Logging` section in appsettings.json

NLog decided to deprecate Microsoft's `Logging` section in the appsettings.json starting with V5. In accordance of that decision we've also transitioned to only using NLog rules for filtering logging output. Although these rules might be a bit more complex, they offer greater flexibility and control. 

To get acquainted with these rules and learn how to migrate your current configuration, check out the [NLog documentation on logging rules](https://github.com/NLog/NLog/wiki/Configuration-file#rules). There's also a guide on the [new finalMinLevel attribute](https://github.com/NLog/NLog/wiki/Logging-Rules-FinalMinLevel) that you might find helpful.

### Removed default logging targets

- **AWS Logger:** The AWS Logger, previously used for logging to CloudWatch, has been removed due to decreased usage. If you still need it, don't worry, you can easily [download it separately](https://www.nuget.org/packages/AWS.Logger.NLog). The configuration remains the same.

- **DatabaseTarget:** The DatabaseTarget is no longer part of the NLog NuGet package. If you use this target, you'll need to [download it separately](https://www.nuget.org/packages/NLog.Database) to ensure its continued functionality.

There are additional targets that have been transitioned into separate packages. Please review [this list](https://nlog-project.org/2021/08/25/nlog-5-0-preview1-ready.html#nlog-targets-extracted-into-their-own-nuget-packages) to determine if any targets you use are affected.
