# Feedback Category Spec | 反馈类规格书

> Reference: TDesign Mobile Vue **反馈** group (`reference/tdesign-mobile-vue/src/components.ts`).
> Skeleton & workflow: [spec-template.md](spec-template.md). File-level checklist: [add-new-control.md](../add-new-control.md).

## 1. Category positioning | 大类定位

Transient surfaces that report an outcome or ask for an immediate decision: confirmations (`Dialog`), choice prompts (`ActionSheet`), passive notifications (`Toast`, `Message`, `NoticeBar`), busy indicators (`Loading`), overlay infrastructure (`Popup`, `Overlay`), anchored menus (`Popover`, `DropdownMenu`), gesture feedback (`PullDownRefresh`, `SwipeCell`), and onboarding (`Guide`).

**Boundaries:**

- The input category (输入) owns every overlay whose purpose is capturing a value — `Picker`, `Calendar`, `DateTimePicker`, `Cascader` — even though they render inside popups. They **consume** this category's infra; they are not part of it.
- Navigation (导航) owns `Drawer`, `NavBar`, `Tabs`. Data display (数据展示) owns `Progress`, `Result`, `Empty`, `Skeleton`.
- Rule of thumb: exists to report or decide → feedback; exists to navigate or to display data long-term → other category.

**Shared infra (already in repo):** `Popup` + `OverlayPopupHost` (`Controls/Primitives/`) are the hosting surface for everything modal in this category. `Picker`/`Calendar` already sit on them — **changes here must not regress those controls** (see §5).

## 2. Component registry | 组件一览

| Component | TDesign | Mafia class | Path | Phase | Depends on | M-rules | Status |
|---|---|---|---|---|---|---|---|
| `Popup` | `popup` | `Controls/Primitives/Popup.cs` ✅ | A (exists) | P0 (audit) | — | M2, M3, M4, M5 | audited, fix pending §4.0 |
| `Overlay` | `overlay` | `OverlayPopupHost` ✅ (no standalone control) | A (exists) | — | Popup | M5 | folded into §4.0 |
| `Dialog` | `dialog` | `Controls/Dialog/Dialog.cs` | A | P0 | Popup audit | M2, M3, M4, M5 | spec'd §4.1 |
| `Toast` | `toast` | `Controls/Toast/Toast.cs` (service) | A | P0 | Popup audit | M3, M4 | spec'd §4.2 |
| `ActionSheet` | `action-sheet` | `Controls/ActionSheet/ActionSheet.cs` | A | P1 | Popup audit | M2 bottom, M3, M4 | spec'd §4.3 |
| `Message` | `message` | `Controls/Message/Message.cs` | A | P1 | — | M1 | spec'd §4.4 |
| `NoticeBar` | `notice-bar` | TBD | A | P2 | marquee primitive | M6 (vertical scroll) | — |
| `Loading` | `loading` | TBD | A/B | P2 | — | M1 | — |
| `Popover` | `popover` | TBD | A | P3 | Popup anchor placement | M3, M5 | — |
| `DropdownMenu` | `dropdown-menu` | TBD | A | P3 | Popover, Cell ✅ | M3, M4 | — |
| `PullDownRefresh` | `pull-down-refresh` | TBD | A | P4 | gesture primitives | M6 | — |
| `SwipeCell` | `swipe-cell` | TBD | A | P4 | gesture primitives | M6 | — |
| `Guide` | `guide` | TBD | A | P4 | Popover | M3 | — |

Rows with `—` status get their §4 subsection filled **before** implementation (template §4.x).

## 3. Shared conventions | 共享约定

