title: Responsive Layout Patterns
description: Adaptive layout strategies, breakpoint design, and multiplatform theming for .NET MAUI.
last_reviewed: 2025-11-03
owners:
  - @prodyum/maui-guild
---

# Responsive Layout ve Gorsel Tasarim

.NET MAUI tek kod tabanindan telefon, tablet, masaustu ve TV form faktorlerine yayin yapmaniza izin verir. Bu bolum, ekran boyutlarina gore duzeni uyarlama, safe-area yonetimi, tema degisimleri ve performans optimizasyonu icin uygulanabilir kaliplar sunar.

## 1. Layout temel taslari

- **Grid:** Satir/sutun tanimlari ve `RowDefinition.Width="*"` gibi oransal degerlerle esnek duzen kurun; .NET 9’daki layout manager guncellemeleri Grid ve FlexLayout olcumlerini daha verimli hale getirdi.citeturn4search3
- **FlexLayout:** Wrap, Align ve Justify secenekleri ile kart veya akiskan listeler olusturun; cihaz yonune gore yatay/dikey gecis yapmayi kolaylastirir.citeturn4search3
- **StackLayout ailesi:** Horizontal/VerticalStackLayout basit form veya detay sayfalarinda Grid’den daha dusuk maliyetlidir; sade senaryolarda bu hafif layoutlari tercih edin.citeturn4search3
- **ControlTemplate:** Tekrarlayan UI’leri tek kaynaktan yonetin; breakpoint’e gore farkli `ContentView` sablonlari swap edebilirsiniz.

## 2. Breakpoint stratejisi

| Ekran genisligi | Tipik cihaz | Onerilen duzen |
|-----------------|------------|----------------|
| < 600 dp | Telefon | Tek sutun, tam ekran dialog veya bottom sheet |
| 600-900 dp | Katlanabilir / kucuk tablet | Liste + detay icin iki sutunlu Grid |
| > 900 dp | Tablet / masaustu | Navigasyon + icerik + ikincil panel (SplitView) |

- `OnIdiom` ve `OnPlatform` tetikleyicileriyle ayni XAML dosyasinda farkli cihaz tiplerine gore kaynak atayin.citeturn2search0
- Genislik tabanli `VisualStateManager` durumlari olusturup `AdaptiveTrigger` ile sutun sayisini degistirin.citeturn4view0

## 3. Safe area ve kenar bosluklari

- iOS ve macOS’ta `On<iOS>.SetUseSafeArea(true)` kullanarak notch ve home indicator alanlarini koruyun; gerekirse ek padding ekleyin.citeturn2search0
- Android’de `WindowInsets` API’sini okuyup navigation/status bar yuksekliklerini hesaba katin; immersive mod kullaniyorsaniz kontrollerin tasmasini engeller.citeturn1search6
- Windows’ta pencere yeniden boyutlandiginda `VisualState` tetikleyerek layout’u yeniden duzenleyin; masaustu uygulamalar grid sutunlarini dinamik olarak genisletebilir.citeturn3search3

## 4. Dinamik icerik ve sanallastirma

- **CollectionView:** Varsayilan olarak sanallastirma aciktir; buyuk listelerde `CachingStrategy="RecycleElement"` kullanarak bellek maliyetini azaltin.citeturn4search3
- **BindableLayout:** Sabit sayida karti hizli sekilde tanimlamak icin hafif bir secenektir; breakpoints arasinda layout degistikce binding bozulmaz.citeturn4search3
- **RefreshView/SwipeView:** Jestlerin scroll davranisini bozmadigini test edin; aksi halde disable edip alternatif buton sunun.citeturn3search3

## 5. Tema ve yogunluk uyarlamalari

- `AppThemeColor` ve `AppThemeBinding` kullanarak acik/koyu temalar arasinda otomatik renk degisimi saglayin.citeturn5search3
- `DeviceDisplay.MainDisplayInfo.Density` degerine gore padding ve ikon boyutlarini ayarlayin; yuksek DPI ekranlarda ikonlar icin SVG tercih edin.citeturn4search1turn3search3
- Kaynak sozluklerinde `Thickness` (Small/Medium/Large) gibi tokenlar belirleyin; layout degisimlerini token degistirerek yonetin.

## 6. Katlanabilir ve masaustu senaryolari

- **TwoPaneView:** Surface Duo gibi cihazlarda iki paneli ayni anda gorebilirsiniz; `PanePriority` ile aktif paneli kontrol edin.citeturn2search4
- **Window.SizeChanged:** Masaustunde pencere genisleyince yan panel acmak icin Grid sutunlarini guncelleyin; pointer destekli VisualState’ler ile hover efektleri ekleyin.citeturn3search3turn1search9
- **PointerGestureRecognizer:** .NET 9’da masaustu icin birinci sinif vatandas haline geldi; fare hareketleriyle komut yuzeylerini zenginlestirin.citeturn2search6

## 7. Test ve gozden gecirme checklist’i

- [ ] Her breakpoint icin gercek cihaz/emulator test edildi (telefon 6", tablet 10", masaustu 13")?
- [ ] Safe area loglari (iOS/Android) QA pipeline’inda temiz?
- [ ] CollectionView performans sayaci 60 fps uzerinde mi?
- [ ] Tema degisiminde kontrast ve ikonlar dogrulandi mi?citeturn5search3
- [ ] Katlanabilir cihaz modunda (Surface Duo vb.) UI bozulmasi yok mu?citeturn2search4

Bu kaliplar, baglanti (REST/GraphQL/gRPC) ve telemetri rehberleriyle birlikte cok platformlu MAUI deneyimini guvence altina alir. Her sprint sonunda bu checklist’i tasarim incelemesinde kullanarak kullanici geri bildirimlerini sistematik sekilde takip edin.
