# Irihi.Mafia Project Structure

This document describes the structure of the Irihi.Mafia project, following the pattern of [Ursa.Avalonia](https://github.com/irihitech/Ursa.Avalonia).

## Solution Structure

The solution is organized into three main folders:

### src/ - Source Code Projects

1. **Irihi.Mafia** - Core library project
   - Target Framework: net8.0
   - Contains the main library code
   - Package ID: Irihi.Mafia

2. **Irihi.Mafia.Themes.TDesign** - Theme library project
   - Target Framework: net8.0
   - Contains TDesign theme implementation
   - Package ID: Irihi.Mafia.Themes.TDesign
   - References: Irihi.Mafia

   1. Controls - ControlTheme implementations
   2. Converters - Value converters
   3. Styles - Global Styles for themes
   4. Themes - ResourceDictionaries for themes
      - /Dark - Dark theme resources
      - /Light - Light theme resources
      - /Shared - Shared theme resources
   5. Tokens - Design tokens

### test/ - Test Projects

3. **Irihi.Mafia.UnitTest** - Unit test project
   - Target Framework: net8.0
   - Test Framework: xUnit
   - References: Irihi.Mafia, Irihi.Mafia.Themes.TDesign

4. **Irihi.Mafia.HeadlessTest** - Headless UI test project
   - Target Framework: net8.0
   - Test Framework: xUnit
   - Uses: Avalonia.Headless for UI testing
   - References: Irihi.Mafia, Irihi.Mafia.Themes.TDesign

### demo/ - Demo Applications

5. **Irihi.Mafia.Demo** - Core demo project (shared code)
   - Target Framework: net10.0
   - Contains the main UI and application logic
   - Used by all platform-specific entry points

6. **Irihi.Mafia.Demo.Android** - Android entry point
   - Target Framework: net10.0-android
   - Application ID: tech.Irihi.Mafia.Demo
   - References: Irihi.Mafia.Demo

7. **Irihi.Mafia.Demo.Desktop** - Desktop entry point
   - Target Framework: net10.0
   - Supports Windows, macOS, and Linux
   - References: Irihi.Mafia.Demo

8. **Irihi.Mafia.Demo.Browser** - Browser/WebAssembly entry point
   - Target Framework: net10.0-browser
   - References: Irihi.Mafia.Demo

9. **Irihi.Mafia.Demo.iOS** - iOS entry point
   - Target Framework: net10.0-ios
   - References: Irihi.Mafia.Demo

## Package Management

The project uses Central Package Management (CPM) with `Directory.Packages.props` at the root level to manage package versions centrally.

Key packages:
- Avalonia UI Framework: 11.3.11
- xUnit Testing Framework: 2.5.3
- Community Toolkit MVVM: 8.4.0

## Building the Projects

### Build all projects (except Android which requires workload):
```bash
dotnet build Irihi.Mafia.slnx
```

### Build specific projects:
```bash
dotnet build src/Irihi.Mafia/Irihi.Mafia.csproj
dotnet build src/Irihi.Mafia.Themes.TDesign/Irihi.Mafia.Themes.TDesign.csproj
```

### Run tests:
```bash
dotnet test test/Irihi.Mafia.UnitTest/Irihi.Mafia.UnitTest.csproj
dotnet test test/Irihi.Mafia.HeadlessTest/Irihi.Mafia.HeadlessTest.csproj
```

### Run demo:
```bash
dotnet run --project demo/Irihi.Mafia.Demo.Desktop/Irihi.Mafia.Demo.Desktop.csproj
```

## Notes

- Android and iOS projects require the respective workloads to be installed
- The Browser project can be run with `dotnet run` or published for static hosting
- All projects follow the naming convention: Irihi.Mafia.*
