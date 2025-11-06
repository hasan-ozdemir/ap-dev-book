---
title: .NET MAUI ToDo Uygulamasi
description: Step-by-step tutorial for building a production-ready ToDo app with .NET 9 and .NET MAUI.
last_reviewed: 2025-11-03
owners:
  - @prodyum/maui-guild
---

# .NET MAUI ToDo Uygulamasi

Bu egitim, .NET 9 ve .NET MAUI kullanarak uretime hazir bir gorev yoneticisini sifirdan insa etmenizi saglar. Amac yalnizca temel CRUD islemlerini gostermek degil; modern MVVM desenleri, yerel SQLite depolama, dayanikli HTTP entegrasyonlari ve Visual Studio 17.14’un sundugu canli gelistirici deneyimlerini kapsayan uctan uca bir yolculuk sunmaktir.citeturn8open0turn10open0

## 1. Ogrenim ciktilari

- Visual Studio 2022 17.14 ve .NET 9 MAUI is yuku ile cok platformlu ortam kurulumunu dogrulamak.citeturn8open0turn9open0
- `dotnet new maui` sablonundan baslayarak cozum mimarisini Prodyum standartlarina gore duzenlemek.citeturn10open0
- CommunityToolkit.Mvvm ve CommunityToolkit.Maui paketleriyle MVVM katmanini kaynak uretecleri ve gelismis UI bilesenleriyle yapilandirmak.citeturn11open0turn12open0
- Microsoft.Data.Sqlite ile platform-sandigina uygun yerel veri depolama ve uygulama ilk calistiginda tohumlama stratejilerini kurmak.citeturn13open0turn14open0
- `HttpClientFactory` ve Polly tabanli dayaniklilik handler’larini kullanarak REST API cagrilarinda geri kazanim politikalari eklemek.citeturn15open0
- Opsiyonel olarak PowerSync gibi offline-first senkron kumelerini entegre etmeye hazirlanmak.citeturn16open0

## 2. Onkosullar

| Bilesen | Surum / Ayar |
|---------|--------------|
| Visual Studio | 2022 17.14 (Current Channel) + .NET MAUI workload, Hot Reload ve yeni Mono debug engine etkin.citeturn8open0 |
| .NET SDK | 9.0.306 STS, `global.json` ile sabitlenmis.citeturn9open0 |
| Android SDK | API 35 (Android 15) zorunludur; Play Store politikasi geregi guncel hedef seviye.citeturn18open0 |
| iOS | Xcode 16.4+, iOS 18.5 SDK; macOS 15.3 veya uzeri.citeturn19open0 |
| NuGet paketleri | `CommunityToolkit.Mvvm`, `CommunityToolkit.Maui`, `Microsoft.Data.Sqlite`, `Microsoft.Extensions.Http.Resilience`.citeturn11open0turn12open0turn13open0turn15open0 |

## 3. Cozume baslangic: CLI & sablon

1. Kok dizinde hedef SDK’yi kilitleyin:
   ```json
   {
     "sdk": {
       "version": "9.0.306",
       "rollForward": "latestFeature",
       "allowPrerelease": false
     }
   }
   ```
2. MAUI sablonunu olusturun:
   ```bash
   dotnet new maui -n Contoso.Todo -o src/Contoso.Todo
   ```
3. Opsiyonel: Microsoft’un Developer Balance ornegini referans almak isterseniz sablonu klonlayin veya projeyi inceleyin (`dotnet new maui --install Microsoft.Maui.Templates`).citeturn10open0

Cozume `Directory.Build.props`, `Directory.Packages.props` ve `global.json` ekleyerek tum projeler icin ortak yapilari yonetin (bkz. Portal’daki Proje Sablonlari rehberi).

## 4. Cozum yapisini duzenleme

```
src/
  Contoso.Todo/
    App.xaml
    App.xaml.cs
    MauiProgram.cs
    Resources/...
    Platforms/Android|iOS|MacCatalyst|Windows
    Models/TodoItem.cs
    Data/AppDbContext.cs
    Data/TodoRepository.cs
    ViewModels/TodoListViewModel.cs
    Views/TodoListPage.xaml
    Services/SyncService.cs
```

Bu dizin yerlesimi, Developer Balance sablonunda onerilen CommunityToolkit + SQLite birlesimini takip eder; `MauiProgram` icinde DI kaydi yapilir.citeturn10open0

## 5. Paketleri ekleyin

