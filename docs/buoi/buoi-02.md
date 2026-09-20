# BUỔI 02: AppShell 5 Tabs, Modal Navigation & Dependency Injection

**Thuộc chặng:** Chặng 1 | **Thời lượng:** ~3 giờ

## 1. Mục tiêu buổi học (Definition of Done)
- Thiết lập `AppShell.xaml` chứa đầy đủ 5 Tabs: **Tổng quan, Giao dịch, Thống kê, Danh mục, Cài đặt**.
- Tạo 5 `ContentPage` tương ứng trong thư mục `Views/` và 5 `ViewModel` tương ứng trong `ViewModels/`.
- Đăng ký toàn bộ Page và ViewModel vào DI Container trong `MauiProgram.cs`.
- Tạo trang **Thêm/Sửa giao dịch** (`TransactionFormPage`), đăng ký Route và mở dưới dạng Modal Popup (`Shell.Current.GoToAsync("//...")`).
- Không có bất kỳ dòng lệnh nào khởi tạo thủ công `new SomeViewModel()` trong Code-behind (`.xaml.cs`).

## 2. Bản chất kiến trúc & Nguyên lý
- **Cơ chế Constructor Injection với Shell:** MAUI Shell sử dụng `DataTemplate` để tạo trang. Khi một Page được khai báo trong `MauiProgram.cs`, MAUI sẽ tự động gọi DI ServiceProvider để khởi tạo Page đó cùng các dependency được khai báo trong constructor của nó (ViewModel, Services).
- **Tránh Memory Leak:** Page và ViewModel cho các màn hình Tab thông thường đăng ký `Transient` hoặc `Singleton` (thường Tab chính dùng `Transient` kết hợp giữ state tại ViewModel nếu cần, hoặc `Singleton` cho trang tĩnh). Trang Modal nên dùng `AddTransient` để giải phóng bộ nhớ khi đóng lại.
- **Route Navigation:** Trang modal không nằm trực tiếp trong thanh TabBar mà được đăng ký qua `Routing.RegisterRoute("ten_route", typeof(TransactionFormPage))`.

## 3. Các bước thực hành chi tiết

### Bước 2.1: Tạo 5 ViewModel cơ bản dùng CommunityToolkit.Mvvm
Ví dụ tạo `OverviewViewModel.cs` trong `src/Client/SoChi.Client/ViewModels/`:
```csharp
using CommunityToolkit.Mvvm.ComponentModel;

namespace SoChi.Client.ViewModels;

public partial class OverviewViewModel : ObservableObject
{
    [ObservableProperty]
    public partial string Title { get; set; } = "Tổng Quan";
}
```
Tạo tương tự cho:
- `TransactionsViewModel.cs` (Tab Giao dịch)
- `StatisticsViewModel.cs` (Tab Thống kê)
- `CategoriesViewModel.cs` (Tab Danh mục)
- `SettingsViewModel.cs` (Tab Cài đặt)
- `TransactionFormViewModel.cs` (Modal Thêm/Sửa)

