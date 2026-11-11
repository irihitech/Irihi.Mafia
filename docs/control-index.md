# Control Index

> **Maintenance rule:** every added, renamed, or removed control **must** be reflected in this table. 
> **维护规则：** 新增、重命名或移除任何控件时，**必须**同步更新此表。

This is the map from every control to its source class, theme, tokens, demo page, and tests. Paths are relative to `src/Irihi.Mafia/`, `src/Irihi.Mafia.Themes.TDesign/`, `demo/Irihi.Mafia.Demo/`, and `test/` respectively.

Control kinds:

- **Custom** — control class lives in `src/Irihi.Mafia/Controls/`.
- **Themed built-in** — an Avalonia control that only gets a ControlTheme in this repo (no class here).
- **Page shell** — Avalonia 12 built-in navigation/page controls (`Avalonia.Controls` namespace), themed only.

---

## 1. Custom controls

| Control | Class | ControlTheme | Component tokens | Demo page | Tests |
|---------|-------|--------------|------------------|-----------|-------|
| `Avatar` | `Controls/Avatar/Avatar.cs` | `Controls/Avatar.axaml` | — (uses global semantic tokens) | `Views/Pages/AvatarPage.axaml` | — |
| `Calendar` | `Controls/Calendar/` (7 files) | `Controls/Calendar.axaml` | `Themes/{Shared,Light,Dark}/Calendar.axaml` | `CalendarPage.axaml` | `UnitTest/CalendarTest.cs` |
| `CalendarDatePicker` | `Controls/Calendar/CalendarDatePicker.cs` | `Controls/CalendarDatePicker.axaml` | Calendar token files | `CalendarDatePickerPage.axaml` | `UnitTest/CalendarDatePickerTest.cs` |
| `Cell` / `CellGroup` | `Controls/Cell/{Cell,CellGroup,ICell}.cs` | `Controls/Cell.axaml` + global `Styles/Cell.axaml` | `Themes/{Shared,Light,Dark}/Cell.axaml` | `CellPage.axaml` | — |
| `Divider` | `Controls/Divider.cs` | `Controls/Divider.axaml` | `Themes/{Shared,Light,Dark}/Divider.axaml` | `DividerPage.axaml` | — |
| `HyperlinkButton` | `Controls/HyperlinkButton.cs` (subclass of the Avalonia built-in; `StyleKeyOverride` → built-in theme; `Size` styled property mirrors `Small`/`Medium`/`Large` classes) | `Controls/HyperlinkButton.axaml` | `Themes/{Shared,Light,Dark}/HyperlinkButton.axaml` | `HyperlinkButtonPage.axaml` | `UnitTest/HyperlinkButtonTest.cs` |
| `IconButton` | `Controls/IconButton.cs` | `Controls/IconButton.axaml` (default + `OutlineIconButton`, `TextIconButton`) | — (`TDIconButtonInnerSpacing` lives in `Themes/Shared/Button.axaml`) | `IconButtonPage.axaml` | — |
| `Input` | `Controls/Input.cs` | `Controls/Input.axaml` | `Themes/{Shared,Light,Dark}/Input.axaml` | `InputPage.axaml` | — |
| `Picker` | `Controls/Picker.cs` | `Controls/Picker.axaml` | `Themes/{Shared,Light,Dark}/Picker.axaml` | `PickerPage.axaml` | — |
| `PickerItem` | `Controls/PickerItem.cs` | themed inside `Controls/Picker.axaml` | Picker token files | (Picker demo) | — |
| `Popup` | `Controls/Primitives/Popup.cs` | `Controls/Popup.axaml` | `Themes/{Shared,Light,Dark}/Popup.axaml` | `PopupPage.axaml` | `UnitTest/PopupTest.cs` |
| `OverlayPopupHost` | `Controls/Primitives/OverlayPopupHost.cs` | themed inside `Controls/Popup.axaml` | Popup token files | — (via Popup demo) | `UnitTest/OverlayPopupHostTest.cs` |
| `StickyPanel` | `Controls/Primitives/StickyPanel.cs` | — (behavior-only primitive) | — | `StickyPanelPage.axaml` | `HeadlessTest/StickyPanelTests.cs` |

