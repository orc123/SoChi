# BUỔI 10: Theme Sáng/Tối, Sinh Trắc Học Vân Tay, Haptic & Xuất File CSV

**Thuộc chặng:** Chặng 5 (Phần 2) | **Thời lượng:** ~3 giờ

## 1. Mục tiêu buổi học (Definition of Done)
- Hoàn thiện tính năng chuyển đổi **Giao diện Sáng / Tối / Theo hệ thống**, lưu tùy chọn của người dùng bền vững qua `Preferences`.
- Tích hợp bảo mật: Cho phép kích hoạt **Khóa vân tay / Face ID** khi mở ứng dụng.
- Kích hoạt **Phản hồi rung nhẹ (`HapticFeedback`)** mỗi khi người dùng bấm phím trên bàn phím số Custom Keypad.
- Viết chức năng **Xuất toàn bộ giao dịch ra file CSV**, lưu trữ cục bộ và gọi hộp thoại chia sẻ native của hệ điều hành (`Share.RequestAsync`).
- Chuẩn hóa toàn bộ hiển thị ngày giờ và tiền tệ theo văn hóa `CultureInfo("vi-VN")`.

## 2. Bản chất kiến trúc & Nguyên lý
- **Lưu trữ tùy chọn nhẹ nhàng với `Preferences`:** Thay vì ghi vào SQLite một giá trị cài đặt nhỏ (như `ThemeMode = Dark`), MAUI cung cấp `Preferences` ánh xạ trực tiếp xuống `SharedPreferences` (Android) hoặc `NSUserDefaults` (iOS) với tốc độ đọc cực nhanh ngay khi khởi động.
- **Haptic Feedback:** Rung phản hồi giúp tăng tính chân thật của Custom Keypad (cảm giác bấm phím vật lý). Chỉ nên dùng `HapticFeedbackType.Click` nhẹ nhàng, tránh dùng rung cảnh báo quá mạnh gây khó chịu.

## 3. Các bước thực hành chi tiết

### Bước 3.1: Quản lý Theme với Preferences
Tạo file `src/Client/SoChi.Client/Services/ThemeService.cs`:
```csharp
namespace SoChi.Client.Services;

public class ThemeService
{
    private const string ThemeKey = "user_theme_preference";

    public void ApplySavedTheme()
    {
        string theme = Preferences.Get(ThemeKey, "System");
        Application.Current!.UserAppTheme = theme switch
        {
            "Light" => AppTheme.Light,
            "Dark" => AppTheme.Dark,
            _ => AppTheme.Unspecified
        };
    }

    public void SetTheme(string theme)
    {
        Preferences.Set(ThemeKey, theme);
        ApplySavedTheme();
    }
}
```
Gọi `themeService.ApplySavedTheme()` ngay trong `App.xaml.cs`.

### Bước 3.2: Tích hợp Rung phản hồi xúc giác (Haptic)
Trong `NumericKeypadView.xaml.cs`:
```csharp
private void OnKeyClicked(object sender, EventArgs e)
{
    try
    {
        HapticFeedback.Default.Perform(HapticFeedbackType.Click);
    }
    catch (FeatureNotSupportedException) { }

    if (sender is Button btn)
    {
        KeyPressedCommand?.Execute(btn.Text);
    }
}
```

