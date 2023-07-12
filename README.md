# Introduction 

Neolution.DotNet.Console is a versatile package designed as a launchpad for .NET console applications. It comes equipped with a built-in dependency injection setup, mirroring the functionality found in ASP.NET Core applications, thus providing a seamless and intuitive user experience.

# Getting Started

To help you kickstart your console application, we've provided sample applications for reference:

1. [Sample Application](/Neolution.DotNet.Console.Sample/Program.cs): This demonstrates the basic usage of this package.

2. [Async Sample Application](/Neolution.DotNet.Console.SampleAsync/Program.cs): Here's an async variant to illustrate how you can incorporate async programming into your application.

# Guides

## Migrate from V1 to V2

V2 introduces breaking changes, primarily because it upgrades NLog to Version 5. For a detailed review of these changes, please refer to the [official NLog release notes](https://nlog-project.org/2021/08/25/nlog-5-0-preview1-ready.html).

### Microsoft "Logging" section in appsettings.json

The "Logging" section in the appsettings.json is now deprecated. We've transitioned to only using NLog rules for filtering logging output. Although these rules might be a bit more complex, they offer greater flexibility and control. 

To get acquainted with these rules and learn how to migrate your current configuration, check out the [NLog documentation on logging rules](https://github.com/NLog/NLog/wiki/Configuration-file#rules). There's also a guide on the [new finalMinLevel attribute](https://github.com/NLog/NLog/wiki/Logging-Rules-FinalMinLevel) that you might find helpful.

### Removed assemblies

- **AWS Logger:** The AWS Logger, previously used for logging to CloudWatch, has been removed due to decreased usage. If you still need it, don't worry, you can easily [download it separately](https://www.nuget.org/packages/AWS.Logger.NLog). The configuration remains the same.

- **DatabaseTarget:** The DatabaseTarget is no longer part of the NLog NuGet package. If you use this target, you'll need to [download it separately](https://www.nuget.org/packages/NLog.Database) to ensure its continued functionality.

There are additional targets that have been transitioned into separate packages. Please review [this list](https://nlog-project.org/2021/08/25/nlog-5-0-preview1-ready.html#nlog-targets-extracted-into-their-own-nuget-packages) to determine if any targets you use are affected.
