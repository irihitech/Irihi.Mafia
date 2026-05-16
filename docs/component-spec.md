# Mobile Component Specification Template

This document is the default checklist for adding or upgrading a component in Irihi.Mafia.

The baseline reference is **TDesign Mobile Vue**. The goal is not only to match visuals, but also to preserve mobile interaction intent when translating to Avalonia.

## 1. Component Summary

- **Component name**:
- **Reference TDesign Mobile Vue component**:
- **Goal**:
- **Out of scope**:

## 2. Reference Inputs

Before implementation, gather these inputs:

- TDesign MCP component docs
- TDesign MCP component demo
- TDesign MCP component DOM when structure matters
- Avalonia Build MCP docs or API lookup for framework-specific behavior
- Semi.Avalonia source patterns when evaluating control/theme organization
  - Repository: https://github.com/irihitech/Semi.Avalonia
- Ursa.Avalonia source patterns when evaluating library architecture or theme organization
  - Repository: https://github.com/irihitech/Ursa.Avalonia
- Existing repository tokens and themes
- Mobile-specific constraints from `docs/mobile-ui-spec.md`

## 3. Public API Contract

- **Control type**: new custom control / styled existing Avalonia control / theme-only enhancement
- **Namespace**:
- **Base class**:
- **Styled properties**:
- **Direct properties**:
- **Commands / events**:
- **Pseudo classes or style classes**:
- **Default content model**: plain content / icon + text / items / templated content / overlay host

Questions to answer:

1. Does this component need a new control in `src/Irihi.Mafia`, or is a themed built-in Avalonia control enough?
2. Which TDesign Mobile Vue concepts must be first-class API, such as `Size`, `Status`, `Placement`, `Direction`, `Theme`, `Closable`, or `OverlayVisible`?
3. Which behaviors must be represented by Avalonia properties instead of ad hoc style classes?
4. Does the component need mobile-only semantics such as safe-area handling, bottom-sheet placement, gesture dismissal, or keyboard avoidance?

## 4. Visual Variants and States

List all variants that must be supported:

- **Variants**:
- **Sizes**:
- **Shapes**:
- **Statuses**:
- **Theme variants**: Light / Dark / Shared

List all interaction states that need explicit treatment:

- Normal
- Hover when meaningful on desktop, but do not depend on hover for core usability
- Pressed / touched
- Focused
- Disabled
- Selected / Checked / Active
- Loading
- Error / Warning / Success when applicable
- Open / Closed for popup-like components
- Entering / Leaving if transitions affect behavior

## 5. Mobile Behavior Checklist

Answer each item that applies:

- **Primary usage context**: page content / overlay / bottom bar / list item / form field / picker
- **Touch target expectation**:
- **Safe area behavior**:
- **Gesture behavior**:
- **Scroll behavior**:
- **Overlay dismiss behavior**:
- **Virtual keyboard behavior**:
- **Portrait / narrow-width assumptions**:

## 6. Token Mapping Plan

Before adding visual values, map the component to tokens.

- **Uses existing global tokens**:
- **Needs new semantic aliases**:
- **Needs new component tokens**:
- **Needs light/dark overrides**:

Rules:

1. Reuse `Tokens/Light.axaml`, `Tokens/Dark.axaml`, `Tokens/Size.axaml`, `Tokens/Radius.axaml`, and `Tokens/Font.axaml` before creating new literals
2. Prefer semantic tokens over direct colors in component theme files
3. Add component tokens with `TD<Component><Meaning>` naming
4. Keep shared structure in `Themes/Shared`, and light/dark differences in `Themes/Light` and `Themes/Dark`
5. Put shared metrics and layout resources such as font size, font weight, spacing, padding, margin, sizing, radius, and indicator thickness in `Themes/Shared/<Component>.axaml`
6. Put theme-variant visual aliases such as foreground, background, border brush, and selected / pressed / disabled color resources in `Themes/Light/<Component>.axaml` and `Themes/Dark/<Component>.axaml`
7. Prefer touch-oriented size and spacing aliases over desktop-sized measurements

## 7. Files Expected for a New Component

Adjust the exact set based on scope, but use this as the default target:

- `src/Irihi.Mafia/Controls/<Component>/`
- `src/Irihi.Mafia.Themes.TDesign/Controls/<Component>.axaml`
- `src/Irihi.Mafia.Themes.TDesign/Themes/Shared/<Component>.axaml`
- `src/Irihi.Mafia.Themes.TDesign/Themes/Light/<Component>.axaml`
- `src/Irihi.Mafia.Themes.TDesign/Themes/Dark/<Component>.axaml`
- `demo/Irihi.Mafia.Demo/...`
- `test/Irihi.Mafia.UnitTest/...`
- `test/Irihi.Mafia.HeadlessTest/...`

## 8. Demo Requirements

Each component demo should show the smallest set that proves API, styling, and mobile usage:

1. Default state
2. Major variants
3. Major sizes
4. Disabled state
5. At least one realistic mobile usage example
6. Safe-area or bottom placement behavior if relevant
7. Overlay or keyboard behavior if relevant

## 9. Test Requirements

Pick the smallest useful set, but do not skip tests when the component introduces behavior.

Rules:

1. If the work is only theme customization for a built-in Avalonia control, tests are usually unnecessary
2. Add tests mainly when introducing a new custom control in `src/Irihi.Mafia`
3. Also add tests when theme work includes new behavior, state logic, coercion, interaction rules, or other runtime logic beyond simple styling

### Unit test candidates

- Styled property default values
- Property coercion or validation
- State/class transitions
- Event or command wiring
- Visibility or placement state logic

### Headless test candidates

- Template application
- Visual state transitions
- Content presentation
- Overlay open/close behavior
- Focus and keyboard-related behavior where practical
- Theme resource usage that affects runtime behavior

## 10. Acceptance Checklist

- [ ] Public API is defined in the right project
- [ ] Theme resources are wired into the existing theme structure
- [ ] Tokens are reused or added in the correct layer
- [ ] Light and dark variants are considered
- [ ] Mobile behavior has been reviewed
- [ ] Demo usage exists
- [ ] Tests cover the new behavior at a basic level when the change adds a custom control or new logic
- [ ] Naming matches existing repository conventions

## 11. Recommended Prompt Format for AI

Use requests shaped like this:

> Implement `<Component>` for Irihi.Mafia as the Avalonia version of TDesign Mobile Vue `<ReferenceComponent>`.  
> Required API: `<properties/events>`  
> Required variants/states: `<variants>`  
> Mobile behavior: `<safe-area/overlay/keyboard/touch rules>`  
> Token expectations: `<token rules>`  
> Deliverables: control/theme/demo/tests.

The more explicit the requested mobile behaviors and states are, the more reliable the generated result will be.
