# Hướng dẫn cấu trúc Project: .NET 10 + .NET MAUI

Tài liệu tự học, giải thích **từng file/thư mục trong một solution MAUI + ASP.NET Core Web API làm gì**, tại sao nó tồn tại, và khi nào bạn phải đụng vào nó.

Đọc theo thứ tự: phần 1–3 để hiểu bức tranh chung, phần 4–7 để hiểu chi tiết từng project, phần 8–10 khi bắt đầu viết code thật.

---

## 1. Ba khái niệm nền tảng phải phân biệt được

Người mới hay nhầm ba thứ này. Nắm rõ trước khi đọc tiếp.

| Khái niệm | Là gì | Ví dụ |
|---|---|---|
| **.NET 10** | Runtime + SDK + BCL (thư viện lõi). Nền tảng chạy code C#. | `dotnet` CLI, `System.Text.Json`, GC |
| **.NET MAUI** | Một **UI framework** chạy *trên* .NET, build app native cho Android/iOS/Windows/macOS từ một codebase | `ContentPage`, XAML, `Shell` |
| **ASP.NET Core** | Một **web framework** cũng chạy *trên* .NET, build Web API/website | `WebApplication`, Controller, Minimal API |

→ MAUI và ASP.NET Core là **anh em cùng cha (.NET)**, không phải cái này nằm trong cái kia. Chúng chia sẻ chung ngôn ngữ C#, chung NuGet, chung DI container, chung `HttpClient` — đó chính là lý do đặt chung một solution rất tiện.

---

## 2. Solution vs Project vs Assembly

```
MAUI.sln                    ← Solution: chỉ là 1 file text liệt kê các project. KHÔNG build ra gì cả.
 ├── src/Client/Client.csproj   ← Project: 1 đơn vị build → ra 1 assembly (Client.dll)
 ├── src/Api/Api.csproj         ← Project → Api.dll
 └── src/Shared/Shared.csproj   ← Project → Shared.dll
```

- **Solution (`.sln`)**: cho IDE biết "những project này thuộc cùng một nhóm". Mở bằng Visual Studio/Rider. `dotnet build` ở thư mục có `.sln` sẽ build tất cả.
- **Project (`.csproj`)**: file XML mô tả *build gì, target framework nào, tham chiếu gì*. Đây là file bạn sẽ đọc/sửa nhiều nhất.
- **Assembly (`.dll`)**: kết quả biên dịch của 1 project.

Tạo solution:

```powershell
dotnet new sln -n MAUI
dotnet new maui        -n Client -o src/Client
dotnet new webapi      -n Api    -o src/Api
dotnet new classlib    -n Shared -o src/Shared
dotnet sln add src/Client src/Api src/Shared

# Nối dây tham chiếu: Client và Api đều "biết" Shared, nhưng Shared không biết ai cả
dotnet add src/Client reference src/Shared
dotnet add src/Api    reference src/Shared
```

**Quy tắc vàng về hướng tham chiếu**: mũi tên luôn đi vào `Shared`, không bao giờ đi ra. `Shared` không được tham chiếu `Client` hay `Api`. Nếu bạn thấy mình muốn làm điều ngược lại → thiết kế đang sai chỗ nào đó.

```
Client (MAUI)  ──┐
Web (Blazor)   ──┼──▶ Shared
Api            ──┘
```

`Web` là phần mở rộng tùy chọn (Buổi 16–17 trong [HUONG-DAN-TUNG-BUOI.md](HUONG-DAN-TUNG-BUOI.md)). Nó khiến `Shared` có **ba** người dùng thay vì hai — và đó là lúc giá trị của contract dùng chung thể hiện rõ nhất: đổi một trường trong DTO làm hỏng build của cả ba project ngay lập tức, thay vì âm thầm sai lúc chạy.

> **Đối chiếu với repo SoChi thật.** Sơ đồ dưới đây dùng tên rút gọn cho dễ đọc. Repo thật lồng sâu thêm một cấp — mỗi project nằm trong thư mục riêng bên trong thư mục nhóm:
>
> | Tài liệu này viết | Repo SoChi thật |
> |---|---|
> | `src/Client/Client.csproj` | `src/Client/SoChi.Client/SoChi.Client.csproj` |
> | `src/Api/Api.csproj` | `src/Api/SoChi.Api/SoChi.Api.csproj` |
> | `src/Shared/Shared.csproj` | `src/Shared/SoChi.Shared/SoChi.Shared.csproj` |
> | `tests/` | `tests/` — giống nhau, không lồng thêm cấp |
>
> [HUONG-DAN-TUNG-BUOI.md](HUONG-DAN-TUNG-BUOI.md) dùng đường dẫn thật, còn tài liệu này giữ tên rút gọn vì nó giải thích **khái niệm**, không phải để copy lệnh.