```bash
dotnet add src/Contoso.Todo/Contoso.Todo.csproj package CommunityToolkit.Mvvm
dotnet add src/Contoso.Todo/Contoso.Todo.csproj package CommunityToolkit.Maui
dotnet add src/Contoso.Todo/Contoso.Todo.csproj package Microsoft.Data.Sqlite
dotnet add src/Contoso.Todo/Contoso.Todo.csproj package Microsoft.Extensions.Http.Resilience
```

`CommunityToolkit.Maui` 12.x ile guncel popup/overlay duzeltmelerini ve .NET 9 uyumlulugunu elde edersiniz; MVVM paketleri kaynaq ureticilerle `ObservableObject`, `RelayCommand` gibi tipleri saglar.citeturn11open0turn12open0

## 6. Alan modeli ve SQLite katmani

`Models/TodoItem.cs`:
```csharp
using System;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Contoso.Todo.Models;

public partial class TodoItem : ObservableObject
{
    [ObservableProperty] private Guid id = Guid.NewGuid();
    [ObservableProperty] private string title = string.Empty;
    [ObservableProperty] private string notes = string.Empty;
    [ObservableProperty] private bool isCompleted;
    [ObservableProperty] private DateTimeOffset createdAt = DateTimeOffset.UtcNow;
    [ObservableProperty] private DateTimeOffset? dueAt;
}
```

`Data/AppDbContext.cs`:
```csharp
using Microsoft.Data.Sqlite;

namespace Contoso.Todo.Data;

public sealed class AppDbContext : IAsyncDisposable
{
    private readonly SqliteConnection _connection;

    public AppDbContext(string databasePath)
    {
        var builder = new SqliteConnectionStringBuilder
        {
            DataSource = databasePath,
            Mode = SqliteOpenMode.ReadWriteCreate,
            Cache = SqliteCacheMode.Shared
        };
        _connection = new SqliteConnection(builder.ConnectionString);
    }

    public async Task InitializeAsync()
    {
        await _connection.OpenAsync();
        var createSql = """
            CREATE TABLE IF NOT EXISTS TodoItems (
                Id TEXT PRIMARY KEY,
                Title TEXT NOT NULL,
                Notes TEXT,
                IsCompleted INTEGER NOT NULL,
                CreatedAt TEXT NOT NULL,
                DueAt TEXT NULL
            );
            """;
        await new SqliteCommand(createSql, _connection).ExecuteNonQueryAsync();
    }

    public SqliteConnection Connection => _connection;

    public async ValueTask DisposeAsync()
    {
        await _connection.CloseAsync();
        await _connection.DisposeAsync();
    }
}
```

`Data/TodoRepository.cs`:
```csharp
using Contoso.Todo.Models;
using Microsoft.Data.Sqlite;

namespace Contoso.Todo.Data;

public sealed class TodoRepository
{
    private readonly AppDbContext _db;

    public TodoRepository(AppDbContext db) => _db = db;

    public async Task<IReadOnlyList<TodoItem>> GetAllAsync()
    {
        var list = new List<TodoItem>();
        var sql = "SELECT Id, Title, Notes, IsCompleted, CreatedAt, DueAt FROM TodoItems ORDER BY CreatedAt DESC;";
        await using var cmd = new SqliteCommand(sql, _db.Connection);
        await using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            list.Add(new TodoItem
            {
                Id = Guid.Parse(reader.GetString(0)),
                Title = reader.GetString(1),
                Notes = reader.IsDBNull(2) ? string.Empty : reader.GetString(2),
                IsCompleted = reader.GetBoolean(3),
                CreatedAt = DateTimeOffset.Parse(reader.GetString(4)),
                DueAt = reader.IsDBNull(5) ? null : DateTimeOffset.Parse(reader.GetString(5))
            });
        }
        return list;
    }

    public async Task UpsertAsync(TodoItem item)
    {
        const string sql = """
            INSERT INTO TodoItems (Id, Title, Notes, IsCompleted, CreatedAt, DueAt)
            VALUES (@id, @title, @notes, @completed, @createdAt, @dueAt)
            ON CONFLICT(Id) DO UPDATE SET
                Title = excluded.Title,
                Notes = excluded.Notes,
                IsCompleted = excluded.IsCompleted,
                CreatedAt = excluded.CreatedAt,
                DueAt = excluded.DueAt;
            """;

        await using var cmd = new SqliteCommand(sql, _db.Connection);
        cmd.Parameters.AddWithValue("@id", item.Id.ToString());
        cmd.Parameters.AddWithValue("@title", item.Title);
        cmd.Parameters.AddWithValue("@notes", item.Notes);
        cmd.Parameters.AddWithValue("@completed", item.IsCompleted);
        cmd.Parameters.AddWithValue("@createdAt", item.CreatedAt.ToString("O"));
        cmd.Parameters.AddWithValue("@dueAt", item.DueAt?.ToString("O") ?? (object)DBNull.Value);
        await cmd.ExecuteNonQueryAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        await using var cmd = new SqliteCommand("DELETE FROM TodoItems WHERE Id = @id;", _db.Connection);
        cmd.Parameters.AddWithValue("@id", id.ToString());
        await cmd.ExecuteNonQueryAsync();
    }
}
```

