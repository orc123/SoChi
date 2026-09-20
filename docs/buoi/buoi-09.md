# BUỔI 09: API Thiết Bị: Chụp/Chọn Ảnh Hóa Đơn, FileSystem & Phân Quyền

**Thuộc chặng:** Chặng 5 (Phần 1) | **Thời lượng:** ~3 giờ

## 1. Mục tiêu buổi học (Definition of Done)
- Khai thác API thiết bị với `MediaPicker`: Cho phép người dùng **Chụp ảnh bằng Camera** hoặc **Chọn ảnh từ Thư viện** để đính kèm vào giao dịch chi tiêu.
- Lưu trữ file ảnh vào thư mục riêng của ứng dụng (`FileSystem.AppDataDirectory`), lưu đường dẫn cục bộ vào trường `ReceiptPath` của `Transaction`.
- Khai báo phân quyền đầy đủ trong `Platforms/Android/AndroidManifest.xml` và xử lý xin quyền runtime (`Permissions.RequestAsync`) chuyên nghiệp (không crash khi bị từ chối).
- Hiển thị ảnh xem trước (Thumbnail) thu nhỏ trong màn hình Thêm giao dịch và cho phép bấm vào để phóng to xem chi tiết.

## 2. Bản chất kiến trúc & Nguyên lý
- **Nguyên lý lưu trữ file đa nền tảng:** Không lưu file ảnh trực tiếp vào database (làm phình database và giảm hiệu năng truy vấn SQLite). Ta chỉ lưu file ảnh dạng vật lý vào đĩa cục bộ của app và lưu chuỗi đường dẫn `string ReceiptPath` vào bảng SQLite.
- **Quy tắc 2 bước xin quyền trên Android:**
  - *Bước 1 (Compile-time):* Khai báo `<uses-permission>` trong `AndroidManifest.xml`.
  - *Bước 2 (Runtime):* Gọi `Permissions.RequestAsync<Permissions.Camera>()` trước khi mở Camera. Nếu thiếu bước 1 app crash; nếu thiếu bước 2 app bị từ chối ngầm.

## 3. Các bước thực hành chi tiết

### Bước 3.1: Cấu hình quyền trong `Platforms/Android/AndroidManifest.xml`
Mở `src/Client/SoChi.Client/Platforms/Android/AndroidManifest.xml` thêm vào trong thẻ `<manifest>`:
```xml
<uses-permission android:name="android.permission.CAMERA" />
<uses-permission android:name="android.permission.READ_MEDIA_IMAGES" />
<uses-feature android:name="android.hardware.camera" android:required="false" />
```

### Bước 3.2: Viết Service quản lý File và Media
Tạo interface `src/Client/SoChi.Client/Services/IMediaService.cs`:
```csharp
namespace SoChi.Client.Services;

public interface IMediaService
{
    Task<string?> TakePhotoAsync();
    Task<string?> PickPhotoAsync();
}
```

Tạo class triển khai `src/Client/SoChi.Client/Services/MediaService.cs`:
```csharp
namespace SoChi.Client.Services;

public class MediaService : IMediaService
{
    public async Task<string?> TakePhotoAsync()
    {
        if (!MediaPicker.Default.IsCaptureSupported)
        {
            await Shell.Current.DisplayAlert("Thông báo", "Thiết bị không hỗ trợ camera", "Đóng");
            return null;
        }

        var status = await Permissions.CheckStatusAsync<Permissions.Camera>();
        if (status != PermissionStatus.Granted)
        {
            status = await Permissions.RequestAsync<Permissions.Camera>();
            if (status != PermissionStatus.Granted)
            {
                await Shell.Current.DisplayAlert("Quyền bị từ chối", "Ứng dụng cần quyền Camera để chụp hóa đơn.", "Đóng");
                return null;
            }
        }

        var photo = await MediaPicker.Default.CapturePhotoAsync();
        return await SaveToAppStorageAsync(photo);
    }

    public async Task<string?> PickPhotoAsync()
    {
        var photo = await MediaPicker.Default.PickPhotoAsync();
        return await SaveToAppStorageAsync(photo);
    }

    private async Task<string?> SaveToAppStorageAsync(FileResult? photo)
    {
        if (photo == null) return null;

        string receiptsDir = Path.Combine(FileSystem.AppDataDirectory, "receipts");
        Directory.CreateDirectory(receiptsDir);

        string fileName = $"{Guid.NewGuid()}{Path.GetExtension(photo.FileName)}";
        string targetPath = Path.Combine(receiptsDir, fileName);

        using var sourceStream = await photo.OpenReadAsync();
        using var targetStream = File.Create(targetPath);
        await sourceStream.CopyToAsync(targetStream);

        return targetPath;
    }
}
```
Đăng ký trong `MauiProgram.cs`:
```csharp
builder.Services.AddSingleton<IMediaService, MediaService>();
```