- **Hosting**: every modal surface (Dialog, ActionSheet, future Popover/DropdownMenu) hosts its content through `Popup`/`OverlayPopupHost` in the same TopLevel overlay stack — never native OS windows, never per-control AdornerLayers. `OverlayPopupHost` owns mask, placement, safe-area padding, and z-order (M5).
- **Visibility**: overlay controls expose `IsOpen` (bool, `TwoWay` default) — matches the existing `Popup.IsOpen`. Services (Toast) manage their own lifecycle and expose no `IsOpen`.
- **Dismiss (M3)**: each component states outside-tap / Escape / close-button behavior in its section; defaults follow TDesign.
- **Scroll lock (M4)**: enforced structurally — the modal mask fills the overlay layer and intercepts pointer/wheel input window-wide (verified against source in §4.0); no per-component API needed. Non-modal popups are planned to pass background input through (§4.0 fix).
- **Safe area (M2)**: bottom-anchored surfaces (ActionSheet) apply bottom inset via `OverlayPopupHost.SafeAreaPadding`; center/top surfaces (Dialog, Toast) do not.
- **Animation**: enter/exit = slide (per placement) + fade, driven by `OverlayPopupHost.Transform` + theme `Transitions`; ~240–300 ms, reduced-motion variant = fade only. Timings live as metrics in `Themes/Shared/Popup.axaml`, not per component.
- **Tokens**: mask already `TDPopupMaskBrush` (`TDMaskActive`). Each component adds its own `TD<Component>*` set per [token-mapping.md](../token-mapping.md); concrete plans in §4. Values below translate the TDesign CSS vars from `_common/style/mobile/components/<name>/_var.less`.

## 4. Components | 组件规格

### 4.0 Popup — gap audit (existing)（弹层基座）

`popup` + `overlay` · exists as `Popup` / `OverlayPopupHost` · **P0 prerequisite for everything below.**

**Current API**: `Popup` — `IsOpen`, `Child`, `IsModal`, `MaskBrush`, `IsLightDismissEnabled`. `OverlayPopupHost` — `Placement` (`PopupPlacement`: Bottom/Left/Right/Center/Top/FullScreen), `SafeAreaPadding`, `ContentOffsetX/Y`, `Transform`, `IsModal`, `MaskBrush`; static z-index counter (M5 ✓).

**Audit vs `TdPopupProps`:**

| TDesign | Mafia today | Verdict |
|---|---|---|
| `visible` | `IsOpen` | ✓ |
| `placement` top/left/right/bottom/center | `PopupPlacement` (superset: + FullScreen) | ✓ |
| `closeOnOverlayClick` | `IsLightDismissEnabled` | ✓ keep Mafia name |
| `showOverlay` | `IsModal` | ✓ |
| `overlayProps` | — | n/a (composition) |
| `preventScrollThrough` (default true) | — (structural) | modal mask intercepts pointer/wheel for the whole window — M4 holds without an API (see notes) |
| `destroyOnClose` | — (content stays alive) | acceptable default; document, don't add |
| `closeBtn`, `duration`, `transitionName` | — | per-component / theme concern, not Popup |

**Audit outcome (P0-a, executed against source):** findings recorded below. The one behavior change it requires (non-modal pass-through) is **pending implementation** — land it before Dialog. Dialog (§4.1) no longer needs `LockBackgroundScroll`; its mapping row reflects that.

Regression scope: `Picker`, `Calendar`, `CalendarDatePicker`, `PopupPage` all ride on this — run their demos after any change.

**Audit findings & planned changes:**

- enter/exit animation verified — enter staged via two render-priority posts (fade 200 ms + slide 250 ms, cubic-bezier), exit mirrors it with host cleanup after a hardcoded 260 ms delay matched to the theme (drift risk if theme timings change; acceptable). `Center`/`FullScreen` slide offset is (0,0) → fade only, TDesign center parity. **Reduced-motion variant: deferred** — no repo-wide motion level exists yet; revisit when one lands.
- Safe area verified: `OverlayPopupHost` reads `TopLevel.InsetsManager.SafeAreaPadding` on attach and resize, applied as margin on the content presenter — bottom sheets sit above the inset ✓. Nuance: the margin applies to **all** placements; a `Center` dialog shifts by top/left insets too — decide in Dialog (§4.1) whether Center should ignore insets.
- Scroll-through (M4): covered **structurally** — the modal mask fills the overlay layer and intercepts pointer/wheel everywhere, verified against source — the mask fills the overlay layer and intercepts pointer/wheel everywhere. No `LockBackgroundScroll` API needed; residual leaks (programmatic scroll) accepted for v1.
- **Fix pending:** non-modal popups use a `Brushes.Transparent` mask — still hit-testable, so background input is blocked (TDesign `showOverlay=false` keeps background interactive). Planned: a null mask — background receives input, sheet content stays interactive, light dismiss becomes naturally impossible for non-modal.
- Planned tests: headless coverage for wheel blocking (M4), light dismiss on/off, and non-modal pass-through. Picker/CalendarDatePicker are unaffected (both modal by default).

