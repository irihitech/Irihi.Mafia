# Mobile UI Specification

This document defines the baseline mobile-first behavior for Irihi.Mafia.

The visual and interaction reference is **TDesign Mobile Vue**, translated into Avalonia for phone-sized and other touch-oriented devices.

## 1. Primary Design Target

- Default target: phone-sized layouts
- Default interaction model: touch first
- Secondary support: larger windows and pointer input, without weakening core mobile usability

Do not design new components around desktop hover-first assumptions.

## 2. Touch Targets

- Interactive elements should have touch-friendly hit areas
- Do not rely on tiny icon-only affordances unless they are visually prominent and have sufficient padding
- Pressed state feedback should be obvious and immediate
- Disabled and loading states should remain legible on small screens

## 3. Safe Area Rules

Apply safe-area awareness when components are anchored to screen edges, especially:

- bottom tab bars
- action sheets
- drawers
- dialogs or popups near edges
- full-screen pages with fixed headers or footers

Rules:

1. Bottom-anchored surfaces should account for bottom insets
2. Top-fixed navigation should avoid clipping into status-bar space when the host app exposes such insets
3. Safe-area handling should be opt-in or configurable when host applications manage it externally

## 4. Overlay and Popup Behavior

For sheets, drawers, pickers, dialogs, dropdown-like popups, and other overlays:

1. Define placement explicitly
2. Define whether outside tap dismisses the overlay
3. Define whether the back action or escape closes the overlay
4. Define whether background scroll is locked while open
5. Define z-order expectations relative to other overlays

When TDesign Mobile Vue uses a bottom sheet or mobile popup pattern, prefer that behavior over a desktop-style floating panel.

## 5. Scroll and Gesture Rules

- Avoid nesting multiple competing scroll containers without strong need
- Consider gesture conflicts between scroll, swipe, drag, and dismiss
- If a component is swipeable or draggable, define which axis owns the gesture
- For long content in overlays, prefer a clear internal scroll area instead of content clipping

## 6. Keyboard and Form Behavior

For input, textarea, search, picker, form, and form-like overlays:

1. Consider whether the virtual keyboard obscures content
2. Keep the focused field visible when the keyboard is open
3. Avoid placing essential actions where the keyboard will cover them
4. Define confirm and cancel behavior for picker-like interactions

## 7. Layout Density

- Favor readable spacing and tap-friendly density over compact desktop density
- Use shared size tokens rather than per-component arbitrary values
- Keep typography readable at phone scale
- Avoid horizontal layouts that only work at wide widths unless the component is specifically for tablet or desktop scenarios

## 8. Theme Resource Layering

For component theme files, keep the resource split explicit:

1. `Themes/Shared/<Component>.axaml` holds shared metrics and structure-oriented aliases such as typography, spacing, padding, margin, sizing, radius, and indicator dimensions
2. `Themes/Light/<Component>.axaml` and `Themes/Dark/<Component>.axaml` hold theme-variant visual aliases such as foreground, background, border brush, and state-specific color resources
3. Use leading-uppercase class names in `Classes` and avoid lowercase variant names; prefer `Primary`, `Large`, `Tag`, and `Round`
4. When variant styling is driven by Avalonia `Classes`, group each class under a single outer `Style` block and nest the related selectors inside it for readability

Do not put shared sizing rules into light/dark files, and do not put theme-specific color aliases into shared files.

## 9. Demo Requirements

Every component demo should answer the mobile question, not just the API question.

Prefer demos that show:

1. Phone-sized layout
2. Typical mobile content density
3. Edge attachment or safe-area behavior where relevant
4. Overlay open and close states where relevant
5. Form or input usage where relevant

The existing demo already includes phone resolution selection. New demos should continue to work well under narrow mobile widths.

## 10. Testing Focus

When behavior matters, prioritize tests around:

- open and close state
- selected and pressed state
- visibility and content presentation
- keyboard- or focus-related state transitions
- safe-area related layout decisions when logic exists in code

Do not add tests only to prove that a built-in Avalonia control can load a theme. Prefer tests for custom controls or for new runtime behavior introduced by the repository.

## 11. AI Usage Rules

When asking AI to implement a component, always specify:

- the TDesign Mobile Vue reference component
- required mobile states
- whether safe area matters
- whether overlay dismissal rules matter
- whether keyboard behavior matters

Good example:

> Implement ActionSheet as the Avalonia version of TDesign Mobile Vue ActionSheet. It should open from the bottom, support safe-area padding, outside-tap dismissal, and demo usage on a phone-width page.