---

## 3. Bản đồ toàn bộ solution

```
MAUI.sln
global.json                  # Ghim phiên bản SDK (rất quan trọng, xem §8)
Directory.Build.props        # Setting áp cho MỌI project (nullable, warnings...)
.editorconfig                # Quy tắc format code
.gitignore

src/
  Shared/                    # DTO/contract dùng chung — đọc phần này TRƯỚC
    Shared.csproj
    Dtos/
      ProductDto.cs
      CreateOrderRequest.cs
    ApiRoutes.cs             # Hằng số đường dẫn API, dùng chung 2 phía

  Api/                       # ASP.NET Core Web API (phục vụ luôn file tĩnh của Web)
    Api.csproj
    Program.cs               # Entry point + cấu hình DI + pipeline
    appsettings.json
    appsettings.Development.json
    Properties/launchSettings.json
    Endpoints/  hoặc  Controllers/
    Services/
    Data/                    # DbContext, Repository

  Web/                       # Blazor WebAssembly — dashboard thống kê & kiểm tra dữ liệu (tùy chọn)
    Web.csproj
    Program.cs               # Entry point + DI phía trình duyệt
    Pages/                   # Trang .razor định tuyến bằng @page
    Components/              # Component tái sử dụng (biểu đồ SVG...)
    Services/
    wwwroot/

  Client/                    # .NET MAUI app
    Client.csproj
    MauiProgram.cs           # Entry point + DI (tương đương Program.cs của Api)
    App.xaml / App.xaml.cs
    AppShell.xaml / AppShell.xaml.cs
    Views/
    ViewModels/
    Services/
    Resources/
    Platforms/
      Android/
      iOS/
      Windows/
      MacCatalyst/

tests/
  Api.Tests/
  Client.Tests/
```

---

## 4. Project `Shared` — trái tim của solution

Đây là project **đơn giản nhất về code nhưng quan trọng nhất về kiến trúc**.

### Vấn đề nó giải quyết

Không có `Shared`, bạn sẽ viết class `Product` hai lần — một ở Api, một ở Client. Rồi một ngày bạn đổi `Price` từ `decimal` sang `long` ở Api mà quên đổi bên Client → app crash lúc runtime, compiler **không hề cảnh báo**.

Có `Shared`, cùng một class được cả hai bên tham chiếu → đổi ở một chỗ, **bên nào chưa cập nhật sẽ lỗi compile ngay**. Bug từ runtime chuyển thành compile-time. Đó là toàn bộ giá trị của nó.

### `Shared.csproj`

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
  </PropertyGroup>
</Project>
```

Ngắn vậy thôi. Lưu ý: **không** thêm reference tới ASP.NET Core hay MAUI vào đây. `Shared` phải "sạch" — chỉ chứa POCO/record và hằng số.

### Nội dung nên và không nên đặt ở đây

| Nên | Không nên |
|---|---|
| DTO, record request/response | Entity của EF Core (đó là chuyện nội bộ của Api) |
| Enum dùng chung | Logic nghiệp vụ |
| Hằng số route (`/api/products`) | ViewModel của MAUI |
| Validation attribute cơ bản | Bất cứ gì phụ thuộc UI hay database |

```csharp
// src/Shared/Dtos/ProductDto.cs
namespace Shared.Dtos;

public record ProductDto(int Id, string Name, decimal Price);

// src/Shared/ApiRoutes.cs
namespace Shared;

public static class ApiRoutes
{
    public const string Products = "api/products";
    public static string ProductById(int id) => $"{Products}/{id}";
}
```

Dùng `ApiRoutes` ở cả hai phía → gõ sai URL cũng thành lỗi compile.

---

## 5. Project `Api` — ASP.NET Core Web API

### `Program.cs` — file quan trọng nhất

Từ .NET 6 trở đi, không còn `Startup.cs`. Mọi thứ nằm trong một file, chia làm **hai nửa rõ rệt**:

```csharp
var builder = WebApplication.CreateBuilder(args);

