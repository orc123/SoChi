# BUỔI 01: Khởi Tạo Solution, Git, Clean Architecture & Khởi Động MVVM

**Thuộc chặng:** Chặng 0 & Khởi động Chặng 1 | **Thời lượng:** ~3 giờ

## 1. Mục tiêu buổi học (Definition of Done)
- Tạo dựng cấu trúc Solution chuẩn gồm 3 dự án: `Client` (MAUI), `Api` (ASP.NET Core Web API), `Shared` (Class Library).
- Cấu hình file `global.json`, `Directory.Build.props`, `.editorconfig`, `.gitignore`.
- Khởi tạo Git repository và thực hiện commit ban đầu.
- Cài đặt package `CommunityToolkit.Mvvm` cho `Client`.
- App chạy thành công trên Windows, hiển thị tiêu đề "SoChi".

## 2. Bản chất kiến trúc & Nguyên lý
- **Quy tắc phụ thuộc một chiều (Dependency Rule):** `Client` và `Api` đều tham chiếu tới `Shared`. `Shared` độc lập 100%, không chứa bất kỳ logic UI hay logic database nào. Điều này đảm bảo khi định nghĩa DTO hay Enum ở `Shared`, cả hai bên đều đồng bộ mã nguồn ở mức compile-time.
- **Pin SDK bằng `global.json`:** Máy tính có thể có nhiều phiên bản SDK .NET (8.0, 9.0, 10.0...). Ghim cụ thể phiên bản đảm bảo build ổn định giữa các máy khác nhau.
- **Tập trung hóa thuộc tính bằng `Directory.Build.props`:** Giúp bật `<Nullable>enable</Nullable>` và `<TreatWarningsAsErrors>true</TreatWarningsAsErrors>` cho toàn bộ project con mà không phải copy-paste từng file `.csproj`.

## 3. Các bước thực hành chi tiết

### Bước 1.1: Khởi tạo cấu trúc Solution và thư mục
Mở PowerShell tại thư mục gốc của repo (`d:\Project\MAUI\SoChi`):

> **Nếu bạn đã làm Buổi 00 thì solution và ba project đã có sẵn** — đọc lướt bước này để hiểu từng lệnh làm gì, rồi nhảy sang Bước 1.3.

```powershell
# Tạo solution (thêm --format slnx nếu muốn định dạng XML mới như repo SoChi)
dotnet new sln -n SoChi --format slnx

# Tạo 3 project theo cấu trúc src/
dotnet new maui        -n SoChi.Client -o src/Client/SoChi.Client
dotnet new webapi      -n SoChi.Api    -o src/Api/SoChi.Api
dotnet new classlib    -n SoChi.Shared -o src/Shared/SoChi.Shared

# Đưa các project vào solution
dotnet sln add src/Client/SoChi.Client/SoChi.Client.csproj
dotnet sln add src/Api/SoChi.Api/SoChi.Api.csproj
dotnet sln add src/Shared/SoChi.Shared/SoChi.Shared.csproj

# Thiết lập quan hệ tham chiếu
dotnet add src/Client/SoChi.Client/SoChi.Client.csproj reference src/Shared/SoChi.Shared/SoChi.Shared.csproj
dotnet add src/Api/SoChi.Api/SoChi.Api.csproj reference src/Shared/SoChi.Shared/SoChi.Shared.csproj
```

### Bước 1.2: Các file cấu hình toàn cục

> `global.json`, `Directory.Build.props` và `.gitignore` **đã được tạo ở Buổi 00 Bước 0.5** — đó mới là nguồn chuẩn. Đừng tạo lại đè lên, vì giá trị ở Buổi 00 khác: SDK ghim đúng bản thật trên máy bạn, và `TreatWarningsAsErrors` bật `true` chứ không phải `false`.

Việc duy nhất còn lại của bước này là thêm `.editorconfig` — file quy định cách format code, được Visual Studio, Rider và `dotnet format` cùng đọc. Tạo ở thư mục gốc repo:

```powershell
dotnet new editorconfig
```

