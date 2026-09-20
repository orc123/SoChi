# PHỤ LỤC A: Trạng Thái Loading, Lỗi & Offline (áp dụng xuyên suốt)

> Đọc phụ lục này sau **Buổi 04**, rồi áp dụng dần cho mọi ViewModel viết từ Buổi 05 trở đi. Đây là phần ngăn cách một app bài tập với một app dùng được thật.

## Vấn đề

Code trong 15 buổi được viết theo hướng "đường đi thuận lợi" — mạng luôn có, database luôn trả về, người dùng bấm đúng thứ tự. Thực tế:

- Truy vấn 3.000 giao dịch mất 2 giây → màn hình trắng, người dùng tưởng app treo và bấm lại.
- Sync thất bại → **không có gì hiển thị cả**, người dùng đinh ninh dữ liệu đã lên server.
- Bấm nút "Lưu" hai lần nhanh → tạo hai giao dịch trùng.

Ba triệu chứng, một nguyên nhân: ViewModel không có khái niệm về **trạng thái** của chính nó.

## Giải pháp: một `BaseViewModel` dùng chung

Tạo `src/Client/SoChi.Client/ViewModels/BaseViewModel.cs`:

```csharp
using CommunityToolkit.Mvvm.ComponentModel;

namespace SoChi.Client.ViewModels;

public partial class BaseViewModel : ObservableObject
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsNotBusy))]
    public partial bool IsBusy { get; set; }

    public bool IsNotBusy => !IsBusy;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasError))]
    public partial string? ErrorMessage { get; set; }

    public bool HasError => !string.IsNullOrEmpty(ErrorMessage);

    /// <summary>
    /// Bọc một thao tác async: tự bật/tắt IsBusy, tự bắt lỗi, tự chống bấm hai lần.
    /// </summary>
    protected async Task RunSafeAsync(Func<Task> operation, string? errorPrefix = null)
    {
        if (IsBusy) return;          // Chống double-tap: lần bấm thứ hai bị bỏ qua

        IsBusy = true;
        ErrorMessage = null;

        try
        {
            await operation();
        }
        catch (HttpRequestException)
        {
            ErrorMessage = "Không kết nối được máy chủ. Dữ liệu vẫn được lưu trên máy.";
        }
        catch (TaskCanceledException)
        {
            ErrorMessage = "Máy chủ phản hồi quá lâu. Thử lại sau.";
        }
        catch (Exception ex)
        {
            ErrorMessage = errorPrefix is null ? ex.Message : $"{errorPrefix}: {ex.Message}";
            System.Diagnostics.Debug.WriteLine(ex);   // chi tiết đầy đủ chỉ dành cho lập trình viên
        }
        finally
        {
            IsBusy = false;
        }
    }
}
```

Hai chi tiết đáng học:

- **`[NotifyPropertyChangedFor]`** khiến `IsNotBusy` và `HasError` tự phát tín hiệu khi property nguồn đổi. Không có nó, XAML bind vào `IsNotBusy` sẽ không bao giờ cập nhật.
- **Thông báo lỗi cho người dùng khác với thông tin cho lập trình viên.** Người dùng cần biết "nên làm gì tiếp theo"; stack trace đi vào `Debug.WriteLine`. Đừng bao giờ `DisplayAlert(ex.ToString())`.

## Dùng trong ViewModel

Chuyển các ViewModel đã viết sang kế thừa `BaseViewModel` và bọc thân hàm:

```csharp
public partial class TransactionsViewModel : BaseViewModel
{
    [RelayCommand]
    private Task LoadAsync() => RunSafeAsync(async () =>
    {
        var items = await _repository.GetTransactionsAsync();
        RebuildGroups(items);
    }, "Không tải được danh sách");
}
```

Toàn bộ `IsBusy = true; try { ... } finally { IsBusy = false; }` lặp đi lặp lại ở mọi ViewModel biến mất — đó là điểm mấu chốt.

## Hiển thị trong XAML

```xml
<Grid RowDefinitions="Auto,Auto,*">

    <!-- Băng báo offline: luôn hiện khi mất mạng -->
    <Border Grid.Row="0" BackgroundColor="#F59E0B" Padding="8"
            IsVisible="{Binding IsOffline}">
        <Label Text="Đang ngoại tuyến — thay đổi sẽ được đồng bộ khi có mạng"
               TextColor="White" FontSize="12" HorizontalOptions="Center" />
    </Border>

    <!-- Băng báo lỗi: có nút thử lại, không phải hộp thoại chặn thao tác -->
    <Border Grid.Row="1" BackgroundColor="#FEE2E2" Padding="12"
            IsVisible="{Binding HasError}">
        <Grid ColumnDefinitions="*,Auto">
            <Label Text="{Binding ErrorMessage}" TextColor="#991B1B" FontSize="13" />
            <Button Grid.Column="1" Text="Thử lại" Command="{Binding LoadCommand}"
                    Padding="12,4" FontSize="12" />
        </Grid>
    </Border>

    <Grid Grid.Row="2">
        <CollectionView ItemsSource="{Binding Groups}" IsVisible="{Binding IsNotBusy}" />

        <ActivityIndicator IsRunning="{Binding IsBusy}"
                           IsVisible="{Binding IsBusy}"
                           VerticalOptions="Center" HorizontalOptions="Center" />
    </Grid>
</Grid>
```

Nút "Lưu" cũng nên khóa lại trong lúc đang chạy:

```xml
<Button Text="Lưu" Command="{Binding SaveCommand}" IsEnabled="{Binding IsNotBusy}" />
```

## Ba nguyên tắc UX cho app offline-first

1. **Mất mạng không phải là lỗi.** SoChi ghi vào SQLite trước, sync sau — nên mất mạng chỉ cần một băng thông báo màu vàng, tuyệt đối không phải hộp thoại đỏ chặn thao tác. Nhầm chỗ này là hỏng toàn bộ trải nghiệm offline-first mà bạn đã xây suốt 14 buổi.
2. **Mọi lỗi phải kèm hành động.** "Đã xảy ra lỗi" là thông báo vô dụng. "Không kết nối được máy chủ. Dữ liệu vẫn được lưu trên máy." vừa nói chuyện gì xảy ra, vừa trấn an đúng nỗi lo thật sự của người dùng.
3. **Loading dưới 300ms thì đừng hiện gì cả.** Vòng xoay nhấp nháy 100ms gây cảm giác giật hơn là không có gì. Trì hoãn hiển thị `ActivityIndicator` bằng một `Task.Delay(300)` rồi mới bật, nếu thao tác xong trước thì thôi.

## Bài tập tự làm

- Thêm `IsOffline` vào `BaseViewModel`, cập nhật nó từ `Connectivity.ConnectivityChanged` (nhớ `MainThread` — xem Buổi 14 Bước 3.3).
- Áp dụng `RunSafeAsync` cho toàn bộ ViewModel đã viết từ Buổi 04 đến Buổi 11.
- Viết unit test: gọi command khi `IsBusy = true` → thao tác bên trong **không** được chạy.

---

🏠 [Mục lục toàn khóa](../HUONG-DAN-TUNG-BUOI.md)

⬅️ [BUỔI 17: Endpoint Báo Cáo, Biểu Đồ SVG Tự Vẽ & Kiểm Chứng Sức Khỏe Dữ Liệu](buoi-17.md)

[PHỤ LỤC B: Nhật Ký Học Tập](phu-luc-b.md) ➡️
