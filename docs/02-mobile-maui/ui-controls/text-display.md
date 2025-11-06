---
title: MAUI Text & Display
description: Detailed guidance for text & display controls with .NET 9-ready XAML and C# samples.
last_reviewed: 2025-10-30
owners:
  - @prodyum/maui-guild
---

## 1. Text & Display

### 1.1 Label

Labels render read-only text, support inline formatting with `FormattedString`, and expose semantic properties for accessibility.

```xml
<Label Text="Welcome to Contoso CRM"
       FontSize="24"
       SemanticProperties.Description="Welcome heading" />
```

```csharp
var title = new Label
{
    Text = "Welcome to Contoso CRM",
    FontSize = 24
};
SemanticProperties.SetDescription(title, "Welcome heading");
```

**Tips**

- Combine `LineBreakMode="WordWrap"` with `MaxLines` to control truncation.
- Use `FormattedString` for inline bold/italic spans without extra controls.

### 1.2 Image

`Image` displays bitmap content from embedded resources, files, or web URIs, automatically handling density-specific assets via platform image sources.

```xml
<Image Source="dotnet_bot.png"
       Aspect="AspectFit"
       HeightRequest="120" />
```

```csharp
var hero = new Image
{
    Source = "dotnet_bot.png",
    Aspect = Aspect.AspectFit,
    HeightRequest = 120
};
```

**Tips**

- Pre-load imagery with `ImageSource.FromFile` during app startup for splash-like transitions.
- Pair with `SemanticProperties.Description` to describe visuals for screen readers.

### 1.3 ImageButton

`ImageButton` merges an image surface with button semantics, supporting hit-test alignment and keyboard focus for accessibility.

```xml
<ImageButton Source="icons/edit.png"
             BackgroundColor="Transparent"
             Clicked="OnEditClicked" />
```

```csharp
var editButton = new ImageButton
{
    Source = ImageSource.FromFile("icons/edit.png"),
    BackgroundColor = Colors.Transparent
};
editButton.Clicked += OnEditClicked;
```

**Tips**

- Set `Padding` for larger touch targets on mobile.
- Combine with `Command` bindings to keep view models platform-agnostic.

### 1.4 BoxView

`BoxView` renders simple rectangles with configurable color, corner radius (via handlers), and transforms—ideal for separators, status chips, or skeleton loaders.

```xml
<BoxView Color="{StaticResource AccentColor}"
         HeightRequest="4"
         HorizontalOptions="Fill" />
```

```csharp
var divider = new BoxView
{
    Color = Application.Current.Resources["AccentColor"] as Color ?? Colors.Purple,
    HeightRequest = 4,
    HorizontalOptions = LayoutOptions.Fill
};
```

**Tips**

- Animate `TranslationX` to highlight active tabs.
- Use `WidthRequest` and `HeightRequest` to create circular dots for step indicators.

### 1.5 Border

`Border` wraps a single child with customizable stroke, corner radius, background, and shadow, superseding many Frame scenarios.

```xml
<Border Stroke="{StaticResource AccentColor}"
        StrokeThickness="2"
        BackgroundColor="{StaticResource SurfaceColor}"
        Padding="12"
        StrokeShape="RoundRectangle 12">
    <Label Text="Upgrade available"
           FontAttributes="Bold" />
</Border>
```

```csharp
var banner = new Border
{
    Stroke = new SolidColorBrush(Colors.MediumPurple),
    StrokeThickness = 2,
    Background = new SolidColorBrush(Colors.White),
    Padding = 12,
    StrokeShape = new RoundRectangle { CornerRadius = 12 },
    Content = new Label { Text = "Upgrade available", FontAttributes = FontAttributes.Bold }
};
```

**Tips**

- Combine with `VisualStateManager` to shift stroke color for focus/hover states on desktop.
- Use `StrokeDashArray` to create striped outlines for warnings.

### 1.6 Frame

`Frame` provides a content container with default shadow on mobile, optional corner rounding, and gesture support through `GestureRecognizers`.

```xml
<Frame HasShadow="True" CornerRadius="16" Padding="16">
    <Label Text="Monthly revenue $42K"
           FontSize="18"
           FontAttributes="Bold" />
</Frame>
```

```csharp
var card = new Frame
{
    HasShadow = true,
    CornerRadius = 16,
    Padding = 16,
    Content = new Label
    {
        Text = "Monthly revenue $42K",
        FontSize = 18,
        FontAttributes = FontAttributes.Bold
    }
};
```

**Tips**

- Disable shadows on Android for better performance in long lists.
- Wrap interactive content (buttons, toggles) to create card-like controls.

### 1.7 ContentView

`ContentView` is a lightweight container for composing reusable UI fragments, typically paired with XAML control templates and view models.

```xml
<ContentView x:Name="ProfileBadge">
    <StackLayout Orientation="Horizontal" Spacing="8">
        <Image Source="{Binding Avatar}" HeightRequest="40" WidthRequest="40" />
        <Label Text="{Binding FullName}" VerticalTextAlignment="Center" />
    </StackLayout>
</ContentView>
```

```csharp
var badge = new ContentView
{
    Content = new HorizontalStackLayout
    {
        Spacing = 8,
        Children =
        {
            new Image { Source = "avatar.png", HeightRequest = 40, WidthRequest = 40 },
            new Label { Text = "Taylor Franklin", VerticalTextAlignment = TextAlignment.Center }
        }
    }
};
```

**Tips**

- Convert complex `ContentView`s into custom controls by adding bindable properties.
- Use partial classes to inject dependency-injected services (e.g., theming) into the view.

### 1.8 GraphicsView

`GraphicsView` exposes a drawing canvas for high-performance rendering with `IDrawable` implementations, ideal for charts or custom shapes.

```xml
<GraphicsView HeightRequest="160" WidthRequest="320">
    <GraphicsView.Drawable>
        <local:SparkLineDrawable />
    </GraphicsView.Drawable>
</GraphicsView>
```

```csharp
var chart = new GraphicsView
{
    HeightRequest = 160,
    WidthRequest = 320,
    Drawable = new SparkLineDrawable()
};
```

**Tips**

- Implement `IDrawable.Draw` to render with `ICanvas` primitives; reuse across platforms without platform-specific renderers.
- Throttle `Invalidate` calls via `Device.StartTimer` to avoid excessive redraws.

---
### 1.9 Button

`Button` triggers commands or events and supports text or image content, command bindings, and platform-optimized visual states.

```xml
<Button Text="Save"
        Command="{Binding SaveCommand}"
        SemanticProperties.Description="Save form" />
```

```csharp
var saveButton = new Button
{
    Text = "Save"
};
saveButton.SetBinding(Button.CommandProperty, "SaveCommand");
SemanticProperties.SetDescription(saveButton, "Save form");
saveButton.Clicked += OnSaveClicked;
```

**Tips**

- Prefer `Command` bindings to keep click logic in view models.
- Combine `ContentLayout` with `ImageSource` for icon-plus-text buttons.

---

## Further reading

- [Microsoft Learn: .NET MAUI controls overview](https://learn.microsoft.com/en-us/dotnet/maui/user-interface/controls/)citeturn10search0
- [Microsoft Learn: Label](https://learn.microsoft.com/en-us/dotnet/maui/user-interface/controls/label)citeturn0search3
- [Microsoft Learn: GraphicsView](https://learn.microsoft.com/en-us/dotnet/maui/user-interface/controls/graphicsview)citeturn11search0



