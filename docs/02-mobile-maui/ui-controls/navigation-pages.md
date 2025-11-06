---
title: MAUI Navigation & Pages
description: Detailed guidance for navigation & pages controls with .NET 9-ready XAML and C# samples.
last_reviewed: 2025-10-30
owners:
  - @prodyum/maui-guild
---

## 4. Navigation & Pages

### 4.1 ContentPage

`ContentPage` is the base unit for most screens, hosting a single visual tree and participating in navigation stacks.

```xml
<ContentPage xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             Title="Dashboard">
    <ScrollView>
        <VerticalStackLayout Padding="24">
            <Label Text="Dashboard" FontSize="32" FontAttributes="Bold" />
            <!-- page content -->
        </VerticalStackLayout>
    </ScrollView>
</ContentPage>
```

```csharp
public partial class DashboardPage : ContentPage
{
    public DashboardPage()
    {
        InitializeComponent();
        Title = "Dashboard";
    }
}
```

**Tips**

- Use partial classes with dependency injection to pass view models.
- Apply page-level resources for theming.

### 4.2 NavigationPage

`NavigationPage` manages a stack of pages with push/pop semantics and provides a navigation bar UI.

```xml
<NavigationPage xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
                xmlns:pages="clr-namespace:Contoso.App.Pages"
                Title="Accounts">
    <x:Arguments>
        <pages:DashboardPage />
    </x:Arguments>
</NavigationPage>
```

```csharp
var nav = new NavigationPage(new DashboardPage())
{
    BarBackgroundColor = Colors.MidnightBlue,
    BarTextColor = Colors.White
};
await nav.PushAsync(new AccountDetailsPage(account));
```

**Tips**

- Customize the navigation bar via handler mappers for platform-specific styling.
- Use `NavigationPage.SetHasBackButton(page, false)` to hide the back button when needed.

### 4.3 TabbedPage

`TabbedPage` presents peer pages organized by tabs with optional swipe navigation on mobile.

```xml
<TabbedPage xmlns="http://schemas.microsoft.com/dotnet/2021/maui">
    <ContentPage Title="Overview" IconImageSource="tab_overview.png">
        <Label Text="Overview Content" />
    </ContentPage>
    <ContentPage Title="Analytics" IconImageSource="tab_analytics.png">
        <Label Text="Analytics Content" />
    </ContentPage>
</TabbedPage>
```

```csharp
var tabs = new TabbedPage
{
    Children =
    {
        new ContentPage { Title = "Overview", Content = new Label{ Text = "Overview Content"} },
        new ContentPage { Title = "Analytics", Content = new Label{ Text = "Analytics Content"} }
    }
};
```

**Tips**

- With Shell apps, prefer `Tab` architecture for enhanced routing.
- Set `CurrentPage` for programmatic tab selection.

### 4.4 FlyoutPage

`FlyoutPage` combines a flyout menu with detail content, suitable for navigation drawers.

```xml
<FlyoutPage>
    <FlyoutPage.Flyout>
        <ContentPage Title="Menu">
            <CollectionView ItemsSource="{Binding MenuItems}" />
        </ContentPage>
    </FlyoutPage.Flyout>
    <FlyoutPage.Detail>
        <NavigationPage>
            <x:Arguments>
                <local:DashboardPage />
            </x:Arguments>
        </NavigationPage>
    </FlyoutPage.Detail>
</FlyoutPage>
```

```csharp
var flyout = new FlyoutPage
{
    Flyout = new MenuPage(),
    Detail = new NavigationPage(new DashboardPage())
};
```

**Tips**

- Collapse the flyout on navigation to maintain focus on detail content.
- Bind `IsPresented` to respond to wide-screen layouts.

### 4.5 Shell

`Shell` unifies navigation patterns (tabs, flyouts, routes) with declarative URI-based routing and integrated dependency injection.

```xml
<Shell>
    <TabBar>
        <ShellContent Route="home"
                      Title="Home"
                      ContentTemplate="{DataTemplate local:HomePage}" />
        <ShellContent Route="settings"
                      Title="Settings"
                      ContentTemplate="{DataTemplate local:SettingsPage}" />
    </TabBar>
</Shell>
```

```csharp
public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        Routing.RegisterRoute("order/details", typeof(OrderDetailsPage));
    }
}
```

**Tips**

- Use `GoToAsync` with query parameters for deep links.
- Apply `ShellItem` visual states to highlight the active route.

---

## Further reading

- [Microsoft Learn: NavigationPage](https://learn.microsoft.com/en-us/dotnet/maui/user-interface/pages/navigationpage)citeturn15search0
- [Microsoft Learn: TabbedPage](https://learn.microsoft.com/en-us/dotnet/maui/user-interface/pages/tabbedpage)citeturn15search1
- [Microsoft Learn: Shell navigation](https://learn.microsoft.com/en-us/dotnet/maui/fundamentals/shell/navigation)citeturn15search2

