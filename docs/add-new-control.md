# Adding a New Control — End-to-End Checklist

How to add a control `Foo` to Irihi.Mafia, with every file that must change. For design questions (API contract, variants, states) fill in [component-spec.md](component-spec.md) first; for token strategy see [token-mapping.md](token-mapping.md); for mobile behavior rules see [mobile-ui-spec.md](mobile-ui-spec.md).

## Step 0 — Category spec

Check [specs/](specs/README.md) for the control's category. If a spec sheet exists, follow its API mapping table and token plan — deviations require editing the spec in the same change, and landing the control backfills its *Implementation notes (landed)* subsection. If the category has no spec yet, fill [component-spec.md](component-spec.md) per control, or create the category spec first ([spec-template.md](specs/spec-template.md)) when adding several controls from one TDesign group.

Pick your path first:

- **Path A — new custom control** (new public API + visuals): do all steps.
- **Path B — theme an Avalonia built-in** (`Button`, `CheckBox`, `Slider`, …): **skip step 1**; in step 2 use `TargetType="Foo"` without the `m:` prefix.
- **Path C — page shell** (`NavigationPage`, `TabbedPage`, `ContentPage`, `DrawerPage`): like Path B — these are Avalonia 12 built-ins; this repo only themes them.

---

## Step 1 — Control class (Path A only)

`src/Irihi.Mafia/Controls/Foo.cs`:

- Namespace is always `Irihi.Mafia.Controls` — subfolders (`Calendar/`, `Cell/`) are file organization only, they do **not** change the namespace.
- Shared enums → `src/Irihi.Mafia/Common/`; non-visual primitives → `Controls/Primitives/` (namespace `Irihi.Mafia.Controls.Primitives`).
- No xmlns registration needed — `Properties/AssemblyInfo.cs` already maps the namespaces to `https://irihi.tech/mafia`.
- Follow the existing patterns:
  - Simple: `Divider.cs` (one styled property, no template logic).
  - Canonical: `IconButton.cs` — `[TemplatePart(PART_RootPanel, typeof(Panel))]`, `[PseudoClasses(...)]`, `public const string PART_…/PC_…`, styled properties with static-ctor default overrides and `AddClassHandler`, pseudo-class updates in `OnApplyTemplate`.
  - Items control + popup: `Picker.cs` — `CreateContainerForItemOverride`, `e.NameScope.Get<Popup>(…)` with subscribe/unsubscribe discipline in `OnApplyTemplate`.
- No XAML in this package — logic only.

## Step 2 — ControlTheme

`src/Irihi.Mafia.Themes.TDesign/Controls/Foo.axaml`:

- A `ResourceDictionary` **without** code-behind.
- Include a `<Design.PreviewWith>` block at the top (live preview; every existing theme has one).
- Default theme: `<ControlTheme x:Key="{x:Type m:Foo}" TargetType="m:Foo">`. Variant themes use string keys + `BasedOn` (see `OutlineIconButton`/`TextIconButton` in `IconButton.axaml`).
- Reference **only** `TDFoo*` component tokens via `DynamicResource` — never hardcode colors, sizes, or radii.
- Icons: `xmlns:tdicon="https://irihi.tech/iconica/tdesign"` (from `Irihi.Iconica.TDesign`).
- **Register**: add `<ResourceInclude Source="Foo.axaml" />` to `Controls/_index.axaml`.

## Step 3 — Component tokens

Create three files (mirror an existing triple, e.g. `Divider` or `Picker`):

| File | Contents |
|------|----------|
| `Themes/Shared/Foo.axaml` | Variant-independent metrics: sizes, paddings, margins, radii, StreamGeometry icons |
| `Themes/Light/Foo.axaml` | Colors: `<StaticResource x:Key="TDFoo…" ResourceKey="TDSemanticToken…" />` mappings |
| `Themes/Dark/Foo.axaml` | Same keys as Light with dark-appropriate semantic mappings |

- Naming: `TD<Foo><Meaning>` → e.g. `TDFooBackground`, `TDFooBackgroundPressed`.
- **Light and Dark must define identical key sets** (Dark mirrors Light).
- **Register each file** in `Themes/Shared/_index.axaml`, `Themes/Light/_index.axaml`, and `Themes/Dark/_index.axaml`. Together with step 2, that is **four `_index.axaml` registrations** — a missed one fails quietly (unthemed control / missing resource at runtime).

