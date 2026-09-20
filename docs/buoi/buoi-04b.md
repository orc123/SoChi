# BUỔI 04B: Quản Lý Danh Mục — CRUD, Chọn Icon/Màu & Đặt Hạn Mức

> **Buổi bổ sung (~2h).** Làm sau Buổi 04, trước Buổi 05. Buổi 11 tính hạn mức chi tiêu dựa trên `Category.MonthlyLimit` — nếu không có màn hình này thì không có chỗ nào nhập giá trị đó, và tab "Danh mục" đã đăng ký trong `AppShell` ở Buổi 02 sẽ mãi là trang trắng.

## 1. Mục tiêu buổi học (Definition of Done)
- `CategoriesPage` liệt kê danh mục, **nhóm theo Chi / Thu**, hiện icon + màu + hạn mức.
- Thêm / sửa danh mục qua modal: tên, icon (emoji), màu, loại, hạn mức tháng.
- Xóa mềm danh mục **có ràng buộc**: danh mục đang được giao dịch sử dụng thì không cho xóa thẳng.
- `MonthlyLimit` có nguồn nhập thật → Buổi 11 dùng được ngay.

## 2. Bản chất kiến trúc & Nguyên lý
- **Danh mục là dữ liệu tham chiếu (reference data)**, khác hẳn giao dịch. Giao dịch trỏ vào nó bằng `CategoryId`. Xóa cứng một danh mục = mọi giao dịch cũ trở thành "mồ côi", hiển thị trống trơn. Vì vậy: **xóa mềm + kiểm tra ràng buộc trước khi xóa**.
- **Vì sao lưu `IconGlyph` là chuỗi emoji chứ không phải đường dẫn ảnh:** emoji là ký tự Unicode — nó tự đổi kích thước, tự theo màu chữ, không tốn asset, không cần resize cho từng mật độ màn hình, và đồng bộ lên server chỉ là vài byte. Đây là lựa chọn thực dụng cho app học tập; app thương mại thường dùng font icon (FontAwesome/Material) theo đúng nguyên tắc đó.
- **Hạn mức lưu ở danh mục, không lưu ở bảng riêng.** Đơn giản hóa có chủ đích: mỗi danh mục một hạn mức tháng, không đổi theo tháng. Nếu sau này muốn hạn mức riêng cho từng tháng, đó là lúc tách bảng `Budget(CategoryId, Month, Limit)` — nhưng đừng làm sớm.

## 3. Các bước thực hành chi tiết

### Bước 4B.1: Mở rộng Repository
Bổ sung vào `src/Client/SoChi.Client/Services/ITransactionRepository.cs`:
```csharp
Task<Category?> GetCategoryByIdAsync(Guid id);
Task<int> SoftDeleteCategoryAsync(Guid id);
Task<int> CountTransactionsByCategoryAsync(Guid categoryId);
```

Triển khai trong `SqliteTransactionRepository.cs`:
```csharp
public async Task<Category?> GetCategoryByIdAsync(Guid id)
{
    var db = await GetDatabaseAsync();
    return await db.Table<Category>().FirstOrDefaultAsync(c => c.Id == id);
}

public async Task<int> CountTransactionsByCategoryAsync(Guid categoryId)
{
    var db = await GetDatabaseAsync();
    return await db.Table<Transaction>()
                   .Where(t => t.CategoryId == categoryId && !t.IsDeleted)
                   .CountAsync();
}

public async Task<int> SoftDeleteCategoryAsync(Guid id)
{
    var db = await GetDatabaseAsync();
    var category = await db.Table<Category>().FirstOrDefaultAsync(c => c.Id == id);
    if (category is null) return 0;

    category.IsDeleted = true;
    category.UpdatedAt = DateTime.UtcNow;   // BẮT BUỘC: server dựa vào mốc này để biết bản ghi đã đổi
    category.SyncState = SyncState.Local;
    return await db.UpdateAsync(category);
}
```

