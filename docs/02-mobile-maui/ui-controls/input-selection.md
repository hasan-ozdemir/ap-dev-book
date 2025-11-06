---
title: MAUI Input & Selection
description: Detailed guidance for input and selection controls with .NET 9-ready XAML and C# samples.
last_reviewed: 2025-10-30
owners:
  - @prodyum/maui-guild
---

# Input & Selection Controls

## Entry

`Entry` captures single-line text with support for placeholders, keyboard configuration, and text validation events.

```xml
<Entry Placeholder="Email address"
       Keyboard="Email"
       Text="{Binding Email, Mode=TwoWay}" />
```

```csharp
var emailEntry = new Entry
{
    Placeholder = "Email address",
    Keyboard = Keyboard.Email
};
emailEntry.SetBinding(Entry.TextProperty, "Email", BindingMode.TwoWay);
```

**Tips**

- Subscribe to `Completed` for quick form submission.
- Use `TextTransform` for uppercase product codes.

## Editor

`Editor` extends text entry to multi-line scenarios, supporting soft return, text prediction, and customizable keyboard behaviors.

```xml
<Editor Placeholder="Share your feedback"
        AutoSize="TextChanges"
        Text="{Binding Feedback, Mode=TwoWay}" />
```

```csharp
var feedbackEditor = new Editor
{
    Placeholder = "Share your feedback",
    AutoSize = EditorAutoSizeOption.TextChanges
};
feedbackEditor.SetBinding(Editor.TextProperty, "Feedback", BindingMode.TwoWay);
```

**Tips**

- Combine with `MaxLength` validation to limit characters.
- Wrap in `ScrollView` for long-form input on small screens.

## SearchBar

`SearchBar` offers user-triggered filtering with built-in cancel button and `SearchCommand` integration for MVVM.

```xml
<SearchBar Placeholder="Search customers"
           Text="{Binding Query}"
           SearchCommand="{Binding RunSearchCommand}" />
```

```csharp
var searchBar = new SearchBar
{
    Placeholder = "Search customers"
};
searchBar.SetBinding(SearchBar.TextProperty, "Query");
searchBar.SetBinding(SearchBar.SearchCommandProperty, "RunSearchCommand");
```

**Tips**

- Handle `TextChanged` for incremental search experiences.
- Combine with `SemanticProperties.Hint` to provide guidance for screen readers.

## Picker

`Picker` displays a modal selector, binding to `ItemsSource` and exposing `SelectedItem`/`SelectedIndex` for data binding.

```xml
<Picker Title="Priority"
        ItemsSource="{Binding PriorityLevels}"
        SelectedItem="{Binding SelectedPriority}" />
```

```csharp
var priorityPicker = new Picker { Title = "Priority" };
priorityPicker.SetBinding(Picker.ItemsSourceProperty, "PriorityLevels");
priorityPicker.SetBinding(Picker.SelectedItemProperty, "SelectedPriority");
```

**Tips**

- Use `ItemDisplayBinding` to show complex objects without overriding `ToString`.
- For cascading pickers, react to `SelectedIndexChanged` and update dependent collections.

## DatePicker

`DatePicker` lets users choose dates with support for minimum/maximum constraints and locale-aware formatting.

```xml
<DatePicker Date="{Binding DueDate, Mode=TwoWay}"
            MinimumDate="{Binding MinimumDueDate}"
            MaximumDate="{Binding MaximumDueDate}" />
```

```csharp
var duePicker = new DatePicker();
duePicker.SetBinding(DatePicker.DateProperty, "DueDate", BindingMode.TwoWay);
duePicker.SetBinding(DatePicker.MinimumDateProperty, "MinimumDueDate");
duePicker.SetBinding(DatePicker.MaximumDateProperty, "MaximumDueDate");
```

**Tips**

- Bind `Format` to display user preferences (e.g., `"ddd, MMM dd"`).
- Combine with `DatePickerHandler.Mapper` for platform-specific styling.

## TimePicker

`TimePicker` captures time-of-day values with `TimeSpan` binding support and custom formatting.

```xml
<TimePicker Time="{Binding ReminderTime}"
            Format="HH:mm"
            MinuteInterval="5" />
```

```csharp
var reminderPicker = new TimePicker
{
    Format = "HH:mm",
    MinuteInterval = 5
};
reminderPicker.SetBinding(TimePicker.TimeProperty, "ReminderTime");
```

**Tips**

- Use `MinuteInterval` for coarse scheduling (e.g., every 15 minutes).
- Pair with `DatePicker` to capture full timestamps.

## CheckBox

`CheckBox` supports binary choice with bindable `IsChecked` and optional content for descriptive labels.

```xml
<CheckBox IsChecked="{Binding AcceptTerms}"
          Content="I agree to the terms and conditions" />
```

```csharp
var optIn = new CheckBox
{
    Content = "I agree to the terms and conditions"
};
optIn.SetBinding(CheckBox.IsCheckedProperty, "AcceptTerms");
```

**Tips**

- Use validation to enforce required acknowledgement before enabling submit buttons.
- Apply `VisualStateManager` to surface error states with color changes.

## RadioButton

`RadioButton` enables mutually-exclusive selections via shared `GroupName` values, with optional content layouts.

```xml
<VerticalStackLayout>
    <RadioButton Content="Standard" GroupName="Plan" IsChecked="True" />
    <RadioButton Content="Pro" GroupName="Plan" />
    <RadioButton Content="Enterprise" GroupName="Plan" />
</VerticalStackLayout>
```

```csharp
var plans = new VerticalStackLayout();
foreach (var option in new[] { "Standard", "Pro", "Enterprise" })
{
    plans.Children.Add(new RadioButton { Content = option, GroupName = "Plan" });
}
((RadioButton)plans.Children[0]).IsChecked = true;
```

