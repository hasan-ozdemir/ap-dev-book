---
title: MAUI Layouts & Containers
description: Detailed guidance for layouts & containers controls with .NET 9-ready XAML and C# samples.
last_reviewed: 2025-10-30
owners:
  - @prodyum/maui-guild
---

## 5. Layouts & Containers

### 5.1 StackLayout / VerticalStackLayout / HorizontalStackLayout

Stack layouts arrange children vertically or horizontally with spacing and alignment configuration; the modern `VerticalStackLayout` and `HorizontalStackLayout` offer performance benefits over `StackLayout`.

```xml
<VerticalStackLayout Spacing="12">
    <Label Text="Name" />
    <Entry Text="{Binding Name}" />
    <Label Text="Email" />
    <Entry Text="{Binding Email}" />
</VerticalStackLayout>
```

```csharp
var form = new VerticalStackLayout { Spacing = 12 };
form.Children.Add(new Label { Text = "Name" });
form.Children.Add(new Entry { Placeholder = "Name" });
```

**Tips**

- Use `HorizontalStackLayout` for button toolbars and filter chips.
- Combine with `Margin` on children for fine-grained spacing.

### 5.2 Grid

`Grid` arranges content in rows and columns with star/auto sizing, enabling complex responsive layouts.

```xml
<Grid ColumnDefinitions="Auto,*"
      RowDefinitions="Auto,Auto,*"
      ColumnSpacing="12"
      RowSpacing="16">
    <Image Grid.RowSpan="2" Source="avatar.png" WidthRequest="64" />
    <Label Grid.Column="1" Text="{Binding Name}" FontSize="18" FontAttributes="Bold" />
    <Label Grid.Row="1" Grid.Column="1" Text="{Binding Title}" FontSize="14" />
    <CollectionView Grid.Row="2" Grid.ColumnSpan="2" ItemsSource="{Binding Metrics}" />
</Grid>
```

```csharp
var grid = new Grid
{
    ColumnDefinitions = new ColumnDefinitionCollection
    {
        new ColumnDefinition(GridLength.Auto),
        new ColumnDefinition(GridLength.Star)
    },
    RowDefinitions = new RowDefinitionCollection
    {
        new RowDefinition(GridLength.Auto),
        new RowDefinition(GridLength.Auto),
        new RowDefinition(GridLength.Star)
    }
};
```

**Tips**

- Combine with adaptive triggers to reconfigure layouts at breakpoints.
- Use `RowDefinition.Height` animations for collapsible sections.

### 5.3 FlexLayout

`FlexLayout` adapts to different screen sizes using wrapping, justification, and alignment options similar to CSS Flexbox.

```xml
<FlexLayout Direction="Row"
           Wrap="Wrap"
           JustifyContent="SpaceBetween">
    <Frame WidthRequest="150" HeightRequest="120" />
    <Frame WidthRequest="150" HeightRequest="120" />
    <Frame WidthRequest="150" HeightRequest="120" />
</FlexLayout>
```

```csharp
var flex = new FlexLayout
{
    Direction = FlexDirection.Row,
    Wrap = FlexWrap.Wrap,
    JustifyContent = FlexJustify.SpaceBetween
};
flex.Children.Add(new Frame { WidthRequest = 150, HeightRequest = 120 });
```

**Tips**

- Use for responsive card galleries and chip groups.
- Combine with `FlexLayout.AlignSelf` for per-item overrides.

### 5.4 AbsoluteLayout

`AbsoluteLayout` positions children using proportional or absolute coordinates, suitable for overlays and complex positioning.

```xml
<AbsoluteLayout>
    <Image Source="map.png"
           AbsoluteLayout.LayoutBounds="0,0,1,1"
           AbsoluteLayout.LayoutFlags="All" />
    <Frame AbsoluteLayout.LayoutBounds="0.5,0.3,120,60"
           AbsoluteLayout.LayoutFlags="PositionProportional">
        <Label Text="HQ" HorizontalTextAlignment="Center" />
    </Frame>
</AbsoluteLayout>
```

```csharp
var abs = new AbsoluteLayout();
var map = new Image { Source = "map.png" };
AbsoluteLayout.SetLayoutBounds(map, new Rect(0, 0, 1, 1));
AbsoluteLayout.SetLayoutFlags(map, AbsoluteLayoutFlags.All);
abs.Children.Add(map);
```

**Tips**

- Combine proportion flags for responsive overlays across screen sizes.
- Use for drag-and-drop surfaces with `PanGestureRecognizer`.

### 5.5 ScrollView

`ScrollView` enables scrollable content, hosting a single child layout that expands beyond viewport boundaries.

```xml
<ScrollView>
    <VerticalStackLayout Padding="16" Spacing="24">
        <!-- long form content -->
    </VerticalStackLayout>
</ScrollView>
```

```csharp
var scroll = new ScrollView
{
    Content = new VerticalStackLayout { Padding = 16, Spacing = 24 }
};
```

**Tips**

- Use `ScrollToAsync` to guide users to validation errors.
- Avoid nested scroll views to prevent gesture conflicts.

---

## Further reading

- [Microsoft Learn: Layouts overview](https://learn.microsoft.com/en-us/dotnet/maui/user-interface/layouts/index)citeturn16search0
- [Microsoft Learn: Grid](https://learn.microsoft.com/en-us/dotnet/maui/user-interface/layouts/grid)citeturn16search1
- [Microsoft Learn: FlexLayout](https://learn.microsoft.com/en-us/dotnet/maui/user-interface/layouts/flexlayout)citeturn16search2

