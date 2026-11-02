# AGENTS.md

Guidance for AI coding agents (and humans) working in this repository. Keep this file as the single source of truth for agent instructions; update it when conventions change.

本文件面向在此仓库中工作的 AI 编程智能体（以及人类贡献者），是智能体指引的唯一事实来源；约定变更时请同步更新本文件。

---

## Project | 项目

**EN:** Irihi.Mafia is a **mobile-first** Avalonia control library plus a TDesign-inspired theme, shipped as two NuGet packages. The visual and interaction reference is **Tencent TDesign Mobile Vue** (not desktop TDesign) — when in doubt, follow the `mobile-vue` component set for behavior, naming, and interaction patterns.

**中文：** Irihi.Mafia 是一个**移动优先**的 Avalonia 控件库与 TDesign 风格主题，以两个 NuGet 包发布。视觉与交互基准是**腾讯 TDesign Mobile Vue**（不是桌面版 TDesign）——拿不准时，一律以 `mobile-vue` 组件集的行为、命名和交互模式为准。

## Commands | 常用命令

```bash
# Build the two library projects（构建两个库项目）
dotnet build src/Irihi.Mafia/Irihi.Mafia.csproj
dotnet build src/Irihi.Mafia.Themes.TDesign/Irihi.Mafia.Themes.TDesign.csproj

# Tests（测试：纯逻辑 xunit / 无头 UI）
dotnet test test/Irihi.Mafia.UnitTest/Irihi.Mafia.UnitTest.csproj
dotnet test test/Irihi.Mafia.HeadlessTest/Irihi.Mafia.HeadlessTest.csproj

# Run the desktop demo（运行桌面演示）
dotnet run --project demo/Irihi.Mafia.Demo.Desktop

# Pack（打包）
dotnet pack src/Irihi.Mafia -o nugets
dotnet pack src/Irihi.Mafia.Themes.TDesign -o nugets
```

> A full-solution build can be blocked locally by mobile platform workloads (Android/iOS) or JDK requirements. Build/test the individual projects above instead.
> 整仓构建可能被移动平台工作负载（Android/iOS）或 JDK 要求阻塞；请改为构建/测试上述单个项目。

## Architecture | 架构

**EN:** Strict two-package separation. `Irihi.Mafia` contains **control logic only, no XAML** — control classes, styled properties, pseudo-classes. `Irihi.Mafia.Themes.TDesign` contains **all visuals** — ControlThemes, templates, tokens. Never put styling in the core package or public API in the theme package.

**中文：** 严格的双包分工。`Irihi.Mafia` 只含**控件逻辑、无 XAML**——控件类、样式属性、伪类。`Irihi.Mafia.Themes.TDesign` 含**全部视觉**——ControlTheme、模板、token。不要把样式写进核心包，也不要把公共 API 写进主题包。