Shared enums (`Position`, `PopupPlacement`, `ControlSize`) live in `src/Irihi.Mafia/Common/`.

## 2. Themed Avalonia built-ins

No class in this repo — only a ControlTheme under `src/Irihi.Mafia.Themes.TDesign/Controls/`, keyed `{x:Type <Name>}` (no `m:` prefix, the type is Avalonia's).

| Control | ControlTheme | Component tokens | Demo page |
|---------|--------------|------------------|-----------|
| `Button` | `Controls/Button.axaml` | `Themes/{Shared,Light,Dark}/Button.axaml` | `ButtonPage.axaml` |
| `CheckBox` | `Controls/CheckBox.axaml` | `Themes/{Shared,Light,Dark}/CheckBox.axaml` | `CheckBoxPage.axaml` |
| `Progress` | `Controls/Progress.axaml` | `Themes/{Shared,Light,Dark}/Progress.axaml` | `ProgressPage.axaml` |
| `RadioButton` | `Controls/RadioButton.axaml` | `Themes/{Shared,Light,Dark}/RadioButton.axaml` | `RadioButtonPage.axaml` |
| `Slider` | `Controls/Slider.axaml` | `Themes/{Shared,Light,Dark}/Slider.axaml` | `SliderPage.axaml` |
| `SplitView` | `Controls/SplitView.axaml` | `Themes/{Shared,Light,Dark}/SplitView.axaml` | — |
| `Switch` | `Controls/Switch.axaml` | `Themes/{Shared,Light,Dark}/Switch.axaml` | `SwitchPage.axaml` |
| `TabControl` | `Controls/TabControl.axaml` | `Themes/{Shared,Light,Dark}/TabControl.axaml` | `TabControlPage.axaml` |
| `ItemsControl` | `Controls/ItemsControl.axaml` | — | — |
| `ScrollViewer` | `Controls/ScrollViewer.axaml` | — | — (via `ScrollPage` demo theme) |
| `PathIcon` | `Controls/PathIcon.axaml` | — | — (see `IconGalleryPage.axaml`) |
| `Window` | `Controls/Window.axaml` | — | — (demo main window) |
| `EmbeddableControlRoot` | `Controls/EmbeddableControlRoot.axaml` | — | — |
| `ThemeVariantScope` | `Controls/ThemeVariantScope.axaml` | — | — |

`Controls/GlyphButton.axaml` holds **string-keyed helper themes** (not `{x:Type}` defaults): `TDGlyphButton` (→ `Button`) and `TDGlyphToggleSwitch` (→ `ToggleSwitch`), consumed by other themes and demos.

## 3. Page shells (Avalonia 12 built-ins)

| Control | ControlTheme | Component tokens | Demo |
|---------|--------------|------------------|------|
| `ContentPage` | `Controls/ContentPage.axaml` | `Themes/{Shared,Light,Dark}/ContentPage.axaml` | Demo-local `ScrollPage` theme extends it (`demo/.../Themes/ContentPage.axaml`) |
| `NavigationPage` | `Controls/NavigationPage.axaml` | `Themes/{Shared,Light,Dark}/NavigationPage.axaml` | App shell: `MainView` |
| `TabbedPage` | `Controls/TabbedPage.axaml` | `Themes/{Shared,Light,Dark}/TabbedPage.axaml` | App shell (`HomeView`) + `TabbedPagePage.axaml` |
| `DrawerPage` | `Controls/DrawerPage.axaml` | `Themes/{Shared,Light,Dark}/DrawerPage.axaml` | — |

## Known gaps | 已知空洞

- No tests: `Divider`, `Cell`, `IconButton`, `Input`, `Picker` (custom controls without logic suites yet — welcome contributions).
- No dedicated demo page: `DrawerPage`, `SplitView`, `ItemsControl`, `OverlayPopupHost`.
- `Controls/_index.axaml` currently contains duplicate includes for `ContentPage`/`NavigationPage` (harmless; do not replicate the habit).