### Bước 3.3: Tích hợp vào `TransactionFormViewModel`
```csharp
[ObservableProperty] public partial string? ReceiptPath { get; set; }

[RelayCommand]
private async Task AttachReceiptAsync()
{
    string action = await Shell.Current.DisplayActionSheet("Đính kèm hóa đơn", "Hủy", null, "Chụp ảnh mới", "Chọn từ thư viện");
    string? path = null;

    if (action == "Chụp ảnh mới")
    {
        path = await _mediaService.TakePhotoAsync();
    }
    else if (action == "Chọn từ thư viện")
    {
        path = await _mediaService.PickPhotoAsync();
    }

    if (!string.IsNullOrEmpty(path))
    {
        ReceiptPath = path;
    }
}

[RelayCommand]
private void RemoveReceipt()
{
    ReceiptPath = null;
}
```

### Bước 3.4: Hiển thị giao diện hóa đơn trên `TransactionFormPage.xaml`
```xml
<VerticalStackLayout Spacing="8">
    <Label Text="Hóa đơn chứng từ" FontAttributes="Bold" />

    <!-- Khi chưa có ảnh -->
    <Button Text="📷 Thêm ảnh hóa đơn" Command="{Binding AttachReceiptCommand}"
            IsVisible="{Binding ReceiptPath, Converter={StaticResource IsNullConverter}}"
            BackgroundColor="#F3F4F6" TextColor="#374151" />

    <!-- Xem trước khi đã có ảnh -->
    <Grid IsVisible="{Binding ReceiptPath, Converter={StaticResource IsNotNullConverter}}" HeightRequest="120" WidthRequest="120" HorizontalOptions="Start">
        <Border StrokeShape="RoundRectangle 8" StrokeThickness="1" Stroke="#D1D5DB">
            <Image Source="{Binding ReceiptPath}" Aspect="AspectFill" />
        </Border>
        <Button Text="✕" Command="{Binding RemoveReceiptCommand}"
                BackgroundColor="#EF4444" TextColor="White"
                HeightRequest="28" WidthRequest="28" CornerRadius="14"
                Padding="0" HorizontalOptions="End" VerticalOptions="Start" Margin="4" />
    </Grid>
</VerticalStackLayout>
```

## 4. Bẫy thường gặp (Pitfalls)
- **Lỗi:** Ứng dụng bị crash văng ra ngoài khi bấm chụp ảnh trên Android Emulator.
  - **Nguyên nhân:** Emulator chưa bật camera ảo (Virtual Scene) hoặc thiếu quyền trong `AndroidManifest.xml`.
- **Lỗi:** Lưu đường dẫn tạm thời từ `FileResult.FullPath` khiến sau khi khởi động lại app thì ảnh biến mất.
  - **Khắc phục:** `FileResult` của `MediaPicker` là file cache tạm (cache directory), bắt buộc phải copy luồng file (`sourceStream.CopyToAsync`) sang thư mục bền vững `FileSystem.AppDataDirectory`.

## 5. Checklist nghiệm thu Buổi 09
- [ ] Chạy được trên **Android Emulator** hoặc điện thoại Android thật.
- [ ] Bấm nút chụp/chọn ảnh -> Popup xin quyền xuất hiện chuẩn xác.
- [ ] Nếu từ chối quyền -> App hiện thông báo nhắc nhở nhẹ nhàng, không crash.
- [ ] Chọn ảnh xong -> Ảnh thumbnail hiển thị rõ ràng trên Form.
- [ ] Bấm Lưu -> Khởi động lại app -> Chi tiết giao dịch vẫn tải được ảnh hóa đơn từ bộ nhớ.

---

🏠 [Mục lục toàn khóa](../HUONG-DAN-TUNG-BUOI.md)

⬅️ [BUỔI 08: Biểu Đồ Cột 6 Tháng (Bar Chart), Responsive & Xoay Màn Hình](buoi-08.md)

[BUỔI 10: Theme Sáng/Tối, Sinh Trắc Học Vân Tay, Haptic & Xuất File CSV](buoi-10.md) ➡️