XAML namespaces (already registered in each package's `Properties/AssemblyInfo.cs` — new controls need no registration):

| Prefix | URI | Maps to |
|--------|-----|---------|
| `m:` | `https://irihi.tech/mafia` | `Irihi.Mafia(.Common|.Controls|.Controls.Primitives)` |
| `td:` | `https://irihi.tech/td` | `Irihi.Mafia.Themes.TDesign(.Tokens|.Converters)` |
| `tdicon:` | `https://irihi.tech/iconica/tdesign` | `Irihi.Iconica.TDesign` icon package |

**Version pinning — do not "fix":** `Directory.Packages.props` (CPM) pins Avalonia to **12.0.3** for src/test; `demo/Directory.Build.targets` overrides demos to **12.1.1**. This split is intentional — edit the props/targets files, never per-project csprojs.

**版本锁定——不要"修复"：** `Directory.Packages.props`（CPM）将 src/test 锁定在 Avalonia **12.0.3**；`demo/Directory.Build.targets` 把 demo 覆盖到 **12.1.1**。这个差异是有意为之——改 props/targets 文件，不要改各项目的 csproj。

Also note: `NavigationPage`, `TabbedPage`, `ContentPage`, `DrawerPage` are **Avalonia 12 built-ins** — this repo only themes them; there is no `NavigationPage.cs` here.

另外：`NavigationPage`、`TabbedPage`、`ContentPage`、`DrawerPage` 是 **Avalonia 12 内置控件**——本仓库只为主题提供样式，这里没有 `NavigationPage.cs`。

## Repository map | 仓库结构

```
src/
├── Irihi.Mafia/                    # Core: control logic only, no XAML
│   ├── Controls/                   #   controls (namespace Irihi.Mafia.Controls)
│   │   ├── Calendar/               #   Calendar, CalendarDatePicker + parts
│   │   ├── Cell/                   #   Cell, CellGroup, ICell
│   │   ├── Avatar/, Divider.cs, IconButton.cs, Input.cs, Picker.cs, PickerItem.cs
│   │   └── Primitives/             #   Popup, OverlayPopupHost, StickyPanel
│   └── Common/                     #   shared enums (Position, PopupPlacement)
└── Irihi.Mafia.Themes.TDesign/     # Theme: all XAML
    ├── TDesignTheme.axaml          #   entry point (<td:TDesignTheme /> in App)
    ├── Controls/                   #   29 ControlThemes + _index.axaml
    ├── Themes/{Shared,Light,Dark}/ #   per-control tokens + _index.axaml each
    ├── Tokens/                     #   Light/Dark/Size/Radius/Font (+ .axaml.cs pairs)
    ├── Converters/                 #   value converters
    └── Styles/                     #   global cross-control styles
demo/
├── Irihi.Mafia.Demo/               # shared demo app: Views/Pages, ViewModels, Services, Resources (resx i18n)
├── Irihi.Mafia.Demo.{Desktop,Browser,Android,iOS}/   # platform heads
└── Sandbox/                        # scratch desktop app
test/
├── Irihi.Mafia.UnitTest/           # plain xunit.v3, control logic only
└── Irihi.Mafia.HeadlessTest/       # Avalonia.Headless.XUnit ([AvaloniaFact])
```

## Core conventions | 核心约定

Full details in [docs/](docs/) — the essentials, in one place:

1. **Reference baseline 参考基准** — TDesign Mobile Vue semantics first, then Avalonia framework correctness, then Semi.Avalonia/Ursa.Avalonia for structural inspiration. 拿不准时先查 mobile-vue 组件文档，再查 Avalonia 官方文档，最后参考 Semi/Ursa 的组织方式。
2. **Tokens, never literals 禁止硬编码** — ControlThemes reference only `TDFoo*` component tokens via `DynamicResource`; component tokens map to semantic tokens via `StaticResource` in `Themes/Light|Dark`. Naming: `TD<Component><Meaning>` (e.g. `TDPickerBackgroundPressed`). See [docs/token-mapping.md](docs/token-mapping.md).
3. **Token file split token 文件分工** — `Themes/Shared/<X>.axaml` holds variant-independent metrics (sizes, spacing, StreamGeometry); `Themes/Light/` + `Themes/Dark/` hold colors and **must define identical key sets**. Register every new file in its folder's `_index.axaml`.
4. **Theme keys 主题键** — default theme key is `{x:Type m:X}`; variant/auxiliary themes use string keys with `BasedOn` chaining (see `OutlineIconButton` in `Controls/IconButton.axaml`).
5. **Classes naming 样式类** — leading-uppercase (`Primary`, `Large`, `Round`, `Tag`); group related class selectors under one outer `Style` (`^.Tag`), not flat siblings.
6. **Mobile-first behavior 移动行为** — touch targets, pressed/disabled/loading feedback, safe areas, overlay stacking, scroll locking, virtual keyboard. See [docs/mobile-ui-spec.md](docs/mobile-ui-spec.md).
7. **Public API 公共 API** — Avalonia styled properties, PascalCase, NRT enabled; keep mobile concepts explicit (`Placement`, `PopupVisible`, `SafeAreaInset`, `Size`, `Status`).
8. **Control inventory 控件清单** — every control (custom, themed built-in, page shell) is indexed in [docs/control-index.md](docs/control-index.md). **Update it when adding controls.** 新增控件必须同步更新该索引。
9. **New control workflow 新增控件流程** — follow the 7-touchpoint checklist in [docs/add-new-control.md](docs/add-new-control.md).

## Reference tooling | 参考工具

`.vscode/mcp.json` registers two MCP servers:

- **`tdesignMobileVue`** (`npx tdesign-mcp-server`) — component semantics/API/examples. Always select the **mobile-vue** framework. Component docs before inventing API or interaction details.
- **`avalonia-docs`** (`https://docs-mcp.avaloniaui.net/mcp`) — official Avalonia docs. Use `get_avalonia_expert_rules` early when implementing controls; use `search_avalonia_docs` / `lookup_avalonia_api` before guessing framework behavior.

Offline references: [Semi.Avalonia](https://github.com/irihitech/Semi.Avalonia) and [Ursa.Avalonia](https://github.com/irihitech/Ursa.Avalonia) — reuse architectural patterns only, never their branding/API names, and never patterns that push components toward desktop-first interaction.

## Definition of Done | 完成定义

A component change is complete only with the relevant subset of:

1. Control API in `src/Irihi.Mafia` (if new public behavior is needed)
2. ControlTheme + tokens in `src/Irihi.Mafia.Themes.TDesign`, registered in all four `_index.axaml` files (`Controls/`, `Themes/Shared/`, `Themes/Light/`, `Themes/Dark/`)
3. Demo: page + route in `Services/NavigationService.cs` + gallery entry in `GalleryTabViewModel` + keys in **both** `Strings.resx` and `Strings.zh-Hans.resx`
4. Tests in `test/` — only for custom controls or new behavior logic; **no tests for theme-only styling of built-in Avalonia controls**
5. Docs updated: [docs/control-index.md](docs/control-index.md), README control tables
6. Mobile behavior considered: touch states, safe area, overlay behavior, keyboard

## Git discipline | Git 纪律

Conventional Commits, one logical change per commit. Never commit or push unless explicitly asked. Destructive git commands require explicit confirmation.

使用 Conventional Commits，一次提交只做一件事。未经明确要求不 commit、不 push。破坏性 git 命令须先确认。

## Docs index | 文档索引

| Doc | Purpose |
|-----|---------|
| [docs/control-index.md](docs/control-index.md) | Every control → source/theme/tokens/demo/tests mapping |
| [docs/add-new-control.md](docs/add-new-control.md) | End-to-end checklist for adding a control (7 touchpoints) |
| [docs/component-spec.md](docs/component-spec.md) | Component design template: API contract, variants, states |
| [docs/token-mapping.md](docs/token-mapping.md) | Token layers, naming, Light/Dark strategy |
| [docs/mobile-ui-spec.md](docs/mobile-ui-spec.md) | Mobile-first behavior baseline: touch, overlays, safe area |
