# Irihi.Mirana Project Structure

This document describes the structure of the Irihi.Mirana project, following the pattern of [Ursa.Avalonia](https://github.com/irihitech/Ursa.Avalonia).

## Solution Structure

The solution is organized into three main folders:

### src/ - Source Code Projects

1. **Irihi.Mirana** - Core library project
   - Target Framework: net8.0
   - Contains the main library code
   - Package ID: Irihi.Mirana

2. **Irihi.Mirana.Themes.TDesign** - Theme library project
   - Target Framework: net8.0
   - Contains TDesign theme implementation
   - Package ID: Irihi.Mirana.Themes.TDesign
   - References: Irihi.Mirana

### test/ - Test Projects

3. **Irihi.Mirana.UnitTest** - Unit test project
   - Target Framework: net8.0
   - Test Framework: xUnit
   - References: Irihi.Mirana, Irihi.Mirana.Themes.TDesign

4. **Irihi.Mirana.HeadlessTest** - Headless UI test project
   - Target Framework: net8.0
   - Test Framework: xUnit
   - Uses: Avalonia.Headless for UI testing
   - References: Irihi.Mirana, Irihi.Mirana.Themes.TDesign

### demo/ - Demo Applications

5. **Irihi.Mirana.Demo** - Core demo project (shared code)
   - Target Framework: net10.0
   - Contains the main UI and application logic
   - Used by all platform-specific entry points

6. **Irihi.Mirana.Demo.Android** - Android entry point
   - Target Framework: net10.0-android
   - Application ID: tech.irihi.Mirana.Demo
   - References: Irihi.Mirana.Demo

7. **Irihi.Mirana.Demo.Desktop** - Desktop entry point
   - Target Framework: net10.0
   - Supports Windows, macOS, and Linux
   - References: Irihi.Mirana.Demo

8. **Irihi.Mirana.Demo.Browser** - Browser/WebAssembly entry point
   - Target Framework: net10.0-browser
   - References: Irihi.Mirana.Demo

9. **Irihi.Mirana.Demo.iOS** - iOS entry point
   - Target Framework: net10.0-ios
   - References: Irihi.Mirana.Demo

## Package Management

The project uses Central Package Management (CPM) with `Directory.Packages.props` at the root level to manage package versions centrally.

Key packages:
- Avalonia UI Framework: 11.3.11
- xUnit Testing Framework: 2.5.3
- Community Toolkit MVVM: 8.4.0

## Building the Projects

### Build all projects (except Android which requires workload):
```bash
dotnet build Irihi.Mirana.slnx
```

### Build specific projects:
```bash
dotnet build src/Irihi.Mirana/Irihi.Mirana.csproj
dotnet build src/Irihi.Mirana.Themes.TDesign/Irihi.Mirana.Themes.TDesign.csproj
```

### Run tests:
```bash
dotnet test test/Irihi.Mirana.UnitTest/Irihi.Mirana.UnitTest.csproj
dotnet test test/Irihi.Mirana.HeadlessTest/Irihi.Mirana.HeadlessTest.csproj
```

### Run demo:
```bash
dotnet run --project demo/Irihi.Mirana.Demo.Desktop/Irihi.Mirana.Demo.Desktop.csproj
```

## Notes

- Android and iOS projects require the respective workloads to be installed
- The Browser project can be run with `dotnet run` or published for static hosting
- All projects follow the naming convention: Irihi.Mirana.*
