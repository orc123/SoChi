# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Trạng thái

Repository đang trống — chưa có solution, project hay source code. Tài liệu này mô tả **quy ước và cấu trúc dự kiến** cho một solution gồm client .NET MAUI + backend ASP.NET Core Web API trên .NET 10. Khi code thực tế được thêm vào, hãy cập nhật lại file này cho khớp (đặc biệt là tên project, lệnh test, và phần Architecture).

## Môi trường

- SDK mặc định: **.NET 10.0.400** (máy còn 2.0 / 2.1 / 3.0 / 5.0 / 8.0 / 9.0 — pin phiên bản bằng `global.json` để tránh dùng nhầm).
- Workload MAUI đã cài: `maui-windows`, `android`, `ios`, `maccatalyst`.
- Máy phát triển là **Windows**. Build được `net10.0-windows` và `net10.0-android`; các target `ios` / `maccatalyst` cần máy Mac hoặc Mac build host qua `Pair to Mac` — đừng cố build chúng ở đây.

## Cấu trúc dự kiến

```
MAUI.sln
src/
  Client/        # MAUI app (multi-target: android; ios; maccatalyst; windows)
  Api/           # ASP.NET Core Web API (.NET 10)
  Shared/        # DTO / contract dùng chung giữa Client và Api (netstandard2.0 hoặc net10.0)
tests/
  Api.Tests/
  Client.Tests/
```

`Shared` là ranh giới quan trọng nhất: mọi DTO đi qua HTTP phải được định nghĩa ở đây và được cả hai phía tham chiếu, để không xảy ra tình trạng contract lệch nhau giữa client và server.

## Lệnh thường dùng

```powershell
# Restore + build toàn solution
dotnet restore
dotnet build

# Chạy API (mặc định https://localhost:7xxx, xem Properties/launchSettings.json)
dotnet run --project src/Api

# Chạy MAUI app trên Windows
dotnet build src/Client -f net10.0-windows10.0.19041.0 -t:Run

# Chạy MAUI app trên Android emulator/thiết bị
dotnet build src/Client -f net10.0-android -t:Run

# Test
dotnet test
dotnet test tests/Api.Tests                                  # một project
dotnet test --filter "FullyQualifiedName~OrderServiceTests"   # một class/nhóm
dotnet test --filter "FullyQualifiedName=Ns.OrderServiceTests.Returns404WhenMissing"  # một test

# Format (dotnet format đi kèm SDK)
dotnet format --verify-no-changes    # kiểm tra, dùng trong CI
dotnet format                        # sửa
```

## Lưu ý khi làm việc

**Địa chỉ API khi chạy trên emulator.** `localhost` trong Android emulator trỏ về chính emulator, không phải máy host. Dùng `10.0.2.2` cho Android emulator, `localhost` cho Windows/MacCatalyst. Base URL nên được đặt ở một chỗ duy nhất (một `ApiOptions` / hằng số theo platform), không rải rác trong từng service.

**Chứng chỉ dev HTTPS.** Client trên Android/iOS không tin chứng chỉ self-signed của ASP.NET Core dev. Khi debug cần cấu hình `HttpClientHandler` bỏ qua validation **chỉ trong `#if DEBUG`**, tuyệt đối không để lọt vào Release.

**Đăng ký DI trong MAUI.** Mọi service, `HttpClient`, ViewModel và Page phải được đăng ký trong `MauiProgram.CreateMauiApp()`. Page/ViewModel dùng constructor injection và được resolve qua Shell routing — không tự `new` chúng trong code-behind.

**Thread UI.** Kết quả từ lệnh gọi API cập nhật lên UI phải qua `MainThread.BeginInvokeOnMainThread` nếu không nằm trong luồng đã capture context của UI.

**Nullable & warnings.** Bật `<Nullable>enable</Nullable>` và `<TreatWarningsAsErrors>true</TreatWarningsAsErrors>` trong `Directory.Build.props` để áp dụng đồng loạt cho mọi project, thay vì lặp lại trong từng `.csproj`.

**XAML hot reload** hoạt động cho thay đổi giao diện; thay đổi trong `MauiProgram`, DI, hoặc constructor cần restart app.
