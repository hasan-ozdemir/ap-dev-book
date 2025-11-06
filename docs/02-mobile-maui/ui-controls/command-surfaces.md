---
title: MAUI Command Surfaces
description: Detailed guidance for command surfaces controls with .NET 9-ready XAML and C# samples.
last_reviewed: 2025-10-30
owners:
  - @prodyum/maui-guild
---

## 8. Command Surfaces

### 8.1 ToolbarItem

`ToolbarItem` adds actions to the top app bar on mobile and desktop. Items can bind to commands and support icons on platforms that allow them.

```xml
<ContentPage.ToolbarItems>
    <ToolbarItem Text="Add"
                 IconImageSource="icon_add.png"
                 Command="{Binding AddCommand}" />
</ContentPage.ToolbarItems>
```

```csharp
var page = new ContentPage();
var add = new ToolbarItem
{
    Text = "Add",
    IconImageSource = "icon_add.png"
};
add.SetBinding(MenuItem.CommandProperty, "AddCommand");
page.ToolbarItems.Add(add);
```

**Tips**

- Use `Order="Secondary"` to place items in the overflow on Android.
- When using Shell, define toolbar items inside `ShellContent` for route-specific actions.

### 8.2 MenuBarItem

`MenuBarItem` provides desktop-style menus on Windows and macOS, integrating with global menu bars.

```xml
<MenuBarItem Text="File">
    <MenuFlyoutItem Text="New"
                    Command="{Binding NewCommand}" />
    <MenuFlyoutSubItem Text="Export">
        <MenuFlyoutItem Text="PDF" Command="{Binding ExportPdfCommand}" />
        <MenuFlyoutItem Text="CSV" Command="{Binding ExportCsvCommand}" />
    </MenuFlyoutSubItem>
</MenuBarItem>
```

```csharp
var fileMenu = new MenuBarItem { Text = "File" };
fileMenu.Items.Add(new MenuFlyoutItem { Text = "New", Command = viewModel.NewCommand });
var export = new MenuFlyoutSubItem { Text = "Export" };
export.Items.Add(new MenuFlyoutItem { Text = "PDF", Command = viewModel.ExportPdfCommand });
export.Items.Add(new MenuFlyoutItem { Text = "CSV", Command = viewModel.ExportCsvCommand });
fileMenu.Items.Add(export);
```

**Tips**

- Combine with `MenuBar` inside `AppShell` for cross-platform desktop experiences.
- Use keyboard accelerators via `MenuFlyoutItem.KeyboardAccelerators` on Windows for productivity shortcuts.

---

## Further reading

- [Microsoft Learn: ToolbarItem](https://learn.microsoft.com/en-us/dotnet/maui/user-interface/toolbaritem)citeturn17search4
- [Microsoft Learn: MenuBarItem](https://learn.microsoft.com/en-us/dotnet/maui/user-interface/menu-bar)citeturn17search5