> ### ⚠️ Phải dùng `partial property`, không dùng field
>
> Đây là thay đổi mà hầu hết tutorial trên mạng chưa cập nhật, và nó sẽ làm bạn mất thời gian nếu không biết trước.
>
> Từ CommunityToolkit.Mvvm 8.4, đặt `[ObservableProperty]` lên một **field** sẽ sinh ra:
>
> ```
> error MVVMTK0045: The field ..._title using [ObservableProperty] will generate code
> that is not AOT compatible in WinRT scenarios (such as UWP XAML and WinUI 3 apps),
> and a partial property should be used instead
> ```
>
> Hai điều khiến lỗi này khó chẩn đoán:
>
> 1. **Chỉ nổ ở target Windows.** WinUI 3 chạy trên WinRT, còn Android/iOS/MacCatalyst thì không. Nên `dotnet build SoChi.slnx` có thể xanh, mà build riêng Windows lại đỏ — dễ tưởng hỏng máy hay hỏng cache.
> 2. **Bản chất nó là *warning*, nhưng thành *error*** vì `TreatWarningsAsErrors=true` đã bật từ Buổi 00.
>
> Cách viết đúng — đổi `private` thành `public partial`, tên sang PascalCase, thêm `{ get; set; }`. Giá trị mặc định giữ nguyên tại chỗ:
>
> ```csharp
> // ❌ Cách cũ (mọi blog trước 2025 đều viết thế này)
> [ObservableProperty]
> private string _title = "Tổng Quan";
>
> // ✅ Cách đúng với .NET 10 + toolkit 8.4 trở lên
> [ObservableProperty]
> public partial string Title { get; set; } = "Tổng Quan";
> ```
>
> Class vẫn phải khai báo `partial` như cũ. **Tên property dùng trong XAML và trong code không đổi** — trước đây toolkit cũng sinh ra đúng tên PascalCase đó từ field (`_title` → `Title`), chỉ khác là giờ bạn viết nó ra tường minh. Vì vậy mọi `{Binding Title}` trong tài liệu này vẫn đúng nguyên.
>
> Nếu đang theo một tutorial cũ và muốn giữ cú pháp field, có thể tắt cảnh báo bằng `<NoWarn>$(NoWarn);MVVMTK0045</NoWarn>` trong `SoChi.Client.csproj`. Nhưng đừng làm vậy: partial property là hướng đi chính thức, và bạn sẽ cần nó khi bật AOT ở Buổi 15.

### Bước 2.2: Tạo các trang Views và gán BindingContext qua DI
Ví dụ tạo `OverviewPage.xaml` trong `src/Client/SoChi.Client/Views/`:
```xml
<?xml version="1.0" encoding="utf-8" ?>
<ContentPage xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             xmlns:vm="clr-namespace:SoChi.Client.ViewModels"
             x:Class="SoChi.Client.Views.OverviewPage"
             x:DataType="vm:OverviewViewModel"
             Title="{Binding Title}">
    <VerticalStackLayout Spacing="20" Padding="20" VerticalOptions="Center">
        <Label Text="Màn hình Tổng Quan" HorizontalOptions="Center" FontSize="20" />
    </VerticalStackLayout>
</ContentPage>
```

Trong `OverviewPage.xaml.cs`:
```csharp
using SoChi.Client.ViewModels;

namespace SoChi.Client.Views;

public partial class OverviewPage : ContentPage
{
    public OverviewPage(OverviewViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel; // DI tự inject ViewModel
    }
}
```

### Bước 2.3: Xây dựng AppShell.xaml với TabBar 5 tabs
Cập nhật `src/Client/SoChi.Client/AppShell.xaml`:
```xml
<?xml version="1.0" encoding="UTF-8" ?>
<Shell
    x:Class="SoChi.Client.AppShell"
    xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
    xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
    xmlns:views="clr-namespace:SoChi.Client.Views"
    Title="SoChi">

    <TabBar>
        <Tab Title="Tổng quan" Icon="icon_overview.png">
            <ShellContent ContentTemplate="{DataTemplate views:OverviewPage}" Route="overview" />
        </Tab>
        <Tab Title="Giao dịch" Icon="icon_transactions.png">
            <ShellContent ContentTemplate="{DataTemplate views:TransactionsPage}" Route="transactions" />
        </Tab>
        <Tab Title="Thống kê" Icon="icon_statistics.png">
            <ShellContent ContentTemplate="{DataTemplate views:StatisticsPage}" Route="statistics" />
        </Tab>
        <Tab Title="Danh mục" Icon="icon_categories.png">
            <ShellContent ContentTemplate="{DataTemplate views:CategoriesPage}" Route="categories" />
        </Tab>
        <Tab Title="Cài đặt" Icon="icon_settings.png">
            <ShellContent ContentTemplate="{DataTemplate views:SettingsPage}" Route="settings" />
        </Tab>
    </TabBar>
</Shell>
```

### Bước 2.4: Đăng ký Route cho Modal trong `AppShell.xaml.cs`
```csharp
using SoChi.Client.Views;

namespace SoChi.Client;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        Routing.RegisterRoute("transaction_form", typeof(TransactionFormPage));
    }
}
```

