# Irihi.Mafia Copilot Instructions

## Project Goal

Irihi.Mafia is an Avalonia component library and theme system for **mobile-first** applications.

The target visual and interaction reference is **Tencent TDesign Mobile Vue** rather than generic TDesign desktop components. When there is uncertainty, prefer the behavior, naming, examples, and interaction patterns from the **`mobile-vue`** component set.

This repository is mainly used to build:

1. Reusable controls in `src/Irihi.Mafia`
2. A TDesign Mobile-inspired theme implementation in `src/Irihi.Mafia.Themes.TDesign`
3. Demo experiences in `demo/` for phone-sized layouts and mobile scenarios
4. Unit and headless UI tests in `test/`

## Official Reference Sources

Use these references in priority order depending on the question being answered:

1. **TDesign Mobile Vue MCP** for component semantics, API naming, examples, and mobile interaction patterns
2. **Avalonia Build MCP** for official Avalonia docs, API reference, and framework-specific implementation rules
3. **Semi.Avalonia** as a local implementation reference for how another Avalonia component library structures themes, controls, and styling patterns when those patterns fit this repository
4. **Ursa.Avalonia** as an additional Avalonia implementation reference for control architecture, theme organization, and library-level patterns when those patterns fit this repository

Reference link for Semi.Avalonia:

- https://github.com/irihitech/Semi.Avalonia

Reference link for Ursa.Avalonia:

- https://github.com/irihitech/Ursa.Avalonia

When using the TDesign MCP tools:

1. Prefer the **`mobile-vue`** framework
2. Check component docs before inventing API or interaction details
3. Use component demo and DOM references when translating structure or states
4. Treat TDesign Mobile Vue as the source of truth for mobile semantics, not desktop TDesign

When using the Avalonia Build MCP tools:

1. Use `get_avalonia_expert_rules` early in a session when implementing or refactoring controls
2. Use `search_avalonia_docs` and `lookup_avalonia_api` before guessing Avalonia API behavior
3. Prefer official Avalonia guidance over generic XAML or WPF assumptions

When using Semi.Avalonia as a reference:

1. Reuse architectural ideas and Avalonia-specific implementation patterns, not Semi branding or API names
2. Prefer Semi.Avalonia for internal structure inspiration only when it does not conflict with TDesign Mobile Vue semantics
3. Do not copy behavior that would push the component toward desktop-first interaction

When using Ursa.Avalonia as a reference:

1. Reuse architecture, project organization, and Avalonia implementation patterns that fit this repository
2. Prefer Ursa.Avalonia as a secondary structural reference, not as the product semantics source
3. Do not copy UX behavior that conflicts with TDesign Mobile Vue mobile-first expectations

## Repository Structure

- `src/Irihi.Mafia/`
  - Core controls and shared runtime code
  - Custom controls belong here when they introduce public API or reusable behavior
- `src/Irihi.Mafia.Themes.TDesign/`
  - TDesign Mobile-inspired styling and resources
  - `Controls/`: control themes and previews
  - `Themes/Shared`: structure and logic shared across light and dark
  - `Themes/Light` and `Themes/Dark`: theme-specific resource overrides
  - `Tokens/`: design tokens and semantic resources
- `demo/Irihi.Mafia.Demo/`
  - Demo views and usage examples
  - New public components should have a mobile-oriented example
- `test/Irihi.Mafia.UnitTest/`
  - Logic and API tests
- `test/Irihi.Mafia.HeadlessTest/`
  - Avalonia headless UI tests

## Working Rules

Keep the existing separation of concerns:

1. Put reusable control API in `src/Irihi.Mafia`
2. Put visual styling in `src/Irihi.Mafia.Themes.TDesign`
3. Put usage examples in `demo/`
4. Put behavior verification in `test/`

Do not implement a component only in the demo or only in theme files if it needs reusable public API.

## Additional Reference Guidance

- If a component behavior question is about **mobile product semantics**, follow TDesign Mobile Vue first
- If a question is about **Avalonia mechanics**, follow Avalonia Build MCP first
- If a question is about **how to organize Avalonia theme/control code**, Semi.Avalonia can be used as a secondary structural reference
- When these sources disagree, prefer:
  1. TDesign Mobile Vue for UX semantics
  2. Avalonia official docs for framework correctness
  3. Semi.Avalonia for implementation inspiration

## Mobile-First Control Conventions

### Public API