### Bước 3.3: Xuất dữ liệu CSV và Chia sẻ
Trong `SettingsViewModel.cs`:
```csharp
[RelayCommand]
private async Task ExportCsvAsync()
{
    var transactions = await _repository.GetTransactionsAsync();
    var categories = (await _repository.GetCategoriesAsync()).ToDictionary(c => c.Id);

    var sb = new System.Text.StringBuilder();
    sb.AppendLine("ID,Ngay,Loai,Danh_Muc,So_Tien_VND,Ghi_Chu");

    foreach (var t in transactions)
    {
        var catName = categories.ContainsKey(t.CategoryId) ? categories[t.CategoryId].Name : "";
        var catKind = categories.ContainsKey(t.CategoryId) ? categories[t.CategoryId].Kind.ToString() : "";
        sb.AppendLine($"{t.Id},{t.OccurredOn:yyyy-MM-dd},{catKind},\"{catName}\",{t.Amount},\"{t.Note.Replace("\"", "\"\"")}\"");
    }

    string fileName = $"SoChi_Export_{DateTime.Now:yyyyMMdd_HHmmss}.csv";
    string filePath = Path.Combine(FileSystem.CacheDirectory, fileName);
    await File.WriteAllTextAsync(filePath, sb.ToString(), System.Text.Encoding.UTF8);

    await Share.Default.RequestAsync(new ShareFileRequest
    {
        Title = "Xuất dữ liệu chi tiêu SoChi",
        File = new ShareFile(filePath)
    });
}
```

### Bước 3.4: Xác thực sinh trắc học vân tay
Cài đặt package hỗ trợ Biometric (hoặc kiểm tra phương thức `BiometricAuthentication`):
```powershell
dotnet add src/Client/SoChi.Client/SoChi.Client.csproj package Plugin.Fingerprint
```
Sử dụng trong `SettingsViewModel.cs` để kích hoạt:
```csharp
using Plugin.Fingerprint;
using Plugin.Fingerprint.Abstractions;

[RelayCommand]
private async Task ToggleBiometricLockAsync(bool isEnabled)
{
    var isAvailable = await CrossFingerprint.Current.IsAvailableAsync();
    if (!isAvailable)
    {
        await Shell.Current.DisplayAlert("Lỗi", "Thiết bị không hỗ trợ hoặc chưa cài vân tay/khuôn mặt", "Đóng");
        return;
    }

    var auth = await CrossFingerprint.Current.AuthenticateAsync(new AuthenticationRequestConfiguration("Xác thực", "Quét vân tay để bật tính năng bảo vệ"));
    if (auth.Authenticated)
    {
        Preferences.Set("biometric_lock_enabled", isEnabled);
    }
}
```

## 4. Bẫy thường gặp (Pitfalls)
- **Lỗi:** Xuất file CSV mở bằng Microsoft Excel trên Windows bị lỗi font tiếng Việt.
  - **Khắc phục:** Sử dụng UTF-8 kèm BOM khi ghi file CSV: `new UTF8Encoding(true)`.
- **Lỗi:** Khi chuyển theme trên Android, giao diện không đổi màu ngay lập tức.
  - **Khắc phục:** Đảm bảo toàn bộ màu sắc nền và chữ trong XAML sử dụng `AppThemeBinding` (Ví dụ: `TextColor="{AppThemeBinding Light=#111827, Dark=#F9FAFB}"`).

## 5. Checklist nghiệm thu Buổi 10
- [ ] Vào tab Cài đặt, chọn theme "Tối" -> Toàn bộ nền app chuyển sang màu đen/xám tối ngay tức thì. Tắt app bật lại theme vẫn giữ nguyên.
- [ ] Gõ phím trên Custom Keypad trên điện thoại thật -> Cảm nhận được máy rung phản hồi nhẹ.
- [ ] Bấm nút "Xuất CSV" -> Hộp thoại chia sẻ của hệ điều hành hiện lên (cho phép gửi qua Zalo, Drive, Gmail, hoặc Lưu vào tệp).
- [ ] Mở file CSV xuất ra -> Tiếng Việt có dấu hiển thị chuẩn, không lỗi định dạng.

---

🏠 [Mục lục toàn khóa](../HUONG-DAN-TUNG-BUOI.md)

⬅️ [BUỔI 09: API Thiết Bị: Chụp/Chọn Ảnh Hóa Đơn, FileSystem & Phân Quyền](buoi-09.md)

[BUỔI 11: Hạn Mức Chi Tiêu (ProgressBar), Lọc, Debounce & xUnit Tests](buoi-11.md) ➡️