### Bước 2.5: Đăng ký DI trong `MauiProgram.cs`
```csharp
// Đăng ký ViewModels
builder.Services.AddTransient<OverviewViewModel>();
builder.Services.AddTransient<TransactionsViewModel>();
builder.Services.AddTransient<StatisticsViewModel>();
builder.Services.AddTransient<CategoriesViewModel>();
builder.Services.AddTransient<SettingsViewModel>();
builder.Services.AddTransient<TransactionFormViewModel>();

// Đăng ký Pages
builder.Services.AddTransient<OverviewPage>();
builder.Services.AddTransient<TransactionsPage>();
builder.Services.AddTransient<StatisticsPage>();
builder.Services.AddTransient<CategoriesPage>();
builder.Services.AddTransient<SettingsPage>();
builder.Services.AddTransient<TransactionFormPage>();
```

### Bước 2.6: Thử nghiệm mở và đóng Modal
Trong `OverviewViewModel.cs`:
```csharp
[RelayCommand]
private async Task OpenAddTransactionAsync()
{
    await Shell.Current.GoToAsync("transaction_form");
}
```
Trong `TransactionFormViewModel.cs`:
```csharp
[RelayCommand]
private async Task CancelAsync()
{
    await Shell.Current.GoToAsync("..");
}
```

## 4. Bẫy thường gặp (Pitfalls)
- **Lỗi:** `System.InvalidOperationException: Unable to resolve service for type 'SoChi.Client.Views.OverviewPage'`.
  - **Nguyên nhân:** Quên thêm `builder.Services.AddTransient<OverviewPage>()` trong `MauiProgram.cs`. Mọi trang có tham số trong constructor đều phải khai báo DI.
- **Lỗi:** Bấm nút mở Modal nhưng app đứng im hoặc ném ngoại lệ `Route not found`.
  - **Nguyên nhân:** Tên route truyền vào `GoToAsync("transaction_form")` không khớp chính xác với chuỗi đăng ký trong `Routing.RegisterRoute`.

- **Lỗi:** Nút bấm không có phản ứng gì cả — không lỗi, không exception, không log.
  - **Nguyên nhân phổ biến nhất:** constructor của Page inject **nhầm ViewModel**. Ví dụ `TransactionFormPage` nhận `TransactionsViewModel` (ViewModel của tab danh sách) thay vì `TransactionFormViewModel`. Khi đó `BindingContext` là kiểu khác, `{Binding CloseCommand}` không tìm thấy gì, `Button.Command` thành `null`, và bấm không ăn.
  - **Vì sao khó tìm:** binding sai **không ném exception**. MAUI coi đó là chuyện bình thường (dữ liệu có thể chưa sẵn sàng) nên chỉ ghi log rồi bỏ qua. Không có gì đỏ để bạn lần theo.
  - **Dấu hiệu nhận biết sớm:** tiêu đề trang hiển thị sai. Nếu cả hai ViewModel đều có property `Title`, binding vẫn chạy nhưng ra nội dung của ViewModel nhầm — đó chính là manh mối.
  - **Cách kiểm tra nhanh:** đặt breakpoint ngay trong constructor của Page và xem tham số `viewModel` thực sự là kiểu gì. Nguyên tắc chung: nút "bấm không ăn" thì nghi `BindingContext` **trước**, nghi command hay route sau.

## 5. Checklist nghiệm thu Buổi 02
- [ ] Chuyển qua lại giữa 5 tab mượt mà, tiêu đề trên mỗi tab hiển thị đúng.
- [ ] Tại màn hình Tổng quan, bấm nút mở Form Thêm giao dịch -> Màn hình Modal hiển thị.
- [ ] Bấm nút "Hủy / Đóng" trên Modal -> Quay về đúng màn hình trước đó.
- [ ] Kiểm tra toàn bộ code-behind (`.xaml.cs`) không có toán tử `new` với ViewModel.
- [ ] Mỗi Page inject **đúng** ViewModel tương ứng của nó — đối chiếu tên trong constructor với `x:DataType` khai báo trong file `.xaml` cùng tên.

---

🏠 [Mục lục toàn khóa](../HUONG-DAN-TUNG-BUOI.md)

⬅️ [BUỔI 01: Khởi Tạo Solution, Git, Clean Architecture & Khởi Động MVVM](buoi-01.md)

[BUỔI 03: Mô Hình Dữ Liệu SQLite, Repository Pattern & Lazy Init](buoi-03.md) ➡️