Nó sinh ra một file rất dài với toàn bộ quy tắc mặc định của .NET. Vài dòng đáng biết mà bạn có thể tự chỉnh:

```ini
[*.{cs,xaml}]
indent_style = space
indent_size = 4
end_of_line = crlf
charset = utf-8-bom
trim_trailing_whitespace = true
insert_final_newline = true

# Ưu tiên khai báo kiểu tường minh thay vì var ở nơi kiểu không hiển nhiên
csharp_style_var_when_type_is_apparent = true:suggestion
```

Vì sao đáng làm ngay từ buổi đầu: nó chấm dứt tranh cãi tab/space và giữ diff Git sạch. Không có nó, một lần Visual Studio tự format lại file là commit của bạn phình lên hàng trăm dòng thay đổi vô nghĩa, che mất thay đổi thật.

Kiểm tra toàn bộ code đã đúng format:
```powershell
dotnet format SoChi.slnx --verify-no-changes
```

### Bước 1.3: Cài đặt thư viện MVVM cho Client
```powershell
dotnet add src/Client/SoChi.Client/SoChi.Client.csproj package CommunityToolkit.Mvvm
```

### Bước 1.4: Tổ chức thư mục chuẩn trong `src/Client/SoChi.Client`
Tạo các thư mục chức năng:
- `src/Client/SoChi.Client/Models/`
- `src/Client/SoChi.Client/Views/`
- `src/Client/SoChi.Client/ViewModels/`
- `src/Client/SoChi.Client/Services/`
- `src/Client/SoChi.Client/Converters/`
- `src/Client/SoChi.Client/Controls/`

### Bước 1.5: Khởi tạo Git và Commit
```powershell
git init
git add .
git commit -m "feat: setup initial solution structure with Client, Api, and Shared"
```

## 4. Bẫy thường gặp (Pitfalls)
- **Lỗi:** Gõ nhầm lệnh build khi chưa chỉ định framework: `dotnet build src/Client/SoChi.Client`.
  - **Cách khắc phục:** Project MAUI đa nền tảng (`TargetFrameworks` số nhiều) nên bắt buộc phải chỉ định `-f`, ví dụ:
    ```powershell
    dotnet build src/Client/SoChi.Client/SoChi.Client.csproj -f net10.0-windows10.0.19041.0
    ```
- **Lỗi:** `Shared` vô tình tham chiếu ngược `Client` hoặc `Api`.
  - **Cách khắc phục:** Mở `SoChi.Shared.csproj`, kiểm tra đảm bảo không có thẻ `<ProjectReference>` nào trong file này.

## 5. Checklist nghiệm thu Buổi 01
- [ ] Lệnh `dotnet build` tại thư mục gốc chạy thành công 100% không báo lỗi.
- [ ] Chạy app trên Windows bằng lệnh:
  ```powershell
  dotnet build src/Client/SoChi.Client/SoChi.Client.csproj -f net10.0-windows10.0.19041.0 -t:Run
  ```
- [ ] Cửa sổ app hiển thị bình thường.
- [ ] `.editorconfig` tồn tại ở thư mục gốc và `dotnet format SoChi.slnx --verify-no-changes` không báo lỗi.
- [ ] `SoChi.Shared.csproj` **không** có thẻ `<ProjectReference>` nào (Shared phải độc lập).
- [ ] `dotnet list src/Client/SoChi.Client/SoChi.Client.csproj package` thấy `CommunityToolkit.Mvvm`.
- [ ] Sáu thư mục `Models/`, `Views/`, `ViewModels/`, `Services/`, `Converters/`, `Controls/` đã có trong `src/Client/SoChi.Client/`.
- [ ] Lịch sử `git log` có commit khởi tạo.

---

🏠 [Mục lục toàn khóa](../HUONG-DAN-TUNG-BUOI.md)

⬅️ [BUỔI 00: Chuẩn Bị Môi Trường & Chuẩn Hóa Cấu Trúc Thư Mục](buoi-00.md)

[BUỔI 02: AppShell 5 Tabs, Modal Navigation & Dependency Injection](buoi-02.md) ➡️