SQLite dosyasi her platformun uygulama dizininde olusturulur; Windows, Android ve iOS farkli sandbox’lara sahip oldugundan tohum verileri her cihaz icin kodla eklemeniz gerekir.citeturn13open0turn14open0

## 7. MVVM katmani

`ViewModels/TodoListViewModel.cs`:
```csharp
using System.Collections.ObjectModel;
using Contoso.Todo.Data;
using Contoso.Todo.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Contoso.Todo.ViewModels;

public partial class TodoListViewModel : ObservableObject
{
    private readonly TodoRepository _repository;

    [ObservableProperty] private bool isBusy;
    public ObservableCollection<TodoItem> Items { get; } = new();

    public TodoListViewModel(TodoRepository repository) => _repository = repository;

    [RelayCommand]
    private async Task InitializeAsync()
    {
        if (IsBusy) return;
        try
        {
            IsBusy = true;
            Items.Clear();
            foreach (var item in await _repository.GetAllAsync())
                Items.Add(item);
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task AddTodoAsync()
    {
        var item = new TodoItem { Title = "Yeni gorev", Notes = "Detay ekleyin" };
        await _repository.UpsertAsync(item);
        Items.Insert(0, item);
    }

    [RelayCommand]
    private async Task ToggleAsync(TodoItem item)
    {
        item.IsCompleted = !item.IsCompleted;
        await _repository.UpsertAsync(item);
    }

    [RelayCommand]
    private async Task DeleteAsync(TodoItem item)
    {
        await _repository.DeleteAsync(item.Id);
        Items.Remove(item);
    }
}
```

CommunityToolkit.Mvvm kaynak uretecleri, `ObservableProperty` ve `RelayCommand` oznitelikleriyle boilerplate kodunu ortadan kaldirir.citeturn12open0

## 8. UI katmani

`Views/TodoListPage.xaml`:
```xml
<?xml version="1.0" encoding="utf-8" ?>
<ContentPage xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             xmlns:toolkit="http://schemas.microsoft.com/dotnet/2022/maui/toolkit"
             x:Class="Contoso.Todo.Views.TodoListPage"
             Title="Gorevler">
    <VerticalStackLayout Padding="16" Spacing="12">
        <Button Text="Gorev Ekle"
                Command="{Binding AddTodoCommand}"
                SemanticProperties.Hint="Yeni bir gorev olusturur" />

        <CollectionView ItemsSource="{Binding Items}">
            <CollectionView.ItemTemplate>
                <DataTemplate>
                    <Frame Padding="12" Margin="0,4">
                        <VerticalStackLayout Spacing="4">
                            <Grid ColumnDefinitions="Auto,*,Auto" ColumnSpacing="8">
                                <CheckBox IsChecked="{Binding IsCompleted}"
                                          Command="{Binding Source={RelativeSource AncestorType={x:Type vm:TodoListViewModel}}, Path=ToggleCommand}"
                                          CommandParameter="{Binding .}" />
                                <Label Grid.Column="1"
                                       Text="{Binding Title}"
                                       TextDecorations="{Binding IsCompleted, Converter={toolkit:BoolToTextDecorationConverter}}" />
                                <Button Grid.Column="2"
                                        Text="Sil"
                                        Command="{Binding Source={RelativeSource AncestorType={x:Type vm:TodoListViewModel}}, Path=DeleteCommand}"
                                        CommandParameter="{Binding .}" />
                            </Grid>
                            <Label Text="{Binding Notes}" FontSize="12" TextColor="{AppThemeBinding Light=#555, Dark=#ccc}" />
                        </VerticalStackLayout>
                    </Frame>
                </DataTemplate>
            </CollectionView.ItemTemplate>
        </CollectionView>

        <ActivityIndicator IsRunning="{Binding IsBusy}" IsVisible="{Binding IsBusy}" />
    </VerticalStackLayout>
</ContentPage>
```