Only if you introduce an entire new token *dictionary* (rare): files under `Tokens/` are instantiated as elements (`<td:Font />`) and therefore need a paired `.axaml.cs` with `public class X : ResourceDictionary;`.

Global cross-control styles (selectors spanning multiple control types, e.g. `Styles/Cell.axaml`) go in `Styles/` and are wired via `StyleInclude` in `TDesignTheme.axaml` — separate mechanism from ControlThemes.

## Step 4 — Demo (4 sub-touchpoints)

1. `demo/Irihi.Mafia.Demo/Views/Pages/FooPage.axaml` (+ `.axaml.cs`):
   - Root is `ContentPage` with `Theme="{DynamicResource ScrollPage}"` (demo-local theme wrapping content in a ScrollViewer).
   - Group content in `demo:GalleryBlock` blocks; the first block gets `Classes="PageStart"`.
   - Text via i18n: `{demo:Translate {x:Static demo:LanguageManager+Keys.Foo_Title}}`.
   - Add a ViewModel in `ViewModels/Pages/FooPageViewModel.cs` only if the page needs state (see `ButtonPageViewModel`); static pages don't need one.
2. Route: `Services/NavigationService.cs` → `["Foo"] = () => new FooPage { DataContext = new FooPageViewModel() },`
3. Gallery entry: `ViewModels/Tabs/GalleryTabViewModel.cs` → `CreateItem("Foo", nameof(LanguageManager.Keys.Gallery_Foo), nameof(LanguageManager.Keys.Gallery_Foo_Desc))` in `_allInteractive` or `_allDisplay`.
4. Localization — **both** `Resources/Strings.resx` and `Resources/Strings.zh-Hans.resx`: `Gallery_Foo`, `Gallery_Foo_Desc`, plus page keys `Foo_Title`, `Foo_Desc`, `Foo_*` variants. The resx are `AdditionalFiles`; `LanguageManager` regenerates at build.

## Step 5 — Tests

- `test/Irihi.Mafia.UnitTest/FooTest.cs` — plain xunit.v3 `[Fact]`, no Avalonia dispatcher: defaults, property changes, commands (see `CalendarTest.cs`).
- `test/Irihi.Mafia.HeadlessTest/` — `[AvaloniaFact]` for layout/template behavior: `window.Show()` + `Dispatcher.UIThread.RunJobs()` then assert `Bounds`/`RenderTransform` (see `StickyPanelTests.cs` + its XAML fixture folder).
- **Do not add tests for theme-only styling of built-in Avalonia controls.**

## Step 6 — Docs

- Add the row to the right table in [control-index.md](control-index.md).
- Add the control to the tables in `README.md` and `README_CN.md`.

## Step 7 — Verify

```bash
dotnet build src/Irihi.Mafia/Irihi.Mafia.csproj
dotnet build src/Irihi.Mafia.Themes.TDesign/Irihi.Mafia.Themes.TDesign.csproj
dotnet test test/Irihi.Mafia.UnitTest/Irihi.Mafia.UnitTest.csproj
dotnet test test/Irihi.Mafia.HeadlessTest/Irihi.Mafia.HeadlessTest.csproj
dotnet run --project demo/Irihi.Mafia.Demo.Desktop   # check gallery → Foo page
```

---

## Gotchas

- **Four `_index.axaml` registrations** (step 2 + step 3) — misses fail silently.
- Demo page naming is `<Control>Page`, which yields `TabbedPagePage` for `TabbedPage`. The gallery `Name` string must **exactly match** the `NavigationService` route key (unknown routes fall back to `PlaceholderPage`).
- Don't "fix" the Avalonia version discrepancy by editing csprojs — demos intentionally override to a newer Avalonia via `demo/Directory.Build.targets`; src/test stay on the pinned baseline in `Directory.Packages.props`.
- `NavigationPage`/`TabbedPage`/`ContentPage`/`DrawerPage` classes are in Avalonia itself — don't go looking for them here, and use unprefixed `TargetType`.
- ControlTheme `TargetType` uses colon (`m:Divider`); type selectors inside `Styles` use pipe (`m|CellGroup`); class selectors use `^.`.
- Keep `Classes` values leading-uppercase (`Primary`, `Round`) and grouped under one outer `Style`.