// ── NỬA 1: ĐĂNG KÝ DỊCH VỤ (DI container) ──
builder.Services.AddDbContext<AppDbContext>(o => o.UseSqlite("Data Source=app.db"));
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddOpenApi();

var app = builder.Build();   // ← Ranh giới. Sau dòng này không đăng ký service được nữa.

// ── NỬA 2: PIPELINE (middleware) — THỨ TỰ CÓ Ý NGHĨA ──
if (app.Environment.IsDevelopment())
    app.MapOpenApi();

app.UseHttpsRedirection();
app.UseAuthentication();     // Phải trước Authorization
app.UseAuthorization();

app.MapGet(ApiRoutes.Products, (IProductService svc) => svc.GetAllAsync());

app.Run();
```

Hai điều phải khắc cốt ghi tâm:

1. **`builder.Build()` là ranh giới**. Trước nó: nói *"app có những dịch vụ nào"*. Sau nó: nói *"request đi qua những bước nào"*.
2. **Thứ tự middleware quan trọng**. Request chạy từ trên xuống, response chạy ngược lên. `UseAuthorization()` trước `UseAuthentication()` = luôn bị 401 và bạn sẽ debug rất lâu.

### Ba lifetime của DI — hay sai nhất

| Lifetime | Tạo mới khi nào | Dùng cho |
|---|---|---|
| `AddSingleton` | Một lần cho cả đời app | Cache, config, `HttpClient` factory |
| `AddScoped` | Một lần mỗi HTTP request | `DbContext`, service nghiệp vụ |
| `AddTransient` | Mỗi lần được inject | Object nhẹ, không giữ state |

Lỗi kinh điển: inject `Scoped` (`DbContext`) vào `Singleton` → `DbContext` sống mãi, dùng chung giữa các request, hỏng dữ liệu. .NET sẽ ném exception lúc startup nếu phát hiện — đừng tắt cảnh báo đó.

### `appsettings.json` vs `launchSettings.json`

Rất hay nhầm:

- **`appsettings.json`** — cấu hình **của ứng dụng**, có mặt cả khi deploy. Connection string, API key, log level.
  `appsettings.Development.json` ghi đè lên nó khi `ASPNETCORE_ENVIRONMENT=Development`.
- **`Properties/launchSettings.json`** — cấu hình **chạy trên máy dev**, chỉ dùng khi `dotnet run` / F5. **Không được deploy**. Đây là nơi ghi port `https://localhost:7xxx`.

Đọc config trong code:

```csharp
// appsettings.json:  { "Api": { "PageSize": 20 } }
builder.Services.Configure<ApiOptions>(builder.Configuration.GetSection("Api"));

// Inject:  IOptions<ApiOptions> options → options.Value.PageSize
```

### Minimal API vs Controller

Cả hai đều hợp lệ. Học thì bắt đầu bằng Minimal API (ít lớp trung gian, dễ thấy toàn cảnh), chuyển sang Controller khi số endpoint nhiều và cần nhóm/filter/attribute.

---

## 6. Project `Client` — .NET MAUI

### `Client.csproj` — hiểu multi-targeting

Đây là điểm khác biệt lớn nhất so với project .NET thường:

```xml
<TargetFrameworks>net10.0-android;net10.0-ios;net10.0-maccatalyst</TargetFrameworks>
<TargetFrameworks Condition="$([MSBuild]::IsOSPlatform('windows'))">
  $(TargetFrameworks);net10.0-windows10.0.19041.0
</TargetFrameworks>
```

`TargetFramework**s**` (số nhiều) nghĩa là project này **build ra nhiều output khác nhau**, mỗi platform một cái. Hệ quả thực tế:

- Mọi lệnh `dotnet build/run` trên project MAUI **phải có `-f`** để chỉ định target, nếu không MSBuild không biết chạy cái nào.
- Trên máy Windows này: build được `android` và `windows`. `ios`/`maccatalyst` cần Mac build host — đừng thử.

```powershell
dotnet build src/Client -f net10.0-windows10.0.19041.0 -t:Run
dotnet build src/Client -f net10.0-android -t:Run
```

Các property khác đáng chú ý trong `Client.csproj`:

| Property | Ý nghĩa |
|---|---|
| `ApplicationId` | ID định danh app (`com.company.app`) — đổi sau khi publish = app khác |
| `ApplicationDisplayName` | Tên hiển thị dưới icon |
| `ApplicationVersion` | Số build (tăng dần, số nguyên) |
| `ApplicationDisplayVersion` | Version người dùng thấy (`1.2.0`) |
| `<MauiImage>`, `<MauiFont>`, `<MauiIcon>` | Khai báo asset để MAUI tự resize cho mọi độ phân giải |

### Chuỗi khởi động: `MauiProgram` → `App` → `AppShell` → `Page`

Đây là thứ tự bạn cần thuộc để biết đặt code ở đâu.

**1. `MauiProgram.cs`** — tương đương `Program.cs` của Api. Đăng ký DI ở đây:

```csharp
public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts => fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular"));

        // HttpClient dùng chung, base URL theo platform (xem §9)
        builder.Services.AddSingleton(sp => new HttpClient { BaseAddress = new Uri(ApiConfig.BaseUrl) });
        builder.Services.AddSingleton<IProductApi, ProductApi>();

        // ViewModel và Page đều phải đăng ký thì Shell mới resolve được
        builder.Services.AddTransient<ProductListViewModel>();
        builder.Services.AddTransient<ProductListPage>();

        return builder.Build();
    }
}
```

**2. `App.xaml` / `App.xaml.cs`** — đại diện cho ứng dụng. `App.xaml` chứa **resource toàn cục** (màu, style, dictionary) mà mọi page dùng được. `App.xaml.cs` tạo cửa sổ chính:

```csharp
protected override Window CreateWindow(IActivationState? state) => new Window(new AppShell());
```

**3. `AppShell.xaml`** — khung điều hướng: tab bar, flyout menu, và **bảng đăng ký route**:

```xml
<ShellContent Title="Sản phẩm" ContentTemplate="{DataTemplate views:ProductListPage}" Route="products" />
```

Route cho phép điều hướng bằng chuỗi, kèm tham số:

```csharp
await Shell.Current.GoToAsync($"productdetail?id={product.Id}");
```

**4. `Views/` + `ViewModels/`** — màn hình cụ thể.

### MVVM — tại sao tách View và ViewModel

MAUI đi kèm MVVM gần như bắt buộc. Ý tưởng:

- **View** (`.xaml`): chỉ mô tả *trông như thế nào*. Không chứa logic.
- **ViewModel**: chứa *dữ liệu và hành vi* của màn hình. Không biết gì về XAML, không tham chiếu `Button` hay `Label`.
- **Binding**: sợi dây nối hai bên. ViewModel đổi property → UI tự cập nhật (nhờ `INotifyPropertyChanged`).

Lợi ích thật sự: ViewModel **unit test được** vì không cần dựng UI.

Đừng tự viết `INotifyPropertyChanged` bằng tay — dùng `CommunityToolkit.Mvvm` (source generator):

```csharp
public partial class ProductListViewModel : ObservableObject
{
    private readonly IProductApi _api;
    public ProductListViewModel(IProductApi api) => _api = api;

    [ObservableProperty]              // sinh ra property IsBusy + notify
    private bool isBusy;

    public ObservableCollection<ProductDto> Products { get; } = new();

    [RelayCommand]                    // sinh ra LoadCommand để XAML bind vào
    private async Task LoadAsync()
    {
        IsBusy = true;
        try
        {
            var items = await _api.GetProductsAsync();
            Products.Clear();
            foreach (var p in items) Products.Add(p);
        }
        finally { IsBusy = false; }
    }
}
```

Nối vào View qua constructor injection:

```csharp
public partial class ProductListPage : ContentPage
{
    public ProductListPage(ProductListViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;     // ← XAML bind vào cái này
    }
}
```

> `InitializeComponent()` là method được **sinh tự động** từ file `.xaml` cùng tên. Đó là lý do class phải có từ khóa `partial`.

### Thư mục `Platforms/` — khi nào bạn phải mở nó

Chứa code và file cấu hình **riêng cho từng OS**. MAUI tự động chỉ biên dịch thư mục khớp với target đang build.

| File | Việc bạn sẽ làm ở đó |
|---|---|
| `Platforms/Android/AndroidManifest.xml` | Xin quyền (INTERNET, CAMERA), đặt tên app |
| `Platforms/Android/MainActivity.cs` | Điểm vào Android |
| `Platforms/iOS/Info.plist` | Xin quyền + mô tả lý do (Apple bắt buộc) |
| `Platforms/Windows/Package.appxmanifest` | Capability của app Windows |

