# BUỔI 06: Custom Numeric Keypad, Format VND Realtime & Snackbar Hoàn Tác

**Thuộc chặng:** Chặng 3 (Phần 2) | **Thời lượng:** ~3 giờ

## 1. Mục tiêu buổi học (Definition of Done)
- Loại bỏ hoàn toàn việc dùng thẻ `Entry` bàn phím số mặc định của hệ thống trong màn hình Thêm giao dịch.
- Tự thiết kế một **Custom Numeric Keypad** chuyên dụng bằng `Grid` gồm các nút từ 0-9, nút xóa lùi (`Backspace`) và nút tiện ích `000`.
- Số tiền hiển thị tức thì với định dạng phân cách hàng nghìn chuẩn Việt Nam (Ví dụ: bấm phím đến đâu hiển thị ngay `1.250.000 ₫`).
- Viết các `IValueConverter`: `CurrencyConverter`, `KindToColorConverter` (Chi tiêu: Đỏ, Thu nhập: Xanh lá).
- Cài đặt `CommunityToolkit.Maui` để hiện **Snackbar hoàn tác (Undo)** sau khi người dùng xóa một giao dịch.

## 2. Bản chất kiến trúc & Nguyên lý
- **Tại sao tự làm bàn phím số thay vì dùng `Entry Keyboard="Numeric"`?** Trên thiết bị di động, bàn phím số mặc định của hệ điều hành chiếm tới 40% diện tích màn hình, thường kèm nút Done che khuất form, và không có phím tiện ích "000" cực kỳ phổ biến của ứng dụng tài chính tại Việt Nam. Tự dựng Keypad mang lại trải nghiệm nhập liệu dưới 5 giây.
- **Xử lý chuỗi số lớn:** Giá trị tiền tệ trong ViewModel luôn được giữ ở dạng số nguyên thô (`long`), chỉ chuỗi hiển thị trên Text Label mới được format. Tránh format ngược chuỗi có dấu chấm/phẩy về số liên tục gây giật lag.

## 3. Các bước thực hành chi tiết

### Bước 3.1: Xây dựng Custom Keypad Control
Tạo ContentView `src/Client/SoChi.Client/Controls/NumericKeypadView.xaml`:
```xml
<?xml version="1.0" encoding="utf-8" ?>
<ContentView xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             x:Class="SoChi.Client.Controls.NumericKeypadView">
    <Grid RowDefinitions="*,*,*,*" ColumnDefinitions="*,*,*" RowSpacing="8" ColumnSpacing="8" Padding="12">
        <Button Grid.Row="0" Grid.Column="0" Text="1" Clicked="OnKeyClicked" Style="{StaticResource KeypadBtnStyle}" />
        <Button Grid.Row="0" Grid.Column="1" Text="2" Clicked="OnKeyClicked" Style="{StaticResource KeypadBtnStyle}" />
        <Button Grid.Row="0" Grid.Column="2" Text="3" Clicked="OnKeyClicked" Style="{StaticResource KeypadBtnStyle}" />

        <Button Grid.Row="1" Grid.Column="0" Text="4" Clicked="OnKeyClicked" Style="{StaticResource KeypadBtnStyle}" />
        <Button Grid.Row="1" Grid.Column="1" Text="5" Clicked="OnKeyClicked" Style="{StaticResource KeypadBtnStyle}" />
        <Button Grid.Row="1" Grid.Column="2" Text="6" Clicked="OnKeyClicked" Style="{StaticResource KeypadBtnStyle}" />

        <Button Grid.Row="2" Grid.Column="0" Text="7" Clicked="OnKeyClicked" Style="{StaticResource KeypadBtnStyle}" />
        <Button Grid.Row="2" Grid.Column="1" Text="8" Clicked="OnKeyClicked" Style="{StaticResource KeypadBtnStyle}" />
        <Button Grid.Row="2" Grid.Column="2" Text="9" Clicked="OnKeyClicked" Style="{StaticResource KeypadBtnStyle}" />

        <Button Grid.Row="3" Grid.Column="0" Text="000" Clicked="OnKeyClicked" Style="{StaticResource KeypadBtnStyle}" />
        <Button Grid.Row="3" Grid.Column="1" Text="0" Clicked="OnKeyClicked" Style="{StaticResource KeypadBtnStyle}" />
        <Button Grid.Row="3" Grid.Column="2" Text="⌫" Clicked="OnBackspaceClicked" Style="{StaticResource KeypadBtnActionStyle}" />
    </Grid>
</ContentView>
```

Trong `NumericKeypadView.xaml.cs`:
```csharp
using System.Windows.Input;

namespace SoChi.Client.Controls;

public partial class NumericKeypadView : ContentView
{
    public static readonly BindableProperty KeyPressedCommandProperty =
        BindableProperty.Create(nameof(KeyPressedCommand), typeof(ICommand), typeof(NumericKeypadView));

    public static readonly BindableProperty BackspaceCommandProperty =
        BindableProperty.Create(nameof(BackspaceCommand), typeof(ICommand), typeof(NumericKeypadView));

    public ICommand KeyPressedCommand
    {
        get => (ICommand)GetValue(KeyPressedCommandProperty);
        set => SetValue(KeyPressedCommandProperty, value);
    }

    public ICommand BackspaceCommand
    {
        get => (ICommand)GetValue(BackspaceCommandProperty);
        set => SetValue(BackspaceCommandProperty, value);
    }

    public NumericKeypadView()
    {
        InitializeComponent();
    }

    private void OnKeyClicked(object sender, EventArgs e)
    {
        if (sender is Button btn)
        {
            KeyPressedCommand?.Execute(btn.Text);
        }
    }

    private void OnBackspaceClicked(object sender, EventArgs e)
    {
        BackspaceCommand?.Execute(null);
    }
}
```