> Ba dòng `IsDeleted` / `UpdatedAt` / `SyncState` luôn đi cùng nhau trong **mọi** thao tác ghi. Quên `UpdatedAt` thì Buổi 14 sẽ coi bản ghi là chưa đổi và không bao giờ đẩy nó lên server.

Lưu ý `GetCategoryByIdAsync` **không** lọc `!IsDeleted`: giao dịch cũ vẫn cần tra ra tên và màu của danh mục đã bị xóa để hiển thị lịch sử. Chỉ danh sách chọn lúc nhập liệu (`GetCategoriesAsync`) mới lọc.

### Bước 4B.2: `CategoriesViewModel`
Tạo `src/Client/SoChi.Client/ViewModels/CategoriesViewModel.cs`:
```csharp
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SoChi.Client.Models;
using SoChi.Client.Services;

namespace SoChi.Client.ViewModels;

public partial class CategoriesViewModel : ObservableObject
{
    private readonly ITransactionRepository _repository;

    public CategoriesViewModel(ITransactionRepository repository) => _repository = repository;

    public ObservableCollection<Category> ExpenseCategories { get; } = new();
    public ObservableCollection<Category> IncomeCategories { get; } = new();

    [ObservableProperty]
    public partial bool IsBusy { get; set; }

    [RelayCommand]
    public async Task LoadAsync()
    {
        if (IsBusy) return;
        IsBusy = true;
        try
        {
            var all = await _repository.GetCategoriesAsync();

            ExpenseCategories.Clear();
            IncomeCategories.Clear();

            foreach (var c in all.Where(c => c.Kind == TransactionKind.Expense).OrderBy(c => c.Name))
                ExpenseCategories.Add(c);

            foreach (var c in all.Where(c => c.Kind == TransactionKind.Income).OrderBy(c => c.Name))
                IncomeCategories.Add(c);
        }
        finally { IsBusy = false; }
    }

    [RelayCommand]
    private Task AddAsync() => Shell.Current.GoToAsync("categoryform");

    [RelayCommand]
    private Task EditAsync(Category category) =>
        Shell.Current.GoToAsync($"categoryform?id={category.Id}");

    [RelayCommand]
    private async Task DeleteAsync(Category category)
    {
        // Ràng buộc: không cho xóa danh mục đang có giao dịch
        var used = await _repository.CountTransactionsByCategoryAsync(category.Id);
        if (used > 0)
        {
            await Shell.Current.DisplayAlert(
                "Không thể xóa",
                $"Danh mục này đang được {used} giao dịch sử dụng. " +
                "Hãy chuyển các giao dịch đó sang danh mục khác trước.",
                "Đã hiểu");
            return;
        }

        var confirmed = await Shell.Current.DisplayAlert(
            "Xóa danh mục", $"Xóa \"{category.Name}\"?", "Xóa", "Hủy");
        if (!confirmed) return;

        await _repository.SoftDeleteCategoryAsync(category.Id);
        await LoadAsync();
    }
}
```