Ngoài ra có **conditional compilation** khi cần code khác nhau theo platform:

```csharp
#if ANDROID
    var baseUrl = "https://10.0.2.2:7001";
#elif WINDOWS
    var baseUrl = "https://localhost:7001";
#endif
```

### `Resources/` — asset

| Thư mục | Nội dung |
|---|---|
| `Resources/AppIcon/` | Icon nguồn (SVG) → MAUI sinh mọi kích thước |
| `Resources/Splash/` | Màn hình chờ |
| `Resources/Images/` | Ảnh trong app |
| `Resources/Fonts/` | Font (phải đăng ký trong `ConfigureFonts`) |
| `Resources/Styles/Colors.xaml`, `Styles.xaml` | Theme, style dùng chung |
| `Resources/Raw/` | File thô (json, db mẫu) đọc bằng `FileSystem.OpenAppPackageFileAsync` |

---

## 7. `tests/` — kiểm thử

- **`Api.Tests`** — test service nghiệp vụ (unit) và test endpoint qua `WebApplicationFactory<Program>` (integration, chạy API thật trong bộ nhớ, không cần mở port).
- **`Client.Tests`** — chỉ test **ViewModel và Service**, không test XAML. Đây chính là lợi ích của MVVM: ViewModel là class C# bình thường, mock `IProductApi` rồi assert.

```powershell
dotnet test
dotnet test tests/Api.Tests
dotnet test --filter "FullyQualifiedName~ProductListViewModelTests"
```

---

## 8. File cấu hình cấp solution

### `global.json` — ghim SDK

Máy này có nhiều SDK (2.0 → 10.0). Không ghim thì một ngày `dotnet build` chọn nhầm bản và lỗi khó hiểu.

```json
{
  "sdk": {
    "version": "10.0.400",
    "rollForward": "latestFeature"
  }
}
```

### `Directory.Build.props` — setting cho mọi project

MSBuild tự nạp file này cho **mọi `.csproj` nằm dưới thư mục chứa nó**. Đặt setting chung ở đây thay vì copy-paste vào từng project:

```xml
<Project>
  <PropertyGroup>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
    <TreatWarningsAsErrors>true</TreatWarningsAsErrors>
    <LangVersion>latest</LangVersion>
  </PropertyGroup>
</Project>
```

`<Nullable>enable</Nullable>` bật kiểm tra null lúc compile: `string` không được null, muốn null phải viết `string?`. Ban đầu hơi phiền nhưng loại bỏ gần hết `NullReferenceException`.

### `.gitignore`

Bắt buộc bỏ qua: `bin/`, `obj/`, `.vs/`, `*.user`, `*.db`. Lấy mẫu chuẩn:

```powershell
dotnet new gitignore
```

---

## 9. Hai cái bẫy chắc chắn bạn sẽ gặp

### Bẫy 1: `localhost` trên Android emulator

Trong Android emulator, `localhost` trỏ về **chính emulator**, không phải máy tính của bạn. Gọi API sẽ timeout và bạn sẽ tưởng API hỏng.

Địa chỉ đúng cho từng nơi:

| Chạy ở đâu | Base URL |
|---|---|
| Windows / MacCatalyst | `https://localhost:7001` |
| Android emulator | `https://10.0.2.2:7001` |
| iOS simulator | `https://localhost:7001` |
| Thiết bị thật | IP LAN của máy dev, ví dụ `https://192.168.1.10:7001` |

Đặt ở **một chỗ duy nhất**, đừng rải rác:

```csharp
// src/Client/Services/ApiConfig.cs
public static class ApiConfig
{
    public static string BaseUrl =>
#if ANDROID
        "https://10.0.2.2:7001";
#else
        "https://localhost:7001";
#endif
}
```

### Bẫy 2: chứng chỉ HTTPS dev bị từ chối

ASP.NET Core dev dùng chứng chỉ self-signed; Android/iOS không tin nó → `HttpRequestException`.

Chỉ bỏ qua validation **trong DEBUG**, và phải bọc `#if DEBUG` — nếu lọt vào Release thì app của bạn dễ bị tấn công man-in-the-middle:

