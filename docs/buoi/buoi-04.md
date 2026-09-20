# BUỔI 04: Seed Data, Form Thêm/Sửa & Hiển Thị CollectionView

**Thuộc chặng:** Chặng 2 (Phần 2) | **Thời lượng:** ~3 giờ

## 1. Mục tiêu buổi học (Definition of Done)
- Tự động Seed 8 danh mục chi tiêu/thu nhập mặc định (Ăn uống, Di chuyển, Nhà cửa, Mua sắm, Lương...) khi khởi chạy lần đầu.
- Màn hình **Thêm giao dịch** (`TransactionFormPage`): Cho phép chọn danh mục, nhập số tiền, chọn ngày, ghi chú và bấm "Lưu".
- Dữ liệu được ghi thẳng vào SQLite cục bộ.
- Màn hình **Giao dịch** (`TransactionsPage`): Sử dụng `CollectionView` để hiển thị danh sách các giao dịch đọc từ database.
- Tắt hẳn ứng dụng -> Mở lại app -> Các giao dịch vừa thêm vẫn xuất hiện nguyên vẹn.

## 2. Bản chất kiến trúc & Nguyên lý
- **Data Seeding an toàn:** Kiểm tra số lượng bản ghi danh mục trong database trước khi chèn (`if (count == 0)`). Tránh việc mỗi lần khởi động app lại sinh thêm các danh mục trùng lặp.
- **`ObservableCollection<T>` vs `List<T>`:** Khi binding dữ liệu lên giao diện `CollectionView`, luôn dùng `ObservableCollection<T>` vì nó phát sinh sự kiện `CollectionChanged` khi thêm/xóa phần tử, giúp UI tự cập nhật mà không cần gọi lại `Reload`.

## 3. Các bước thực hành chi tiết

### Bước 4.1: Viết phương thức Seeding danh mục mặc định
Thêm phương thức Seed vào `SqliteTransactionRepository.cs`:
```csharp
public async Task SeedDefaultCategoriesAsync()
{
    var db = await GetDatabaseAsync();
    var count = await db.Table<Category>().CountAsync();
    if (count > 0) return;

    var defaults = new List<Category>
    {
        new() { Name = "Ăn uống", IconGlyph = "🍔", ColorHex = "#EF4444", Kind = TransactionKind.Expense },
        new() { Name = "Di chuyển", IconGlyph = "🚗", ColorHex = "#F59E0B", Kind = TransactionKind.Expense },
        new() { Name = "Nhà cửa", IconGlyph = "🏠", ColorHex = "#3B82F6", Kind = TransactionKind.Expense },
        new() { Name = "Mua sắm", IconGlyph = "🛍️", ColorHex = "#EC4899", Kind = TransactionKind.Expense },
        new() { Name = "Hóa đơn & Dịch vụ", IconGlyph = "⚡", ColorHex = "#8B5CF6", Kind = TransactionKind.Expense },
        new() { Name = "Giải trí", IconGlyph = "🎬", ColorHex = "#10B981", Kind = TransactionKind.Expense },
        new() { Name = "Tiền lương", IconGlyph = "💰", ColorHex = "#10B981", Kind = TransactionKind.Income },
        new() { Name = "Thu nhập khác", IconGlyph = "💵", ColorHex = "#06B6D4", Kind = TransactionKind.Income }
    };

    await db.InsertAllAsync(defaults);
}
```
Gọi `SeedDefaultCategoriesAsync()` ngay trong hàm `InitializeAsync()`.

### Bước 4.2: Xây dựng ViewModel và View cho Form Thêm/Sửa
Cập nhật `TransactionFormViewModel.cs`:
```csharp
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SoChi.Client.Models;
using SoChi.Client.Services;
using System.Collections.ObjectModel;

namespace SoChi.Client.ViewModels;

public partial class TransactionFormViewModel : ObservableObject
{
    private readonly ITransactionRepository _repository;

    [ObservableProperty] public partial long Amount { get; set; }
    [ObservableProperty] public partial string Note { get; set; } = string.Empty;
    [ObservableProperty] public partial DateTime OccurredOn { get; set; } = DateTime.Today;
    [ObservableProperty] public partial Category? SelectedCategory { get; set; }
    [ObservableProperty] public partial TransactionKind SelectedKind { get; set; } = TransactionKind.Expense;

    public ObservableCollection<Category> Categories { get; } = new();

    public TransactionFormViewModel(ITransactionRepository repository)
    {
        _repository = repository;
    }

    public async Task LoadCategoriesAsync()
    {
        Categories.Clear();
        var list = await _repository.GetCategoriesAsync();
        foreach (var item in list.Where(c => c.Kind == SelectedKind))
        {
            Categories.Add(item);
        }
        SelectedCategory = Categories.FirstOrDefault();
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        if (Amount <= 0)
        {
            await Shell.Current.DisplayAlert("Lỗi", "Vui lòng nhập số tiền hợp lệ lớn hơn 0", "Đóng");
            return;
        }

        if (SelectedCategory == null)
        {
            await Shell.Current.DisplayAlert("Lỗi", "Vui lòng chọn một danh mục", "Đóng");
            return;
        }

        var transaction = new Transaction
        {
            Amount = Amount,
            CategoryId = SelectedCategory.Id,
            OccurredOn = OccurredOn,
            Note = Note
        };

        await _repository.SaveTransactionAsync(transaction);
        await Shell.Current.GoToAsync("..");
    }
}
```

