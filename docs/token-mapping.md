# TDesign Mobile Vue Token Mapping Guide

This document defines how **TDesign Mobile Vue** design decisions should be translated into Avalonia resources inside `Irihi.Mafia.Themes.TDesign`.

The goal is to keep token usage consistent, themeable, and appropriate for mobile-oriented UI.

## 1. Token Layers

Use tokens in this order from generic to component-specific:

1. **Foundation tokens**
   - Raw palette, typography, radius, spacing, and size primitives
   - Examples: `TDBrandColor7`, `TDGrayColor3`, `TDSize6`, `TDRadiusDefault`
2. **Semantic tokens**
   - Meaningful aliases preferred by component themes
   - Examples: `TDTextColorPrimary`, `TDBgColorContainer`, `TDComponentBorder`
3. **Component tokens**
   - Component-scoped aliases derived from semantic tokens
   - Examples: `TDButtonPrimaryBackground`, `TDButtonBaseDisabledForeground`
4. **Theme files**
   - `Themes/Shared`, `Themes/Light`, `Themes/Dark` consume component tokens and define behavior

Do not jump from raw colors directly into control theme files unless the value is truly incidental and unlikely to be reused.

## 2. Current Token Sources in This Repository

Use these files as the default source of truth:

- `src/Irihi.Mafia.Themes.TDesign/Tokens/Light.axaml`
- `src/Irihi.Mafia.Themes.TDesign/Tokens/Dark.axaml`
- `src/Irihi.Mafia.Themes.TDesign/Tokens/Size.axaml`
- `src/Irihi.Mafia.Themes.TDesign/Tokens/Radius.axaml`
- `src/Irihi.Mafia.Themes.TDesign/Tokens/Font.axaml`

### What belongs where

- `Light.axaml` / `Dark.axaml`
  - color palette
  - brushes
  - semantic text, background, border, and shadow resources
- `Size.axaml`
  - sizing primitives
  - component heights
  - paddings and margins
- `Radius.axaml`
  - corner radius primitives
- `Font.axaml`
  - font sizes, line heights, weights, and typography primitives

## 3. Mapping Rules

### Colors

Map TDesign Mobile Vue colors in this sequence:

1. TDesign palette token
2. Repository semantic color token
3. Component token
4. Control theme usage

Example:

```text
TDesign Mobile primary brand color
-> TDBrandColor7
-> TDTextColorBrand or TDBrandColor
-> TDButtonPrimaryBackground
-> Button theme setter
```

### Spacing and size

Prefer `TDSize*` and `TDCompSize*` resources before introducing component-only measurements.

When the TDesign Mobile Vue reference is denser or larger than current desktop-like sizing, prefer a shared touch-oriented alias instead of scattering literals in component theme files.

If a component needs its own size resource, create an alias such as:

```text
TDButtonMediumMinHeight -> TDCompSizeXL
```

This keeps component tokens readable while preserving linkage to the shared mobile scale.

### Radius

Map TDesign radius concepts to shared radius tokens first:

- default radius -> `TDRadiusDefault`
- rounded pill/capsule -> `TDRadiusRound`
- fully circular use cases should still be expressed by control logic plus appropriate size constraints

### Typography

Prefer shared font resources such as `TDFontSizeBodyLarge` and shared font weights before introducing component-only font values.

For phone-first components, prefer readable touch-scale typography over compact desktop density.

## 4. Naming Convention

Use the following naming direction:

- Foundation: `TD<Domain><Token>`
  - `TDBrandColor7`
  - `TDSize6`
  - `TDRadiusDefault`
- Semantic: `TD<Text|Bg|Border|Component|Shadow><Meaning>`
  - `TDTextColorPrimary`
  - `TDBgColorContainer`
  - `TDComponentBorder`
- Component: `TD<Component><Meaning>`
  - `TDButtonPrimaryBackground`
  - `TDButtonOutlineDangerBorderBrush`

Prefer descriptive suffixes such as:

- `Foreground`
- `Background`
- `BorderBrush`
- `PressedBackground`
- `DisabledForeground`
- `MinHeight`
- `FontSize`
- `PaddingHorizontal`
- `PaddingVertical`

## 5. Light and Dark Strategy

Follow this split:

1. Put shared control structure and shared metrics in `Themes/Shared/<Component>.axaml`
2. Shared files should hold typography, font weight, spacing, padding, margin, sizing, radius, and other layout-oriented aliases
3. Put light-only visual resource values in `Themes/Light/<Component>.axaml`
4. Put dark-only visual resource values in `Themes/Dark/<Component>.axaml`
5. Light and dark files should hold foreground, background, border brush, and state-specific color aliases

If a component token has different values per theme, the resource key should stay the same while the light and dark dictionaries provide different values.

That lets control themes bind to one stable key.

## 6. Mobile-First Token Decisions

When deciding whether to add or reuse a token, consider these mobile-specific questions:

1. Is the value tied to touch target size or dense tap areas?
2. Is the value specific to overlays, sheets, tab bars, or mobile navigation?
3. Does the value change when safe-area padding is applied?
4. Does the value need to remain consistent across narrow phone layouts?

If yes, prefer a shared semantic or component token rather than inline literals.

## 7. When to Add a New Token

Add a new token when at least one of these is true:

1. The meaning is reused by multiple components
2. The meaning must differ between light and dark themes
3. The value expresses a stable TDesign Mobile Vue semantic concept
4. The value improves readability versus repeating a low-level token chain
5. The value captures a reusable mobile concept such as bottom safe-area padding, action-sheet spacing, or tab-bar height

Do not add a new token when the value is only a one-off incidental literal that is unlikely to be reused.

## 8. Example: Button

The existing button implementation is a reference pattern for token layering, even if sizing and semantics may still evolve toward stronger mobile-first alignment.

Example layering:

```text
TDRadiusDefault
-> TDButtonRectangleBorderRadius
-> BaseButtonControlTheme CornerRadius
```

```text
TDFontSizeBodyLarge
-> TDButtonMediumFontSize
-> BaseButtonControlTheme FontSize
```

```text
TDBrandColor / semantic brush
-> TDButtonPrimaryBackground
-> Button Primary style
```

## 9. AI Working Guidance

When asking AI to map tokens for a new component, provide:

1. The target TDesign Mobile Vue component
2. Which visual states must be supported
3. Which existing semantic tokens should be preferred
4. Whether a new component token layer is expected
5. Any mobile-specific layout or safe-area requirements

Good request example:

> Add token mapping for TDesign Mobile Vue TabBar in Avalonia. Reuse existing semantic text, background, border, and size tokens where possible, create `TDTabBar*` aliases only for state-specific values, and keep the same resource keys across light and dark themes.
