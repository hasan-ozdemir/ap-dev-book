---
title: MAUI Shapes & Drawing
description: Detailed guidance for shapes & drawing controls with .NET 9-ready XAML and C# samples.
last_reviewed: 2025-10-30
owners:
  - @prodyum/maui-guild
---

## 7. Shapes & Drawing

### 7.1 Ellipse

`Ellipse` draws circles or ovals using width/height sizing and supports fills, strokes, and gradients.

```xml
<Ellipse WidthRequest="72"
         HeightRequest="72"
         Fill="{StaticResource AccentBrush}"
         Stroke="White"
         StrokeThickness="2" />
```

```csharp
var ellipse = new Ellipse
{
    WidthRequest = 72,
    HeightRequest = 72,
    Fill = (Brush)Application.Current.Resources["AccentBrush"],
    Stroke = new SolidColorBrush(Colors.White),
    StrokeThickness = 2
};
```

**Tips**

- Use `RadialGradientBrush` for badges and notification indicators.
- Combine with `Shadow` for floating action buttons.

### 7.2 Rectangle

`Rectangle` renders straight-sided shapes with optional corner radius, ideal for progress bars or placeholders.

```xml
<Rectangle WidthRequest="160"
           HeightRequest="8"
           RadiusX="4"
           RadiusY="4"
           Fill="#334A90E2" />
```

```csharp
var bar = new Rectangle
{
    WidthRequest = 160,
    HeightRequest = 8,
    RadiusX = 4,
    RadiusY = 4,
    Fill = new SolidColorBrush(Color.FromArgb("#334A90E2"))
};
```

**Tips**

- Animate `ScaleX` to create lightweight progress indicators.
- Overlay with `Clip` regions to produce skeleton loaders.

### 7.3 Line

`Line` draws straight segments and is useful for separators, spark lines, or chart axes.

```xml
<Line X1="0"
      Y1="0"
      X2="120"
      Y2="0"
      Stroke="Silver"
      StrokeThickness="1.5" />
```

```csharp
var line = new Line
{
    X1 = 0,
    Y1 = 0,
    X2 = 120,
    Y2 = 0,
    Stroke = new SolidColorBrush(Colors.Silver),
    StrokeThickness = 1.5
};
```

**Tips**

- Bind `Stroke` to theme resources to keep separators consistent.
- Combine with `RotateTransform` for vertical dividers.

### 7.4 Path

`Path` draws custom vector shapes via geometry data, supporting complex icons and logos.

```xml
<Path Data="M10,50 L50,10 90,50 50,90Z"
      Stroke="DodgerBlue"
      StrokeThickness="3"
      Fill="#224A90E2" />
```

```csharp
var path = new Path
{
    Data = Geometry.Parse("M10,50 L50,10 90,50 50,90Z"),
    Stroke = new SolidColorBrush(Colors.DodgerBlue),
    StrokeThickness = 3,
    Fill = new SolidColorBrush(Color.FromArgb("#224A90E2"))
};
```

**Tips**

- Combine with `PathGeometry` or `StreamGeometry` for reusable logos.
- Use `PathButton` (derived from `Path`) when the shape needs hit testing.

### 7.5 Polygon

`Polygon` renders closed shapes defined by a set of points, supporting strokes, fills, and gradient brushes.

```xml
<Polygon Points="40,10 90,80 10,80"
         Fill="#336400FF"
         Stroke="#6400FF"
         StrokeThickness="2" />
```

```csharp
var polygon = new Polygon
{
    Points = new PointCollection { new Point(40, 10), new Point(90, 80), new Point(10, 80) },
    Fill = new SolidColorBrush(Color.FromArgb("#336400FF")),
    Stroke = new SolidColorBrush(Color.FromArgb("#6400FF")),
    StrokeThickness = 2
};
```

**Tips**

- Use for badges or custom pointers; animate `RotateTransform` for pulsing highlights.
- Combine with `SkiaSharp`-rendered backgrounds for layered visuals.

### 7.6 Polyline

`Polyline` connects a series of points without automatically closing the shape, making it ideal for charts or routes.

```xml
<Polyline Points="0,60 20,40 60,30 100,10"
          Stroke="DeepSkyBlue"
          StrokeThickness="3"
          StrokeLineJoin="Round" />
```

```csharp
var polyline = new Polyline
{
    Points = new PointCollection
    {
        new Point(0,60),
        new Point(20,40),
        new Point(60,30),
        new Point(100,10)
    },
    Stroke = new SolidColorBrush(Colors.DeepSkyBlue),
    StrokeThickness = 3,
    StrokeLineJoin = PenLineJoin.Round
};
```

**Tips**

- Combine with `GradientBrush` for trend lines that change color over time.
- Overlay on maps to visualize turn-by-turn routes or heatmaps.

---

## Further reading

- [Microsoft Learn: Shapes](https://learn.microsoft.com/en-us/dotnet/maui/user-interface/controls/shapes)citeturn17search3