Thiết kế giao diện `TransactionFormPage.xaml`:
```xml
<?xml version="1.0" encoding="utf-8" ?>
<ContentPage xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             xmlns:vm="clr-namespace:SoChi.Client.ViewModels"
             xmlns:models="clr-namespace:SoChi.Client.Models"
             x:Class="SoChi.Client.Views.TransactionFormPage"
             x:DataType="vm:TransactionFormViewModel"
             Title="Thêm Giao Dịch">
    <ScrollView>
        <VerticalStackLayout Padding="20" Spacing="16">
            <Label Text="Số tiền (VNĐ)" FontAttributes="Bold" />
            <Entry Text="{Binding Amount}" Keyboard="Numeric" Placeholder="Nhập số tiền" FontSize="24" />

            <Label Text="Danh mục" FontAttributes="Bold" />
            <Picker ItemsSource="{Binding Categories}"
                    ItemDisplayBinding="{Binding Name}"
                    SelectedItem="{Binding SelectedCategory}" />

            <Label Text="Ngày chi/thu" FontAttributes="Bold" />
            <DatePicker Date="{Binding OccurredOn}" />

            <Label Text="Ghi chú" FontAttributes="Bold" />
            <Entry Text="{Binding Note}" Placeholder="Ví dụ: Ăn trưa phở bò" />

            <Button Text="Lưu giao dịch" Command="{Binding SaveCommand}"
                    BackgroundColor="#3B82F6" TextColor="White" HeightRequest="50" Margin="0,20,0,0" />
        </VerticalStackLayout>
    </ScrollView>
</ContentPage>
```

Trong `TransactionFormPage.xaml.cs`:
```csharp
protected override async void OnAppearing()
{
    base.OnAppearing();
    if (BindingContext is TransactionFormViewModel vm)
    {
        await vm.LoadCategoriesAsync();
    }
}
```

### Bước 4.3: Hiển thị danh sách trên `TransactionsPage`
Trong `TransactionsViewModel.cs`:
```csharp
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SoChi.Client.Models;
using SoChi.Client.Services;
using System.Collections.ObjectModel;

namespace SoChi.Client.ViewModels;

public partial class TransactionsViewModel : ObservableObject
{
    private readonly ITransactionRepository _repository;
    public ObservableCollection<Transaction> Transactions { get; } = new();

    public TransactionsViewModel(ITransactionRepository repository)
    {
        _repository = repository;
    }

    [RelayCommand]
    public async Task LoadTransactionsAsync()
    {
        Transactions.Clear();
        var items = await _repository.GetTransactionsAsync();
        foreach (var item in items)
        {
            Transactions.Add(item);
        }
    }
}
```

Trong `TransactionsPage.xaml`:
```xml
<CollectionView ItemsSource="{Binding Transactions}">
    <CollectionView.ItemTemplate>
        <DataTemplate x:DataType="models:Transaction">
            <Border StrokeThickness="1" Stroke="#E5E7EB" Padding="12" Margin="10,4">
                <Grid ColumnDefinitions="*, Auto">
                    <VerticalStackLayout Grid.Column="0">
                        <Label Text="{Binding Note}" FontAttributes="Bold" FontSize="16" />
                        <Label Text="{Binding OccurredOn, StringFormat='{0:dd/MM/yyyy}'}" TextColor="Gray" FontSize="12" />
                    </VerticalStackLayout>
                    <Label Grid.Column="1" Text="{Binding Amount, StringFormat='{0:N0} đ'}"
                           FontAttributes="Bold" FontSize="16" VerticalOptions="Center" />
                </Grid>
            </Border>
        </DataTemplate>
    </CollectionView.ItemTemplate>
</CollectionView>
```

## 4. Bẫy thường gặp (Pitfalls)
- **Lỗi:** Thêm giao dịch xong, đóng modal quay lại Tab Giao dịch nhưng màn hình không hiển thị bản ghi mới.
  - **Nguyên nhân:** Tab Giao dịch đã được khởi tạo trước đó và không tự động tải lại dữ liệu.
  - **Khắc phục:** Gọi `LoadTransactionsAsync()` trong sự kiện `OnAppearing` của `TransactionsPage.xaml.cs`.

## 5. Checklist nghiệm thu Buổi 04
- [ ] Khi khởi động app lần đầu, bảng `Categories` có 8 bản ghi mẫu.
- [ ] Nhập giao dịch 50.000đ, chọn danh mục Ăn uống, bấm "Lưu giao dịch" -> Modal tự đóng.
- [ ] Tab Giao dịch hiển thị bản ghi vừa lưu.
- [ ] Tắt hẳn app (hoặc dừng debug), sau đó mở lại app -> Bản ghi 50.000đ vẫn hiển thị.

---

🏠 [Mục lục toàn khóa](../HUONG-DAN-TUNG-BUOI.md)

⬅️ [BUỔI 03: Mô Hình Dữ Liệu SQLite, Repository Pattern & Lazy Init](buoi-03.md)

[BUỔI 04B: Quản Lý Danh Mục — CRUD, Chọn Icon/Màu & Đặt Hạn Mức](buoi-04b.md) ➡️
