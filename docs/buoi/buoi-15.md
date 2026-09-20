# BUỔI 15: Kiểm Thử Kịch Bản Sync Thực Tế & Đóng Gói Release (APK/MSIX)

**Thuộc chặng:** Chặng 7 (Hoàn thiện) & Chặng 8 | **Thời lượng:** ~3 giờ

## 1. Mục tiêu buổi học (Definition of Done)
- Hoàn thành toàn diện 4 kịch bản kiểm thử đồng bộ thực tế khắt khe.
- Thay thế toàn bộ App Icon và Splash Screen mặc định của .NET MAUI bằng bộ nhận diện thương hiệu thật của **SoChi**.
- Tạo Keystore ký mã (Code Signing), cấu hình `Release` build và xuất file cài đặt **Android APK / AAB**.
- Xuất gói cài đặt **Windows MSIX**.
- Kích hoạt tối ưu hóa rút gọn mã nguồn (`PublishTrimmed`) và kiểm tra thời gian khởi động lạnh đạt **dưới 3 giây**.

## 2. Bản chất kiến trúc & Nguyên lý
- **Keystore trên Android:** Mọi ứng dụng Android cài lên máy thật bắt buộc phải được ký số điện tử bằng một file Keystore bí mật. Nếu làm mất file Keystore này, bạn sẽ không bao giờ có thể cập nhật phiên bản mới cho ứng dụng của mình.
- **Tối ưu hóa Ahead-Of-Time (AOT) & Trimming:** Khi build Release, .NET loại bỏ các đoạn code không dùng đến (Trimming) và biên dịch trước sang mã máy gốc (AOT), giúp giảm 40% dung lượng APK và tăng gấp đôi tốc độ khởi động app.

## 3. Các bước thực hành chi tiết

### Bước 3.1: Thực hiện kịch bản kiểm thử xung đột 2 thiết bị
Thực hiện chính xác 4 bước nghiệm thu:
1. **Thiết bị 1 & Thiết bị 2** cùng mở app, cùng ngắt mạng.
2. Trên **Thiết bị 1**, sửa số tiền của giao dịch X từ `50.000đ` thành `70.000đ` lúc 09:00.
3. Trên **Thiết bị 2**, sửa số tiền của giao dịch X từ `50.000đ` thành `90.000đ` lúc 09:02.
4. Bật mạng cả 2 thiết bị -> Thiết bị 2 sửa sau nên giá trị `90.000đ` thắng và đồng bộ cho cả hai máy, không nhân đôi bản ghi.

### Bước 3.2: Thay thế Icon và Splash Screen
Thay thế các file trong `src/Client/SoChi.Client/Resources/`:
- `Resources/AppIcon/appicon.svg` (Biểu tượng cuốn sổ màu tím hoặc xanh)
- `Resources/Splash/splash.svg` (Màn hình khởi động)

Kiểm tra trong `SoChi.Client.csproj`:
```xml
<MauiIcon Include="Resources\AppIcon\appicon.svg" ForegroundFile="Resources\AppIcon\appiconfg.svg" Color="#3B82F6" />
<MauiSplashScreen Include="Resources\Splash\splash.svg" Color="#3B82F6" BaseSize="128,128" />
```

### Bước 3.3: Tạo Keystore và Build file APK Android
Mở PowerShell tạo Keystore bằng `keytool`:
```powershell
keytool -genkey -v -keystore sochi.keystore -alias sochi_key -keyalg RSA -keysize 2048 -validity 10000
```

Chạy lệnh Publish xuất file APK Release:
```powershell
dotnet publish src/Client/SoChi.Client/SoChi.Client.csproj `
  -f net10.0-android `
  -c Release `
  -p:AndroidKeyStore=true `
  -p:AndroidSigningKeyStore=sochi.keystore `
  -p:AndroidSigningKeyAlias=sochi_key `
  -p:AndroidSigningKeyPass=MatKhauCuaBan `
  -p:AndroidSigningStorePass=MatKhauCuaBan
```
File APK thành phẩm nằm tại: `src/Client/SoChi.Client/bin/Release/net10.0-android/publish/`.

### Bước 3.4: Xuất gói Windows MSIX
```powershell
dotnet publish src/Client/SoChi.Client/SoChi.Client.csproj `
  -f net10.0-windows10.0.19041.0 `
  -c Release `
  -p:GenerateAppxPackageOnBuild=true
```

## 4. Bẫy thường gặp (Pitfalls)
- **Lỗi:** Quên tắt chế độ bỏ qua chứng chỉ SSL dev (`ServerCertificateCustomValidationCallback`) trong bản Release.
  - **Khắc phục:** Luôn bọc đoạn code bỏ qua chứng chỉ trong khối biên dịch điều kiện `#if DEBUG ... #endif`.

## 5. Checklist nghiệm thu Buổi 15
- [ ] Copy file `.apk` vào điện thoại Android thật và cài đặt thành công.
- [ ] App mở lên có Splash Screen và Icon thương hiệu riêng, không còn icon con bot màu tím mặc định của .NET MAUI.
- [ ] Thời gian mở ứng dụng (Cold Start) dưới 3 giây.
- [ ] Gọi API thật trên máy chủ thành công và đồng bộ dữ liệu trơn tru.

---

🏠 [Mục lục toàn khóa](../HUONG-DAN-TUNG-BUOI.md)

⬅️ [BUỔI 14: Offline-First Sync Hai Chiều, Connectivity & Last-Write-Wins](buoi-14.md)

[BUỔI 16: Blazor WebAssembly — Dựng Web Dashboard, JWT Auth & Công Cụ Kiểm Tra Dữ Liệu](buoi-16.md) ➡️
