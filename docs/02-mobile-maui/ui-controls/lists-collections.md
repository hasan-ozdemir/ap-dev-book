---
title: MAUI Lists & Collections
description: Detailed guidance for lists & collections controls with .NET 9-ready XAML and C# samples.
last_reviewed: 2025-10-30
owners:
  - @prodyum/maui-guild
---

## 3. Lists & Collections

### 3.1 CollectionView

`CollectionView` efficiently renders lists and grids with virtualization, templating, and selection modes.

```xml
<CollectionView ItemsSource="{Binding Orders}"
                SelectionMode="Single"
                SelectedItem="{Binding SelectedOrder}">
    <CollectionView.ItemTemplate>
        <DataTemplate>
            <Border StrokeThickness="1" Padding="12">
                <VerticalStackLayout>
                    <Label Text="{Binding Number}" FontAttributes="Bold" />
                    <Label Text="{Binding Customer}" FontSize="12" />
                </VerticalStackLayout>
            </Border>
        </DataTemplate>
    </CollectionView.ItemTemplate>
</CollectionView>
```

```csharp
var ordersView = new CollectionView
{
    SelectionMode = SelectionMode.Single,
    ItemTemplate = new DataTemplate(() =>
    {
        var orderNumber = new Label { FontAttributes = FontAttributes.Bold };
        orderNumber.SetBinding(Label.TextProperty, "Number");

        var customer = new Label { FontSize = 12 };
        customer.SetBinding(Label.TextProperty, "Customer");

        return new Border
        {
            StrokeThickness = 1,
            Padding = 12,
            Content = new VerticalStackLayout
            {
                Children = { orderNumber, customer }
            }
        };
    })
};
ordersView.SetBinding(ItemsView.ItemsSourceProperty, "Orders");
ordersView.SetBinding(SelectableItemsView.SelectedItemProperty, "SelectedOrder");
```

**Tips**

- Use `ItemsLayout` to switch between vertical lists, horizontal carousels, or grids.
- Implement `RemainingItemsThreshold` for infinite scroll.

### 3.2 CarouselView

`CarouselView` presents swipeable pages of content, sharing virtualization infrastructure with `CollectionView`.

```xml
<CarouselView ItemsSource="{Binding Highlights}">
    <CarouselView.ItemTemplate>
        <DataTemplate>
            <Image Source="{Binding HeroImage}" Aspect="AspectFill" />
        </DataTemplate>
    </CarouselView.ItemTemplate>
</CarouselView>
```

```csharp
var carousel = new CarouselView
{
    ItemTemplate = new DataTemplate(() => new Image { Aspect = Aspect.AspectFill })
};
carousel.SetBinding(ItemsView.ItemsSourceProperty, "Highlights");
```

**Tips**

- Pair with `IndicatorView` for page indicators.
- Set `IsSwipeEnabled="False"` for programmatic-only transitions.

### 3.3 IndicatorView

`IndicatorView` displays position indicators for paging controls such as `CarouselView`, with customizable shapes and colors.

```xml
<IndicatorView IndicatorsShape="Circle"
               IndicatorColor="#33000000"
               SelectedIndicatorColor="#FF000000"
               Count="{Binding Highlights.Count}"
               Position="{Binding HighlightIndex}" />
```

```csharp
var indicators = new IndicatorView
{
    IndicatorsShape = IndicatorShape.Circle,
    IndicatorColor = Color.FromArgb("#33000000"),
    SelectedIndicatorColor = Colors.Black
};
indicators.SetBinding(IndicatorView.CountProperty, "Highlights.Count");
indicators.SetBinding(IndicatorView.PositionProperty, "HighlightIndex");
```

**Tips**

- Connect to a `CarouselView` with the `IndicatorView.IndicatorTemplate` or `carousel.IndicatorView` property.
- Use `IndicatorTemplate` for thumbnails or numeric badges.

### 3.4 ListView

`ListView` provides legacy data presentation with built-in grouping and context actions, still useful for quick migrations.

```xml
<ListView ItemsSource="{Binding LegacyItems}">
    <ListView.ItemTemplate>
        <DataTemplate>
            <TextCell Text="{Binding Title}" Detail="{Binding Subtitle}" />
        </DataTemplate>
    </ListView.ItemTemplate>
</ListView>
```

```csharp
var listView = new ListView
{
    ItemTemplate = new DataTemplate(() =>
    {
        var cell = new TextCell();
        cell.SetBinding(TextCell.TextProperty, "Title");
        cell.SetBinding(TextCell.DetailProperty, "Subtitle");
        return cell;
    })
};
listView.SetBinding(ListView.ItemsSourceProperty, "LegacyItems");
```

**Tips**

- Prefer `CollectionView` for new screens; reserve `ListView` when migrating existing Xamarin.Forms code.
- Enable `CachingStrategy="RecycleElement"` for large data sets.

### 3.5 TableView

`TableView` organizes content into sections with text or custom view cells, perfect for settings pages.

```xml
<TableView Intent="Settings">
    <TableRoot>
        <TableSection Title="Notifications">
            <SwitchCell Text="Email alerts" On="{Binding EmailAlerts}" />
            <SwitchCell Text="Push notifications" On="{Binding PushAlerts}" />
        </TableSection>
    </TableRoot>
</TableView>
```

```csharp
var table = new TableView
{
    Intent = TableIntent.Settings,
    Root = new TableRoot
    {
        new TableSection("Notifications")
        {
            new SwitchCell { Text = "Email alerts" },
            new SwitchCell { Text = "Push notifications" }
        }
    }
};
((SwitchCell)table.Root[0][0]).SetBinding(SwitchCell.OnProperty, "EmailAlerts");
((SwitchCell)table.Root[0][1]).SetBinding(SwitchCell.OnProperty, "PushAlerts");
```

**Tips**

- Combine text cells and custom view cells to mix toggles with navigation actions.
- Set `HasUnevenRows="True"` when using varying cell heights.

### 3.6 RefreshView

`RefreshView` decorates scrollable content with pull-to-refresh gestures and exposes async commands.

```xml
<RefreshView IsRefreshing="{Binding IsRefreshing}"
             Command="{Binding RefreshCommand}">
    <CollectionView ItemsSource="{Binding Items}" />
</RefreshView>
```

```csharp
var refreshView = new RefreshView
{
    Content = new CollectionView()
};
refreshView.SetBinding(RefreshView.IsRefreshingProperty, "IsRefreshing");
refreshView.SetBinding(RefreshView.CommandProperty, "RefreshCommand");
```

**Tips**

- Use `RefreshColor` to align with brand colors.
- Combine with `TaskCompletionSource` to await server calls before ending refresh.

---

## Further reading

- [Microsoft Learn: CollectionView](https://learn.microsoft.com/en-us/dotnet/maui/user-interface/controls/collectionview)citeturn13search0
- [Microsoft Learn: CarouselView](https://learn.microsoft.com/en-us/dotnet/maui/user-interface/controls/carouselview)citeturn13search1
- [Microsoft Learn: ListView](https://learn.microsoft.com/en-us/dotnet/maui/user-interface/controls/listview)citeturn13search2