### 4.1 Dialog（对话框）

`dialog` · Path A · Phase P0 · Depends on: Popup audit (gaps 1–2)

**TDesign behavior summary**: centered modal for an immediate decision or notice — title, content, confirm/cancel pair. Not for forms (use a page or bottom sheet), not for menus (ActionSheet).

**API mapping:**

| TDesign prop/event | Type / default | Mafia API | Type | Default | Notes |
|---|---|---|---|---|---|
| `visible` | boolean | `IsOpen` | bool | false | TwoWay |
| `title` | string \| TNode | `Title` / `TitleTemplate` | string? / IDataTemplate? | null | |
| `content` | string \| TNode | `Content` / `ContentTemplate` | object? / IDataTemplate? | null | |
| `confirmBtn` | string \| ButtonProps \| TNode \| null | `ConfirmButtonText` | string? | null (= hidden) | v1 string only; object form = custom `Content` |
| `cancelBtn` | string \| ButtonProps \| TNode \| null | `CancelButtonText` | string? | null (= hidden) | same |
| `buttonLayout` | 'horizontal' (default) / 'vertical' | `ButtonLayout` | `DialogButtonLayout` | Horizontal | enum → `Common/` |
| `closeBtn` | false | `IsCloseButtonVisible` | bool | false | icon button, top-right |
| `closeOnOverlayClick` | false | `CloseOnOverlayClick` | bool | false | M3 |
| `showOverlay` | true | `IsModal` | bool | true | |
| `preventScrollThrough` | true | — | — | — | structural: modal mask intercepts background input (§4.0 audit) |
| `destroyOnClose` | false | `DestroyOnClose` | bool | false | hide vs detach content |
| `width` | number/string | — | — | — | fixed by token `TDDialogWidth` (311px), mobile precedent |
| `zIndex` | 1500/2500 | — | — | — | overlay stack owns it (M5) |
| `beforeClose` | hook, async | `Closing` | routed event | — | `CancelEventArgs`; async confirm-loading deferred to P1 |
| `onConfirm` / `onCancel` | events | `Confirmed` / `Cancelled` | routed events | — | raised before close; close via `IsOpen` |
| `onClose` / `onClosed` | events | `Closing` / `Closed` | routed events | — | `Closed` = after exit animation |
| `actions` / `top` / `middle` / `overlayProps` | — | — | — | — | out of scope v1 — custom button rows go through `ContentTemplate` |

**Variants & states**: two-button horizontal (confirm = brand filled, cancel = neutral), vertical stack, close-button mode, content-only. Loading state on confirm (async `beforeClose`) = P1.

**Mobile behavior**: M3 — outside-tap closes only when `CloseOnOverlayClick` (default off, TDesign parity); Escape closes when a cancel affordance exists; M4 on; M5 in stack; M2 n/a (centered).

**Token plan** (TDesign `dialog/_var.less`):

| Token | Kind | Light target | Dark target |
|---|---|---|---|
| `TDDialogWidth` = 311 | metric | Shared | — |
| `TDDialogRadius` ← `@radius-extraLarge` | metric | Shared | — |
| `TDDialogBodyMaxHeight` = 456 | metric | Shared | — |
| `TDDialogTitleFontSize` ← `@font-title-large` | metric | Shared | — |
| `TDDialogTitleFontWeight` = 600 | metric | Shared | — |
| `TDDialogContentFontSize` ← `@font-body-large` | metric | Shared | — |
| `TDDialogCloseIconSize` = 22 | metric | Shared | — |
| `TDDialogBackground` | color | `TDBgColorContainer` | mirror |
| `TDDialogTitleForeground` | color | `TDTextColorPrimary` | mirror |
| `TDDialogContentForeground` | color | `TDTextColorSecondary` | mirror |
| `TDDialogCloseForeground` | color | `TDTextColorPlaceholder` | mirror |