**Tips**

- Bind `Command` for immediate reaction without waiting for form submission.
- Combine with `Border` to stylize options as cards.

## Switch

`Switch` toggles boolean values with animated transitions and optional color customization via `OnColor`.

```xml
<Switch IsToggled="{Binding NotificationsEnabled}"
        OnColor="{StaticResource AccentColor}" />
```

```csharp
var toggle = new Switch
{
    OnColor = Colors.Green
};
toggle.SetBinding(Switch.IsToggledProperty, "NotificationsEnabled");
```

**Tips**

- Attach to `Toggled` to schedule work like background sync.
- Pair with a `Label` that describes the switch's effect for accessibility.

## Slider

`Slider` captures numeric ranges with customizable min/max values, small/large steps, and live value change events.

```xml
<Slider Minimum="0"
        Maximum="100"
        Value="{Binding Volume, Mode=TwoWay}" />
```

```csharp
var volumeSlider = new Slider
{
    Minimum = 0,
    Maximum = 100
};
volumeSlider.SetBinding(Slider.ValueProperty, "Volume", BindingMode.TwoWay);
```

**Tips**

- Use `ValueChanged` to update UI indicators in real time.
- Bind `SemanticProperties.Hint` to describe the range for screen readers.

## Stepper

`Stepper` increments numeric values using plus/minus buttons with optional wrapping.

```xml
<Stepper Minimum="0"
         Maximum="10"
         Increment="0.5"
         Value="{Binding Quantity, Mode=TwoWay}" />
```

```csharp
var quantityStepper = new Stepper
{
    Minimum = 0,
    Maximum = 10,
    Increment = 0.5
};
quantityStepper.SetBinding(Stepper.ValueProperty, "Quantity", BindingMode.TwoWay);
```

**Tips**

- Pair with `Label` to display the current value using `ValueChanged`.
- Use `Wraps="True"` for cyclical ranges like hours on a clock.

## ActivityIndicator

`ActivityIndicator` shows an indeterminate animation to signal ongoing work, controlled via `IsRunning` and `IsVisible`.

```xml
<ActivityIndicator IsRunning="{Binding IsBusy}"
                   IsVisible="{Binding IsBusy}" />
```

```csharp
var spinner = new ActivityIndicator();
spinner.SetBinding(ActivityIndicator.IsRunningProperty, "IsBusy");
spinner.SetBinding(VisualElement.IsVisibleProperty, "IsBusy");
```

**Tips**

- Wrap long-running tasks in `try/finally` to ensure the indicator stops even on error.
- Combine with `SemanticProperties.Description` to narrate progress context.

## ProgressBar

`ProgressBar` communicates determinate progress, binding to values between 0 and 1 and supporting animated transitions.

```xml
<ProgressBar Progress="{Binding SyncProgress}"
             ProgressColor="{StaticResource AccentColor}" />
```

```csharp
var progress = new ProgressBar();
progress.SetBinding(ProgressBar.ProgressProperty, "SyncProgress");
progress.ProgressColor = Colors.BlueViolet;
```

**Tips**

- Call `ProgressTo` for smooth animations during staged updates.
- Use `WidthRequest` or `HorizontalOptions="Fill"` to ensure full-width alignment on mobile.

## SwipeView

`SwipeView` enables contextual actions revealed by swiping left or right on content, supporting multi-directional menus and command bindings.

```xml
<SwipeView>
    <SwipeView.LeftItems>
        <SwipeItems Mode="Reveal">
            <SwipeItem Text="Pin"
                       IconImageSource="pin.png"
                       Command="{Binding PinCommand}" />
        </SwipeItems>
    </SwipeView.LeftItems>
    <Grid Padding="16">
        <Grid.RowDefinitions>
            <RowDefinition Height="Auto" />
            <RowDefinition Height="Auto" />
        </Grid.RowDefinitions>
        <Label Text="{Binding Title}" FontAttributes="Bold" />
        <Label Grid.Row="1" Text="{Binding Subtitle}" />
    </Grid>
</SwipeView>
```

```csharp
var swipeView = new SwipeView();
var grid = new Grid
{
    Padding = 16,
    RowDefinitions =
    {
        new RowDefinition { Height = GridLength.Auto },
        new RowDefinition { Height = GridLength.Auto }
    }
};
var title = new Label { FontAttributes = FontAttributes.Bold };
title.SetBinding(Label.TextProperty, "Title");
grid.Children.Add(title);

var subtitle = new Label();
Grid.SetRow(subtitle, 1);
subtitle.SetBinding(Label.TextProperty, "Subtitle");
grid.Children.Add(subtitle);

swipeView.Content = grid;

var pinItem = new SwipeItem { Text = "Pin", IconImageSource = "pin.png" };
pinItem.SetBinding(MenuItem.CommandProperty, "PinCommand");
var leftItems = new SwipeItems { Mode = SwipeMode.Reveal };
leftItems.Add(pinItem);
swipeView.LeftItems = leftItems;
```

**Tips**

- Set `SwipeTransitionMode="Reveal"` or `SwipeTransitionMode="Drag"` to control animation behaviour.
- Combine with `SwipeItemView` for complex menu layouts including icons and text.

---

## Further reading

- [Microsoft Learn: Entry](https://learn.microsoft.com/en-us/dotnet/maui/user-interface/controls/entry)citeturn12search0
- [Microsoft Learn: Picker](https://learn.microsoft.com/en-us/dotnet/maui/user-interface/controls/picker)citeturn13search1
- [Microsoft Learn: SwipeView](https://learn.microsoft.com/en-us/dotnet/maui/user-interface/controls/swipeview)citeturn0search0