```csharp
public static HttpClient Create()
{
#if DEBUG
    var handler = new HttpClientHandler
    {
        ServerCertificateCustomValidationCallback = (_, _, _, _) => true
    };
    return new HttpClient(handler) { BaseAddress = new Uri(ApiConfig.BaseUrl) };
#else
    return new HttpClient { BaseAddress = new Uri(ApiConfig.BaseUrl) };
#endif
}
```

Bổ sung: Android 9+ chặn HTTP thường (cleartext). Nếu dùng `http://` khi dev, phải khai báo `network_security_config.xml` trong `Platforms/Android/`.

### Bẫy 3 (nhỏ hơn nhưng hay gặp): cập nhật UI sai thread

Kết quả từ background thread không được gán thẳng vào property đang bind:

```csharp
MainThread.BeginInvokeOnMainThread(() => Products.Add(item));
```

Nếu bạn `await` trong ViewModel và không dùng `ConfigureAwait(false)` thì thường đã ở đúng UI thread — nhưng callback từ timer, event native, hoặc `Task.Run` thì phải bọc.

---

## 10. Lộ trình tự học đề xuất

Làm theo thứ tự, mỗi bước chạy được rồi mới sang bước sau.

1. **Dựng khung**: chạy các lệnh ở §2, `dotnet build` xanh. Thêm `global.json` + `Directory.Build.props`.
2. **API trước, client sau**: viết 1 endpoint `GET /api/products` trả list cứng. Test bằng trình duyệt hoặc `.http` file.
3. **Đưa DTO vào `Shared`**, cho Api tham chiếu. Cảm nhận việc đổi DTO làm gì với build.
4. **MAUI chạy trên Windows trước** (nhanh nhất, không cần emulator): hiển thị list cứng bằng `CollectionView`.
5. **Nối dây**: `HttpClient` trong `MauiProgram`, service gọi API, bind vào ViewModel. Đây là lúc bạn gặp Bẫy 1 và 2.
6. **Chuyển sang Android emulator**: sửa base URL, sửa cert. Hiểu tại sao phải sửa.
7. **Thêm điều hướng**: list → detail, truyền tham số qua Shell route.
8. **Thêm database thật** ở Api (EF Core + SQLite), CRUD đầy đủ.
9. **Viết test**: 1 test cho service Api, 1 test cho ViewModel.
10. **Xử lý trạng thái thật**: loading, lỗi mạng, offline, pull-to-refresh.

---

## 11. Bảng tra nhanh: "tôi cần sửa cái này thì mở file nào?"

| Muốn làm | Mở file |
|---|---|
| Đăng ký service/DI cho app MAUI | `src/Client/MauiProgram.cs` |
| Đăng ký service/DI cho API | `src/Api/Program.cs` |
| Thêm endpoint API | `src/Api/Program.cs` hoặc `Endpoints/`, `Controllers/` |
| Đổi connection string | `src/Api/appsettings.json` |
| Đổi port khi dev | `src/Api/Properties/launchSettings.json` |
| Thêm tab / menu / route | `src/Client/AppShell.xaml` |
| Đổi màu, style toàn app | `src/Client/Resources/Styles/Colors.xaml`, `Styles.xaml` |
| Xin quyền Android | `src/Client/Platforms/Android/AndroidManifest.xml` |
| Xin quyền iOS | `src/Client/Platforms/iOS/Info.plist` |
| Đổi icon / splash | `src/Client/Resources/AppIcon/`, `Splash/` + `Client.csproj` |
| Đổi tên app, version | `src/Client/Client.csproj` |
| Sửa DTO dùng chung | `src/Shared/Dtos/` |
| Thêm trang cho web dashboard | `src/Web/Pages/` |
| Đăng ký service/DI cho web | `src/Web/Program.cs` |
| Bật/tắt warning-as-error | `Directory.Build.props` |
| Ghim phiên bản SDK | `global.json` |

---

## 12. Tài liệu chính thức

- .NET MAUI: <https://learn.microsoft.com/dotnet/maui/>
- ASP.NET Core: <https://learn.microsoft.com/aspnet/core/>
- CommunityToolkit.Mvvm: <https://learn.microsoft.com/dotnet/communitytoolkit/mvvm/>
- MSBuild property tham khảo: <https://learn.microsoft.com/visualstudio/msbuild/common-msbuild-project-properties>
