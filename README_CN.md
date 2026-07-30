# Irihi.Mafia

**移动端优先的 Avalonia UI 控件库与 TDesign 风格主题。**

Irihi.Mafia 是一款**移动端优先**的 Avalonia 控件库 — 从底层开始即为触摸交互优化，提供拇指友好（thumb-friendly）的尺寸和响应式布局。同时提供基于 [TDesign](https://tdesign.tencent.com/) 设计语言的完整视觉主题，支持明暗模式。

## 特性

- 📱 **移动端优先设计** — 触摸优化的控件、充足的点击区域、拇指友好的间距
- 🎨 **TDesign 风格主题** — 包含设计令牌系统的完整明暗主题（色彩、排版、间距、圆角）
- 🧩 **自定义控件** — Avatar、Cell/ CellGroup、Divider、IconButton、Picker、Popup 等
- 🌓 **明暗模式** — 内置明暗双主题，统一配色体系
- 📦 **模块化包** — 核心逻辑库与主题库分离为独立 NuGet 包
- 🖥️ **跨平台** — 通过 Avalonia 支持 Windows、macOS、Linux、Android、iOS 和浏览器

## 包

| 包名 | 说明 | NuGet |
|---------|-------------|-------|
| `Irihi.Mafia` | 核心控件库 — 提供自定义控件逻辑与基元 | [![NuGet](https://img.shields.io/nuget/v/Irihi.Mafia)](https://www.nuget.org/packages/Irihi.Mafia) |
| `Irihi.Mafia.Themes.TDesign` | TDesign 风格视觉主题 — 控件模板、设计令牌、明暗配色 | [![NuGet](https://img.shields.io/nuget/v/Irihi.Mafia.Themes.TDesign)](https://www.nuget.org/packages/Irihi.Mafia.Themes.TDesign) |

## 快速开始

### 1. 安装包

```bash
dotnet add package Irihi.Mafia
dotnet add package Irihi.Mafia.Themes.TDesign
```

> 如果你打算编写自己的主题，只需 `Irihi.Mafia`；大多数项目建议两个包都安装。

### 2. 应用主题

在 `App.axaml` 中添加 TDesign 主题：

```xml
<Application xmlns="https://github.com/avaloniaui"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             xmlns:td="https://irihi.tech/td"
             x:Class="MyApp.App">
    <Application.Styles>
        <td:TDesignTheme />
    </Application.Styles>
</Application>
```

### 3. 使用控件

```xml
xmlns:m="https://irihi.tech/mafia"

<!-- IconButton -->
<m:IconButton Icon="emoji_smile"
              Shape="Circle"
              Variant="Outline" />

<!-- Cell -->
<m:Cell Description="用户设置"
        Note="管理你的偏好"
        IsRequired="True" />

<!-- Avatar -->
<m:Avatar Width="40" Height="40"
          Source="{Binding ProfileImage}" />
```

## 控件一览

| 控件 | 说明 |
|---------|-------------|
| `Avatar` | 带图片源的按钮，适用于用户头像 |
| `Cell` | 带描述、备注和左右内容插槽的列表项 |
| `CellGroup` | 将 `Cell` 项分组显示，带分组标题 |
| `Divider` | 水平或垂直分隔线 |
| `IconButton` | 带图标、加载状态、形状和变体选项的按钮 |
| `Picker` | 带确认模式和搜索功能的下拉选择控件 |
| `Popup` | 灵活的弹出层，支持模态遮罩、位置和动画 |

## 项目结构

```
src/
├── Irihi.Mafia/                      # 核心控件库
│   ├── Controls/                     # 自定义控件实现
│   │   ├── Avatar/
│   │   ├── Cell/
│   │   ├── Divider.cs
│   │   ├── IconButton.cs
│   │   ├── Picker.cs
│   │   └── Primitives/               # Popup、OverlayPopupHost
│   └── Common/                       # 共享枚举（Position、PopupPlacement）
│
└── Irihi.Mafia.Themes.TDesign/       # TDesign 风格视觉库
    ├── TDesignTheme.axaml             # 主题入口
    ├── Controls/                      # 24 个 ControlTheme 定义
    ├── Tokens/                        # 设计令牌（颜色、字体、圆角、尺寸）
    ├── Themes/                        # 明暗变体资源字典
    └── Styles/                        # 全局样式选择器

demo/
├── Irihi.Mafia.Demo/                 # 共享演示应用
├── Irihi.Mafia.Demo.Desktop          # Windows/macOS/Linux 启动器
├── Irihi.Mafia.Demo.Browser          # WebAssembly 启动器
├── Irihi.Mafia.Demo.Android          # Android 启动器
└── Irihi.Mafia.Demo.iOS              # iOS 启动器
```

## 主题定制

TDesign 主题采用分层令牌体系，定制十分直观：

1. **设计令牌**（`Tokens/`）— 颜色、字体、圆角、间距的原子值
2. **共享尺寸**（`Themes/Shared/`）— 组件级别的尺寸和内边距引用
3. **主题变体**（`Themes/Light/`、`Themes/Dark/`）— 明暗模式的颜色映射
4. **控件模板**（`Controls/`）— 每个控件的视觉树和绑定

从应用中覆盖任意 `DynamicResource` 键即可定制：

```xml
<Application.Resources>
    <SolidColorBrush x:Key="TDBrandColor" Color="#FF5722" />
</Application.Resources>
```

## 从源码构建

```bash
# 克隆仓库
git clone https://github.com/irihi/irihi-mafia.git
cd irihi-mafia

# 构建
dotnet build

# 运行测试
dotnet test

# 启动桌面演示
dotnet run --project demo/Irihi.Mafia.Demo.Desktop
```

## 贡献

欢迎提交 Issue 或 Pull Request！

## 许可证

本项目基于 [MIT License](LICENSE) 许可证。

Copyright (c) 2026 IRIHI Technology