Buttons reuse existing `TDButton*` tokens — no dialog-scoped button colors in v1.

**Files**: `Controls/Dialog/Dialog.cs`; `Common/DialogButtonLayout.cs`; theme `Controls/Dialog.axaml`; token triple `Themes/{Shared,Light,Dark}/Dialog.axaml`; demo `DialogPage` (+ resx keys `Gallery_Dialog`, `Dialog_*`); `UnitTest/DialogTest` (defaults, TwoWay `IsOpen`, `Closing` cancellation, event order Confirm→Closed); HeadlessTest: template apply + opens in overlay host.

**Risks & open questions**: ~~focus trapping~~ (partially covered — `OverlayPopupHost` sets `TabNavigation=Cycle`; a full focus trap stays open); `DialogService.ShowAsync` imperative wrapper = P1, decide host resolution (TopLevel from visual root).

**Resolved decisions (spec):**

- As spec'd: `IsOpen` (TwoWay) / `Title`+`TitleTemplate` / `Content`+`ContentTemplate` / `ConfirmButtonText` / `CancelButtonText` / `ButtonLayout` / `IsCloseButtonVisible` / `CloseOnOverlayClick` (default false, TDesign parity) / `IsModal`; `Show()`/`Close()`; surface hosted through `Popup` (Placement=Center) per the Picker `PART_Popup` pattern. Metrics from `dialog/_var.less`: width 311, radius `TDRadiusExtraLarge`, body max-height 456, title `TDFontSizeTitleLarge`/600, content `TDFontSizeBodyLarge`, close icon 22.
- **Deviation — events are CLR events, not routed events**: `Confirmed`/`Cancelled`/`Closed` (`EventHandler`) and `Closing` (`CancelEventArgs`) follow the existing `Popup`/`Picker` precedent; routed-event equivalents can wrap them later without breaking callers.
- **Deviation — `DestroyOnClose` dropped**: the popup host already detaches the surface from the visual tree on close and template content survives round-trips, so the property would be dead API. Revisit if a state-reset use case appears.
- **Deviation — vertical button order**: one `UniformGrid` (`Rows` flipped by the `:vertical` pseudo-class) keeps a single `PART_ConfirmButton`/`PART_CancelButton` pair; children stay cancel-then-confirm in both layouts, so vertical puts Cancel on top (TDesign vertical stacks Confirm first). Acceptable v1; revisit if TDesign fidelity matters.
- Buttons reuse the built-in `Button` theme (confirm = `Classes="Primary"`, close = `TextButton` + `tdicon:Close`); `TDDialogCancelForeground` from the token plan was dropped as unused. Escape closes when a cancel affordance exists (`CancelButtonText` or close button) and raises `Cancelled`.
- Center placement inherits the host's uniform `SafeAreaPadding` margin (§4.0 nuance) — kept: on notched phones the dialog stays clear of status-bar insets.
- Planned tests: unit (defaults, pseudo-class, template-less Show/Close) + headless (overlay hosting, confirm/cancel event flow, cancellable Closing, mask dismiss on/off).

### 4.2 Toast（轻提示）

`toast` · Path A · Phase P0 · Depends on: Popup audit (gap 1)

**TDesign behavior summary**: transient, minimal, auto-dismissing notice — imperative API (`toast({ message, theme })`), singleton by default (new toast replaces the old). Not for decisions (Dialog), not for stacked banners (Message).

**API mapping — service, not a control:**

| TDesign prop/event | Type / default | Mafia API | Type | Default | Notes |
|---|---|---|---|---|---|
| `message` | string \| TNode | `Show(message, options?)` parameter | string | required | object content via `ContentTemplate` on options |
| `theme` | loading/success/warning/error | `Toast.Success/Warning/Error/Loading(...)` + `ToastTheme` on options | enum | — | icon + tint per theme |
| `duration` | 2000 | `Duration` | TimeSpan | 2 s | `Timeout.InfiniteTimeSpan` = until `Close()` (loading pattern) |
| `placement` | 'middle' | `Placement` | `ToastPlacement` | Middle | Top/Middle/Bottom |
| `direction` | 'row' | `Direction` | `ToastDirection` | Row | icon beside/below text |
| `icon` | TNode | `Icon` / `IconTemplate` | — | theme default | overrides theme icon |
| `showOverlay` | false | `ShowOverlay` | bool | false | transparent mask blocks input when true |
| `preventScrollThrough` | false | — | — | — | n/a — non-modal popups pass background input through by design (§4.0) |
| `overlayProps` / attach / className / style | — | — | — | — | n/a |
| `onClose` | event | `Closed` callback on options | Action? | null | |
| singleton semantics | new replaces old | same — one active toast per TopLevel | | | queueing rejected: TDesign parity |