### Bước 4B.3: `CategoriesPage.xaml`
```xml
<ContentPage xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             xmlns:models="clr-namespace:SoChi.Client.Models"
             xmlns:vm="clr-namespace:SoChi.Client.ViewModels"
             x:Class="SoChi.Client.Views.CategoriesPage"
             x:DataType="vm:CategoriesViewModel"
             Title="Danh mục">

    <Grid RowDefinitions="*,Auto">
        <ScrollView>
            <VerticalStackLayout Padding="16" Spacing="12">

                <Label Text="KHOẢN CHI" FontSize="12" FontAttributes="Bold" Opacity="0.6" />
                <CollectionView ItemsSource="{Binding ExpenseCategories}">
                    <CollectionView.ItemTemplate>
                        <DataTemplate x:DataType="models:Category">
                            <SwipeView>
                                <SwipeView.RightItems>
                                    <SwipeItems Mode="Execute">
                                        <SwipeItem Text="Xóa" BackgroundColor="#EF4444"
                                                   Command="{Binding Source={RelativeSource AncestorType={x:Type vm:CategoriesViewModel}}, Path=DeleteCommand}"
                                                   CommandParameter="{Binding .}" />
                                    </SwipeItems>
                                </SwipeView.RightItems>

                                <Grid ColumnDefinitions="44,*,Auto" Padding="8" ColumnSpacing="12">
                                    <Border Grid.Column="0" WidthRequest="40" HeightRequest="40"
                                            BackgroundColor="{Binding ColorHex}"
                                            StrokeThickness="0"
                                            StrokeShape="RoundRectangle 20">
                                        <Label Text="{Binding IconGlyph}" FontSize="18"
                                               HorizontalOptions="Center" VerticalOptions="Center" />
                                    </Border>

                                    <VerticalStackLayout Grid.Column="1" VerticalOptions="Center">
                                        <Label Text="{Binding Name}" FontAttributes="Bold" />
                                        <Label FontSize="12" Opacity="0.6"
                                               Text="{Binding MonthlyLimit, StringFormat='Hạn mức: {0:N0} đ'}"
                                               IsVisible="{Binding MonthlyLimit, Converter={StaticResource NotNullConverter}}" />
                                    </VerticalStackLayout>

                                    <Label Grid.Column="2" Text="&gt;" FontSize="20" VerticalOptions="Center" Opacity="0.4" />

                                    <Grid.GestureRecognizers>
                                        <TapGestureRecognizer
                                            Command="{Binding Source={RelativeSource AncestorType={x:Type vm:CategoriesViewModel}}, Path=EditCommand}"
                                            CommandParameter="{Binding .}" />
                                    </Grid.GestureRecognizers>
                                </Grid>
                            </SwipeView>
                        </DataTemplate>
                    </CollectionView.ItemTemplate>
                </CollectionView>

                <Label Text="KHOẢN THU" FontSize="12" FontAttributes="Bold" Opacity="0.6" Margin="0,16,0,0" />
                <CollectionView ItemsSource="{Binding IncomeCategories}">
                    <!-- Dùng lại y hệt ItemTemplate ở trên.
                         Cách gọn hơn: tách DataTemplate ra ResourceDictionary của trang
                         rồi tham chiếu bằng ItemTemplate="{StaticResource CategoryItemTemplate}" -->
                </CollectionView>
            </VerticalStackLayout>
        </ScrollView>

        <Button Grid.Row="1" Text="+ Thêm danh mục" Margin="16" Command="{Binding AddCommand}" />
    </Grid>
</ContentPage>
```

> **Bài tập nhỏ trong bước này:** đừng copy-paste `DataTemplate` hai lần. Tách nó vào `<ContentPage.Resources>` với `x:Key="CategoryItemTemplate"`, rồi cả hai `CollectionView` cùng tham chiếu. Đây là cách tái sử dụng template chuẩn trong XAML.

Code-behind gọi `LoadAsync` mỗi lần trang hiện lên (danh mục có thể vừa được sửa ở modal):
```csharp
public partial class CategoriesPage : ContentPage
{
    private readonly CategoriesViewModel _vm;

    public CategoriesPage(CategoriesViewModel vm)
    {
        InitializeComponent();
        BindingContext = _vm = vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _vm.LoadAsync();
    }
}
```

### Bước 4B.4: `CategoryFormViewModel` — nhận tham số điều hướng
Đây là chỗ học `IQueryAttributable`: cách Shell truyền tham số qua route `categoryform?id=...`.

