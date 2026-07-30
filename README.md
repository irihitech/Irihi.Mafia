# Irihi.Mafia

**Modern Avalonia UI control library and TDesign-inspired theme.**

Irihi.Mafia provides a set of polished, reusable Avalonia controls alongside a complete visual theme based on [TDesign](https://tdesign.tencent.com/) design language, with full light/dark mode support.

## Features

- 🎨 **TDesign-inspired theme** — A comprehensive light & dark theme with design token system (colors, typography, spacing, radius)
- 🧩 **Custom controls** — Avatar, Cell/ CellGroup, Divider, IconButton, Picker, Popup and more
- 🌓 **Light/Dark mode** — Built-in theme variants with consistent color palettes
- 📦 **Modular packages** — Core logic library and theme library are separate NuGet packages
- 🖥️ **Cross-platform** — Works on Windows, macOS, Linux, Android, iOS and Browser via Avalonia

## Packages

| Package | Description | NuGet |
|---------|-------------|-------|
| `Irihi.Mafia` | Core control library — provides custom control logic & primitives | [![NuGet](https://img.shields.io/nuget/v/Irihi.Mafia)](https://www.nuget.org/packages/Irihi.Mafia) |
| `Irihi.Mafia.Themes.TDesign` | TDesign-inspired visual theme — control templates, design tokens, light/dark color schemes | [![NuGet](https://img.shields.io/nuget/v/Irihi.Mafia.Themes.TDesign)](https://www.nuget.org/packages/Irihi.Mafia.Themes.TDesign) |

## Getting Started

### 1. Install the packages

```bash
dotnet add package Irihi.Mafia
dotnet add package Irihi.Mafia.Themes.TDesign
```

> You only need `Irihi.Mafia` if you want to write your own theme; for most projects, install both packages.

### 2. Apply the theme

In your `App.axaml`, add the TDesign theme:

```xml
<Application xmlns="https://github.com/avaloniaui"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             xmlns:td="https://irihi.tech/td"
             x:Class="MyApp.App">
    <Application.Styles>
        <td:TDesignTheme />
    </Application.Styles>
</Application>
```

### 3. Use the controls

```xml
xmlns:m="https://irihi.tech/mafia"

<!-- IconButton -->
<m:IconButton Icon="emoji_smile"
              Shape="Circle"
              Variant="Outline" />

<!-- Cell -->
<m:Cell Description="User settings"
        Note="Manage your preferences"
        IsRequired="True" />

<!-- Avatar -->
<m:Avatar Width="40" Height="40"
          Source="{Binding ProfileImage}" />
```

## Controls Overview

| Control | Description |
|---------|-------------|
| `Avatar` | Button with an image source, for user avatars |
| `Cell` | List item with description, note, and inner content slots |
| `CellGroup` | Groups `Cell` items under a header |
| `Divider` | A horizontal or vertical separator line |
| `IconButton` | Button with icon, loading state, shape and variant options |
| `Picker` | Drop-down selection control with confirm mode and search |
| `Popup` | Flexible popup with modal mask, placement and animation support |

## Project Structure

```
src/
├── Irihi.Mafia/                      # Core control library
│   ├── Controls/                     # Custom control implementations
│   │   ├── Avatar/
│   │   ├── Cell/
│   │   ├── Divider.cs
│   │   ├── IconButton.cs
│   │   ├── Picker.cs
│   │   └── Primitives/               # Popup, OverlayPopupHost
│   └── Common/                       # Shared enums (Position, PopupPlacement)
│
└── Irihi.Mafia.Themes.TDesign/       # TDesign-themed visual library
    ├── TDesignTheme.axaml             # Theme entry point
    ├── Controls/                      # 24 ControlTheme definitions
    ├── Tokens/                        # Design tokens (colors, fonts, radius, sizes)
    ├── Themes/                        # Light & Dark variant resource dictionaries
    └── Styles/                        # Global style selectors

demo/
├── Irihi.Mafia.Demo/                 # Shared demo application
├── Irihi.Mafia.Demo.Desktop          # Windows/macOS/Linux launcher
├── Irihi.Mafia.Demo.Browser          # WebAssembly launcher
├── Irihi.Mafia.Demo.Android          # Android launcher
└── Irihi.Mafia.Demo.iOS              # iOS launcher
```

## Theme Customization

The TDesign theme uses a layered token system, making customization straightforward:

1. **Design tokens** (`Tokens/`) — Atomic values for colors, fonts, radius, and spacing
2. **Shared dimensions** (`Themes/Shared/`) — Component-level size and padding references
3. **Theme variant** (`Themes/Light/`, `Themes/Dark/`) — Color mappings per light/dark mode
4. **Control templates** (`Controls/`) — Visual tree and bindings for each control

To customize, override any `DynamicResource` key from your app:

```xml
<Application.Resources>
    <SolidColorBrush x:Key="TDBrandColor" Color="#FF5722" />
</Application.Resources>
```

## Build from Source

```bash
# Clone the repository
git clone https://github.com/irihi/irihi-mafia.git
cd irihi-mafia

# Build
dotnet build

# Run tests
dotnet test

# Run the desktop demo
dotnet run --project demo/Irihi.Mafia.Demo.Desktop
```

## Contributing

Contributions are welcome! Please open an issue or submit a pull request.

## License

This project is licensed under the [MIT License](LICENSE).

Copyright (c) 2026 IRIHI Technology