`Toast.Close()` dismisses early. Each `Show` targets the `TopLevel` hosting the anchor visual — signature `Show(Control anchor, string message, ToastOptions? options = null)` (+ convenience overloads).

**Variants & states**: theme icon tint (loading spinner inline in the theme — simple arc + rotation transition; the standalone `Loading` control stays P2), column layout, text-only (`IconVisible = false`).

**Mobile behavior**: M3 n/a (no dismiss affordance — duration owns it); M4 off by default; M2 n/a (inset placement, not edge).

**Token plan** (TDesign `toast/_var.less`):

| Token | Kind | Light target | Dark target |
|---|---|---|---|
| `TDToastMaxWidth` = 185 | metric | Shared | — |
| `TDToastRadius` ← `@radius-default` | metric | Shared | — |
| `TDToastRowIconSize` = 24 / `TDToastColumnIconSize` = 32 | metric | Shared | — |
| `TDToastForeground` ← `@text-color-anti` | color | `TDTextColorAnti` | mirror |
| `TDToastBackground` ← `@mask-active` | color | `TDMaskActive` | mirror (same source as popup mask — verify contrast in Light) |

**Files**: `Controls/Toast/Toast.cs` (+ `ToastOptions`, `ToastTheme`/`ToastPlacement`/`ToastDirection` — enums co-located, namespace stays `Irihi.Mafia.Controls`); theme `Controls/Toast.axaml`; token triple; demo `ToastPage`; `UnitTest/ToastTest` (options defaults, singleton replacement with a fake host, duration logic with controllable timer); HeadlessTest: shows in overlay layer, closes after duration.

**Risks & open questions**: ~~z-order vs Dialog~~ — resolved by design: toast hosts take z-index 10000+ (modal popups live at 1000+), so toasts always sit above open dialogs.

**Resolved decisions (spec):**

- As spec'd: `Show/Success/Warning/Error/Loading` + `Close` over `ToastOptions` (Duration 2 s / `Zero` = manual, Placement Middle, Direction Row, ShowOverlay false); singleton-per-TopLevel replacement (old toast hides immediately, new one fades in); presenter hosted directly on `OverlayPopupHost`, fade-only, Center/Top/Bottom placement. Metrics from `toast/_var.less`: max-width 185, radius `TDRadiusDefault`, row icon 24 / column icon 32, bg `TDMaskActive`, fg `TDTextColorAnti`.
- **Deviation — `ToastTheme` → `ToastStatus` / property `Status`**: `Theme` on any `StyledElement`-derived surface collides with `StyledElement.Theme` (CS0108, hit on the presenter). Aligns with ActionSheet §4.3 and Message §4.4; §4.2's naming was the outlier and is corrected here.
- **Deviation — `Close()` → `Close(Control anchor)`**: the singleton is per-TopLevel, so multi-window apps must name the window through any anchored control; a parameterless `Close()` would be ambiguous.
- **Deviation — no injected clock**: duration logic is covered by real-delay headless tests (`Task.Delay`) instead of a testable timer abstraction; revisit if suites get slow.
- **Deferred — `Icon`/`IconTemplate` custom-icon override**: v1 ships built-in theme icons only (stroke variants per `toast.tsx`: Loading/CheckCircle/ErrorCircle/CloseCircle); the spinner is a 1 s infinite `RotateTransform.Angle` style animation.
- ShowOverlay=true composes a modal host + `Brushes.Transparent` mask (blocks input, invisible) — exactly the §4.0 mask semantics; default passes background input through.
- Planned tests: unit (options defaults, no-root no-op, null guards) + headless (display/auto-close, status pseudo-classes, singleton replacement, loading lifetime, pass-through and blocking overlay).

