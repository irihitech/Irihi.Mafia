# Category Spec Template

Skeleton for `docs/specs/<category>.md`. Copy sections 1–6, fill every table cell, and delete component subsections you are not detailing yet (keep their registry rows). Write specs in English, matching the rest of `docs/`.

## Inputs | 输入来源

| Input | Where |
|---|---|
| Category membership + component list | `reference/tdesign-mobile-vue/src/components.ts` (comment headers 基础/导航/输入/数据展示/反馈) |
| Per-component API (authoritative) | `reference/tdesign-mobile-vue/src/<component>/type.ts` — `Td<Component>Props` |
| Behavior & demos | `reference/tdesign-mobile-vue/src/<component>/<component>.md` + `demos/` |
| CSS tokens / default metrics | `reference/tdesign-mobile-vue/src/<component>/style/` + `_common/style` |
| Online docs when local checkout is stale | tdesignMobileVue MCP (`npx tdesign-mcp-server`, framework = mobile-vue) |
| Mafia conventions | [add-new-control.md](../add-new-control.md), [token-mapping.md](../token-mapping.md), existing controls listed in [control-index.md](../control-index.md) |

## M-rules — mobile behavior IDs

Stable IDs for referencing [mobile-ui-spec.md](../mobile-ui-spec.md) compactly from specs. Keep this table in sync when mobile-ui-spec changes.

| ID | Rule | Source |
|---|---|---|
| M1 | Touch targets & pressed feedback; hover never required for usability | §2 |
| M2 | Safe-area insets on edge-anchored surfaces — spec must name the edges (e.g. "M2 bottom") | §3 |
| M3 | Overlay dismiss contract — every overlay spec states: outside-tap closes? escape/back closes? close button? | §4 |
| M4 | Background scroll lock while a modal overlay is open | §4.4 |
| M5 | Overlay z-order: single overlay stack per TopLevel, later-opened on top | §4.5 |
| M6 | Gesture axis ownership for draggable/swipeable components | §5 |
| M7 | Virtual keyboard avoidance for input-bearing overlays | §6 |
| M8 | Phone-scale density; shared size tokens over per-component literals | §7 |

---

## Skeleton | 骨架

### 1. Category positioning | 大类定位

- What problem space the category covers (one paragraph).
- **Boundaries**: which nearby TDesign components belong to *other* categories and why (this prevents scope drift between specs).
- What infra the category shares (primitives, host surfaces).

### 2. Component registry | 组件一览

One row per TDesign component in the category. **Write this table before any implementation** — it is the category's backlog.

| Component | TDesign | Mafia class | Path | Phase | Depends on | M-rules | Status |
|---|---|---|---|---|---|---|---|
| `Dialog` | `dialog` | `Controls/Dialog/Dialog.cs` | A | P0 | Popup infra | M2, M3, M4, M5 | spec'd |

- **Path**: A (new custom control) / B (theme an Avalonia built-in) / C (page shell) — same paths as add-new-control.md.
- **Phase**: P0 = foundation + highest value first; higher numbers later. Dependencies must point to equal-or-lower phases.
- **Status**: `spec'd` / `landed` / `blocked (<reason>)` / `—`.

### 3. Shared conventions | 共享约定

Decided **once** for the category, not per component: hosting surface (e.g. everything modal goes through `OverlayPopupHost`), visibility property name, animation timings, z-order policy, shared tokens, class naming.

### 4. Components | 组件规格

Repeat this subsection per component being detailed. Undetailed components keep only their registry row.

```markdown
### 4.x <Component>（<TDesign name>）
`<tdesign slug>` · Path <A/B/C> · Phase <Pn> · Depends on: <...>

**TDesign behavior summary**: what it does, when to use it, when *not* to.

**API mapping**:
| TDesign prop/event | Type / default | Mafia API | Type | Default | Notes |
|---|---|---|---|---|---|
| `visible` | boolean | `IsOpen` | bool | false | TwoWay; deviations named & justified here |

Rows dropped or deferred get `—` with a reason in Notes. Events map to routed events with
strongly-typed args (`SelectedEventArgs.Item`, not sender sniffing).

**Variants & states**: leading-uppercase `Classes`, sizes, statuses; pressed/disabled/loading treatment.

**Mobile behavior**: cite M-rules with the concrete decision, e.g. "M3: outside-tap closes, Escape closes, no close button".

**Token plan**:
| Token | Kind | Light target | Dark target |
|---|---|---|---|
| `TDDialogBackground` | color | `TDBgColorContainer` | (mirror) |

Metrics (sizes/paddings/radii) are `Kind: metric`, live in `Themes/Shared/`, and cite the TDesign
CSS value they translate. Light/Dark rows must define identical key sets.

**Files**: the concrete 7-touchpoint list (control / ControlTheme / token triple / demo 4 / tests),
noting which steps a Path B component skips.

**Risks & open questions**: naming collisions, framework gaps, anything needing a spike.

**Implementation notes (landed)**: *backfilled after implementation — actual property names,
tokens, deviations from this spec, gotchas.*
```

### 5. Implementation order & dependencies | 实现顺序与依赖

- Wave list (P0 → Pn) with one line each on why this order.
- **Cross-category dependencies**: other categories consuming the same infra (e.g. Picker uses Popup) — changes must not regress them.

### 6. Related | 相关文档

Links to neighboring category specs, token-mapping.md, mobile-ui-spec.md, and the TDesign sources used.
