# BUỔI 13: Client Tích Hợp API, DelegatingHandler, SecureStorage & 10.0.2.2

**Thuộc chặng:** Chặng 7 (Phần 2) | **Thời lượng:** ~3 giờ

## 1. Mục tiêu buổi học (Definition of Done)
- Cấu hình địa chỉ API đa nền tảng cho Client: Tự động dùng `http://10.0.2.2:5xxx` khi chạy trên Android Emulator và `https://localhost:7xxx` khi chạy trên Windows.
- Viết `AuthHeaderHandler` (kế thừa `DelegatingHandler`) để tự động đính kèm header `Authorization: Bearer <Token>` vào mọi request HTTP đi ra.
- Lưu trữ an toàn JWT Token tại client bằng `SecureStorage`.
- Xây dựng giao diện màn hình Đăng nhập / Đăng ký trên MAUI.
- Đăng nhập thành công từ cả máy tính Windows và máy ảo Android Emulator.

## 2. Bản chất kiến trúc & Nguyên lý
- **Bí mật địa chỉ `10.0.2.2` trên Android Emulator:** Máy ảo Android chạy trong một mạng ảo riêng biệt. Nếu bạn gọi `http://localhost`, máy ảo sẽ trỏ vào chính nó thay vì máy tính của bạn (nơi đang chạy Web API). Địa chỉ `10.0.2.2` là alias đặc biệt do Google thiết kế để từ máy ảo trỏ ngược về `localhost` của máy tính host.
- **Tại sao dùng `SecureStorage` thay vì `Preferences` cho Token?** `Preferences` lưu dưới dạng văn bản rõ (plain text XML). Nếu máy bị root, bất kỳ ai cũng có thể đọc trộm Token. `SecureStorage` sử dụng Android KeyStore hoặc Windows DPAPI để mã hóa token trước khi lưu vào đĩa cứng.

## 3. Các bước thực hành chi tiết

### Bước 3.1: Cấu hình địa chỉ Server động theo Platform
Tạo file `src/Client/SoChi.Client/Services/ApiConfig.cs`:
```csharp
namespace SoChi.Client.Services;

public static class ApiConfig
{
    // Cấu hình Port khớp với launchSettings.json của Api
    private const string Port = "5000";

    public static string BaseUrl =>
        DeviceInfo.Platform == DevicePlatform.Android
            ? $"http://10.0.2.2:{Port}/"
            : $"http://localhost:{Port}/";
}
```

### Bước 3.2: Viết DelegatingHandler tự động gắn Token
Tạo file `src/Client/SoChi.Client/Services/AuthHeaderHandler.cs`:
```csharp
using System.Net.Http.Headers;

namespace SoChi.Client.Services;

public class AuthHeaderHandler : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        string? token = await SecureStorage.Default.GetAsync("auth_token");
        if (!string.IsNullOrEmpty(token))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        return await base.SendAsync(request, cancellationToken);
    }
}
```

### Bước 3.3: Đăng ký HttpClient trong `MauiProgram.cs`
```csharp
builder.Services.AddTransient<AuthHeaderHandler>();

builder.Services.AddHttpClient("SoChiApi", client =>
{
    client.BaseAddress = new Uri(ApiConfig.BaseUrl);
    client.Timeout = TimeSpan.FromSeconds(15);
})
.AddHttpMessageHandler<AuthHeaderHandler>();
```

### Bước 3.4: Xây dựng Service Đăng nhập / Đăng ký
Tạo `src/Client/SoChi.Client/Services/IAuthService.cs`:
```csharp
using System.Net.Http.Json;
using SoChi.Shared;
using SoChi.Shared.Dtos;

namespace SoChi.Client.Services;

public class AuthService
{
    private readonly IHttpClientFactory _httpClientFactory;

    public AuthService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<bool> LoginAsync(string email, string password)
    {
        var client = _httpClientFactory.CreateClient("SoChiApi");
        var response = await client.PostAsJsonAsync(ApiRoutes.Login, new LoginRequest(email, password));

        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadFromJsonAsync<AuthResponse>();
            if (result != null)
            {
                await SecureStorage.Default.SetAsync("auth_token", result.Token);
                await SecureStorage.Default.SetAsync("user_email", result.Email);
                return true;
            }
        }
        return false;
    }
}
```

## 4. Bẫy thường gặp (Pitfalls)
- **Lỗi:** Android Emulator ném lỗi `System.Net.Http.HttpRequestException: CLEARTEXT communication to 10.0.2.2 not permitted by network security policy`.
  - **Nguyên nhân:** Từ Android 9 (API 28), Android mặc định cấm kết nối HTTP thường (chưa có SSL/HTTPS).
  - **Khắc phục:** Thêm thuộc tính `android:usesCleartextTraffic="true"` vào thẻ `<application>` trong file `Platforms/Android/AndroidManifest.xml` trong môi trường phát triển.

## 5. Checklist nghiệm thu Buổi 13
- [ ] Mở đồng thời cả Backend API và Client MAUI.
- [ ] Màn hình Đăng nhập hiển thị trên Android Emulator.
- [ ] Nhập tài khoản và mật khẩu -> Bấm "Đăng nhập" -> Kết nối thành công tới API máy host qua `10.0.2.2`.
- [ ] Kiểm tra `SecureStorage`: Chuỗi Token được lưu trữ bảo mật.

---

🏠 [Mục lục toàn khóa](../HUONG-DAN-TUNG-BUOI.md)

⬅️ [BUỔI 12: Backend ASP.NET Core Web API, PostgreSQL, Shared DTOs & JWT Auth](buoi-12.md)

[BUỔI 14: Offline-First Sync Hai Chiều, Connectivity & Last-Write-Wins](buoi-14.md) ➡️