### 4.3 ActionSheet（动作面板）

`action-sheet` · Path A · Phase P1 · Depends on: Popup audit (gaps 1–3)

**TDesign behavior summary**: bottom sheet listing immediate actions — list rows or icon grid, optional description, separated cancel. For choosing among 3–6 peer actions; for value capture use Picker, for confirmation use Dialog.

**API mapping:**

| TDesign prop/event | Type / default | Mafia API | Type | Default | Notes |
|---|---|---|---|---|---|
| `visible` | boolean | `IsOpen` | bool | false | TwoWay |
| `items` | Array\<string \| ActionSheetItem\> | `ItemsSource` | IEnumerable? | null | default template renders `ActionSheetItem` and plain strings |
| — (item shape) | `{label, color?, disabled?, icon?, suffixIcon?, badge?}` | `Controls/ActionSheet/ActionSheetItem.cs` | model | — | `Label`, `Foreground` (IBrush? — destructive red), `IsDisabled`, `Icon`, `Badge` v2 |
| `theme` | 'list' / 'grid' | `DisplayMode` | `ActionSheetDisplayMode` | List | **renamed**: `Theme` collides with ControlTheme semantics |
| `align` | 'left'? (verify in `action-sheet.md`) | `TextAlignment` | TextAlignment | verify | list rows only |
| `description` | string | `Description` | string? | null | header above items |
| `showCancel` / `cancelText` | true / '取消' | `IsCancelVisible` / `CancelText` | bool / string? | true / localized | |
| `showOverlay` | true | `IsModal` | bool | true | |
| `count` (grid paging) | 8 | — | — | — | v2 — grid paging with dots deferred |
| `popupProps` | object | — | — | — | host = `OverlayPopupHost` directly, bottom placement |
| `onSelected` | (item, index) | `Selected` | routed event | — | `ActionSheetSelectedEventArgs(Item, Index)` |
| `onCancel` / `onClose` | events | `Cancelled` / `Closed` | routed events | — | `Closed(trigger)` carries source |

**Variants & states**: list (icon? + label, disabled, destructive color), grid (icon above label), description header, cancel block separated by a gap strip.

**Mobile behavior**: M2 bottom — inset via host `SafeAreaPadding`; M3 outside-tap closes, cancel closes, no Escape affordance on touch (Escape closes on desktop); M4 on; M5 in stack.

**Token plan** (TDesign `action-sheet/_var.less`):

| Token | Kind | Light target | Dark target |
|---|---|---|---|
| `TDActionSheetRadius` ← `@radius-extraLarge` (top corners) | metric | Shared | — |
| `TDActionSheetItemHeight` (verify vs CSS) | metric | Shared | — |
| `TDActionSheetDescriptionFontSize` ← `@font-body-medium` | metric | Shared | — |
| `TDActionSheetGapHeight` (cancel separator) | metric | Shared | — |
| `TDActionSheetBackground` | color | `TDBgColorContainer` | mirror |
| `TDActionSheetItemForeground` | color | `TDTextColorPrimary` | mirror |
| `TDActionSheetItemForegroundDisabled` | color | `TDTextColorDisabled` | mirror |
| `TDActionSheetItemBackgroundPressed` | color | `TDBgColorContainerActive` | mirror |
| `TDActionSheetDestructiveForeground` | color | `TDErrorColor` | mirror (item `color` override) |
| `TDActionSheetStroke` | color | `TDComponentStroke` | mirror |
| `TDActionSheetGapBackground` | color | `TDBgColorPage` | mirror |

**Files**: `Controls/ActionSheet/{ActionSheet,ActionSheetItem}.cs` (+ enums); theme `Controls/ActionSheet.axaml` (list + grid templates); token triple; demo `ActionSheetPage` (list, grid, destructive, disabled, description); `UnitTest/ActionSheetTest` (selection args with index, cancel/close triggers, `IsOpen` TwoWay).

**Risks & open questions**: grid paging (`count` + dots) deferred — confirm with a v2 issue; `align` default needs verifying against `action-sheet.md`.

**Resolved decisions (spec):**