### Bước 3.2: Xử lý logic nhập tiền trong `TransactionFormViewModel`
```csharp
[ObservableProperty] public partial string RawAmountString { get; set; } = "0";

public string FormattedDisplayAmount
{
    get
    {
        if (long.TryParse(RawAmountString, out long val))
        {
            return $"{val:N0} ₫";
        }
        return "0 ₫";
    }
}

[RelayCommand]
private void AppendKey(string key)
{
    if (RawAmountString == "0")
    {
        if (key == "0" || key == "000") return;
        RawAmountString = key;
    }
    else
    {
        if (RawAmountString.Length >= 12) return; // Giới hạn tối đa 12 chữ số
        RawAmountString += key;
    }
    Amount = long.Parse(RawAmountString);
    OnPropertyChanged(nameof(FormattedDisplayAmount));
}

[RelayCommand]
private void Backspace()
{
    if (RawAmountString.Length <= 1)
    {
        RawAmountString = "0";
    }
    else
    {
        RawAmountString = RawAmountString.Substring(0, RawAmountString.Length - 1);
    }
    Amount = long.Parse(RawAmountString);
    OnPropertyChanged(nameof(FormattedDisplayAmount));
}
```

### Bước 3.3: Viết Value Converters
Tạo file `src/Client/SoChi.Client/Converters/TransactionConverters.cs`:
```csharp
using System.Globalization;
using SoChi.Client.Models;

namespace SoChi.Client.Converters;

public class CurrencyConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is long amount)
        {
            return $"{amount:N0} ₫";
        }
        return "0 ₫";
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => throw new NotImplementedException();
}

public class KindToColorConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is TransactionKind kind)
        {
            return kind == TransactionKind.Expense ? Color.FromArgb("#EF4444") : Color.FromArgb("#10B981");
        }
        return Color.FromArgb("#111827");
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => throw new NotImplementedException();
}
```

### Bước 3.4: Tích hợp Snackbar Hoàn Tác (Undo)
Cài đặt package:
```powershell
dotnet add src/Client/SoChi.Client/SoChi.Client.csproj package CommunityToolkit.Maui
```
Đăng ký trong `MauiProgram.cs`:
```csharp
builder.UseMauiApp<App>().UseMauiCommunityToolkit();
```
Trong `TransactionsViewModel.cs` khi xóa giao dịch:
```csharp
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;

// Khi thực hiện xóa:
await _repository.SoftDeleteTransactionAsync(item.Id);
await RefreshDataAsync();

var snackbar = Snackbar.Make(
    $"Đã xóa '{item.Note}'",
    actionButtonText: "HOÀN TÁC",
    action: async () =>
    {
        // Khôi phục lại bản ghi
        var record = await _repository.GetTransactionByIdAsync(item.Id);
        if (record != null)
        {
            record.IsDeleted = false;
            await _repository.SaveTransactionAsync(record);
            await RefreshDataAsync();
        }
    },
    duration: TimeSpan.FromSeconds(4));

await snackbar.Show();
```

## 4. Bẫy thường gặp (Pitfalls)
- **Lỗi:** Bấm vào nút bàn phím số mà số hiển thị không thay đổi.
  - **Nguyên nhân:** Quên kích hoạt thông báo thay đổi `OnPropertyChanged(nameof(FormattedDisplayAmount))` sau khi thay đổi `RawAmountString`.
- **Lỗi:** Crash app khi gọi `Snackbar.Make()`:
  - **Nguyên nhân:** Quên thêm `.UseMauiCommunityToolkit()` trong `MauiProgram.cs`.

## 5. Checklist nghiệm thu Buổi 06
- [ ] Bấm các phím số từ 0-9: Số tiền nhảy lập tức theo định dạng Việt Nam (`50.000 ₫`, `1.250.000 ₫`).
- [ ] Bấm phím "000": Tự động thêm 3 chữ số 0.
- [ ] Bấm phím "⌫": Xóa từng số một; xóa hết thì quay về `0 ₫`.
- [ ] Xóa một giao dịch -> Thanh thông báo màu tối hiện ở đáy màn hình có chữ "HOÀN TÁC".
- [ ] Bấm "HOÀN TÁC" trong vòng 4 giây -> Giao dịch ngay lập tức xuất hiện trở lại danh sách.

---

🏠 [Mục lục toàn khóa](../HUONG-DAN-TUNG-BUOI.md)

⬅️ [BUỔI 05: Danh Sách Nâng Cao: Grouping Theo Ngày, SwipeView & RefreshView](buoi-05.md)

[BUỔI 07: Đồ Họa 2D Thuần: Donut Chart Tự Vẽ Với Microsoft.Maui.Graphics](buoi-07.md) ➡️
