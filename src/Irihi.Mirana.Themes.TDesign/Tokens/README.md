# TDesign Design Tokens

This directory contains Avalonia ResourceDictionary files converted from TDesign's Design Token system.

## Structure

### Palette
Contains color theme definitions for Light and Dark modes:
- `Light.axaml` - Light theme color palette
- `Dark.axaml` - Dark theme color palette

Each palette includes:
- Brand colors (10 shades)
- Warning colors (10 shades)
- Error colors (10 shades)
- Success colors (10 shades)
- Gray colors (14 shades)
- Font colors with opacity
- Background colors
- Text colors
- Border colors
- Shadow definitions
- Scrollbar colors

### Font
Typography-related tokens:
- `Typography.axaml` - Font families, sizes, and line heights

Includes font definitions for:
- Link (small, medium, large)
- Body text (small, medium, large)
- Titles (small, medium, large, extra large)
- Headlines (small, medium, large)
- Display text (medium, large)

### Radius
Border radius definitions:
- `CornerRadius.axaml` - Corner radius values

Includes:
- Small (2px)
- Default (3px)
- Medium (6px)
- Large (9px)
- Extra Large (12px)
- Round (999px)

### Size
Size and spacing tokens:
- `Spacing.axaml` - Base sizes, component sizes, padding, and margins

Includes:
- Base sizes (1-16)
- Component sizes (height values from XXXS to XXXXXL)
- Popup padding (S to XXL)
- Component horizontal padding (XXS to XXL)
- Component vertical padding (XXS to XXL)
- Component margins (XXS to XXXXL)

## Usage

These tokens are automatically included in the MiranaTheme and can be referenced using `{StaticResource}` or `{DynamicResource}` markup extensions in your XAML files.

Example:
```xaml
<Border Background="{DynamicResource TDBgColorContainer}"
        CornerRadius="{StaticResource TDRadiusMedium}"
        Padding="{StaticResource TDCompPaddingLRM}">
    <TextBlock Text="Hello World"
               Foreground="{DynamicResource TDTextColorPrimary}"
               FontSize="{StaticResource TDFontSizeBodyMedium}" />
</Border>
```

## Source

These tokens were converted from TDesign's Design Token system:
https://github.com/Tencent/tdesign-common/tree/develop/style/web/theme