- Prefer Avalonia styled properties for configurable control state
- Keep property names PascalCase
- Keep nullable reference types enabled
- Reuse Avalonia concepts where possible, but align semantic concepts with **TDesign Mobile Vue**
- Keep mobile concepts explicit when needed, such as `Placement`, `PopupVisible`, `SafeAreaInset`, `Size`, `Status`, `Orientation`, or `Trigger`

### Interaction

- Optimize for touch interaction over pointer-precision interaction
- Prefer larger hit targets and clear pressed / disabled / loading feedback
- Consider safe areas for bottom bars, action sheets, dialogs, and full-screen pages
- Consider virtual keyboard effects for inputs, search, picker, and form-like components
- Consider overlay stacking, dismiss behavior, and scroll locking for drawer / popup / picker-like components
- Prefer mobile patterns from TDesign Mobile Vue when desktop and mobile behavior differ

### Styling

- Use `ControlTheme`, `Style`, `ControlTemplate`, and resource dictionaries in the existing Avalonia structure
- Keep shared control structure in `Themes/Shared`
- Put shared metrics and structure resources such as typography, spacing, sizing, padding, margin, corner radius, and layout-oriented aliases in `Themes/Shared`
- Keep light/dark value differences in `Themes/Light` and `Themes/Dark`
- Put theme-variant visual resources such as foregrounds, backgrounds, border brushes, and state-specific color aliases in `Themes/Light` and `Themes/Dark`
- Use leading-uppercase class names in `Classes` and avoid lowercase variant names; prefer forms like `Primary`, `Large`, `Tag`, and `Round`
- When styling `Classes`, prefer merging related class-based selectors under one outer `Style` such as `^.Tag` or `^.Round` instead of scattering many flat sibling `Style` blocks
- Reuse existing tokens before adding new ones
- Add component-level tokens only when semantic tokens are not enough

### Tokens

- Base tokens belong in `Tokens/`
- Semantic aliases are preferred over hard-coded literals
- Component-specific resources should follow `TD<Component><Meaning>` naming
- Mobile spacing, height, corner radius, and typography should follow a consistent touch-oriented scale
- Avoid duplicating literal colors, spacing, or radii across multiple components

### Demo and Tests

For every meaningful new component or feature, add the relevant subset of:

1. A mobile-oriented demo example
2. A preview or sample state if the existing theme files follow that pattern
3. Basic tests for public API or UI behavior when practical

Testing rule:

- Do not add tests just for theme-only styling of built-in Avalonia controls
- Add tests when introducing a new custom control in `src/Irihi.Mafia` or new behavior/state logic beyond simple theme resource wiring

## What Copilot Should Optimize For

When implementing components, optimize for:

1. TDesign Mobile Vue API and interaction consistency
2. Mobile-first usability on phone-sized layouts
3. Reusable Avalonia patterns
4. Clear token layering
5. Themeability across light and dark modes
6. Demoability and testability

## Definition of Done for a Component Change

A component change is usually not complete until it includes the relevant subset of:

1. Control API in `src/Irihi.Mafia`
2. TDesign theme resources in `src/Irihi.Mafia.Themes.TDesign`
3. Token additions or mappings if new visual semantics were introduced
4. Demo usage in `demo/`
5. Unit or headless tests in `test/` when the change adds a custom control or new behavior logic
6. Mobile behavior considerations such as touch states, safe area, overlay behavior, or keyboard handling when applicable

## Commands

Use the existing repository commands:

```bash
dotnet build src/Irihi.Mafia/Irihi.Mafia.csproj
dotnet build src/Irihi.Mafia.Themes.TDesign/Irihi.Mafia.Themes.TDesign.csproj
dotnet test test/Irihi.Mafia.UnitTest/Irihi.Mafia.UnitTest.csproj
dotnet test test/Irihi.Mafia.HeadlessTest/Irihi.Mafia.HeadlessTest.csproj
dotnet run --project demo/Irihi.Mafia.Demo.Desktop/Irihi.Mafia.Demo.Desktop.csproj
```

Note: full solution build can be blocked locally by mobile platform workloads or JDK requirements.

## Preferred Task Framing for Copilot

When asked to implement a component, assume the request should include:

- Which **TDesign Mobile Vue** component is being mirrored
- Required variants, sizes, and states
- Expected public API
- Required token mappings
- Mobile interaction expectations
- Demo expectations
- Test expectations

If any of those are missing, make the smallest reasonable choice that stays consistent with existing repository patterns and TDesign Mobile Vue behavior.