`Views/TodoListPage.xaml.cs`:
```csharp
using Contoso.Todo.ViewModels;

namespace Contoso.Todo.Views;

public partial class TodoListPage : ContentPage
{
    public TodoListPage(TodoListViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
        Loaded += async (_, _) => await viewModel.InitializeCommand.ExecuteAsync(null);
    }
}
```

Toolkit donusturuculeri (orn. `BoolToTextDecorationConverter`) ile UI durumu kolayca yonetilir.citeturn11open0

## 9. MauiProgram.cs yapilandirmasi

```csharp
using Contoso.Todo.Data;
using Contoso.Todo.ViewModels;
using Contoso.Todo.Views;
using CommunityToolkit.Maui;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http.Resilience;

namespace Contoso.Todo;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit();

        builder.Services.AddSingleton<AppDbContext>(_ =>
        {
            var path = Path.Combine(FileSystem.AppDataDirectory, "todo.db3");
            var context = new AppDbContext(path);
            Task.Run(context.InitializeAsync).Wait();
            return context;
        });

        builder.Services.AddSingleton<TodoRepository>();
        builder.Services.AddTransient<TodoListViewModel>();
        builder.Services.AddTransient<TodoListPage>();

        builder.Services.AddHttpClient("TodoApi", client =>
        {
            client.BaseAddress = new Uri("https://api.contoso.todo");
            client.Timeout = TimeSpan.FromSeconds(30);
        })
        .AddStandardResilienceHandler(options =>
        {
            options.Retry.MaxRetryAttempts = 3;
            options.CircuitBreaker.MinimumThroughput = 20;
        });

        return builder.Build();
    }
}
```

`.AddStandardResilienceHandler` .NET 9 ile gelen onerilen dayaniklilik politikalarini (retry, timeout, circuit breaker) tek satirda saglar.citeturn15open0

## 10. Senkronizasyon ve offline stratejileri

- **Yerel senkronizasyon**: SQLite dosyasi her cihazda bagimsizdir; platformlar arasi paylasilan veri istiyorsaniz uygulama baslangicinda tohum verileri kodla ekleyin veya sunucu ile delta senkronizasyonu planlayin.citeturn14open0
- **Bulut senkronizasyonu**: PowerSync’in .NET istemcisi, lokal SQLite ile Postgres/Mongo gibi sunucular arasinda gercek zamanli replike saglamak icin alpha asamasinda kullanilabilir.citeturn16open0
- **Bildirimler & telemetri**: Azure Notification Hubs ve Application Insights entegrasyonlari icin portalin Azure bolumlerine gecin.

## 11. Calistirma ve dogrulama

```bash
dotnet build src/Contoso.Todo/Contoso.Todo.csproj -t:Run -f net9.0-android
dotnet build src/Contoso.Todo/Contoso.Todo.csproj -t:Run -f net9.0-ios
```

- Visual Studio’nun yeni Copilot Agent Mode ozelligiyle, “`TodoListViewModel icin hataya dayanikli API cagrisi ekle`” gibi dogal dil talepleri vererek hizli iterasyon saglayabilirsiniz.citeturn17open0
- Android API 35 emulatoru veya fiziksel cihaz uzerinde test etmeyi unutmayin; aksi halde Play Store uygunluk kontrolleri basarisiz olur.citeturn18open0

## 12. Sonraki adimlar

1. **Azure Entegrasyonu** → portalin Azure Playbook bolumundeki App Services ve Notification Hubs recetelerini takip edin.
2. **UI gelistirmeleri** → Telerik veya diger ucuncu taraf kontrollerle (orn. BottomSheet, yeni tema motoru) tasarimi zenginlestirin.citeturn20open0
3. **Kalite guvence** → Testing & Quality bolumundeki unit/UI test stratejilerini uygulayin; `dotnet test` ile otomatik test konusu.
4. **DevOps** → CI/CD playbook’u izleyerek pipeline’iniza `.NET 9` workload yuklemelerini ve `maui-check --ci` dogrulamasini ekleyin.

Bu yolculugu tamamladiginizda .NET 9 MAUI ekosisteminde uctan uca uretim mobil uygulamayi tasarlayabilecek, veri katmanini yonetebilecek ve modern dayaniklilik/dizinleme pratiklerini uygulayabilecek yetkinlige ulasacaksiniz.
CommunityToolkit.Mvvm kaynak uretecleri, `ObservableProperty` ve `RelayCommand` oznitelikleriyle boilerplate kodunu ortadan kaldirir.citeturn12open0

