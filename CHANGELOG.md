# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

## [5.0.0] - 2025-01-27

### Added

- Added cancellation token support for commands
- Added a built-in `check-deps` command that validates the application's Dependency Injection setup by ensuring all required services are fully and correctly registered in the composition root.

### Changed

- Updated Scrutor to v6.0.1
- Updated NLog.Extensions.Logging to v5.3.15
- Updated Microsoft.Extensions.Configuration.Json to v8.0.1
- Updated Microsoft.Extensions.Configuration.UserSecrets to v8.0.1
- Updated Microsoft.Extensions.DependencyInjection to v8.0.1
- Updated Microsoft.Extensions.Hosting to v8.0.1
- Updated Microsoft.Extensions.Logging to v8.0.1
- Updated Microsoft.Extensions.Logging.Configuration to v8.0.1

## [3.0.4] - 2024-11-28

### Fixed

- Resolved another case where appsettings.json was being loaded from the current working directory instead of the executable's directory.

## [3.0.3] - 2024-10-09

### Fixed

- Resolved an issue where appsettings.json was being loaded from the current working directory instead of the executable's directory.

## [3.0.2] - 2024-10-04

### Fixed

- If a default verb is defined, calls of the application that only use parameters should be allowed

### Changed

- Updated NLog.Extensions.Logging to v5.3.14

## [3.0.1] - 2024-10-02

### Fixed

- Bump version number to 3.0.1 to release the package to nuget.org

## [3.0.0] - 2024-09-30

### Changed

- Updated everything to .NET 8
- Enabled nullable reference types
- Changed app builder initialization code to resemble the current default ASP.NET Core hosting model
- If a default verb is defined, the verb passed by the user must strictly match one of the available verbs
- Updated NLog to v5.3.4
- Updated NLog.Extensions.Logging to v5.3.13

### Added

- Commands are now async by default, they must use the new `IDotNetConsoleCommand` interface instead of `IConsoleAppCommand` and `IAsyncConsoleAppCommand`.

### Removed

- Lots of classes that were originally copied from the ASP.NET Core hosting model source code and are now no longer needed
- Removed `IAsyncConsoleAppCommand` and `IConsoleAppCommand` in favor of the new `IDotNetConsoleCommand` interface

## [2.0.2] - 2024-06-06

### Fixed

- Added README.md file to the NuGet package

## [2.0.1] - 2024-06-06

### Changed

- Updated NLog to v5.3
- Updated Neolution.CodeAnalysis to v3.0.5

## [2.0.0] - 2024-02-07

### Fixed

- Create Console App Scope asynchronously
- Cleaned repository from now unused Azure DevOps files

### Added

- Async Console Application variant
- MIT licence expression for NuGet package

### Changed

- Updated NLog to V5
- Updated CommandLineParser to v2.9.1

### Removed

- Removed AWS NLog Logger assembly that was used for logging to Amazon CloudWatch

[unreleased]: https://github.com/neolution-ch/Neolution.DotNet.Console/compare/v5.0.0...HEAD
[3.0.1]: https://github.com/neolution-ch/Neolution.DotNet.Console/compare/v3.0.0...v3.0.1
[3.0.0]: https://github.com/neolution-ch/Neolution.DotNet.Console/compare/v2.0.2...v3.0.0
[2.0.2]: https://github.com/neolution-ch/Neolution.DotNet.Console/compare/v2.0.1...v2.0.2
[2.0.1]: https://github.com/neolution-ch/Neolution.DotNet.Console/compare/v2.0.0...v2.0.1
[2.0.0]: https://github.com/neolution-ch/Neolution.DotNet.Console/compare/v1.1.0-beta1...v2.0.0
[3.0.4]: https://github.com/neolution-ch/Neolution.DotNet.Console/compare/v3.0.3...v3.0.4
[3.0.3]: https://github.com/neolution-ch/Neolution.DotNet.Console/compare/v3.0.2...v3.0.3
[3.0.2]: https://github.com/neolution-ch/Neolution.DotNet.Console/compare/v3.0.2-rc.0...v3.0.2
[5.0.0]: https://github.com/neolution-ch/Neolution.DotNet.Console/compare/v4.0.0...v5.0.0
[4.0.0]: https://github.com/neolution-ch/Neolution.DotNet.Console/compare/v4.0.0-rc.0...v4.0.0