```csharp
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SoChi.Client.Models;
using SoChi.Client.Services;

namespace SoChi.Client.ViewModels;

public partial class CategoryFormViewModel : ObservableObject, IQueryAttributable
{
    private readonly ITransactionRepository _repository;
    private Guid _editingId = Guid.Empty;

    public CategoryFormViewModel(ITransactionRepository repository) => _repository = repository;

    public string[] IconChoices { get; } =
        ["🍔", "🚗", "🏠", "🛍️", "⚡", "🎬", "💊", "📚", "✈️", "🎁", "☕", "👕", "💰", "💵", "🏦", "📈"];

    public string[] ColorChoices { get; } =
        ["#EF4444", "#F59E0B", "#3B82F6", "#EC4899", "#8B5CF6", "#10B981", "#06B6D4", "#64748B"];

    [ObservableProperty] public partial string Name { get; set; } = string.Empty;
    [ObservableProperty] public partial string SelectedIcon { get; set; } = "🍔";
    [ObservableProperty] public partial string SelectedColor { get; set; } = "#3B82F6";
    [ObservableProperty] public partial bool IsIncome { get; set; }
    [ObservableProperty] public partial string MonthlyLimitText { get; set; } = string.Empty;
    [ObservableProperty] public partial string Title { get; set; } = "Thêm danh mục";

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (!query.TryGetValue("id", out var raw) || !Guid.TryParse(raw?.ToString(), out var id))
            return;

        _editingId = id;
        Title = "Sửa danh mục";
        _ = LoadExistingAsync(id);
    }

    private async Task LoadExistingAsync(Guid id)
    {
        var c = await _repository.GetCategoryByIdAsync(id);
        if (c is null) return;

        Name = c.Name;
        SelectedIcon = c.IconGlyph;
        SelectedColor = c.ColorHex;
        IsIncome = c.Kind == TransactionKind.Income;
        MonthlyLimitText = c.MonthlyLimit?.ToString("N0") ?? string.Empty;
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        if (string.IsNullOrWhiteSpace(Name))
        {
            await Shell.Current.DisplayAlert("Thiếu thông tin", "Tên danh mục không được để trống.", "OK");
            return;
        }

        long? limit = null;
        var digits = new string(MonthlyLimitText.Where(char.IsDigit).ToArray());
        if (digits.Length > 0 && long.TryParse(digits, out var parsed) && parsed > 0)
            limit = parsed;

        var category = _editingId == Guid.Empty
            ? new Category()
            : await _repository.GetCategoryByIdAsync(_editingId) ?? new Category();

        category.Name = Name.Trim();
        category.IconGlyph = SelectedIcon;
        category.ColorHex = SelectedColor;
        category.Kind = IsIncome ? TransactionKind.Income : TransactionKind.Expense;
        category.MonthlyLimit = limit;

        await _repository.SaveCategoryAsync(category);   // hàm này đã tự set UpdatedAt + SyncState
        await Shell.Current.GoToAsync("..");
    }

    [RelayCommand]
    private Task CancelAsync() => Shell.Current.GoToAsync("..");
}
```

### Bước 4B.5: `CategoryFormPage.xaml` — lưới chọn icon và màu
Phần đáng học nhất là dùng `CollectionView` với `SelectionMode="Single"` làm bộ chọn, thay vì `Picker` mặc định:

```xml
<VerticalStackLayout Padding="16" Spacing="16">

    <Entry Placeholder="Tên danh mục" Text="{Binding Name}" MaxLength="30" />

    <Label Text="Biểu tượng" FontAttributes="Bold" />
    <CollectionView ItemsSource="{Binding IconChoices}"
                    SelectedItem="{Binding SelectedIcon}"
                    SelectionMode="Single"
                    HeightRequest="120">
        <CollectionView.ItemsLayout>
            <GridItemsLayout Orientation="Vertical" Span="8" />
        </CollectionView.ItemsLayout>
        <CollectionView.ItemTemplate>
            <DataTemplate x:DataType="x:String">
                <Label Text="{Binding .}" FontSize="24" Margin="6"
                       HorizontalOptions="Center" VerticalOptions="Center" />
            </DataTemplate>
        </CollectionView.ItemTemplate>
    </CollectionView>

    <Label Text="Màu" FontAttributes="Bold" />
    <CollectionView ItemsSource="{Binding ColorChoices}"
                    SelectedItem="{Binding SelectedColor}"
                    SelectionMode="Single"
                    HeightRequest="60">
        <CollectionView.ItemsLayout>
            <GridItemsLayout Orientation="Horizontal" />
        </CollectionView.ItemsLayout>
        <CollectionView.ItemTemplate>
            <DataTemplate x:DataType="x:String">
                <Border BackgroundColor="{Binding .}" WidthRequest="36" HeightRequest="36"
                        Margin="6" StrokeThickness="0" StrokeShape="RoundRectangle 18" />
            </DataTemplate>
        </CollectionView.ItemTemplate>
    </CollectionView>

    <HorizontalStackLayout Spacing="12">
        <Label Text="Là khoản thu" VerticalOptions="Center" />
        <Switch IsToggled="{Binding IsIncome}" />
    </HorizontalStackLayout>

    <Entry Placeholder="Hạn mức tháng (để trống nếu không đặt)"
           Text="{Binding MonthlyLimitText}"
           Keyboard="Numeric" />

    <Grid ColumnDefinitions="*,*" ColumnSpacing="12">
        <Button Grid.Column="0" Text="Hủy" Command="{Binding CancelCommand}" />
        <Button Grid.Column="1" Text="Lưu" Command="{Binding SaveCommand}" />
    </Grid>
</VerticalStackLayout>
```

