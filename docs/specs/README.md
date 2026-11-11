# Component Specs

Pre-filled, **category-level** specification sheets written **before** implementation. A spec answers "what to build and why"; [add-new-control.md](../add-new-control.md) answers "which files to touch"; [control-index.md](../control-index.md) records what landed.

**规范先行：** 每个类别一份规格书，实现前写好，实现后回填。规格书回答"做什么"，add-new-control 回答"改哪些文件"，control-index 记录"已落地"。

## Why spec-first | 为什么规格先行

Filling the spec up front removes per-control document lookups during implementation: the API mapping table, mobile rules, token plan, and file targets are all decided once per category, so controls in the same wave can be implemented in parallel against a stable contract.

## The loop | 工作循环

1. **Fill** `specs/<category>.md` from the TDesign Mobile Vue sources (see [spec-template.md](spec-template.md) for inputs and the skeleton).
2. **Implement** component by component with [add-new-control.md](../add-new-control.md) as the file checklist. Keep to the spec's API table; deviations require editing the spec in the same change.
3. **Backfill** the component's *Implementation notes (landed)* subsection with what actually shipped (property names that differ, tokens added, gotchas hit).
4. **Sync** the category registry table and [control-index.md](../control-index.md).

## Status | 规格状态

| Category spec | TDesign group | Components | State |
|---|---|---|---|
| [feedback.md](feedback.md) | 反馈 | Popup, Overlay, Dialog, Toast, ActionSheet, Message, NoticeBar, Loading, Popover, DropdownMenu, PullDownRefresh, SwipeCell, Guide | Pilot — §4 detailed for the P0–P1 wave |
| forms.md *(not created yet)* | 输入 | Input ✅, Picker ✅, Calendar ✅, Checkbox ✅, … | Not started |
| navigation.md *(not created yet)* | 导航 | NavBar, Tabs, TabBar, Drawer, … | Not started |
| display.md *(not created yet)* | 数据展示 | Cell ✅, Avatar ✅, Progress ✅, … | Not started |
| basics.md *(not created yet)* | 基础 | Button ✅, Divider ✅, Icon, Fab, … | Not started |

✅ = control already exists in the repo (spec would start from a gap audit, like Popup in feedback.md).

## Deviation policy | 偏离政策

When the Avalonia idiom forces a rename (e.g. TDesign's `theme="list|grid"` collides with ControlTheme semantics), the spec records the new name **and the reason** in the mapping table. Never silently diverge from the spec, and never change a shipped property without updating its spec row.