- As spec'd: `IsOpen` (TwoWay) / `ItemsSource` (strings + `ActionSheetItem{Label, Foreground, IsDisabled, Icon}`) / `DisplayMode` (List/Grid via `:grid`) / `Description` / `IsModal`; CLR events `Selected(ActionSheetSelectedEventArgs{Item, Index})`, `Cancelled`, `Closing` (cancellable), `Closed` — same event style as Dialog §4.1. Bottom sheet on `Popup` with light dismiss on (TDesign default), Escape closes when a cancel affordance exists, M2 bottom inset via host `SafeAreaPadding`. Metrics per `_var.less` + CSS: radius 12 top corners, item height 48, gap 8, item font `TDFontSizeBodyLarge`.
- **Deviation — `IsCancelVisible` dropped**: `CancelText == null` hides the cancel row, mirroring Dialog's `ConfirmButtonText` convention (one way to say hidden).
- **Verified — `align` default is `center`** (action-sheet.md); rows are center-aligned text. The `left` value is unsupported in v1 — no `TextAlignment` property (DataTemplate content can't reach the templated parent across the overlay subtree).
- **Deferred per plan — grid paging (`count` + dots)**; grid v1 = `UniformGrid` 4 columns, no paging. Item selection uses routed `Button.ClickEvent` on the overlay surface + nearest-`ItemsControl`/DataContext resolution (the overlay subtree never routes through the control itself).
- Planned tests: unit (defaults, pseudo-class, template-less Show/Close) + headless (bottom hosting, selection with index, disabled-item ignore, cancel flow, grid mode).

### 4.4 Message（全局消息）

`message` · Path A · Phase P1 · Depends on: —

**TDesign behavior summary**: themed banner (info/success/warning/error) with icon, optional link and close button, optional duration. Persistent notice, in-page or floating at top — unlike Toast it is content, can stay open, and stacks.

**TDesign behavior note**: in TDesign the component floats with a fixed z-index (15000) and offset; in Mafia v1 it ships as a plain control the host places (top of page, or in an overlay). A hosting helper (auto-stack + placement) is P2 with `NoticeBar`.

**API mapping:**

| TDesign prop/event | Type / default | Mafia API | Type | Default | Notes |
|---|---|---|---|---|---|
| `content` | string \| TNode | `Content` / `ContentTemplate` | object? / IDataTemplate? | null | |
| `theme` | info/success/warning/error | `Status` | `MessageStatus` | Info | **renamed** to `Status` per repo convention (explicit mobile concepts) |
| `align` | 'left' | `TextAlignment` | TextAlignment | Left | |
| `duration` | 3000, 0 = none | `Duration` | TimeSpan | 3 s | `Zero` = no timer; raises `DurationEnded` (does not auto-hide — host decides) |
| `closeBtn` | false | `IsCloseButtonVisible` | bool | false | |
| `icon` | true | `IsIconVisible` / `Icon` | bool / — | true | theme icon via `tdicon:` |
| `link` | string \| object \| TNode | `LinkText` | string? | null | + `LinkClick` routed event |
| `marquee` | false | `IsMarqueeEnabled` | bool | false | scrolling text when overflow; needs a text-scroll animation part |
| `gap` | 12 | — | — | — | container concern (v2 stacking host) |
| `offset` / `placement` / `visible` / z-index | — | — | — | — | hosting is the app's layout in v1 |
| `onClose` | event | `Closed` | routed event | — | via close button |
| `onDurationEnd` | event | `DurationEnded` | routed event | — | |

**Variants & states**: four statuses (icon + accent color band on the icon), marquee on/off, link suffix, close button.

**Mobile behavior**: M1 (≥48 px height, touch-safe close); no overlay rules — it is inline content.

**Token plan** (TDesign `message/_var.less`):

| Token | Kind | Light target | Dark target |
|---|---|---|---|
| `TDMessageMinHeight` = 48 | metric | Shared | — |
| `TDMessagePaddingH` = 16 / `TDMessagePaddingV` = 12 | metric | Shared | — |
| `TDMessageIconSize` = 22 | metric | Shared | — |
| `TDMessageFontSize` ← `@font-body-medium` | metric | Shared | — |
| `TDMessageRadius` ← `@radius-default` | metric | Shared | — |
| `TDMessageBackground` | color | `TDBgColorContainer` | mirror |
| `TDMessageForeground` | color | `TDTextColorPrimary` | mirror |
| `TDMessageInfoAccent` | color | `TDBrandColor` | mirror |
| `TDMessageSuccessAccent` | color | `TDSuccessColor` | mirror |
| `TDMessageWarningAccent` | color | `TDWarningColor` | mirror |
| `TDMessageErrorAccent` | color | `TDErrorColor` | mirror |
| `TDMessageShadow` | color (shadow) | verify `@shadow-4` equivalent | mirror |

**Files**: `Controls/Message/Message.cs` (+ `MessageStatus`); theme `Controls/Message.axaml`; token triple; demo `MessagePage` (four statuses, marquee, link, close); `UnitTest/MessageTest` (status pseudo-classes, duration timer, `DurationEnded`); HeadlessTest optional (marquee is visual — assert via state only).

**Risks & open questions**: marquee implementation (offset animation on a clipped TextBlock) — spike; `@shadow-4` mapping to an existing Mafia shadow token or a literal `BoxShadows` in Light/Dark (shadows are variant-dependent).

**Resolved decisions (spec):**

- As spec'd: `Status` (Info/Success/Warning/Error) / `Content`+`ContentTemplate` / `TextAlignment` (Left) / `Duration` (3 s, `Zero` = off, `DispatcherTimer` started on attach) / `IsCloseButtonVisible` / `IsIconVisible` / `LinkText`; CLR events `DurationEnded`, `Closed`, `LinkClick`. Metrics per `_var.less`: min-height 48, padding 16/12, icon 22, `TDFontSizeBodyMedium`, radius `TDRadiusDefault`; shadow maps `@shadow-4` → existing `TDShadow2`. Icons per `message.tsx` (InfoCircleFilled/CheckCircleFilled/ErrorCircleFilled — warning and error share the icon, differ by accent).
- **Implementation note:** status pseudo-classes must be initialized in the constructor — a default `Info` message would otherwise never get `:info`, so its icon would never show.
- **Deviation — `IsMarqueeEnabled` deferred entirely** (property not shipped): marquee needs a dynamic-distance scroll animation; it lands as a shared part with NoticeBar (P2), which also needs it.
- `Closed` does not hide the control — the host owns visibility (inline component, not an overlay); the demo shows the pattern.
- Planned tests: unit (defaults, pseudo-class changes) + headless (duration fires once, close button raises `Closed`, status swaps the visible icon).

## 5. Implementation order & dependencies | 实现顺序与依赖

- **P0-a Popup audit + non-modal pass-through fix** — findings recorded (§4.0); apply the fix before Dialog. Everything below renders through it.
- **P0-b Dialog** — spec'd (§4.1).
- **P0-c Toast** — spec'd (§4.2).
- **P1 ActionSheet, Message** — spec'd (§4.3, §4.4).
- **P2 NoticeBar, Loading** — Loading generalizes the Toast spinner into a control; NoticeBar reuses the Message marquee part.
- **P3 Popover → DropdownMenu** — anchor placement is new Popup capability; DropdownMenu stacks Popover + Cell.
- **P4 PullDownRefresh, SwipeCell, Guide** — gesture primitives and Popover-dependent; fill §4 rows first.

**Cross-category dependency:** `Picker`, `Calendar`, `CalendarDatePicker` (input category) consume `Popup`/`OverlayPopupHost`. The P0-a audit must keep their demos green; scroll lock must be opt-in enough not to freeze the Picker dropdown's host page.

## 6. Related | 相关文档

- Neighbors: forms.md (输入, not started) · navigation.md (导航, not started)
- [token-mapping.md](../token-mapping.md) · [mobile-ui-spec.md](../mobile-ui-spec.md) (M-rules) · [add-new-control.md](../add-new-control.md)
- TDesign sources: `reference/tdesign-mobile-vue/src/{popup,overlay,dialog,toast,action-sheet,message}/type.ts` and `_common/style/mobile/components/{dialog,toast,action-sheet,message}/_var.less`