### Bước 4B.6: Đăng ký Route và DI
Trong `AppShell.xaml.cs`:
```csharp
Routing.RegisterRoute("categoryform", typeof(Views.CategoryFormPage));
```

Trong `MauiProgram.cs`:
```csharp
builder.Services.AddTransient<CategoriesViewModel>();
builder.Services.AddTransient<CategoryFormViewModel>();
builder.Services.AddTransient<CategoriesPage>();
builder.Services.AddTransient<CategoryFormPage>();
```

## 4. Bẫy thường gặp (Pitfalls)
- **Lỗi:** `Command` đặt trong `DataTemplate` không chạy khi bấm.
  - **Nguyên nhân:** Bên trong `DataTemplate`, `BindingContext` là **một `Category`**, không phải ViewModel. Phải trỏ ngược lên bằng `RelativeSource AncestorType` như code ở trên.
- **Lỗi:** Sửa danh mục xong quay lại danh sách vẫn thấy dữ liệu cũ.
  - **Nguyên nhân:** `CategoriesPage` vẫn nằm trong stack điều hướng nên constructor không chạy lại. Phải reload trong `OnAppearing()`.
- **Lỗi:** Xóa danh mục xong, các giao dịch cũ hiển thị trống tên và mất màu.
  - **Nguyên nhân:** Hàm tra cứu danh mục lọc `!IsDeleted` nên không tìm thấy. Xem ghi chú ở Bước 4B.1 — tra cứu để hiển thị lịch sử phải bỏ điều kiện đó.
- **Lỗi:** `ApplyQueryAttributes` cần `await` nhưng chữ ký là `void`.
  - **Nguyên nhân:** Interface quy định vậy. Dùng `_ = LoadExistingAsync(id)` rồi để binding tự cập nhật khi dữ liệu về — đây chính là lợi thế của MVVM so với gán trực tiếp vào control.

## 5. Checklist nghiệm thu Buổi 04B
- [ ] Tab "Danh mục" hiện đủ 8 danh mục seed, chia đúng hai nhóm Chi / Thu.
- [ ] Thêm danh mục mới "Thú cưng 🐶" màu tím → xuất hiện ngay trong danh sách **và** trong bộ chọn danh mục ở form giao dịch.
- [ ] Sửa hạn mức của "Ăn uống" thành 3.000.000 → tắt mở lại app vẫn còn.
- [ ] Vuốt xóa một danh mục **đang có giao dịch** → hiện cảnh báo, không xóa.
- [ ] Vuốt xóa một danh mục **chưa dùng** → xóa được.
- [ ] Giao dịch cũ thuộc danh mục đã xóa vẫn hiển thị đúng tên và màu, không bị trống.

---

🏠 [Mục lục toàn khóa](../HUONG-DAN-TUNG-BUOI.md)

⬅️ [BUỔI 04: Seed Data, Form Thêm/Sửa & Hiển Thị CollectionView](buoi-04.md)

[BUỔI 05: Danh Sách Nâng Cao: Grouping Theo Ngày, SwipeView & RefreshView](buoi-05.md) ➡️
