# Hướng Dẫn Chi Tiết Từng Buổi Học: Dự Án SoChi (19 Buổi + 2 Phụ Lục)

> Tài liệu thực hành chi tiết (Lab Guide & Syllabus) đồng hành cùng [DU-AN-HOC-TAP.md](DU-AN-HOC-TAP.md) và [HUONG-DAN-PROJECT.md](HUONG-DAN-PROJECT.md).  
> Thiết kế theo mô hình **15 buổi thực hành (~3h/buổi)**, chia làm **8 chặng**, cộng thêm **2 buổi bổ sung** (Buổi 00 chuẩn bị môi trường, Buổi 04B quản lý danh mục), **Chặng 9 mở rộng** gồm 2 buổi Blazor WebAssembly (Buổi 16–17), và **2 phụ lục** áp dụng xuyên suốt ở cuối tài liệu. Mỗi buổi học cung cấp mục tiêu rõ ràng, nguyên lý kiến trúc, hướng dẫn từng bước (code mẫu thực tế), các bẫy thường gặp và checklist nghiệm thu trên máy thật.

---

## 📑 Bảng Lộ Trình 19 Buổi

| Buổi | Chặng | Tên buổi học & Trọng tâm kỹ thuật | Sản phẩm đạt được sau 3h |
|:---:|:---:|---|---|
| **00** | **Chuẩn bị** | Chuẩn bị môi trường, chuẩn hóa cấu trúc thư mục & cấu hình toàn cục | Toolchain sẵn sàng, emulator chạy được, layout `src/` chuẩn, commit Git đầu tiên |
| **01** | **Chặng 0 & 1** | Khởi tạo Solution, Git, Clean Architecture & Khởi động MVVM | Solution chuẩn 3 tầng, build xanh, chạy app rỗng trên Windows |
| **02** | **Chặng 1** | AppShell 5 Tabs, Modal Navigation & Dependency Injection | Điều hướng 5 tabs, mở modal Thêm giao dịch, DI tự động |
| **03** | **Chặng 2** | Mô hình dữ liệu SQLite, Repository Pattern & Lazy Init | SQLite sẵn sàng, Data Model chuẩn Guid + VND + Sync State |
| **04** | **Chặng 2** | Seed Data, Form Thêm/Sửa & Hiển thị CollectionView | Nhập giao dịch -> Lưu SQLite -> Tắt mở lại app vẫn còn |
| **04B** | **Chặng 2** | Quản lý Danh mục: CRUD, chọn icon/màu, đặt hạn mức tháng | Tab Danh mục hoạt động đầy đủ, có nguồn nhập `MonthlyLimit` cho Buổi 11 |
| **05** | **Chặng 3** | Danh sách nâng cao: Grouping theo ngày, SwipeView & RefreshView | Danh sách nhóm ngày, tính tổng ngày, vuốt xóa, kéo làm mới |
| **06** | **Chặng 3** | Custom Numeric Keypad, Format VND Realtime & Snackbar Hoàn tác | Bàn phím số tự chế không dùng Entry, format tiền, Undo xóa |
| **07** | **Chặng 4** | Đồ họa 2D thuần: Donut Chart tự vẽ với Microsoft.Maui.Graphics | Biểu đồ tròn tỉ trọng chi tiêu tự vẽ, hỗ trợ Dark/Light mode |
| **08** | **Chặng 4** | Biểu đồ cột 6 tháng (Bar Chart), Responsive & Xoay màn hình | Biểu đồ cột doanh thu/chi tiêu, tính toán trục tọa độ tự động |
| **09** | **Chặng 5** | API thiết bị: Chụp/Chọn ảnh hóa đơn, FileSystem & Phân quyền | Chụp ảnh hóa đơn, lưu AppData, thumbnail, xin quyền Android |
| **10** | **Chặng 5** | Theme sáng/tối, Sinh trắc học vân tay, Haptic & Xuất file CSV | Đổi theme tức thì, bảo mật vân tay, rung phím, chia sẻ CSV |
| **11** | **Chặng 6** | Hạn mức chi tiêu (ProgressBar), Lọc, Debounce & xUnit Tests | Đặt hạn mức đổi màu, tìm kiếm debounce, tối thiểu 8 unit test xanh |
| **12** | **Chặng 7** | Backend ASP.NET Core Web API, Shared DTOs & JWT Auth | Backend Web API chạy ngon lành, cấp JWT token, PostgreSQL + EF Core |
| **13** | **Chặng 7** | Client tích hợp API, DelegatingHandler, SecureStorage & 10.0.2.2 | Client đăng nhập/đăng ký thành công từ cả Windows & Android Emulator |
| **14** | **Chặng 7** | Offline-First Sync hai chiều, Connectivity & Last-Write-Wins | Chế độ máy bay ghi nhận bình thường, có mạng tự sync không đè mất |
| **15** | **Chặng 7 & 8** | Kiểm thử kịch bản Sync thực tế & Đóng gói Release (APK/MSIX) | File APK cài máy thật, MSIX Windows, tối ưu Trim/AOT |
| **16** | **Chặng 9** | Blazor WebAssembly: dựng Web Dashboard, JWT Auth & công cụ kiểm tra dữ liệu | Web app phục vụ từ chính API, đăng nhập được, xem dữ liệu thô trên server |
| **17** | **Chặng 9** | Endpoint báo cáo, biểu đồ SVG tự vẽ & kiểm chứng sức khỏe dữ liệu | Trang thống kê có donut + bar chart, phát hiện bản ghi mồ côi, soi được xung đột sync |

---

# BUỔI 00: Chuẩn Bị Môi Trường & Chuẩn Hóa Cấu Trúc Thư Mục

> **Buổi bổ sung (~1h).** Làm trước Buổi 01. Bỏ qua buổi này thì đến Buổi 12–13 bạn sẽ mắc kẹt vì đường dẫn project không khớp và chứng chỉ HTTPS chưa được tin.

## 1. Mục tiêu buổi học (Definition of Done)
- Xác nhận SDK .NET 10 và workload MAUI sẵn sàng, chứng chỉ HTTPS dev đã được tin.
- Có ít nhất một Android emulator khởi động được đến màn hình chính.
- **Chuẩn hóa cấu trúc thư mục thật về đúng dạng `src/`** mà 15 buổi sau sử dụng.
- Có `global.json`, `Directory.Build.props`, `.gitignore` và commit Git đầu tiên.

## 2. Bản chất kiến trúc & Nguyên lý
- **Vì sao phải ghim SDK:** máy này cài song song nhiều SDK (2.0 → 10.0). Không có `global.json`, MSBuild chọn bản mới nhất tìm thấy — hôm nay đúng, ngày mai cài thêm bản khác là lỗi khó hiểu.
- **Workload MAUI tách rời SDK:** `dotnet new maui` chỉ chạy được khi workload `maui-*` đã cài cho **đúng** bản SDK đang active.
- **Vì sao chuẩn hóa thư mục ngay:** Visual Studio tạo project ở thư mục ngang hàng solution, còn toàn bộ 15 buổi dưới đây viết theo layout `src/Client/SoChi.Client`, `src/Api/SoChi.Api`, `src/Shared/SoChi.Shared`. Sửa lệch đường dẫn một lần ở đây tốn 10 phút; để đến Buổi 12 mới sửa thì phải dò lại hàng chục lệnh `dotnet add reference` và `using`.

## 3. Các bước thực hành chi tiết

### Bước 0.1: Kiểm tra SDK và workload
```powershell
dotnet --info
dotnet --list-sdks
dotnet workload list
```
Kết quả `dotnet workload list` phải có ít nhất `maui-windows` và `android`. Nếu thiếu:
```powershell
dotnet workload install maui
dotnet workload update
```

Bảng target build được trên máy Windows này:

| Target Framework | Build trên Windows? |
|---|---|
| `net10.0-windows10.0.19041.0` | ✅ |
| `net10.0-android` | ✅ |
| `net10.0-ios`, `net10.0-maccatalyst` | ❌ cần Mac build host (`Pair to Mac`) |

### Bước 0.2: Tin chứng chỉ HTTPS dev
```powershell
dotnet dev-certs https --check
dotnet dev-certs https --trust
```
Bấm **Yes** ở hộp thoại Windows hiện lên. Sau bước này, Client chạy trên **Windows / MacCatalyst** gọi `https://localhost:7xxx` sẽ không còn lỗi chứng chỉ.

Lưu ý: bước này **không** giải quyết được cho Android/iOS — hệ điều hành đó có kho chứng chỉ riêng, không đọc kho của Windows. Đó là lý do Buổi 13 vẫn phải viết đoạn bỏ qua validation trong `#if DEBUG`.

### Bước 0.3: Chuẩn bị Android emulator
Cách nhanh nhất là qua Visual Studio: **Tools → Android → Android Device Manager → New**, chọn Pixel 5, API 34 (Android 14), Image `x86_64`.

Kiểm tra bằng CLI:
```powershell
& "$env:LOCALAPPDATA\Android\Sdk\emulator\emulator.exe" -list-avds
& "$env:LOCALAPPDATA\Android\Sdk\platform-tools\adb.exe" devices
```
Emulator chạy chậm như rùa hoặc không boot được thường là do thiếu tăng tốc phần cứng. Bật **Hyper-V** và **Windows Hypervisor Platform** trong "Turn Windows features on or off", khởi động lại máy.

### Bước 0.4: Đối chiếu cấu trúc thư mục

Repo thật đặt tại `d:\Project\MAUI\SoChi` — đây cũng là **thư mục gốc của Git**. Layout theo quy ước *mỗi project nằm trong thư mục riêng, bên trong một thư mục nhóm*:

```
SoChi/                       ← gốc repo (chứa .git)
  SoChi.slnx
  global.json
  Directory.Build.props
  .gitignore   .gitattributes
  docs/
  src/
    Client/SoChi.Client/SoChi.Client.csproj    # app MAUI
    Api/SoChi.Api/SoChi.Api.csproj             # Web API
    Shared/SoChi.Shared/SoChi.Shared.csproj    # DTO dùng chung
  tests/
    SoChi.Client.Tests/      # tạo ở Buổi 11
    SoChi.Api.Tests/         # tạo ở Buổi 12
```

Toàn bộ tài liệu này (19 buổi + 2 phụ lục) đã dùng đúng các đường dẫn trên. Ba điều cần nhớ để không gõ nhầm:

1. **Thư mục nhóm và thư mục project là hai cấp khác nhau.** `src/Client` là nhóm, `src/Client/SoChi.Client` mới là project. Mọi lệnh `dotnet` phải trỏ tới cấp trong cùng — nơi chứa file `.csproj`.
2. **Thư mục test tên là `tests/`** (số nhiều, đúng quy ước phổ biến trong hệ sinh thái .NET), và mỗi project test nằm thẳng trong đó, không có thêm cấp nhóm như bên `src/`.
3. Lợi ích của layout này: sau này muốn thêm `src/Client/SoChi.Client.Maui.Tests` hay tách một thư viện con cho Client thì đã có sẵn chỗ, không phải sắp xếp lại.

**Về file `SoChi.slnx`.** Đây là định dạng solution mới (XML, thay cho `.sln` cú pháp cũ) và chạy bình thường với `dotnet build`, `dotnet sln`, `dotnet test`. Điểm dễ nhầm: các thẻ `<Folder Name="/src/Client/">` chỉ là **thư mục ảo hiển thị trong IDE**, không tạo thư mục thật trên đĩa. Thứ trỏ tới file thật là thuộc tính `Path` của `<Project>`:

```xml
<Folder Name="/src/Client/">
  <Project Path="src/Client/SoChi.Client/SoChi.Client.csproj" ... />
</Folder>
```

Nếu `src/Api` hoặc `src/Shared` chưa có, tạo và nối dây như sau (chạy từ gốc repo):

```powershell
dotnet new webapi   -n SoChi.Api    -o src/Api/SoChi.Api
dotnet new classlib -n SoChi.Shared -o src/Shared/SoChi.Shared

dotnet sln SoChi.slnx add src/Api/SoChi.Api/SoChi.Api.csproj src/Shared/SoChi.Shared/SoChi.Shared.csproj

dotnet add src/Client/SoChi.Client/SoChi.Client.csproj reference src/Shared/SoChi.Shared/SoChi.Shared.csproj
dotnet add src/Api/SoChi.Api/SoChi.Api.csproj          reference src/Shared/SoChi.Shared/SoChi.Shared.csproj
```

Kiểm tra hướng tham chiếu đã đúng — cả hai đều trỏ **vào** `Shared`, và `Shared` không trỏ đi đâu cả:

```powershell
dotnet list src/Client/SoChi.Client/SoChi.Client.csproj reference
dotnet list src/Api/SoChi.Api/SoChi.Api.csproj reference
dotnet list src/Shared/SoChi.Shared/SoChi.Shared.csproj reference
```

### Bước 0.5: Ba file cấu hình cấp solution
Tạo `global.json` ở gốc repo (`d:\Project\MAUI\SoChi`):
```json
{
  "sdk": {
    "version": "10.0.400",
    "rollForward": "latestFeature"
  }
}
```

Xác nhận nó có tác dụng — lệnh sau phải in ra đúng bản đã ghim:
```powershell
dotnet --version
```

Tạo `Directory.Build.props` cùng cấp:
```xml
<Project>
  <PropertyGroup>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
    <TreatWarningsAsErrors>true</TreatWarningsAsErrors>
    <LangVersion>latest</LangVersion>
  </PropertyGroup>
</Project>
```

`.gitignore` và commit đầu tiên:
```powershell
dotnet new gitignore
git init
git add .
git commit -m "chore(session-00): bootstrap solution structure and toolchain config"
```

### Bước 0.6: Cài Android SDK Platform khớp với workload

Đây là lỗi gần như ai cũng gặp ở lần build Android đầu tiên, và thông báo lỗi thì không nói thẳng ra:

```
error XA5207: Could not find android.jar for API level 36.
Expected: ...\Android\Sdk\platforms\android-36\android.jar
```

Nguyên nhân: workload `android` biên dịch theo một API level cố định (ví dụ 36), nhưng Android SDK trên máy chỉ có các platform cũ hơn. Ba target còn lại (`windows`, `ios`, `maccatalyst`) vẫn build xanh, nên rất dễ tưởng project hỏng.

Xem đang có những platform nào:
```powershell
Get-ChildItem "$env:LOCALAPPDATA\Android\Sdk\platforms"
```

Cài platform còn thiếu — MSBuild tự tải đúng bản mà project cần:
```powershell
dotnet build src/Client/SoChi.Client/SoChi.Client.csproj `
  -t:InstallAndroidDependencies -f net10.0-android `
  -p:AndroidSdkDirectory="$env:LOCALAPPDATA\Android\Sdk" `
  -p:AcceptAndroidSDKLicenses=True
```

> **API của emulator không cần trùng API biên dịch.** Emulator API 35 chạy tốt app biên dịch theo API 36, vì `SupportedOSPlatformVersion` của project là 21.0 — đó mới là phiên bản Android tối thiểu. API 36 chỉ cần cho khâu *biên dịch*, không phải khâu *chạy*.

### Bước 0.7: Build thử toàn bộ
```powershell
dotnet build SoChi.slnx
dotnet build src/Client/SoChi.Client/SoChi.Client.csproj -f net10.0-windows10.0.19041.0 -t:Run
```
App mặc định (`MainPage.xaml` với nút Click me) hiện lên là đạt. Buổi 02 sẽ thay `MainPage` bằng `AppShell` — chưa xóa nó vội, vì `App.xaml.cs` còn đang trỏ tới.

## 4. Bẫy thường gặp (Pitfalls)
- **Lỗi:** `error XA5207: Could not find android.jar for API level 36` khi build solution, trong khi Windows/iOS/MacCatalyst vẫn xanh.
  - **Nguyên nhân:** Thiếu Android SDK Platform tương ứng. Xem Bước 0.6.
- **Lỗi:** `dotnet build` báo không tìm thấy project, hoặc `MSB1003: Specify a project or solution file`.
  - **Nguyên nhân:** Trỏ vào thư mục nhóm (`src/Client`) thay vì thư mục chứa `.csproj` (`src/Client/SoChi.Client`). Đây là nhầm lẫn phổ biến nhất với layout hai cấp.
- **Lỗi:** Mở solution trong Visual Studio báo "The project file could not be found".
  - **Nguyên nhân:** Thuộc tính `Path` trong `SoChi.slnx` không khớp vị trí thật của `.csproj`. Nhớ rằng `<Folder>` chỉ là thư mục ảo, sửa nó không làm project di chuyển.
- **Lỗi:** `dotnet workload list` trống dù Visual Studio đã cài .NET MAUI.
  - **Nguyên nhân:** VS quản lý workload theo bản SDK riêng của nó. Sau khi tạo `global.json` ghim bản SDK, chạy lại `dotnet workload install maui` để cài cho đúng bản đó.
- **Lỗi:** `TreatWarningsAsErrors` làm build đỏ ngay từ project MAUI mặc định.
  - **Nguyên nhân:** Template sinh vài warning nhỏ. Sửa cho sạch — đừng tắt cờ này, nó là thứ giữ codebase lành mạnh suốt 19 buổi.
- **Lỗi:** Emulator chạy chậm như rùa hoặc không boot.
  - **Nguyên nhân:** Thiếu tăng tốc phần cứng. Bật **Hyper-V** và **Windows Hypervisor Platform** trong "Turn Windows features on or off", khởi động lại máy.

## 5. Checklist nghiệm thu Buổi 00
- [ ] `dotnet workload list` liệt kê đủ `maui-windows` và `android`.
- [ ] `dotnet --version` in ra đúng phiên bản ghi trong `global.json`.
- [ ] `dotnet dev-certs https --check --trust` báo có chứng chỉ **trusted** (chỉ `--check` thôi là chưa đủ — nó không kiểm tra tính tin cậy).
- [ ] `Get-ChildItem "$env:LOCALAPPDATA\Android\Sdk\platforms"` có platform khớp API mà project biên dịch.
- [ ] Android emulator boot được tới màn hình chính (chưa cần chạy app).
- [ ] Ba file `.csproj` nằm đúng chỗ: `src/Client/SoChi.Client/`, `src/Api/SoChi.Api/`, `src/Shared/SoChi.Shared/`.
- [ ] `dotnet list ... reference` cho thấy Client và Api đều tham chiếu Shared, còn Shared không tham chiếu ai.
- [ ] `dotnet build SoChi.slnx` xanh **cả bốn target**, không warning.
- [ ] `git log --oneline` hiện commit đầu tiên.

---

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

# BUỔI 02: AppShell 5 Tabs, Modal Navigation & Dependency Injection

**Thuộc chặng:** Chặng 1 | **Thời lượng:** ~3 giờ

## 1. Mục tiêu buổi học (Definition of Done)
- Thiết lập `AppShell.xaml` chứa đầy đủ 5 Tabs: **Tổng quan, Giao dịch, Thống kê, Danh mục, Cài đặt**.
- Tạo 5 `ContentPage` tương ứng trong thư mục `Views/` và 5 `ViewModel` tương ứng trong `ViewModels/`.
- Đăng ký toàn bộ Page và ViewModel vào DI Container trong `MauiProgram.cs`.
- Tạo trang **Thêm/Sửa giao dịch** (`TransactionFormPage`), đăng ký Route và mở dưới dạng Modal Popup (`Shell.Current.GoToAsync("//...")`).
- Không có bất kỳ dòng lệnh nào khởi tạo thủ công `new SomeViewModel()` trong Code-behind (`.xaml.cs`).

## 2. Bản chất kiến trúc & Nguyên lý
- **Cơ chế Constructor Injection với Shell:** MAUI Shell sử dụng `DataTemplate` để tạo trang. Khi một Page được khai báo trong `MauiProgram.cs`, MAUI sẽ tự động gọi DI ServiceProvider để khởi tạo Page đó cùng các dependency được khai báo trong constructor của nó (ViewModel, Services).
- **Tránh Memory Leak:** Page và ViewModel cho các màn hình Tab thông thường đăng ký `Transient` hoặc `Singleton` (thường Tab chính dùng `Transient` kết hợp giữ state tại ViewModel nếu cần, hoặc `Singleton` cho trang tĩnh). Trang Modal nên dùng `AddTransient` để giải phóng bộ nhớ khi đóng lại.
- **Route Navigation:** Trang modal không nằm trực tiếp trong thanh TabBar mà được đăng ký qua `Routing.RegisterRoute("ten_route", typeof(TransactionFormPage))`.

## 3. Các bước thực hành chi tiết

### Bước 2.1: Tạo 5 ViewModel cơ bản dùng CommunityToolkit.Mvvm
Ví dụ tạo `OverviewViewModel.cs` trong `src/Client/SoChi.Client/ViewModels/`:
```csharp
using CommunityToolkit.Mvvm.ComponentModel;

namespace SoChi.Client.ViewModels;

public partial class OverviewViewModel : ObservableObject
{
    [ObservableProperty]
    public partial string Title { get; set; } = "Tổng Quan";
}
```
Tạo tương tự cho:
- `TransactionsViewModel.cs` (Tab Giao dịch)
- `StatisticsViewModel.cs` (Tab Thống kê)
- `CategoriesViewModel.cs` (Tab Danh mục)
- `SettingsViewModel.cs` (Tab Cài đặt)
- `TransactionFormViewModel.cs` (Modal Thêm/Sửa)

> ### ⚠️ Phải dùng `partial property`, không dùng field
>
> Đây là thay đổi mà hầu hết tutorial trên mạng chưa cập nhật, và nó sẽ làm bạn mất thời gian nếu không biết trước.
>
> Từ CommunityToolkit.Mvvm 8.4, đặt `[ObservableProperty]` lên một **field** sẽ sinh ra:
>
> ```
> error MVVMTK0045: The field ..._title using [ObservableProperty] will generate code
> that is not AOT compatible in WinRT scenarios (such as UWP XAML and WinUI 3 apps),
> and a partial property should be used instead
> ```
>
> Hai điều khiến lỗi này khó chẩn đoán:
>
> 1. **Chỉ nổ ở target Windows.** WinUI 3 chạy trên WinRT, còn Android/iOS/MacCatalyst thì không. Nên `dotnet build SoChi.slnx` có thể xanh, mà build riêng Windows lại đỏ — dễ tưởng hỏng máy hay hỏng cache.
> 2. **Bản chất nó là *warning*, nhưng thành *error*** vì `TreatWarningsAsErrors=true` đã bật từ Buổi 00.
>
> Cách viết đúng — đổi `private` thành `public partial`, tên sang PascalCase, thêm `{ get; set; }`. Giá trị mặc định giữ nguyên tại chỗ:
>
> ```csharp
> // ❌ Cách cũ (mọi blog trước 2025 đều viết thế này)
> [ObservableProperty]
> private string _title = "Tổng Quan";
>
> // ✅ Cách đúng với .NET 10 + toolkit 8.4 trở lên
> [ObservableProperty]
> public partial string Title { get; set; } = "Tổng Quan";
> ```
>
> Class vẫn phải khai báo `partial` như cũ. **Tên property dùng trong XAML và trong code không đổi** — trước đây toolkit cũng sinh ra đúng tên PascalCase đó từ field (`_title` → `Title`), chỉ khác là giờ bạn viết nó ra tường minh. Vì vậy mọi `{Binding Title}` trong tài liệu này vẫn đúng nguyên.
>
> Nếu đang theo một tutorial cũ và muốn giữ cú pháp field, có thể tắt cảnh báo bằng `<NoWarn>$(NoWarn);MVVMTK0045</NoWarn>` trong `SoChi.Client.csproj`. Nhưng đừng làm vậy: partial property là hướng đi chính thức, và bạn sẽ cần nó khi bật AOT ở Buổi 15.

### Bước 2.2: Tạo các trang Views và gán BindingContext qua DI
Ví dụ tạo `OverviewPage.xaml` trong `src/Client/SoChi.Client/Views/`:
```xml
<?xml version="1.0" encoding="utf-8" ?>
<ContentPage xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             xmlns:vm="clr-namespace:SoChi.Client.ViewModels"
             x:Class="SoChi.Client.Views.OverviewPage"
             x:DataType="vm:OverviewViewModel"
             Title="{Binding Title}">
    <VerticalStackLayout Spacing="20" Padding="20" VerticalOptions="Center">
        <Label Text="Màn hình Tổng Quan" HorizontalOptions="Center" FontSize="20" />
    </VerticalStackLayout>
</ContentPage>
```

Trong `OverviewPage.xaml.cs`:
```csharp
using SoChi.Client.ViewModels;

namespace SoChi.Client.Views;

public partial class OverviewPage : ContentPage
{
    public OverviewPage(OverviewViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel; // DI tự inject ViewModel
    }
}
```

### Bước 2.3: Xây dựng AppShell.xaml với TabBar 5 tabs
Cập nhật `src/Client/SoChi.Client/AppShell.xaml`:
```xml
<?xml version="1.0" encoding="UTF-8" ?>
<Shell
    x:Class="SoChi.Client.AppShell"
    xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
    xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
    xmlns:views="clr-namespace:SoChi.Client.Views"
    Title="SoChi">

    <TabBar>
        <Tab Title="Tổng quan" Icon="icon_overview.png">
            <ShellContent ContentTemplate="{DataTemplate views:OverviewPage}" Route="overview" />
        </Tab>
        <Tab Title="Giao dịch" Icon="icon_transactions.png">
            <ShellContent ContentTemplate="{DataTemplate views:TransactionsPage}" Route="transactions" />
        </Tab>
        <Tab Title="Thống kê" Icon="icon_statistics.png">
            <ShellContent ContentTemplate="{DataTemplate views:StatisticsPage}" Route="statistics" />
        </Tab>
        <Tab Title="Danh mục" Icon="icon_categories.png">
            <ShellContent ContentTemplate="{DataTemplate views:CategoriesPage}" Route="categories" />
        </Tab>
        <Tab Title="Cài đặt" Icon="icon_settings.png">
            <ShellContent ContentTemplate="{DataTemplate views:SettingsPage}" Route="settings" />
        </Tab>
    </TabBar>
</Shell>
```

### Bước 2.4: Đăng ký Route cho Modal trong `AppShell.xaml.cs`
```csharp
using SoChi.Client.Views;

namespace SoChi.Client;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        Routing.RegisterRoute("transaction_form", typeof(TransactionFormPage));
    }
}
```

### Bước 2.5: Đăng ký DI trong `MauiProgram.cs`
```csharp
// Đăng ký ViewModels
builder.Services.AddTransient<OverviewViewModel>();
builder.Services.AddTransient<TransactionsViewModel>();
builder.Services.AddTransient<StatisticsViewModel>();
builder.Services.AddTransient<CategoriesViewModel>();
builder.Services.AddTransient<SettingsViewModel>();
builder.Services.AddTransient<TransactionFormViewModel>();

// Đăng ký Pages
builder.Services.AddTransient<OverviewPage>();
builder.Services.AddTransient<TransactionsPage>();
builder.Services.AddTransient<StatisticsPage>();
builder.Services.AddTransient<CategoriesPage>();
builder.Services.AddTransient<SettingsPage>();
builder.Services.AddTransient<TransactionFormPage>();
```

### Bước 2.6: Thử nghiệm mở và đóng Modal
Trong `OverviewViewModel.cs`:
```csharp
[RelayCommand]
private async Task OpenAddTransactionAsync()
{
    await Shell.Current.GoToAsync("transaction_form");
}
```
Trong `TransactionFormViewModel.cs`:
```csharp
[RelayCommand]
private async Task CancelAsync()
{
    await Shell.Current.GoToAsync("..");
}
```

## 4. Bẫy thường gặp (Pitfalls)
- **Lỗi:** `System.InvalidOperationException: Unable to resolve service for type 'SoChi.Client.Views.OverviewPage'`.
  - **Nguyên nhân:** Quên thêm `builder.Services.AddTransient<OverviewPage>()` trong `MauiProgram.cs`. Mọi trang có tham số trong constructor đều phải khai báo DI.
- **Lỗi:** Bấm nút mở Modal nhưng app đứng im hoặc ném ngoại lệ `Route not found`.
  - **Nguyên nhân:** Tên route truyền vào `GoToAsync("transaction_form")` không khớp chính xác với chuỗi đăng ký trong `Routing.RegisterRoute`.

## 5. Checklist nghiệm thu Buổi 02
- [ ] Chuyển qua lại giữa 5 tab mượt mà, tiêu đề trên mỗi tab hiển thị đúng.
- [ ] Tại màn hình Tổng quan, bấm nút mở Form Thêm giao dịch -> Màn hình Modal hiển thị.
- [ ] Bấm nút "Hủy / Đóng" trên Modal -> Quay về đúng màn hình trước đó.
- [ ] Kiểm tra toàn bộ code-behind (`.xaml.cs`) không có toán tử `new` với ViewModel.

---

# BUỔI 03: Mô Hình Dữ Liệu SQLite, Repository Pattern & Lazy Init

**Thuộc chặng:** Chặng 2 (Phần 1) | **Thời lượng:** ~3 giờ

## 1. Mục tiêu buổi học (Definition of Done)
- Cài đặt package `sqlite-net-pcl` và cấu hình lưu trữ database tại `FileSystem.AppDataDirectory`.
- Thiết kế 2 Entity cốt lõi: `Category` và `Transaction` theo đúng các quy tắc vàng:
  - Khóa chính là `Guid`.
  - Tiền tệ lưu số nguyên `long` (VND).
  - Có các trường phục vụ đồng bộ: `UpdatedAt`, `IsDeleted`, `SyncState`.
- Xây dựng tầng truy cập dữ liệu (Repository Pattern) áp dụng kỹ thuật **Lazy Async Initialization** an toàn (chống deadlock).
- Đăng ký Repository vào DI Container dạng `Singleton`.

## 2. Bản chất kiến trúc & Nguyên lý
- **Tại sao dùng Guid thay vì Int Auto-Increment?** Khi hoạt động Offline, client tạo dữ liệu cục bộ. Nếu dùng `int` tự tăng, Client A tạo ID = 1, Client B tạo ID = 1, khi sync lên Server sẽ gây va chạm ID. `Guid` cho phép sinh định danh duy nhất toàn cầu ở bất kỳ thiết bị nào mà không cần hỏi server.
- **Tại sao tiền tệ dùng số nguyên (`long`) thay vì `double`?** `double` dùng dấu phẩy động nhị phân, phép tính cộng trừ có sai số thập phân (vd: `0.1 + 0.2 != 0.3`). Tiền VND không có đơn vị lẻ thập phân, dùng `long` đảm bảo chính xác 100%.
- **Lazy Async Initialization:** Constructor trong C# không hỗ trợ từ khóa `async`. Nếu gọi `db.CreateTableAsync().Wait()` hoặc `.Result` trong constructor sẽ dẫn tới hiện tượng **UI Deadlock**. Giải pháp là sử dụng phương thức khởi tạo bất đồng bộ trễ (Lazy init pattern).

## 3. Các bước thực hành chi tiết

### Bước 3.1: Cài đặt thư viện SQLite
```powershell
dotnet add src/Client/SoChi.Client/SoChi.Client.csproj package sqlite-net-pcl
dotnet add src/Client/SoChi.Client/SoChi.Client.csproj package SQLitePCLRaw.bundle_green
```

### Bước 3.2: Định nghĩa các Enum và Model
Tạo file `src/Client/SoChi.Client/Models/Enums.cs`:
```csharp
namespace SoChi.Client.Models;

public enum TransactionKind
{
    Expense = 0, // Chi tiêu
    Income = 1   // Thu nhập
}

public enum SyncState
{
    Local = 0,   // Mới tạo hoặc sửa ở máy, chưa sync
    Synced = 1   // Đã đồng bộ thành công với server
}
```

Tạo file `src/Client/SoChi.Client/Models/Category.cs`:
```csharp
using SQLite;

namespace SoChi.Client.Models;

[Table("Categories")]
public class Category
{
    [PrimaryKey]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Indexed]
    public string Name { get; set; } = string.Empty;

    public string IconGlyph { get; set; } = string.Empty;

    public string ColorHex { get; set; } = "#3B82F6";

    public TransactionKind Kind { get; set; } = TransactionKind.Expense;

    public long? MonthlyLimit { get; set; }

    // Trường đồng bộ
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public bool IsDeleted { get; set; } = false;
    public SyncState SyncState { get; set; } = SyncState.Local;
}
```

Tạo file `src/Client/SoChi.Client/Models/Transaction.cs`:
```csharp
using SQLite;

namespace SoChi.Client.Models;

[Table("Transactions")]
public class Transaction
{
    [PrimaryKey]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Indexed]
    public Guid CategoryId { get; set; }

    public long Amount { get; set; } // Đơn vị: Đồng

    [Indexed]
    public DateTime OccurredOn { get; set; } = DateTime.Today;

    public string Note { get; set; } = string.Empty;

    public string? ReceiptPath { get; set; }

    // Trường đồng bộ
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public bool IsDeleted { get; set; } = false;
    public SyncState SyncState { get; set; } = SyncState.Local;
}
```

### Bước 3.3: Xây dựng Database Service với Lazy Async Init
Tạo interface `src/Client/SoChi.Client/Services/ITransactionRepository.cs`:
```csharp
using SoChi.Client.Models;

namespace SoChi.Client.Services;

public interface ITransactionRepository
{
    Task<List<Transaction>> GetTransactionsAsync();
    Task<Transaction?> GetTransactionByIdAsync(Guid id);
    Task<int> SaveTransactionAsync(Transaction item);
    Task<int> SoftDeleteTransactionAsync(Guid id);
    
    Task<List<Category>> GetCategoriesAsync();
    Task<int> SaveCategoryAsync(Category category);
    Task InitializeAsync();
}
```

Tạo class triển khai `src/Client/SoChi.Client/Services/SqliteTransactionRepository.cs`:
```csharp
using SQLite;
using SoChi.Client.Models;

namespace SoChi.Client.Services;

public class SqliteTransactionRepository : ITransactionRepository
{
    private SQLiteAsyncConnection? _database;
    private readonly string _dbPath;
    private bool _isInitialized;
    private readonly SemaphoreSlim _initLock = new(1, 1);

    public SqliteTransactionRepository()
    {
        _dbPath = Path.Combine(FileSystem.AppDataDirectory, "sochi_v1.db3");
    }

    public async Task InitializeAsync()
    {
        if (_isInitialized) return;

        await _initLock.WaitAsync();
        try
        {
            if (_isInitialized) return;

            _database = new SQLiteAsyncConnection(_dbPath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.SharedCache);
            await _database.CreateTableAsync<Category>();
            await _database.CreateTableAsync<Transaction>();

            _isInitialized = true;
        }
        finally
        {
            _initLock.Release();
        }
    }

    private async Task<SQLiteAsyncConnection> GetDatabaseAsync()
    {
        if (!_isInitialized)
        {
            await InitializeAsync();
        }
        return _database!;
    }

    public async Task<List<Transaction>> GetTransactionsAsync()
    {
        var db = await GetDatabaseAsync();
        return await db.Table<Transaction>()
                       .Where(t => !t.IsDeleted)
                       .OrderByDescending(t => t.OccurredOn)
                       .ToListAsync();
    }

    public async Task<Transaction?> GetTransactionByIdAsync(Guid id)
    {
        var db = await GetDatabaseAsync();
        return await db.Table<Transaction>().FirstOrDefaultAsync(t => t.Id == id && !t.IsDeleted);
    }

    public async Task<int> SaveTransactionAsync(Transaction item)
    {
        var db = await GetDatabaseAsync();
        item.UpdatedAt = DateTime.UtcNow;
        item.SyncState = SyncState.Local;

        var existing = await GetTransactionByIdAsync(item.Id);
        if (existing == null)
        {
            return await db.InsertAsync(item);
        }
        return await db.UpdateAsync(item);
    }

    public async Task<int> SoftDeleteTransactionAsync(Guid id)
    {
        var db = await GetDatabaseAsync();
        var existing = await db.Table<Transaction>().FirstOrDefaultAsync(t => t.Id == id);
        if (existing != null)
        {
            existing.IsDeleted = true;
            existing.UpdatedAt = DateTime.UtcNow;
            existing.SyncState = SyncState.Local;
            return await db.UpdateAsync(existing);
        }
        return 0;
    }

    public async Task<List<Category>> GetCategoriesAsync()
    {
        var db = await GetDatabaseAsync();
        return await db.Table<Category>().Where(c => !c.IsDeleted).ToListAsync();
    }

    public async Task<int> SaveCategoryAsync(Category category)
    {
        var db = await GetDatabaseAsync();
        category.UpdatedAt = DateTime.UtcNow;
        category.SyncState = SyncState.Local;

        var existing = await db.Table<Category>().FirstOrDefaultAsync(c => c.Id == category.Id);
        if (existing == null) return await db.InsertAsync(category);
        return await db.UpdateAsync(category);
    }
}
```

### Bước 3.4: Đăng ký Repository vào DI
Trong `MauiProgram.cs`:
```csharp
builder.Services.AddSingleton<ITransactionRepository, SqliteTransactionRepository>();
```

## 4. Bẫy thường gặp (Pitfalls)
- **Lỗi:** Gọi `.Wait()` hoặc `.Result` khi mở kết nối database trong hàm khởi tạo:
  ```csharp
  public SqliteTransactionRepository() {
      _database.CreateTableAsync<Category>().Wait(); // NGUY HIỂM: Gây treo app ngay lập tức trên Mobile!
  }
  ```
  - **Khắc phục:** Luôn tuân thủ cơ chế `InitializeAsync()` kết hợp SemaphoreSlim như hướng dẫn ở trên.

## 5. Checklist nghiệm thu Buổi 03
- [ ] Database file được cấu hình tạo đúng tại `FileSystem.AppDataDirectory`.
- [ ] Hai class `Category` và `Transaction` định nghĩa đủ các thuộc tính (Guid, long, IsDeleted, SyncState).
- [ ] Gọi thử `SaveTransactionAsync()` và `GetTransactionsAsync()` trong hàm kiểm tra không gặp deadlock.

---

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

# BUỔI 05: Danh Sách Nâng Cao: Grouping Theo Ngày, SwipeView & RefreshView

**Thuộc chặng:** Chặng 3 (Phần 1) | **Thời lượng:** ~3 giờ

## 1. Mục tiêu buổi học (Definition of Done)
- Chuyển đổi `CollectionView` phẳng thành **Danh sách nhóm theo ngày (Grouped Collection)**.
- Header của mỗi nhóm hiển thị ngày giao dịch kèm **Tổng tiền thu/chi** của ngày đó.
- Thêm tính năng **Kéo xuống để làm mới (Pull-to-refresh)** với `RefreshView`.
- Thêm tính năng **Vuốt sang trái để xóa (`SwipeView`)** kèm hộp thoại xác nhận.
- Cấu hình giao diện trạng thái rỗng (`EmptyView`) với hình minh họa khi chưa có giao dịch nào.

## 2. Bản chất kiến trúc & Nguyên lý
- **Cấu trúc dữ liệu cho Grouping trong MAUI:** `CollectionView` không thể tự nhóm một danh sách phẳng. Bạn phải cung cấp một `ObservableCollection<TGroup>` trong đó `TGroup` kế thừa từ `List<TItem>` hoặc `ObservableCollection<TItem>`. Header của nhóm sẽ bind vào thuộc tính của `TGroup`, còn từng item con sẽ bind vào `TItem`.
- **Hiệu năng cuộn mượt:** Không dùng lồng nhiều tầng Layout (`StackLayout` lồng `StackLayout`) bên trong `ItemTemplate`. Luôn ưu tiên dùng `Grid` với kích thước cột rõ ràng để tránh đo đạc layout nhiều lần khi cuộn.

## 3. Các bước thực hành chi tiết

### Bước 5.1: Xây dựng Model Grouping
Tạo file `src/Client/SoChi.Client/Models/TransactionGroup.cs`:
```csharp
using System.Collections.ObjectModel;

namespace SoChi.Client.Models;

public class TransactionItemDisplay
{
    public Guid Id { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public string CategoryIcon { get; set; } = "💸";
    public string CategoryColor { get; set; } = "#3B82F6";
    public long Amount { get; set; }
    public string Note { get; set; } = string.Empty;
    public TransactionKind Kind { get; set; }
    public DateTime OccurredOn { get; set; }
}

public class TransactionGroup : ObservableCollection<TransactionItemDisplay>
{
    public DateTime Date { get; set; }
    public long TotalAmount => this.Sum(t => t.Kind == TransactionKind.Expense ? -t.Amount : t.Amount);

    public string FormattedDate => Date.Date == DateTime.Today
        ? $"Hôm nay ({Date:dd/MM})"
        : (Date.Date == DateTime.Today.AddDays(-1) ? $"Hôm qua ({Date:dd/MM})" : Date.ToString("dddd, dd/MM/yyyy"));

    public TransactionGroup(DateTime date, IEnumerable<TransactionItemDisplay> items) : base(items)
    {
        Date = date;
    }
}
```

### Bước 5.2: Cập nhật `TransactionsViewModel` xử lý nhóm
```csharp
[ObservableProperty] public partial bool IsRefreshing { get; set; }

public ObservableCollection<TransactionGroup> GroupedTransactions { get; } = new();

[RelayCommand]
public async Task RefreshDataAsync()
{
    IsRefreshing = true;
    try
    {
        var rawTransactions = await _repository.GetTransactionsAsync();
        var categories = (await _repository.GetCategoriesAsync()).ToDictionary(c => c.Id);

        var displayItems = rawTransactions.Select(t => new TransactionItemDisplay
        {
            Id = t.Id,
            Amount = t.Amount,
            Note = string.IsNullOrWhiteSpace(t.Note) ? (categories.ContainsKey(t.CategoryId) ? categories[t.CategoryId].Name : "Không tên") : t.Note,
            OccurredOn = t.OccurredOn,
            CategoryName = categories.ContainsKey(t.CategoryId) ? categories[t.CategoryId].Name : "Khác",
            CategoryIcon = categories.ContainsKey(t.CategoryId) ? categories[t.CategoryId].IconGlyph : "🏷️",
            CategoryColor = categories.ContainsKey(t.CategoryId) ? categories[t.CategoryId].ColorHex : "#6B7280",
            Kind = categories.ContainsKey(t.CategoryId) ? categories[t.CategoryId].Kind : TransactionKind.Expense
        });

        var groups = displayItems
            .GroupBy(t => t.OccurredOn.Date)
            .OrderByDescending(g => g.Key)
            .Select(g => new TransactionGroup(g.Key, g));

        GroupedTransactions.Clear();
        foreach (var g in groups)
        {
            GroupedTransactions.Add(g);
        }
    }
    finally
    {
        IsRefreshing = false;
    }
}

[RelayCommand]
private async Task DeleteTransactionAsync(TransactionItemDisplay item)
{
    bool confirm = await Shell.Current.DisplayAlert("Xác nhận", $"Bạn có chắc muốn xóa '{item.Note}'?", "Xóa", "Hủy");
    if (!confirm) return;

    await _repository.SoftDeleteTransactionAsync(item.Id);
    await RefreshDataAsync();
}
```

### Bước 5.3: Xây dựng XAML với GroupHeaderTemplate, SwipeView & EmptyView
Cập nhật `TransactionsPage.xaml`:
```xml
<RefreshView IsRefreshing="{Binding IsRefreshing}" Command="{Binding RefreshDataCommand}">
    <CollectionView ItemsSource="{Binding GroupedTransactions}"
                    IsGrouped="True">
        
        <!-- Header của từng nhóm ngày -->
        <CollectionView.GroupHeaderTemplate>
            <DataTemplate x:DataType="models:TransactionGroup">
                <Grid ColumnDefinitions="*, Auto" Padding="16,10" BackgroundColor="#F3F4F6">
                    <Label Text="{Binding FormattedDate}" FontAttributes="Bold" TextColor="#374151" />
                    <Label Grid.Column="1" Text="{Binding TotalAmount, StringFormat='{0:N0} đ'}"
                           FontAttributes="Bold" TextColor="#4B5563" />
                </Grid>
            </DataTemplate>
        </CollectionView.GroupHeaderTemplate>

        <!-- Item chi tiết trong nhóm -->
        <CollectionView.ItemTemplate>
            <DataTemplate x:DataType="models:TransactionItemDisplay">
                <SwipeView>
                    <SwipeView.RightItems>
                        <SwipeItems Mode="Reveal">
                            <SwipeItem Text="Xóa" BackgroundColor="#EF4444"
                                       Command="{Binding Source={RelativeSource AncestorType={x:Type vm:TransactionsViewModel}}, Path=DeleteTransactionCommand}"
                                       CommandParameter="{Binding .}" />
                        </SwipeItems>
                    </SwipeView.RightItems>

                    <Grid ColumnDefinitions="48, *, Auto" Padding="16,12" ColumnSpacing="12">
                        <!-- Icon Danh mục -->
                        <Border Grid.Column="0" WidthRequest="44" HeightRequest="44"
                                StrokeShape="RoundRectangle 22" BackgroundColor="#F3F4F6" StrokeThickness="0">
                            <Label Text="{Binding CategoryIcon}" FontSize="20" HorizontalOptions="Center" VerticalOptions="Center" />
                        </Border>

                        <!-- Tên & Danh mục -->
                        <VerticalStackLayout Grid.Column="1" VerticalOptions="Center" Spacing="2">
                            <Label Text="{Binding Note}" FontSize="15" FontAttributes="Bold" TextColor="#111827" />
                            <Label Text="{Binding CategoryName}" FontSize="12" TextColor="#6B7280" />
                        </VerticalStackLayout>

                        <!-- Số tiền -->
                        <Label Grid.Column="2" Text="{Binding Amount, StringFormat='{0:N0} đ'}"
                               FontAttributes="Bold" FontSize="15" VerticalOptions="Center" TextColor="#EF4444" />
                    </Grid>
                </SwipeView>
            </DataTemplate>
        </CollectionView.ItemTemplate>

        <!-- Trạng thái rỗng -->
        <CollectionView.EmptyView>
            <VerticalStackLayout VerticalOptions="Center" HorizontalOptions="Center" Spacing="12" Padding="40">
                <Label Text="🍃" FontSize="60" HorizontalOptions="Center" />
                <Label Text="Chưa có giao dịch nào" FontAttributes="Bold" FontSize="18" HorizontalOptions="Center" TextColor="#6B7280" />
                <Label Text="Nhấn nút '+' để thêm khoản chi tiêu đầu tiên" TextColor="#9CA3AF" HorizontalOptions="Center" HorizontalTextAlignment="Center" />
            </VerticalStackLayout>
        </CollectionView.EmptyView>
    </CollectionView>
</RefreshView>
```

## 4. Bẫy thường gặp (Pitfalls)
- **Lỗi:** Danh sách bị trắng trơn dù database có dữ liệu.
  - **Nguyên nhân:** Quên khai báo `IsGrouped="True"` trên thẻ `CollectionView`.
- **Lỗi:** Nút xóa trong `SwipeItem` không kích hoạt lệnh `DeleteTransactionCommand`.
  - **Nguyên nhân:** Do `SwipeItem` có context là `TransactionItemDisplay`, không phải `TransactionsViewModel`. Cần dùng cú pháp `{RelativeSource AncestorType={x:Type vm:TransactionsViewModel}}` để trỏ về ViewModel cha.

## 5. Checklist nghiệm thu Buổi 05
- [ ] Giao dịch được phân tách thành từng ngày rõ ràng ("Hôm nay", "Hôm qua", hoặc định dạng thứ ngày).
- [ ] Header hiển thị chuẩn tổng thu/chi của cả ngày đó.
- [ ] Vuốt sang trái trên một dòng -> Nút "Xóa" màu đỏ lộ ra. Bấm xóa -> Xuất hiện popup xác nhận -> Xóa thành công.
- [ ] Kéo từ đỉnh màn hình xuống -> Vòng xoay Refresh xuất hiện và tự tắt khi cập nhật xong.
- [ ] Khi xóa sạch dữ liệu -> Giao diện EmptyView hiển thị biểu tượng chiếc lá.

---

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

# BUỔI 07: Đồ Họa 2D Thuần: Donut Chart Tự Vẽ Với Microsoft.Maui.Graphics

**Thuộc chặng:** Chặng 4 (Phần 1) | **Thời lượng:** ~3 giờ

## 1. Mục tiêu buổi học (Definition of Done)
- **Tuyệt đối không sử dụng bất kỳ thư viện chart bên thứ ba nào** (như LiveCharts, Microcharts...).
- Nắm vững kiến thức cốt lõi về đồ họa Canvas: `GraphicsView`, `IDrawable`, `ICanvas`, tọa độ và cung tròn (Arc/Angles).
- Xây dựng một custom control `DonutChartView` kế thừa `IDrawable` để vẽ biểu đồ tròn khuyết (Donut Chart) biểu diễn tỉ trọng chi tiêu theo từng danh mục.
- Hiển thị tổng chi tiêu ở chính giữa tâm hình tròn.
- Tích hợp biểu đồ lên màn hình **Tổng quan** (`OverviewPage`), tự động cập nhật lại khi có giao dịch mới.

## 2. Bản chất kiến trúc & Nguyên lý
- **Hệ tọa độ và Góc trong Canvas:** `ICanvas.DrawArc` tính góc theo độ (degrees) từ 0° đến 360°, chiều kim đồng hồ bắt đầu từ vị trí 3 giờ (trục hoành dương). Để tính cung cho từng danh mục:
  $$\text{Góc của danh mục} = \left(\frac{\text{Chi tiêu danh mục}}{\text{Tổng chi tiêu}}\right) \times 360^{\circ}$$
- **Vòng đời vẽ đồ họa:** Giao diện Canvas không tự vẽ lại nếu dữ liệu ngầm thay đổi. Khi danh sách dữ liệu trong ViewModel được cập nhật, ta phải gọi phương thức `graphicsView.Invalidate()` để yêu cầu hệ điều hành vẽ lại khung hình tiếp theo.

## 3. Các bước thực hành chi tiết

### Bước 3.1: Tạo Data Model cho biểu đồ
Tạo file `src/Client/SoChi.Client/Models/ChartItem.cs`:
```csharp
namespace SoChi.Client.Models;

public class ChartSegment
{
    public string Label { get; set; } = string.Empty;
    public double Value { get; set; }
    public Color SegmentColor { get; set; } = Colors.Blue;
}
```

### Bước 3.2: Viết lớp Drawable tính toán và vẽ Donut Chart
Tạo file `src/Client/SoChi.Client/Controls/DonutChartDrawable.cs`:
```csharp
using SoChi.Client.Models;

namespace SoChi.Client.Controls;

public class DonutChartDrawable : IDrawable
{
    public List<ChartSegment> Segments { get; set; } = new();
    public string CenterText { get; set; } = "0 ₫";
    public Color CenterTextColor { get; set; } = Colors.Black;

    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        canvas.Antialias = true;

        float width = dirtyRect.Width;
        float height = dirtyRect.Height;
        float size = Math.Min(width, height);
        float strokeWidth = size * 0.18f; // Độ dày vành khuyên

        float x = (width - size) / 2 + strokeWidth / 2;
        float y = (height - size) / 2 + strokeWidth / 2;
        float arcSize = size - strokeWidth;

        double total = Segments.Sum(s => s.Value);

        if (total <= 0)
        {
            // Vẽ vành xám rỗng khi chưa có dữ liệu
            canvas.StrokeColor = Color.FromArgb("#E5E7EB");
            canvas.StrokeSize = strokeWidth;
            canvas.DrawEllipse(x, y, arcSize, arcSize);

            DrawCenterText(canvas, dirtyRect, "0 ₫");
            return;
        }

        float startAngle = -90f; // Bắt đầu từ vị trí 12 giờ đỉnh trên cùng
        canvas.StrokeSize = strokeWidth;

        foreach (var segment in Segments)
        {
            float sweepAngle = (float)((segment.Value / total) * 360f);
            if (sweepAngle <= 0) continue;

            canvas.StrokeColor = segment.SegmentColor;
            // DrawArc nhận (x, y, width, height, startAngle, endAngle, clockwise, closed)
            canvas.DrawArc(x, y, arcSize, arcSize, startAngle, startAngle + sweepAngle, true, false);

            startAngle += sweepAngle;
        }

        DrawCenterText(canvas, dirtyRect, CenterText);
    }

    private void DrawCenterText(ICanvas canvas, RectF dirtyRect, string text)
    {
        canvas.FontColor = CenterTextColor;
        canvas.FontSize = 18f;
        canvas.DrawString("Tổng chi", dirtyRect.Center.X, dirtyRect.Center.Y - 12, HorizontalAlignment.Center);

        canvas.FontColor = CenterTextColor;
        canvas.FontSize = 22f;
        canvas.DrawString(text, dirtyRect.Center.X, dirtyRect.Center.Y + 16, HorizontalAlignment.Center);
    }
}
```

### Bước 3.3: Tạo Custom Control bao bọc `GraphicsView`
Tạo file `src/Client/SoChi.Client/Controls/DonutChartView.cs`:
```csharp
using SoChi.Client.Models;

namespace SoChi.Client.Controls;

public class DonutChartView : GraphicsView
{
    private readonly DonutChartDrawable _drawable = new();

    public static readonly BindableProperty SegmentsProperty =
        BindableProperty.Create(nameof(Segments), typeof(List<ChartSegment>), typeof(DonutChartView), null,
            propertyChanged: (bindable, _, newValue) =>
            {
                if (bindable is DonutChartView chart && newValue is List<ChartSegment> list)
                {
                    chart._drawable.Segments = list;
                    chart.Invalidate();
                }
            });

    public static readonly BindableProperty CenterTextProperty =
        BindableProperty.Create(nameof(CenterText), typeof(string), typeof(DonutChartView), "0 ₫",
            propertyChanged: (bindable, _, newValue) =>
            {
                if (bindable is DonutChartView chart && newValue is string text)
                {
                    chart._drawable.CenterText = text;
                    chart.Invalidate();
                }
            });

    public List<ChartSegment> Segments
    {
        get => (List<ChartSegment>)GetValue(SegmentsProperty);
        set => SetValue(SegmentsProperty, value);
    }

    public string CenterText
    {
        get => (string)GetValue(CenterTextProperty);
        set => SetValue(CenterTextProperty, value);
    }

    public DonutChartView()
    {
        Drawable = _drawable;
        HeightRequest = 240;
        WidthRequest = 240;
    }
}
```

### Bước 3.4: Đưa Donut Chart lên màn hình Tổng Quan (`OverviewPage`)
Trong `OverviewViewModel.cs`:
```csharp
[ObservableProperty] public partial List<ChartSegment> ExpenseSegments { get; set; } = new();
[ObservableProperty] public partial string TotalExpenseText { get; set; } = "0 ₫";

[RelayCommand]
public async Task LoadOverviewDataAsync()
{
    var transactions = await _repository.GetTransactionsAsync();
    var categories = (await _repository.GetCategoriesAsync()).ToDictionary(c => c.Id);

    var currentMonthExpenses = transactions
        .Where(t => t.OccurredOn.Month == DateTime.Today.Month && t.OccurredOn.Year == DateTime.Today.Year)
        .Where(t => categories.ContainsKey(t.CategoryId) && categories[t.CategoryId].Kind == TransactionKind.Expense)
        .ToList();

    long total = currentMonthExpenses.Sum(t => t.Amount);
    TotalExpenseText = $"{total:N0} ₫";

    var segments = currentMonthExpenses
        .GroupBy(t => t.CategoryId)
        .Select(g => new ChartSegment
        {
            Label = categories[g.Key].Name,
            Value = g.Sum(x => x.Amount),
            SegmentColor = Color.FromArgb(categories[g.Key].ColorHex)
        })
        .ToList();

    ExpenseSegments = segments;
}
```

Trong `OverviewPage.xaml`:
```xml
<controls:DonutChartView Segments="{Binding ExpenseSegments}"
                         CenterText="{Binding TotalExpenseText}"
                         HorizontalOptions="Center"
                         Margin="0,16" />
```

## 4. Bẫy thường gặp (Pitfalls)
- **Lỗi:** Thay đổi dữ liệu trong ViewModel nhưng biểu đồ không vẽ lại.
  - **Nguyên nhân:** Khi tạo một `List<ChartSegment>` mới, nếu quên gọi `Invalidate()` trong callback `propertyChanged` của `BindableProperty`, hệ thống sẽ không phát lệnh render lại giao diện đồ họa.
- **Lỗi:** Vòng tròn bị méo thành hình bầu dục elip khi xoay ngang màn hình.
  - **Khắc phục:** Luôn lấy `float size = Math.Min(dirtyRect.Width, dirtyRect.Height)` làm đường kính và căn tâm bằng `dirtyRect.Center`.

## 5. Checklist nghiệm thu Buổi 07
- [ ] Không có package thư viện đồ họa thứ ba nào được cài đặt trong `.csproj`.
- [ ] Biểu đồ Donut vẽ tròn đều, vành khuyên sắc nét.
- [ ] Các cung màu hiển thị đúng theo màu sắc (`ColorHex`) đã gán của từng danh mục.
- [ ] Ở chính giữa tâm hiển thị chuẩn xác chữ "Tổng chi" và số tiền định dạng VNĐ.
- [ ] Thêm một giao dịch chi tiêu mới -> Quay lại tab Tổng quan -> Biểu đồ tự phân chia lại tỉ lệ các cung màu.

---

# BUỔI 08: Biểu Đồ Cột 6 Tháng (Bar Chart), Responsive & Xoay Màn Hình

**Thuộc chặng:** Chặng 4 (Phần 2) | **Thời lượng:** ~3 giờ

## 1. Mục tiêu buổi học (Definition of Done)
- Tự vẽ **Biểu đồ cột (Bar Chart)** hiển thị biến động thu/chi trong **6 tháng gần nhất** trên màn hình Thống kê (`StatisticsPage`).
- Mỗi tháng gồm 2 cột đặt cạnh nhau: Cột Chi tiêu (màu đỏ) và Cột Thu nhập (màu xanh lá).
- Tự động chia tỉ lệ chiều cao cột dựa trên giá trị lớn nhất ($Max$).
- Có trục hoành ghi tên các tháng (T1, T2, ... hoặc MM/yy) và đường gióng ngang mờ.
- Biểu đồ tự động tính toán lại kích thước hiển thị (Responsive) mượt mà khi xoay ngang/dọc màn hình thiết bị mà không vỡ chữ.

## 2. Bản chất kiến trúc & Nguyên lý
- **Thuật toán chuẩn hóa chiều cao cột:**
  $$\text{Chiều cao cột} = \left(\frac{\text{Giá trị của cột}}{\text{Giá trị lớn nhất trong 6 tháng}}\right) \times (\text{Chiều cao khả dụng của biểu đồ})$$
- **Hỗ trợ Theme Sáng/Tối trong Canvas:** `ICanvas` không tự nhận diện `DynamicResource` như XAML thông thường. Ta phải chủ động lấy màu sắc phù hợp thông qua `Application.Current?.RequestedTheme` để quyết định màu chữ nhãn tháng (đen khi Light mode, trắng/xám khi Dark mode).

## 3. Các bước thực hành chi tiết

### Bước 3.1: Tạo Model cho cột 6 tháng
Tạo file `src/Client/SoChi.Client/Models/MonthlyBarData.cs`:
```csharp
namespace SoChi.Client.Models;

public class MonthlyBarData
{
    public string MonthLabel { get; set; } = string.Empty; // Ví dụ: "T03"
    public long Expense { get; set; }
    public long Income { get; set; }
}
```

### Bước 3.2: Viết lớp Drawable cho Bar Chart
Tạo file `src/Client/SoChi.Client/Controls/BarChartDrawable.cs`:
```csharp
using SoChi.Client.Models;

namespace SoChi.Client.Controls;

public class BarChartDrawable : IDrawable
{
    public List<MonthlyBarData> Data { get; set; } = new();

    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        canvas.Antialias = true;

        if (Data == null || Data.Count == 0) return;

        float bottomPadding = 30f;
        float topPadding = 20f;
        float leftPadding = 20f;
        float rightPadding = 20f;

        float chartHeight = dirtyRect.Height - bottomPadding - topPadding;
        float chartWidth = dirtyRect.Width - leftPadding - rightPadding;

        // Tìm giá trị lớn nhất để làm trần tỉ lệ
        long maxValue = Data.Max(d => Math.Max(d.Expense, d.Income));
        if (maxValue == 0) maxValue = 1000000; // Mặc định tránh chia cho 0

        float slotWidth = chartWidth / Data.Count;
        float barWidth = Math.Min(slotWidth * 0.35f, 20f); // Giới hạn bề rộng cột tối đa

        // Xác định màu chữ theo Theme
        var isDark = Application.Current?.RequestedTheme == AppTheme.Dark;
        Color labelColor = isDark ? Color.FromArgb("#9CA3AF") : Color.FromArgb("#4B5563");
        Color gridLineColor = isDark ? Color.FromArgb("#374151") : Color.FromArgb("#E5E7EB");

        // Vẽ đường trục hoành đáy
        canvas.StrokeColor = gridLineColor;
        canvas.StrokeSize = 1;
        float baselineY = dirtyRect.Height - bottomPadding;
        canvas.DrawLine(leftPadding, baselineY, dirtyRect.Width - rightPadding, baselineY);

        for (int i = 0; i < Data.Count; i++)
        {
            var item = Data[i];
            float groupCenterX = leftPadding + (i * slotWidth) + (slotWidth / 2);

            // Tọa độ 2 cột
            float expenseBarX = groupCenterX - barWidth - 2;
            float incomeBarX = groupCenterX + 2;

            float expenseHeight = (float)((double)item.Expense / maxValue * chartHeight);
            float incomeHeight = (float)((double)item.Income / maxValue * chartHeight);

            // Vẽ Cột Chi tiêu (Đỏ)
            canvas.FillColor = Color.FromArgb("#EF4444");
            canvas.FillRoundedRectangle(expenseBarX, baselineY - expenseHeight, barWidth, expenseHeight, 4, 4, 0, 0);

            // Vẽ Cột Thu nhập (Xanh)
            canvas.FillColor = Color.FromArgb("#10B981");
            canvas.FillRoundedRectangle(incomeBarX, baselineY - incomeHeight, barWidth, incomeHeight, 4, 4, 0, 0);

            // Vẽ nhãn tháng phía dưới trục
            canvas.FontColor = labelColor;
            canvas.FontSize = 12f;
            canvas.DrawString(item.MonthLabel, groupCenterX, baselineY + 18, HorizontalAlignment.Center);
        }
    }
}
```

### Bước 3.3: Tạo Custom Control `BarChartView`
Tạo file `src/Client/SoChi.Client/Controls/BarChartView.cs`:
```csharp
using SoChi.Client.Models;

namespace SoChi.Client.Controls;

public class BarChartView : GraphicsView
{
    private readonly BarChartDrawable _drawable = new();

    public static readonly BindableProperty DataProperty =
        BindableProperty.Create(nameof(Data), typeof(List<MonthlyBarData>), typeof(BarChartView), null,
            propertyChanged: (bindable, _, newValue) =>
            {
                if (bindable is BarChartView chart && newValue is List<MonthlyBarData> list)
                {
                    chart._drawable.Data = list;
                    chart.Invalidate();
                }
            });

    public List<MonthlyBarData> Data
    {
        get => (List<MonthlyBarData>)GetValue(DataProperty);
        set => SetValue(DataProperty, value);
    }

    public BarChartView()
    {
        Drawable = _drawable;
        HeightRequest = 220;
    }
}
```

### Bước 3.4: Tích hợp vào `StatisticsViewModel` và XAML
Trong `StatisticsViewModel.cs`:
```csharp
[ObservableProperty] public partial List<MonthlyBarData> Last6MonthsData { get; set; } = new();

[RelayCommand]
public async Task LoadStatisticsAsync()
{
    var transactions = await _repository.GetTransactionsAsync();
    var categories = (await _repository.GetCategoriesAsync()).ToDictionary(c => c.Id);

    var list = new List<MonthlyBarData>();
    var now = DateTime.Today;

    // Lấy 6 tháng gần nhất từ quá khứ đến hiện tại
    for (int i = 5; i >= 0; i--)
    {
        var targetMonth = now.AddMonths(-i);
        var inMonth = transactions.Where(t => t.OccurredOn.Year == targetMonth.Year && t.OccurredOn.Month == targetMonth.Month).ToList();

        long expense = inMonth
            .Where(t => categories.ContainsKey(t.CategoryId) && categories[t.CategoryId].Kind == TransactionKind.Expense)
            .Sum(t => t.Amount);

        long income = inMonth
            .Where(t => categories.ContainsKey(t.CategoryId) && categories[t.CategoryId].Kind == TransactionKind.Income)
            .Sum(t => t.Amount);

        list.Add(new MonthlyBarData
        {
            MonthLabel = $"T{targetMonth:MM}",
            Expense = expense,
            Income = income
        });
    }

    Last6MonthsData = list;
}
```

Trong `StatisticsPage.xaml`:
```xml
<VerticalStackLayout Padding="16" Spacing="20">
    <Label Text="Biến động Thu - Chi 6 tháng" FontAttributes="Bold" FontSize="18" />
    <Border StrokeShape="RoundRectangle 12" StrokeThickness="0" BackgroundColor="#F9FAFB" Padding="12">
        <controls:BarChartView Data="{Binding Last6MonthsData}" HorizontalOptions="FillAndExpand" />
    </Border>
</VerticalStackLayout>
```

## 4. Bẫy thường gặp (Pitfalls)
- **Lỗi:** Cột bị vẽ lấn ra ngoài màn hình phía trên (chiều cao âm).
  - **Nguyên nhân:** Quên kiểm tra `maxValue == 0` hoặc công thức vẽ hình chữ nhật nhận tọa độ `y` đỉnh trên: `baselineY - expenseHeight`.
- **Lỗi:** Xoay ngang màn hình chữ bị đè dính vào nhau.
  - **Khắc phục:** Luôn tính toán `slotWidth = chartWidth / Data.Count` động theo độ rộng `dirtyRect.Width`.

## 5. Checklist nghiệm thu Buổi 08
- [ ] Biểu đồ thể hiện đủ 6 tháng gần nhất với các nhãn rõ ràng (ví dụ T10, T11, T12, T01, T02, T03).
- [ ] Mỗi tháng có 2 cột: Cột Chi (Đỏ) và Cột Thu (Xanh) đặt cạnh nhau cân đối.
- [ ] Chuyển đổi giữa chế độ Sáng và Tối (Light/Dark mode) -> Màu nhãn tháng tự đổi tương phản rõ nét.
- [ ] Thử nghiệm xoay ngang màn hình (hoặc kéo giãn cửa sổ Windows) -> Các cột tự động giãn đều, không vỡ layout.

---

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

# BUỔI 11: Hạn Mức Chi Tiêu (ProgressBar), Lọc, Debounce & xUnit Tests

**Thuộc chặng:** Chặng 6 | **Thời lượng:** ~3 giờ

## 1. Mục tiêu buổi học (Definition of Done)
- Tính năng **Hạn mức chi tiêu tháng (`MonthlyLimit`)** cho từng danh mục: Thanh `ProgressBar` tự động đổi màu sắc cảnh báo (Xanh: <80%, Cam: 80-100%, Đỏ: vượt hạn mức >100%).
- Bộ lọc đa điều kiện: Lọc theo khoảng ngày (Từ ngày -> Đến ngày), theo danh mục, hoặc theo loại thu/chi.
- Ô tìm kiếm giao dịch theo từ khóa ghi chú, áp dụng cơ chế **Debounce 300ms** (sử dụng `CancellationToken`) để không truy vấn database liên tục trên từng ký tự gõ.
- Tạo project `tests/SoChi.Client.Tests`, viết tối thiểu **8 Unit Test có ý nghĩa** bằng **xUnit** cho phần logic tính toán hạn mức và ViewModel mà không cần khởi động UI.

## 2. Bản chất kiến trúc & Nguyên lý
- **Debouncing:** Khi người dùng gõ "Ăn trưa", sự kiện TextChanged phát ra 8 lần. Nếu truy vấn DB cả 8 lần sẽ gây khựng UI. Debounce chờ người dùng dừng gõ 300ms rồi mới thực thi tìm kiếm 1 lần duy nhất bằng cách hủy (`Cancel()`) token trước đó.
- **Tách biệt ViewModel để Unit Test:** ViewModel không được chứa bất kỳ lệnh gọi UI trực tiếp nào như `new Window()` hay gọi `Shell.Current.DisplayAlert` không qua abstraction. Khởi tạo ViewModel trong xUnit bằng cách mock hoặc inject Fake Repository.

## 3. Các bước thực hành chi tiết

### Bước 3.1: Tính toán hạn mức và cảnh báo màu sắc
Tạo helper `src/Client/SoChi.Client/Helpers/BudgetCalculator.cs`:
```csharp
namespace SoChi.Client.Helpers;

public enum BudgetStatus
{
    Safe,    // Dưới 80% (Xanh)
    Warning, // 80% - 100% (Cam)
    Exceeded // Trên 100% (Đỏ)
}

public static class BudgetCalculator
{
    public static double CalculateProgress(long spent, long limit)
    {
        if (limit <= 0) return 0;
        return Math.Min((double)spent / limit, 1.0);
    }

    public static BudgetStatus GetStatus(long spent, long limit)
    {
        if (limit <= 0) return BudgetStatus.Safe;
        double ratio = (double)spent / limit;
        if (ratio >= 1.0) return BudgetStatus.Exceeded;
        if (ratio >= 0.8) return BudgetStatus.Warning;
        return BudgetStatus.Safe;
    }
}
```

### Bước 3.2: Tìm kiếm với Debounce sử dụng `CancellationToken`
Trong `TransactionsViewModel.cs`:
```csharp
private CancellationTokenSource? _searchCts;

[ObservableProperty] public partial string SearchText { get; set; } = string.Empty;

async partial void OnSearchTextChanged(string value)
{
    _searchCts?.Cancel();
    _searchCts = new CancellationTokenSource();
    var token = _searchCts.Token;

    try
    {
        await Task.Delay(300, token); // Chờ 300ms dừng gõ
        await PerformSearchAsync(value, token);
    }
    catch (TaskCanceledException)
    {
        // Người dùng vẫn đang tiếp tục gõ, bỏ qua lần tìm kiếm này
    }
}

private async Task PerformSearchAsync(string keyword, CancellationToken token)
{
    var all = await _repository.GetTransactionsAsync();
    if (token.IsCancellationRequested) return;

    var filtered = string.IsNullOrWhiteSpace(keyword)
        ? all
        : all.Where(t => t.Note.Contains(keyword, StringComparison.OrdinalIgnoreCase)).ToList();

    // Cập nhật lại danh sách hiển thị
}
```

### Bước 3.3: Khởi tạo Project Unit Test
Tại thư mục gốc:
```powershell
dotnet new xunit -n SoChi.Client.Tests -o tests/SoChi.Client.Tests
dotnet sln add tests/SoChi.Client.Tests/SoChi.Client.Tests.csproj
dotnet add tests/SoChi.Client.Tests/SoChi.Client.Tests.csproj reference src/Client/SoChi.Client/SoChi.Client.csproj
```

### Bước 3.4: Viết các Unit Test kiểm thử logic nghiệp vụ
Tạo file `tests/SoChi.Client.Tests/BudgetCalculatorTests.cs`:
```csharp
using SoChi.Client.Helpers;
using Xunit;

namespace SoChi.Client.Tests;

public class BudgetCalculatorTests
{
    [Theory]
    [InlineData(500000, 1000000, 0.5)]
    [InlineData(1000000, 1000000, 1.0)]
    [InlineData(1500000, 1000000, 1.0)] // Không vượt quá 1.0 trên ProgressBar
    [InlineData(0, 1000000, 0.0)]
    public void CalculateProgress_ReturnsCorrectRatio(long spent, long limit, double expected)
    {
        var result = BudgetCalculator.CalculateProgress(spent, limit);
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(799000, 1000000, BudgetStatus.Safe)]
    [InlineData(800000, 1000000, BudgetStatus.Warning)]
    [InlineData(999999, 1000000, BudgetStatus.Warning)]
    [InlineData(1000000, 1000000, BudgetStatus.Exceeded)]
    [InlineData(1200000, 1000000, BudgetStatus.Exceeded)]
    public void GetStatus_ReturnsExpectedStatus(long spent, long limit, BudgetStatus expected)
    {
        var status = BudgetCalculator.GetStatus(spent, limit);
        Assert.Equal(expected, status);
    }
}
```

Tạo file `tests/SoChi.Client.Tests/TransactionGroupingTests.cs`:
```csharp
using SoChi.Client.Models;
using Xunit;

namespace SoChi.Client.Tests;

public class TransactionGroupingTests
{
    [Fact]
    public void TotalAmount_CalculatesExpensesAndIncomeCorrectly()
    {
        var date = new DateTime(2026, 9, 7);
        var items = new List<TransactionItemDisplay>
        {
            new() { Amount = 50000, Kind = TransactionKind.Expense },
            new() { Amount = 30000, Kind = TransactionKind.Expense },
            new() { Amount = 200000, Kind = TransactionKind.Income }
        };

        var group = new TransactionGroup(date, items);

        // -50k - 30k + 200k = +120k
        Assert.Equal(120000, group.TotalAmount);
    }
}
```

## 4. Bẫy thường gặp (Pitfalls)
- **Lỗi:** Chạy `dotnet test` bị báo lỗi không thể nạp các assembly của MAUI (do `Microsoft.Maui.Controls` không tương thích mặc định với target test console).
  - **Khắc phục:** Không test các thành phần gắn chặt với UI platform. Đảm bảo logic tính toán thuần C# hoặc cấu hình project test tham chiếu đúng framework.

## 5. Checklist nghiệm thu Buổi 11
- [ ] Chạy lệnh `dotnet test` ở thư mục gốc -> Toàn bộ tối thiểu 8 test đều xanh (Passed 100%).
- [ ] Gõ từ khóa tìm kiếm -> Sau khi dừng tay 300ms danh sách mới lọc lại, không bị giật lag khi gõ nhanh.
- [ ] Đặt hạn mức chi ăn uống 1.000.000đ. Khi chi tiêu đạt 850.000đ -> Thanh tiến trình hiển thị màu cam cảnh báo; khi vượt 1.000.000đ -> Chuyển sang màu đỏ rực.

---

# BUỔI 12: Backend ASP.NET Core Web API, PostgreSQL, Shared DTOs & JWT Auth

**Thuộc chặng:** Chặng 7 (Phần 1) | **Thời lượng:** ~3 giờ

## 1. Mục tiêu buổi học (Definition of Done)
- Hoàn thiện mã nguồn backend ASP.NET Core Web API trong project `src/Api/SoChi.Api`.
- Di chuyển toàn bộ Contracts, DTOs (Data Transfer Objects) và Hằng số Routes sang project `src/Shared/SoChi.Shared`.
- Thiết lập **PostgreSQL** cho Server qua EF Core + Npgsql (bản cài native trên Windows).
- Xây dựng chức năng **Xác thực JWT (JSON Web Token)**: Đăng ký tài khoản (`/api/auth/register`), Đăng nhập (`/api/auth/login`).
- Xây dựng các Endpoint CRUD dữ liệu đồng bộ: `/api/sync/pull` và `/api/sync/push` có gắn xác thực `[Authorize]`.

## 2. Bản chất kiến trúc & Nguyên lý
- **Lợi ích tuyệt đối của `Shared`:** Cả Client và API cùng dùng chung một định nghĩa `SyncPushRequest` và `SyncPullResponse`. Nếu Server sửa kiểu dữ liệu một trường, Client sẽ lập tức báo lỗi compile, triệt tiêu 100% rủi ro bất đồng bộ schema.
- **Thứ tự Middleware trong ASP.NET Core:**
  ```csharp
  app.UseAuthentication(); // 1. Bạn là ai? (Giải mã JWT Token)
  app.UseAuthorization();  // 2. Bạn có quyền truy cập không?
  ```
  Nếu đảo ngược thứ tự 2 dòng trên, toàn bộ API có `[Authorize]` sẽ trả về lỗi `401 Unauthorized`.

## 3. Các bước thực hành chi tiết

### Bước 3.1: Định nghĩa DTOs trong `src/Shared/SoChi.Shared`
Tạo file `src/Shared/SoChi.Shared/Dtos/AuthDtos.cs`:
```csharp
namespace SoChi.Shared.Dtos;

public record RegisterRequest(string Email, string Password, string FullName);
public record LoginRequest(string Email, string Password);
public record AuthResponse(string Token, string Email, string FullName, Guid UserId);
```

Tạo file `src/Shared/SoChi.Shared/Dtos/SyncDtos.cs`:
```csharp
namespace SoChi.Shared.Dtos;

public record CategorySyncDto(Guid Id, string Name, string IconGlyph, string ColorHex, int Kind, long? MonthlyLimit, DateTime UpdatedAt, bool IsDeleted);

public record TransactionSyncDto(Guid Id, Guid CategoryId, long Amount, DateTime OccurredOn, string Note, string? ReceiptPath, DateTime UpdatedAt, bool IsDeleted);

public record SyncPushRequest(List<CategorySyncDto> Categories, List<TransactionSyncDto> Transactions);

public record SyncPullResponse(List<CategorySyncDto> Categories, List<TransactionSyncDto> Transactions, DateTime ServerTimeUtc);
```

Tạo file `src/Shared/SoChi.Shared/ApiRoutes.cs`:
```csharp
namespace SoChi.Shared;

public static class ApiRoutes
{
    public const string Register = "api/auth/register";
    public const string Login = "api/auth/login";
    public const string SyncPush = "api/sync/push";
    public const string SyncPull = "api/sync/pull";
}
```

### Bước 3.2: Cài PostgreSQL, cấu hình EF Core cho `src/Api/SoChi.Api`

> **Ranh giới cần nắm rõ trước khi làm:** PostgreSQL chỉ thay database của **`src/Api/SoChi.Api`**. Client vẫn dùng SQLite và **không được đổi** — đó không phải lựa chọn mà là ràng buộc kỹ thuật: offline-first cần một database *nhúng trong app*, chạy ngay trên điện thoại khi mất mạng. PostgreSQL là một tiến trình server riêng, không nhúng vào app di động được. Kiến trúc đúng là: **SQLite trên máy người dùng ↔ sync ↔ PostgreSQL trên server**. Toàn bộ Buổi 03–11 giữ nguyên, không sửa một dòng nào.

#### 3.2.1 — Cài PostgreSQL trên Windows

Tải installer tại <https://www.postgresql.org/download/windows/> (bản 17 trở lên). Trong quá trình cài:

| Mục | Chọn |
|---|---|
| Components | Giữ **PostgreSQL Server** và **Command Line Tools**. pgAdmin 4 nên cài để xem dữ liệu bằng giao diện. Stack Builder bỏ được. |
| Password cho user `postgres` | Đặt một mật khẩu bạn nhớ được, ví dụ `dev`. **Ghi lại** — không có cách khôi phục, chỉ có cách reset thủ công. |
| Port | Để mặc định `5432` |
| Locale | Để mặc định |

Sau khi cài, PostgreSQL chạy nền như một **Windows Service** (tự khởi động cùng máy). Kiểm tra:

```powershell
Get-Service postgresql*
& "C:\Program Files\PostgreSQL\17\bin\psql.exe" -U postgres -c "SELECT version();"
```

Thêm thư mục `bin` vào PATH để đỡ phải gõ đường dẫn đầy đủ mỗi lần:

```powershell
$pg = "C:\Program Files\PostgreSQL\17\bin"
[Environment]::SetEnvironmentVariable("Path", "$env:Path;$pg", "User")
```
Mở lại terminal rồi thử `psql --version`.

#### 3.2.2 — Tạo hai database

Một cho ứng dụng, một cho test tự động (Bước 3.10). **Không bao giờ để test chạy trên database đang dùng thật** — test sẽ xóa sạch dữ liệu.

```powershell
psql -U postgres -c "CREATE DATABASE sochi;"
psql -U postgres -c "CREATE DATABASE sochi_test;"
psql -U postgres -l          # liệt kê kiểm tra
```

#### 3.2.3 — Cài package cho `src/Api/SoChi.Api`

```powershell
dotnet add src/Api/SoChi.Api/SoChi.Api.csproj package Npgsql.EntityFrameworkCore.PostgreSQL
dotnet add src/Api/SoChi.Api/SoChi.Api.csproj package Microsoft.AspNetCore.Authentication.JwtBearer
```

`Npgsql.EntityFrameworkCore.PostgreSQL` là provider EF Core cho PostgreSQL. Điểm đáng chú ý về mặt học tập: **đây là toàn bộ thay đổi ở tầng dữ liệu**. Entity, LINQ, `AppDbContext`, mọi endpoint ở Bước 3.7 và 3.9 không đổi một ký tự nào — đó chính là giá trị của việc viết code đúng tầng trừu tượng.

Kiểu dữ liệu cũng được ánh xạ đúng bản chất hơn hẳn SQLite:

| C# | PostgreSQL | SQLite (để so sánh) |
|---|---|---|
| `Guid` | `uuid` (16 byte, có kiểu thật) | TEXT — lưu chuỗi 36 ký tự |
| `long` | `bigint` | INTEGER |
| `DateTime` (UTC) | `timestamptz` — **bắt lỗi nếu sai `Kind`** | TEXT — mất `DateTimeKind`, âm thầm sai |
| `bool` | `boolean` | INTEGER 0/1 |

Tạo `AppDbContext.cs` trong `src/Api/SoChi.Api/Data/`:
```csharp
using Microsoft.EntityFrameworkCore;
using SoChi.Api.Entities;

namespace SoChi.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<UserEntity> Users => Set<UserEntity>();
    public DbSet<CategoryEntity> Categories => Set<CategoryEntity>();
    public DbSet<TransactionEntity> Transactions => Set<TransactionEntity>();
}
```

### Bước 3.3: Cấu hình `Program.cs` của API
```csharp
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SoChi.Api.Data;

var builder = WebApplication.CreateBuilder(args);

// Database
builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")
        ?? throw new InvalidOperationException(
            "Thiếu ConnectionStrings:DefaultConnection. Xem Bước 3.6.")));

// JWT Authentication
var jwtKey = builder.Configuration["Jwt:Key"] ?? "KhoaBiMatRatDaiVaBaoMatChoDuAnSoChi123456789!";
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseAuthentication();
app.UseAuthorization();

// Đăng ký Minimal API Endpoints
app.MapPost(SoChi.Shared.ApiRoutes.Login, async (SoChi.Shared.Dtos.LoginRequest req, AppDbContext db) => {
    // Kiểm tra đăng nhập và cấp JWT Token
    // Trả về AuthResponse
});

app.Run();
```

### Bước 3.4: Định nghĩa Entities phía Server

`AppDbContext` ở Bước 3.2 tham chiếu ba entity chưa tồn tại. Tạo `src/Api/SoChi.Api/Entities/Entities.cs`:

```csharp
namespace SoChi.Api.Entities;

public class UserEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class CategoryEntity
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }          // ← Cột quan trọng nhất của toàn bộ backend
    public string Name { get; set; } = string.Empty;
    public string IconGlyph { get; set; } = string.Empty;
    public string ColorHex { get; set; } = "#3B82F6";
    public int Kind { get; set; }
    public long? MonthlyLimit { get; set; }
    public DateTime UpdatedAt { get; set; }
    public bool IsDeleted { get; set; }
}

public class TransactionEntity
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid CategoryId { get; set; }
    public long Amount { get; set; }
    public DateTime OccurredOn { get; set; }
    public string Note { get; set; } = string.Empty;
    public string? ReceiptPath { get; set; }
    public DateTime UpdatedAt { get; set; }
    public bool IsDeleted { get; set; }
}
```

Ba khác biệt so với model SQLite ở Client, phải hiểu rõ:

| | Client (SQLite) | Server (PostgreSQL) |
|---|---|---|
| `UserId` | Không có — máy chỉ một người dùng | **Bắt buộc** — dữ liệu nhiều người nằm chung bảng |
| `SyncState` | Có — đánh dấu "chưa đẩy lên" | Không có — server là nguồn chân lý, không sync đi đâu nữa |
| `Id` | Do client sinh (`Guid.NewGuid()`) | **Nhận từ client**, không tự sinh — nếu server sinh lại id thì mỗi lần sync sẽ tạo bản ghi trùng |

Ràng buộc và index — thêm vào `AppDbContext`:

```csharp
protected override void OnModelCreating(ModelBuilder b)
{
    b.Entity<UserEntity>().HasIndex(u => u.Email).IsUnique();
    b.Entity<CategoryEntity>().HasIndex(c => new { c.UserId, c.UpdatedAt });
    b.Entity<TransactionEntity>().HasIndex(t => new { t.UserId, t.UpdatedAt });
}
```

Index `(UserId, UpdatedAt)` chính là index mà endpoint `/api/sync/pull` sẽ dùng — nó lọc đúng theo hai cột đó.

### Bước 3.5: Băm mật khẩu — tuyệt đối không lưu mật khẩu thô

Lưu mật khẩu dạng chữ thường trong database là lỗi bảo mật nghiêm trọng nhất mà người mới hay mắc: database rò rỉ một lần là mất toàn bộ mật khẩu của người dùng — và vì nhiều người dùng lại mật khẩu đó cho email và ngân hàng, thiệt hại lan ra ngoài phạm vi app của bạn.

Tạo `src/Api/SoChi.Api/Security/PasswordHasher.cs` (dùng PBKDF2 có sẵn trong BCL, **không cần cài thư viện ngoài**):

```csharp
using System.Security.Cryptography;

namespace SoChi.Api.Security;

public static class PasswordHasher
{
    private const int SaltSize = 16;
    private const int KeySize = 32;
    private const int Iterations = 100_000;
    private static readonly HashAlgorithmName Algorithm = HashAlgorithmName.SHA256;

    public static string Hash(string password)
    {
        var salt = RandomNumberGenerator.GetBytes(SaltSize);
        var key = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, Algorithm, KeySize);
        return $"{Iterations}.{Convert.ToBase64String(salt)}.{Convert.ToBase64String(key)}";
    }

    public static bool Verify(string password, string stored)
    {
        var parts = stored.Split('.', 3);
        if (parts.Length != 3 || !int.TryParse(parts[0], out var iterations))
            return false;

        var salt = Convert.FromBase64String(parts[1]);
        var key = Convert.FromBase64String(parts[2]);
        var attempt = Rfc2898DeriveBytes.Pbkdf2(password, salt, iterations, Algorithm, key.Length);

        // So sánh thời gian cố định, chống tấn công đo thời gian (timing attack)
        return CryptographicOperations.FixedTimeEquals(attempt, key);
    }
}
```

Ba ý tưởng cần nắm, vì chúng đúng cho mọi ngôn ngữ chứ không riêng .NET:

1. **Salt ngẫu nhiên cho từng người dùng.** Hai người đặt cùng mật khẩu `123456` vẫn cho ra hai chuỗi hash khác nhau → bảng tra sẵn (rainbow table) vô dụng.
2. **Lặp 100.000 vòng.** Cố tình làm hàm băm *chậm*. Người dùng đăng nhập chỉ tốn vài chục mili-giây, nhưng kẻ dò 10 tỷ mật khẩu thì tốn gấp 100.000 lần.
3. **Lưu cả số vòng lặp vào chuỗi kết quả.** Vài năm nữa máy tính mạnh lên, bạn tăng lên 200.000 vòng mà **các tài khoản cũ vẫn đăng nhập được**, vì `Verify` đọc số vòng từ chính chuỗi đã lưu.

### Bước 3.6: Đưa `Jwt:Key` vào User Secrets

Ở Bước 3.3, `Program.cs` có dòng này:

```csharp
var jwtKey = builder.Configuration["Jwt:Key"] ?? "KhoaBiMatRatDaiVaBaoMatChoDuAnSoChi123456789!";
```

Giá trị dự phòng đó nằm trong mã nguồn, và mã nguồn thì nằm trong Git. Ai đọc được repo là **tự ký được token hợp lệ** cho bất kỳ tài khoản nào — coi như không có xác thực. Sửa thành:

```csharp
var jwtKey = builder.Configuration["Jwt:Key"]
    ?? throw new InvalidOperationException(
        "Thiếu cấu hình Jwt:Key. Chạy: dotnet user-secrets set \"Jwt:Key\" \"<khóa>\" --project src/Api/SoChi.Api");
```

App **fail nhanh lúc khởi động** thay vì chạy với khóa yếu — nguyên tắc "fail fast".

Nạp khóa thật bằng User Secrets (file nằm ngoài thư mục project, không bao giờ lọt vào Git):

```powershell
dotnet user-secrets init --project src/Api/SoChi.Api
dotnet user-secrets set "Jwt:Key" "$([Convert]::ToBase64String((1..48 | ForEach-Object { Get-Random -Maximum 256 })))" --project src/Api/SoChi.Api
dotnet user-secrets list --project src/Api/SoChi.Api
```

**Chuỗi kết nối PostgreSQL cũng phải nằm ở đây**, vì nó chứa mật khẩu database:

```powershell
dotnet user-secrets set "ConnectionStrings:DefaultConnection" `
  "Host=localhost;Port=5432;Database=sochi;Username=postgres;Password=dev" `
  --project src/Api/SoChi.Api
```

Đây là khác biệt thực tế so với SQLite: chuỗi `Data Source=server_sochi.db` vô hại nên đặt thẳng trong `appsettings.json` cũng được. Chuỗi kết nối PostgreSQL thì **luôn có thông tin đăng nhập** — commit nó vào Git là để lộ quyền truy cập database. Quy tắc chung: bất cứ chuỗi nào chứa `Password=` đều không được nằm trong file theo dõi bởi Git.

Khóa HMAC-SHA256 phải dài tối thiểu 32 byte, nếu không `AddJwtBearer` sẽ ném lỗi lúc chạy.

> Khi deploy thật, User Secrets **không** dùng được (nó chỉ dành cho máy dev). Lúc đó khóa đến từ biến môi trường `Jwt__Key` hoặc dịch vụ quản lý bí mật. Điểm hay là **code không đổi một dòng nào** — `IConfiguration` đọc từ nguồn nào cũng vậy.

### Bước 3.7: Hoàn thiện endpoint Register và Login

Bước 3.3 mới để lại phần thân rỗng. Viết đầy đủ — tạo `src/Api/SoChi.Api/Endpoints/AuthEndpoints.cs`:

```csharp
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SoChi.Api.Data;
using SoChi.Api.Entities;
using SoChi.Api.Security;
using SoChi.Shared;
using SoChi.Shared.Dtos;

namespace SoChi.Api.Endpoints;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this WebApplication app, string jwtKey)
    {
        app.MapPost($"/{ApiRoutes.Register}", async (RegisterRequest req, AppDbContext db) =>
        {
            if (string.IsNullOrWhiteSpace(req.Email) || req.Password.Length < 6)
                return Results.BadRequest(new { message = "Email không hợp lệ hoặc mật khẩu dưới 6 ký tự." });

            var email = req.Email.Trim().ToLowerInvariant();
            if (await db.Users.AnyAsync(u => u.Email == email))
                return Results.Conflict(new { message = "Email đã được đăng ký." });

            var user = new UserEntity
            {
                Email = email,
                FullName = req.FullName,
                PasswordHash = PasswordHasher.Hash(req.Password)
            };

            db.Users.Add(user);
            await db.SaveChangesAsync();

            return Results.Ok(new AuthResponse(CreateToken(user, jwtKey), user.Email, user.FullName, user.Id));
        });

        app.MapPost($"/{ApiRoutes.Login}", async (LoginRequest req, AppDbContext db) =>
        {
            var email = req.Email.Trim().ToLowerInvariant();
            var user = await db.Users.FirstOrDefaultAsync(u => u.Email == email);

            // Thông báo giống hệt nhau cho hai trường hợp "sai email" và "sai mật khẩu":
            // nếu tách riêng, kẻ tấn công dò được email nào đã đăng ký.
            if (user is null || !PasswordHasher.Verify(req.Password, user.PasswordHash))
                return Results.Unauthorized();

            return Results.Ok(new AuthResponse(CreateToken(user, jwtKey), user.Email, user.FullName, user.Id));
        });
    }

    private static string CreateToken(UserEntity user, string jwtKey)
    {
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var creds = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.UtcNow.AddDays(30),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
```

Thêm một hàm mở rộng nhỏ để mọi endpoint lấy `UserId` từ token — tạo `src/Api/SoChi.Api/Security/ClaimsExtensions.cs`:

```csharp
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace SoChi.Api.Security;

public static class ClaimsExtensions
{
    public static Guid GetUserId(this ClaimsPrincipal principal)
    {
        var raw = principal.FindFirstValue(JwtRegisteredClaimNames.Sub)
               ?? principal.FindFirstValue(ClaimTypes.NameIdentifier);

        return Guid.TryParse(raw, out var id)
            ? id
            : throw new UnauthorizedAccessException("Token không chứa UserId hợp lệ.");
    }
}
```

> Vì sao phải thử cả hai claim: ASP.NET Core mặc định **ánh xạ lại** claim `sub` thành `ClaimTypes.NameIdentifier` (một URI dài). Muốn tắt hành vi đó, thêm `JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();` trước khi cấu hình authentication.

Trong `Program.cs`, thay đoạn `MapPost(ApiRoutes.Login, ...)` bỏ trống bằng:

```csharp
app.MapAuthEndpoints(jwtKey);
app.MapSyncEndpoints();      // viết ở Bước 3.9
```

### Bước 3.8: Tạo schema database phía server

`AppDbContext` mới chỉ mô tả cấu trúc, và database `sochi` bạn vừa tạo ở Bước 3.2.2 đang **rỗng hoàn toàn**. Chạy API lúc này sẽ lỗi `42P01: relation "Users" does not exist`.

**Cách nhanh, chỉ để thử cho chạy được** — thêm vào `Program.cs` ngay sau `var app = builder.Build();`:

```csharp
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
}
```

`EnsureCreated()` tạo bảng khớp model hiện tại. Điểm yếu: **nó không biết cách nâng cấp**. Thêm một cột mới vào entity rồi chạy lại → cột đó không xuất hiện, và bạn sẽ ngồi debug rất lâu. Cách thoát duy nhất là `DROP DATABASE sochi;` rồi tạo lại — mất sạch dữ liệu.

**Với PostgreSQL, hãy dùng Migrations ngay từ đầu.** Lý do thực tế: Bước 3.10 cần Migrations để dựng database test tự động, nên đằng nào bạn cũng phải có nó.

```powershell
dotnet tool install --global dotnet-ef
dotnet add src/Api/SoChi.Api/SoChi.Api.csproj package Microsoft.EntityFrameworkCore.Design

dotnet ef migrations add InitialCreate --project src/Api/SoChi.Api
dotnet ef database update --project src/Api/SoChi.Api    # áp vào database `sochi`
```

Mỗi lần đổi entity: `dotnet ef migrations add <TenThayDoi> --project src/Api/SoChi.Api` rồi `database update`. Thư mục `Migrations/` được commit vào Git — nó là **lịch sử tiến hóa của schema**, và là thứ cho phép nâng cấp database production mà không mất dữ liệu.

Nếu dùng Migrations thì thay `EnsureCreated()` bằng:

```csharp
db.Database.Migrate();
```

> Không bao giờ dùng lẫn hai cách. `EnsureCreated()` tạo schema **không có** bảng `__EFMigrationsHistory`, nên `Migrate()` sau đó sẽ cố chạy lại migration đầu tiên và báo lỗi `relation "Users" already exists`. Lỡ dính thì: `psql -U postgres -c "DROP DATABASE sochi;"` rồi tạo lại và chạy `dotnet ef database update`.

### Bước 3.9: Endpoint đồng bộ `/api/sync/push` và `/api/sync/pull`

Đây là phần Buổi 14 sẽ gọi tới. Không có nó, Sync Engine phía client sẽ nhận `404`.

Tạo `src/Api/SoChi.Api/Endpoints/SyncEndpoints.cs`:

```csharp
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using SoChi.Api.Data;
using SoChi.Api.Entities;
using SoChi.Api.Security;
using SoChi.Shared;
using SoChi.Shared.Dtos;

namespace SoChi.Api.Endpoints;

public static class SyncEndpoints
{
    public static void MapSyncEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/sync").RequireAuthorization();

        // ─── ĐẨY LÊN: client gửi các bản ghi có SyncState = Local ───
        group.MapPost("/push", async (SyncPushRequest req, ClaimsPrincipal principal, AppDbContext db) =>
        {
            var userId = principal.GetUserId();

            foreach (var dto in req.Categories)
            {
                var existing = await db.Categories
                    .FirstOrDefaultAsync(c => c.Id == dto.Id && c.UserId == userId);

                if (existing is null)
                {
                    db.Categories.Add(new CategoryEntity
                    {
                        Id = dto.Id,
                        UserId = userId,
                        Name = dto.Name,
                        IconGlyph = dto.IconGlyph,
                        ColorHex = dto.ColorHex,
                        Kind = dto.Kind,
                        MonthlyLimit = dto.MonthlyLimit,
                        UpdatedAt = dto.UpdatedAt,
                        IsDeleted = dto.IsDeleted
                    });
                }
                else if (dto.UpdatedAt > existing.UpdatedAt)   // ← LAST-WRITE-WINS
                {
                    existing.Name = dto.Name;
                    existing.IconGlyph = dto.IconGlyph;
                    existing.ColorHex = dto.ColorHex;
                    existing.Kind = dto.Kind;
                    existing.MonthlyLimit = dto.MonthlyLimit;
                    existing.UpdatedAt = dto.UpdatedAt;
                    existing.IsDeleted = dto.IsDeleted;
                }
                // Ngược lại: bản trên server mới hơn -> BỎ QUA.
                // Client sẽ nhận bản mới đó ở lần pull kế tiếp.
            }

            foreach (var dto in req.Transactions)
            {
                var existing = await db.Transactions
                    .FirstOrDefaultAsync(t => t.Id == dto.Id && t.UserId == userId);

                if (existing is null)
                {
                    db.Transactions.Add(new TransactionEntity
                    {
                        Id = dto.Id,
                        UserId = userId,
                        CategoryId = dto.CategoryId,
                        Amount = dto.Amount,
                        OccurredOn = dto.OccurredOn,
                        Note = dto.Note,
                        ReceiptPath = dto.ReceiptPath,
                        UpdatedAt = dto.UpdatedAt,
                        IsDeleted = dto.IsDeleted
                    });
                }
                else if (dto.UpdatedAt > existing.UpdatedAt)
                {
                    existing.CategoryId = dto.CategoryId;
                    existing.Amount = dto.Amount;
                    existing.OccurredOn = dto.OccurredOn;
                    existing.Note = dto.Note;
                    existing.ReceiptPath = dto.ReceiptPath;
                    existing.UpdatedAt = dto.UpdatedAt;
                    existing.IsDeleted = dto.IsDeleted;
                }
            }

            await db.SaveChangesAsync();
            return Results.Ok(new { ServerTimeUtc = DateTime.UtcNow });
        });

        // ─── KÉO VỀ: trả mọi bản ghi thay đổi sau mốc "since" ───
        group.MapGet("/pull", async (DateTime? since, ClaimsPrincipal principal, AppDbContext db) =>
        {
            var userId = principal.GetUserId();
            var mark = since?.ToUniversalTime() ?? DateTime.MinValue;

            // Chốt mốc thời gian TRƯỚC khi truy vấn (xem Pitfall bên dưới)
            var serverTime = DateTime.UtcNow;

            var categories = await db.Categories
                .Where(c => c.UserId == userId && c.UpdatedAt > mark)
                .Select(c => new CategorySyncDto(
                    c.Id, c.Name, c.IconGlyph, c.ColorHex, c.Kind,
                    c.MonthlyLimit, c.UpdatedAt, c.IsDeleted))
                .ToListAsync();

            var transactions = await db.Transactions
                .Where(t => t.UserId == userId && t.UpdatedAt > mark)
                .Select(t => new TransactionSyncDto(
                    t.Id, t.CategoryId, t.Amount, t.OccurredOn, t.Note,
                    t.ReceiptPath, t.UpdatedAt, t.IsDeleted))
                .ToListAsync();

            return Results.Ok(new SyncPullResponse(categories, transactions, serverTime));
        });
    }
}
```

Bốn quyết định thiết kế trong đoạn code trên — hiểu chúng quan trọng hơn chép chúng:

1. **Mọi truy vấn đều kèm `&& c.UserId == userId`.** Bỏ điều kiện này đi thì người dùng A gửi lên `Id` của người dùng B là **ghi đè được dữ liệu người khác**. Đây là lỗ hổng IDOR — lỗi bảo mật phổ biến bậc nhất trong API thực tế, và nó không hề gây lỗi lúc chạy nên rất khó phát hiện.
2. **`pull` trả cả bản ghi `IsDeleted = true`.** Nếu lọc chúng đi, máy B sẽ không bao giờ biết máy A đã xóa gì — đúng cái bẫy đã ghi ở Buổi 14.
3. **`ServerTimeUtc` do server cấp, client lưu lại làm mốc `since` lần sau.** Không dùng giờ máy client: đồng hồ điện thoại lệch vài phút là đủ để bỏ sót hoặc kéo trùng dữ liệu.
4. **Chốt `serverTime` trước khi truy vấn.** Nếu lấy sau, những bản ghi được ghi vào *trong lúc* truy vấn đang chạy sẽ nằm trong khoảng thời gian bị nhảy qua và **mất vĩnh viễn** ở lần sync sau.

Cuối cùng, thêm dòng này vào **cuối** `Program.cs` để Bước 3.10 test được:

```csharp
app.Run();

// Cho phép WebApplicationFactory<Program> nhìn thấy lớp Program tự sinh của Minimal API
public partial class Program { }
```

### Bước 3.10: Integration Test cho API với `WebApplicationFactory`

Buổi 11 đã test logic phía Client. Nhưng phần dễ sai nhất của toàn dự án — last-write-wins và lọc theo `UserId` — lại nằm ở server, và **không thể kiểm tra bằng tay một cách đáng tin cậy**. Kịch bản xung đột hai thiết bị ở Buổi 15 rất khó tái hiện thủ công; viết test thì chạy lại trong một giây.

```powershell
dotnet new xunit -n SoChi.Api.Tests -o tests/SoChi.Api.Tests
dotnet sln add tests/SoChi.Api.Tests/SoChi.Api.Tests.csproj
dotnet add tests/SoChi.Api.Tests/SoChi.Api.Tests.csproj reference src/Api/SoChi.Api/SoChi.Api.csproj
dotnet add tests/SoChi.Api.Tests/SoChi.Api.Tests.csproj package Microsoft.AspNetCore.Mvc.Testing
dotnet add tests/SoChi.Api.Tests/SoChi.Api.Tests.csproj package Npgsql
```

Tạo `tests/SoChi.Api.Tests/ApiFactory.cs`. Mỗi lần chạy test, factory **tự tạo một database PostgreSQL mới toanh** với tên ngẫu nhiên, chạy migration lên nó, và xóa sạch khi xong:

```csharp
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using SoChi.Api.Data;
using Xunit;

namespace SoChi.Api.Tests;

public class ApiFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    // Kết nối tới database hệ thống `postgres` để có quyền CREATE/DROP DATABASE
    private const string AdminConnection =
        "Host=localhost;Port=5432;Database=postgres;Username=postgres;Password=dev";

    private readonly string _dbName = $"sochi_test_{Guid.NewGuid():N}";

    private string TestConnection =>
        $"Host=localhost;Port=5432;Database={_dbName};Username=postgres;Password=dev";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseSetting("Jwt:Key", "khoa-test-chi-dung-trong-unit-test-dai-hon-32-byte!!");
        builder.UseSetting("ConnectionStrings:DefaultConnection", TestConnection);
    }

    public async Task InitializeAsync()
    {
        await using (var admin = new NpgsqlConnection(AdminConnection))
        {
            await admin.OpenAsync();
            await using var cmd = new NpgsqlCommand($"CREATE DATABASE \"{_dbName}\"", admin);
            await cmd.ExecuteNonQueryAsync();
        }

        using var scope = Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await db.Database.MigrateAsync();
    }

    // Khai báo tường minh (explicit) để không đụng độ với DisposeAsync của WebApplicationFactory
    async Task IAsyncLifetime.DisposeAsync()
    {
        await base.DisposeAsync();

        await using var admin = new NpgsqlConnection(AdminConnection);
        await admin.OpenAsync();
        await using var cmd = new NpgsqlCommand(
            $"DROP DATABASE IF EXISTS \"{_dbName}\" WITH (FORCE)", admin);
        await cmd.ExecuteNonQueryAsync();
    }
}
```

Bốn chi tiết đáng học trong đoạn này:

1. **Database riêng cho mỗi lần chạy, tên ngẫu nhiên.** Chạy test song song hay chạy hai lần cùng lúc cũng không đụng nhau, và không bao giờ chạm vào database `sochi` thật.
2. **`MigrateAsync()` chứ không phải `EnsureCreated()`.** Test chạy trên đúng schema mà migration sinh ra — nếu migration hỏng, test đỏ ngay. Đây là lợi ích lớn: bạn kiểm chứng luôn cả file migration, không chỉ code.
3. **`WITH (FORCE)`** ngắt mọi kết nối còn sót rồi mới xóa. Thiếu nó, PostgreSQL từ chối với lỗi "database is being accessed by other users" và bạn sẽ tích tụ hàng chục database rác.
4. **Khai báo `DisposeAsync` tường minh.** `WebApplicationFactory` đã có sẵn `ValueTask DisposeAsync()`, còn xUnit `IAsyncLifetime` đòi `Task DisposeAsync()` — hai chữ ký xung đột. Viết `async Task IAsyncLifetime.DisposeAsync()` là cách hợp lệ để cả hai cùng tồn tại. (xUnit v3 đổi sang `ValueTask`; nếu dùng v3 thì bỏ `Task` thành `ValueTask`.)

> **Vì sao không giữ SQLite in-memory cho test cho nhanh?** Vì như vậy bạn đang kiểm chứng trên một database **khác** với production, mà khác biệt lại nằm đúng chỗ dễ sai nhất của dự án này: cách xử lý `DateTime`. SQLite chấp nhận mọi `DateTimeKind`, PostgreSQL thì ném lỗi. Test xanh trên SQLite rồi vỡ trên PostgreSQL là kịch bản tệ nhất — mất niềm tin vào chính bộ test của mình.

**Điều kiện tiên quyết:** PostgreSQL service phải đang chạy và `dotnet ef migrations add InitialCreate` đã được thực hiện ở Bước 3.8, nếu không `MigrateAsync()` sẽ không có gì để áp.

Tạo `tests/SoChi.Api.Tests/SyncEndpointTests.cs`:

```csharp
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using SoChi.Shared.Dtos;
using Xunit;

namespace SoChi.Api.Tests;

public class SyncEndpointTests : IClassFixture<ApiFactory>
{
    private readonly ApiFactory _factory;
    public SyncEndpointTests(ApiFactory factory) => _factory = factory;

    private async Task<HttpClient> CreateAuthenticatedClientAsync(string email)
    {
        var client = _factory.CreateClient();
        var res = await client.PostAsJsonAsync("/api/auth/register",
            new RegisterRequest(email, "matkhau123", "Người dùng test"));

        var auth = await res.Content.ReadFromJsonAsync<AuthResponse>();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", auth!.Token);
        return client;
    }

    [Fact]
    public async Task Pull_KhongCoToken_TraVe401()
    {
        var client = _factory.CreateClient();
        var res = await client.GetAsync("/api/sync/pull");
        Assert.Equal(HttpStatusCode.Unauthorized, res.StatusCode);
    }

    [Fact]
    public async Task Push_RoiPull_TraVeDungBanGhiVuaDay()
    {
        var client = await CreateAuthenticatedClientAsync("a@test.com");

        var tx = new TransactionSyncDto(Guid.NewGuid(), Guid.NewGuid(), 250_000,
            DateTime.Today, "Cà phê", null, DateTime.UtcNow, false);

        await client.PostAsJsonAsync("/api/sync/push", new SyncPushRequest([], [tx]));

        var pulled = await client.GetFromJsonAsync<SyncPullResponse>("/api/sync/pull");

        Assert.Single(pulled!.Transactions);
        Assert.Equal(250_000, pulled.Transactions[0].Amount);
    }

    [Fact]
    public async Task Push_BanCuHon_KhongDeLenBanMoiHon()
    {
        var client = await CreateAuthenticatedClientAsync("b@test.com");
        var id = Guid.NewGuid();
        var moc = DateTime.UtcNow;

        var moiHon = new TransactionSyncDto(id, Guid.NewGuid(), 900,
            DateTime.Today, "Bản mới", null, moc, false);
        var cuHon = moiHon with { Amount = 100, Note = "Bản cũ", UpdatedAt = moc.AddMinutes(-10) };

        await client.PostAsJsonAsync("/api/sync/push", new SyncPushRequest([], [moiHon]));
        await client.PostAsJsonAsync("/api/sync/push", new SyncPushRequest([], [cuHon]));

        var pulled = await client.GetFromJsonAsync<SyncPullResponse>("/api/sync/pull");

        Assert.Equal(900, pulled!.Transactions[0].Amount);   // bản mới hơn phải thắng
    }

    [Fact]
    public async Task Pull_KhongThayDuLieuCuaNguoiKhac()
    {
        var clientA = await CreateAuthenticatedClientAsync("nguoi-a@test.com");
        var clientB = await CreateAuthenticatedClientAsync("nguoi-b@test.com");

        var cuaA = new TransactionSyncDto(Guid.NewGuid(), Guid.NewGuid(), 500_000,
            DateTime.Today, "Bí mật của A", null, DateTime.UtcNow, false);

        await clientA.PostAsJsonAsync("/api/sync/push", new SyncPushRequest([], [cuaA]));

        var cuaB = await clientB.GetFromJsonAsync<SyncPullResponse>("/api/sync/pull");

        Assert.Empty(cuaB!.Transactions);   // B tuyệt đối không được thấy dữ liệu của A
    }
}
```

Test cuối cùng là test quan trọng nhất trong cả dự án. Nó là thứ duy nhất chứng minh bạn không viết ra một API rò rỉ dữ liệu giữa các người dùng.


## 4. Bẫy thường gặp (Pitfalls)
- **Lỗi:** Gọi API từ Postman trả về lỗi 401 dù đã truyền Header Authorization.
  - **Nguyên nhân:** Quên thêm tiền tố `Bearer ` trước chuỗi Token (`Authorization: Bearer eyJhbGciOi...`).
- **Lỗi:** Chạy API báo `42P01: relation "Users" does not exist`.
  - **Nguyên nhân:** Database `sochi` đã tồn tại nhưng chưa có bảng nào. Xem Bước 3.8 — chạy `dotnet ef database update --project src/Api/SoChi.Api`.
- **Lỗi:** `Cannot write DateTime with Kind=Unspecified to PostgreSQL type 'timestamp with time zone'`.
  - **Nguyên nhân:** Npgsql ánh xạ `DateTime` sang `timestamptz` và **bắt buộc** giá trị ghi vào phải có `Kind = Utc`. Trường `OccurredOn` được gán bằng `DateTime.Today` — giá trị này có `Kind = Unspecified` nên bị chặn ngay.
  - **Khắc phục:** phân biệt đúng bản chất hai loại trường. `OccurredOn` là **một ngày trên lịch**, không phải mốc thời gian tuyệt đối → ánh xạ sang kiểu `date`. Thêm vào `OnModelCreating`:
    ```csharp
    b.Entity<TransactionEntity>().Property(t => t.OccurredOn).HasColumnType("date");
    ```
    Còn `UpdatedAt` là mốc thời gian thật, luôn tạo bằng `DateTime.UtcNow` nên đã đúng sẵn.
  - **Đây là điểm cộng của PostgreSQL, không phải phiền toái.** Cũng lỗi tư duy đó, SQLite im lặng chấp nhận rồi trả về sai lệch 7 tiếng, khiến so sánh last-write-wins sai một cách ngẫu nhiên và cực kỳ khó truy. PostgreSQL chặn ngay tại dòng ghi đầu tiên.
- **Lỗi:** `28P01: password authentication failed for user "postgres"`.
  - **Nguyên nhân:** Mật khẩu trong chuỗi kết nối không khớp mật khẩu đặt lúc cài PostgreSQL. Kiểm tra bằng `dotnet user-secrets list --project src/Api/SoChi.Api`, và thử đăng nhập trực tiếp bằng `psql -U postgres`.
- **Lỗi:** `Failed to connect to 127.0.0.1:5432`.
  - **Nguyên nhân:** Service PostgreSQL chưa chạy. `Get-Service postgresql*` để kiểm tra, `Start-Service postgresql-x64-17` để bật.
- **Lỗi:** Đã chạy `dotnet ef database update` thành công nhưng API vẫn báo `relation "Users" does not exist`.
  - **Nguyên nhân:** Migration áp vào một database, còn app đang trỏ vào database khác. `dotnet ef` đọc chuỗi kết nối từ cùng nguồn cấu hình với app — nếu bạn vừa đổi user-secrets thì phải chắc chắn cả hai cùng trỏ tới `Database=sochi`.
- **Lỗi:** `WebApplicationFactory<Program>` báo `'Program' is inaccessible due to its protection level`.
  - **Nguyên nhân:** Minimal API sinh ra lớp `Program` dạng `internal`. Thêm `public partial class Program { }` vào cuối `Program.cs` (Bước 3.9).
- **Lỗi:** Người dùng A sync xong thì thấy cả giao dịch của người dùng B.
  - **Nguyên nhân:** Thiếu điều kiện `&& x.UserId == userId` trong một truy vấn nào đó. Lỗi này **không gây exception**, chỉ lộ dữ liệu — đó là lý do bắt buộc phải có test tự động cho nó.

## 5. Checklist nghiệm thu Buổi 12
- [ ] Chạy lệnh `dotnet run --project src/Api/SoChi.Api` -> Server khởi động thành công.
- [ ] Sử dụng Postman hoặc Swagger gọi `/api/auth/register` -> Tạo được user mới vào server database.
- [ ] Gọi `/api/auth/login` -> Trả về chuỗi JWT Token hợp lệ.
- [ ] Dùng Token đó gọi endpoint có `[Authorize]` -> Nhận mã phản hồi `200 OK`.
- [ ] `dotnet user-secrets list --project src/Api/SoChi.Api` hiện `Jwt:Key`. Xóa secret đi thì API **không khởi động được** — đúng như thiết kế fail fast.
- [ ] Mở database server, cột `PasswordHash` là chuỗi ba phần dạng `100000.xxxx.yyyy`, tuyệt đối không phải mật khẩu thô.
- [ ] `POST /api/sync/push` rồi `GET /api/sync/pull` trả về đúng bản ghi vừa đẩy lên.
- [ ] Đẩy một bản ghi có `UpdatedAt` cũ hơn bản trên server -> server **giữ nguyên** bản mới hơn.
- [ ] `dotnet test tests/SoChi.Api.Tests` xanh cả 4 test, đặc biệt là `Pull_KhongThayDuLieuCuaNguoiKhac`.
- [ ] `Get-Service postgresql*` báo `Running`; `psql -U postgres -l` liệt kê đủ `sochi` và `sochi_test`.
- [ ] `dotnet ef database update` chạy xong, mở pgAdmin thấy đủ 3 bảng `Users`, `Categories`, `Transactions` cùng bảng `__EFMigrationsHistory`.
- [ ] Chuỗi kết nối nằm trong user-secrets, **không** có dòng nào chứa `Password=` trong `appsettings.json`.
- [ ] Chạy `dotnet test` hai lần liên tiếp đều xanh, và `psql -U postgres -l` **không** còn database rác tên `sochi_test_...` nào sót lại.

---

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

# BUỔI 14: Offline-First Sync Hai Chiều, Connectivity & Last-Write-Wins

**Thuộc chặng:** Chặng 7 (Phần 3) | **Thời lượng:** ~3 giờ

## 1. Mục tiêu buổi học (Definition of Done)
- Hoàn thiện cơ chế **Đồng bộ hai chiều (Two-way Synchronization)**:
  - **Push:** Đẩy toàn bộ các bản ghi cục bộ có `SyncState = Local` lên máy chủ.
  - **Pull:** Kéo các bản ghi trên máy chủ có `UpdatedAt > Lần sync gần nhất` về cập nhật lại SQLite.
- Áp dụng thuật toán giải quyết xung đột **Last-Write-Wins (LWW)** dựa trên mốc thời gian UTC `UpdatedAt`.
- Xử lý đồng bộ **Xóa mềm (`IsDeleted = true`)**: Khi một bên xóa giao dịch, bên kia khi sync về cũng đánh dấu xóa mềm, không làm xuất hiện lại dữ liệu đã xóa.
- Giám sát trạng thái mạng tự động bằng `Connectivity.Current`: Mất mạng thì tiếp tục dùng bình thường; khi có mạng trở lại thì app tự động kích hoạt đồng bộ ngầm.

## 2. Bản chất kiến trúc & Nguyên lý
- **Triết lý Offline-First:** Ứng dụng luôn đọc và ghi trực tiếp vào SQLite cục bộ trước, sau đó mới đồng bộ với Server trong background. Người dùng không bao giờ phải nhìn thấy vòng xoay tải trang (loading spinner) khi thêm một giao dịch.
- **Thuật toán Last-Write-Wins (LWW):** Khi cùng một giao dịch bị sửa ở cả Client và Server trong lúc ngắt mạng:
  - Nếu `Client.UpdatedAt > Server.UpdatedAt`: Bản ghi từ Client ghi đè lên Server.
  - Nếu `Server.UpdatedAt > Client.UpdatedAt`: Bản ghi từ Server ghi đè lên Client.
  - *Hạn chế cần nhớ:* Phụ thuộc vào đồng hồ thiết bị. Cần luôn dùng chuẩn `DateTime.UtcNow`.

## 3. Các bước thực hành chi tiết

### Bước 3.1: Xây dựng Sync Engine tại Client
Tạo file `src/Client/SoChi.Client/Services/ISyncService.cs`:
```csharp
using System.Net.Http.Json;
using SoChi.Client.Models;
using SoChi.Shared;
using SoChi.Shared.Dtos;

namespace SoChi.Client.Services;

public class SyncService
{
    private readonly ITransactionRepository _repository;
    private readonly IHttpClientFactory _httpClientFactory;
    private const string LastSyncKey = "last_sync_timestamp_utc";

    public SyncService(ITransactionRepository repository, IHttpClientFactory httpClientFactory)
    {
        _repository = repository;
        _httpClientFactory = httpClientFactory;
    }

    public async Task<bool> SynchronizeAsync()
    {
        if (Connectivity.Current.NetworkAccess != NetworkAccess.Internet)
            return false;

        try
        {
            var client = _httpClientFactory.CreateClient("SoChiApi");

            // 1. PUSH: Lấy các bản ghi local chưa sync
            var localTransactions = await _repository.GetTransactionsAsync();
            var dirtyTransactions = localTransactions.Where(t => t.SyncState == SyncState.Local).ToList();

            var pushRequest = new SyncPushRequest(
                new List<CategorySyncDto>(),
                dirtyTransactions.Select(t => new TransactionSyncDto(
                    t.Id, t.CategoryId, t.Amount, t.OccurredOn, t.Note, t.ReceiptPath, t.UpdatedAt, t.IsDeleted
                )).ToList()
            );

            var pushResponse = await client.PostAsJsonAsync(ApiRoutes.SyncPush, pushRequest);
            if (!pushResponse.IsSuccessStatusCode) return false;

            // Đánh dấu các bản ghi đã push thành Synced
            foreach (var t in dirtyTransactions)
            {
                t.SyncState = SyncState.Synced;
                await _repository.SaveTransactionAsync(t);
            }

            // 2. PULL: Kéo dữ liệu mới từ Server
            DateTime lastSync = Preferences.Get(LastSyncKey, DateTime.MinValue);
            var pullResponse = await client.GetFromJsonAsync<SyncPullResponse>($"{ApiRoutes.SyncPull}?sinceUtc={lastSync:O}");

            if (pullResponse != null)
            {
                foreach (var serverItem in pullResponse.Transactions)
                {
                    var localItem = await _repository.GetTransactionByIdAsync(serverItem.Id);
                    // Áp dụng Last-Write-Wins
                    if (localItem == null || serverItem.UpdatedAt > localItem.UpdatedAt)
                    {
                        var updated = new Transaction
                        {
                            Id = serverItem.Id,
                            CategoryId = serverItem.CategoryId,
                            Amount = serverItem.Amount,
                            OccurredOn = serverItem.OccurredOn,
                            Note = serverItem.Note,
                            ReceiptPath = serverItem.ReceiptPath,
                            UpdatedAt = serverItem.UpdatedAt,
                            IsDeleted = serverItem.IsDeleted,
                            SyncState = SyncState.Synced
                        };
                        await _repository.SaveTransactionAsync(updated);
                    }
                }

                Preferences.Set(LastSyncKey, pullResponse.ServerTimeUtc);
            }

            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }
}
```

### Bước 3.2: Lắng nghe sự kiện thay đổi mạng
Trong `App.xaml.cs`:
```csharp
protected override void OnStart()
{
    base.OnStart();
    Connectivity.Current.ConnectivityChanged += OnConnectivityChanged;
}

private async void OnConnectivityChanged(object? sender, ConnectivityChangedEventArgs e)
{
    if (e.NetworkAccess == NetworkAccess.Internet)
    {
        var syncService = Handler?.MauiContext?.Services.GetService<SyncService>();
        if (syncService != null)
        {
            await syncService.SynchronizeAsync();
        }
    }
}
```

### Bước 3.3: Cập nhật UI từ luồng nền — `MainThread`

Đây là lỗi sẽ xuất hiện **ngay khi bạn chạy trên Android**, và hầu như không xuất hiện trên Windows — nên rất dễ tưởng là code đã đúng.

Sự kiện `Connectivity.ConnectivityChanged` ở Bước 3.2 được hệ điều hành phát đi từ **luồng nền**. Mọi thứ chạy tiếp sau nó cũng ở luồng nền. Nếu trong đó bạn sửa `ObservableCollection` đang được `CollectionView` hiển thị, Android sẽ ném exception:

> `Only the original thread that created a view hierarchy can touch its views.`

Nguyên tắc: **mọi thay đổi chạm tới UI phải nằm trên UI thread.** Sửa hàm xử lý sự kiện thành:

```csharp
private async void OnConnectivityChanged(object? sender, ConnectivityChangedEventArgs e)
{
    if (e.NetworkAccess != NetworkAccess.Internet) return;

    var changed = await _syncService.SynchronizeAsync();
    if (!changed) return;

    // Nạp lại dữ liệu ở luồng nền là an toàn...
    var items = await _repository.GetTransactionsAsync();

    // ...nhưng đổ vào collection đang bind thì bắt buộc phải qua UI thread
    MainThread.BeginInvokeOnMainThread(() =>
    {
        Transactions.Clear();
        foreach (var item in items)
            Transactions.Add(item);

        SyncStatusText = $"Đồng bộ lúc {DateTime.Now:HH:mm}";
    });
}
```

Ba tình huống **bắt buộc** dùng `MainThread` trong dự án này:

| Tình huống | Vì sao |
|---|---|
| `ConnectivityChanged` (Buổi 14) | Sự kiện platform, luôn ở luồng nền |
| Callback từ `Timer` / `Task.Run` | Không giữ context của UI |
| Sync tự động chạy nền khi app khởi động | Không bắt nguồn từ tương tác người dùng |

Và tình huống **không cần**: khi bạn `await` bên trong một `[RelayCommand]` do người dùng bấm. Lệnh đó bắt đầu trên UI thread, nên sau mỗi `await` mã sẽ tự quay lại đúng luồng đó (`SynchronizationContext` lo việc này). Đây chính là lý do các buổi trước chưa cần đến `MainThread`.

Nếu cần trả về giá trị, dùng bản `async`:

```csharp
var confirmed = await MainThread.InvokeOnMainThreadAsync(
    () => Shell.Current.DisplayAlert("Xung đột", "Dữ liệu đã đổi ở máy khác.", "OK", "Hủy"));
```

> **Mẹo debug:** thêm `System.Diagnostics.Debug.WriteLine($"UI thread? {MainThread.IsMainThread}");` vào đầu hàm đang nghi ngờ. Đây là cách nhanh nhất để biết mình đang đứng ở luồng nào.

### Bước 3.4: Chống mạng chập chờn — Retry, Timeout & tránh sync chồng nhau

Sync là thao tác mạng dài nhất trong app, lại thường chạy đúng lúc mạng vừa mới có lại — tức là lúc kết nối yếu nhất. Không xử lý thì một lần 4G phập phù là sync thất bại và dữ liệu nằm lại máy vô thời hạn.

Cài gói khả năng chịu lỗi (đây là Polly được đóng gói sẵn cho `HttpClient`, cách dùng chuẩn từ .NET 8 trở lên):

```powershell
dotnet add src/Client/SoChi.Client/SoChi.Client.csproj package Microsoft.Extensions.Http.Resilience
```

Trong `MauiProgram.cs`, gắn vào đúng `HttpClient` đã đăng ký ở Buổi 13:

```csharp
builder.Services.AddHttpClient("SoChiApi", client =>
{
    client.BaseAddress = new Uri(ApiConfig.BaseUrl);
    client.Timeout = TimeSpan.FromSeconds(60);   // trần tổng thể cho cả chuỗi retry
})
.AddHttpMessageHandler<AuthTokenHandler>()
.AddStandardResilienceHandler(options =>
{
    options.Retry.MaxRetryAttempts = 3;
    options.Retry.Delay = TimeSpan.FromSeconds(2);
    options.Retry.BackoffType = Polly.DelayBackoffType.Exponential;

    options.AttemptTimeout.Timeout = TimeSpan.FromSeconds(15);   // cho MỖI lần thử
    options.TotalRequestTimeout.Timeout = TimeSpan.FromSeconds(55);
});
```

Bốn cơ chế bạn vừa bật, và ý nghĩa của từng cái:

| Cơ chế | Làm gì | Vì sao cần |
|---|---|---|
| **Retry** | Thử lại khi gặp lỗi mạng hoặc HTTP 5xx | Mạng di động rớt gói là chuyện bình thường, không phải lỗi thật |
| **Exponential backoff** | Giãn dần 2s → 4s → 8s | Thử lại dồn dập chỉ làm nghẽn thêm mạng đang yếu |
| **Attempt timeout** | Cắt mỗi lần thử ở 15s | Không để một request treo chiếm hết ngân sách thời gian |
| **Circuit breaker** | Tự ngắt khi server hỏng liên tục | Ngừng gọi vô ích, tiết kiệm pin và dữ liệu di động |

Quan trọng: `AddStandardResilienceHandler` **chỉ thử lại các phương thức idempotent một cách an toàn** khi lỗi là timeout hoặc 5xx. Endpoint `/api/sync/push` ở Buổi 12 an toàn với việc thử lại vì nó dùng **upsert theo `Id` do client sinh** — gửi hai lần cùng một bản ghi cho ra đúng một dòng trong database, không nhân bản. Đó không phải may mắn: đó là hệ quả trực tiếp của quyết định "khóa chính là `Guid` do client sinh" từ Buổi 03.

Cuối cùng, **chặn sync chồng nhau**. Người dùng có thể vừa kéo-để-làm-mới vừa đúng lúc mạng bật lại, khiến hai lượt sync chạy song song và ghi đè lẫn nhau. Thêm khóa vào `SyncService`:

```csharp
private readonly SemaphoreSlim _syncLock = new(1, 1);

public async Task<bool> SynchronizeAsync(CancellationToken ct = default)
{
    if (Connectivity.Current.NetworkAccess != NetworkAccess.Internet)
        return false;

    // Đang có lượt sync chạy -> bỏ qua lượt này, không xếp hàng chờ
    if (!await _syncLock.WaitAsync(0, ct))
        return false;

    try
    {
        // ... toàn bộ logic push/pull ...
        return true;
    }
    catch (HttpRequestException)
    {
        return false;   // mất mạng giữa chừng: im lặng bỏ qua, lần sau sync lại
    }
    catch (TaskCanceledException)
    {
        return false;   // hết timeout sau khi đã retry đủ
    }
    finally
    {
        _syncLock.Release();
    }
}
```

`WaitAsync(0)` là điểm tinh tế: nó **không chờ** — chiếm được khóa thì làm, không thì trả `false` ngay. Đúng với ngữ nghĩa của sync: chạy lượt thứ hai ngay sau lượt thứ nhất là vô nghĩa.


## 4. Bẫy thường gặp (Pitfalls)
- **Lỗi:** Xóa mềm ở máy A, sau khi sync máy B thì dữ liệu ở máy B bị nhân bản hoặc hiển thị lại.
  - **Nguyên nhân:** Khi kéo dữ liệu từ server về máy B, nếu không cập nhật cờ `IsDeleted = true` vào SQLite máy B thì hàm `GetTransactionsAsync()` vẫn sẽ đọc nó ra.
- **Lỗi:** App crash ngay khi mạng bật lại, log ghi `Only the original thread that created a view hierarchy can touch its views.`
  - **Nguyên nhân:** `ConnectivityChanged` chạy ở luồng nền, còn code trong đó lại sửa `ObservableCollection` đang được `CollectionView` hiển thị. Xem Bước 3.3.
- **Lỗi:** Sync trên 4G thất bại liên tục dù server vẫn sống.
  - **Nguyên nhân:** Chưa có retry/timeout. Xem Bước 3.4.
- **Lỗi:** Giao dịch bị nhân đôi sau khi mạng chập chờn.
  - **Nguyên nhân:** Hai lượt sync chạy song song. Dùng `SemaphoreSlim` với `WaitAsync(0)` như Bước 3.4. (Nếu bạn đã làm đúng phần `Guid` do client sinh từ Buổi 03 thì việc đẩy trùng **không** tạo bản ghi mới — nhân đôi ở đây là do logic client tạo `Id` mới khi retry.)

## 5. Checklist nghiệm thu Buổi 14
- [ ] Bật chế độ máy bay (ngắt hoàn toàn WiFi/4G trên điện thoại) -> Nhập thêm 3 giao dịch mới -> App hoạt động trơn tru không báo lỗi.
- [ ] Tắt chế độ máy bay (bật mạng lại) -> App tự động đẩy 3 giao dịch lên Server database.
- [ ] Mở app ở thiết bị thứ hai (hoặc mở trên Windows), đăng nhập cùng tài khoản -> Toàn bộ 3 giao dịch xuất hiện đầy đủ.
- [ ] Sync chạy nền xong, danh sách tự cập nhật mà **không crash trên Android** (đã bọc `MainThread`).
- [ ] Tắt hẳn server rồi bấm đồng bộ -> App hiện thông báo lỗi tử tế, không văng, dữ liệu cục bộ còn nguyên.
- [ ] Bấm đồng bộ liên tục 5 lần thật nhanh -> Chỉ một lượt thực sự chạy, không nhân bản dữ liệu.

---

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

# BUỔI 16: Blazor WebAssembly — Dựng Web Dashboard, JWT Auth & Công Cụ Kiểm Tra Dữ Liệu

**Thuộc chặng:** Chặng 9 (mở rộng) | **Thời lượng:** ~3 giờ

> **Buổi bổ sung.** Yêu cầu đã xong Buổi 12 (API + PostgreSQL). Đặt ở đây vì đến lúc này server đã có dữ liệu thật do client đồng bộ lên. Nếu muốn có công cụ debug sớm hơn để hỗ trợ Buổi 14, bạn hoàn toàn có thể làm buổi này ngay sau Buổi 12 — chỉ cần bơm vài bản ghi bằng Postman hoặc bằng chính bộ test ở Bước 3.10.

## 1. Mục tiêu buổi học (Definition of Done)
- Dựng project `src/Web/SoChi.Web` bằng Blazor WebAssembly, **được phục vụ trực tiếp từ `src/Api/SoChi.Api`** (cùng origin, không cần cấu hình CORS).
- Đăng nhập bằng chính endpoint `/api/auth/login` đã viết ở Buổi 12, token lưu ở `localStorage`.
- Có trang **Kiểm tra dữ liệu**: xem toàn bộ bản ghi thô trên server, gồm cả bản đã xóa mềm, kèm `UpdatedAt` và `UserId`.
- `src/Shared/SoChi.Shared` được **ba** project cùng tham chiếu: Client (MAUI), Api, Web.

## 2. Bản chất kiến trúc & Nguyên lý

**Blazor WebAssembly là gì.** Toàn bộ .NET runtime được biên dịch sang WebAssembly và tải về trình duyệt. Code C# của bạn chạy **trong trình duyệt người dùng**, không phải trên server. Nghĩa là: cùng ngôn ngữ, cùng DTO, cùng `HttpClient`, cùng mô hình DI mà bạn đã dùng suốt 15 buổi — chỉ đổi đích chạy.

Phân biệt với Blazor Server (dễ nhầm):

| | Blazor WebAssembly | Blazor Server |
|---|---|---|
| Code C# chạy ở | Trình duyệt | Server |
| Cần kết nối liên tục | Không — hoạt động cả khi mất mạng | Có — đứt SignalR là đứng hình |
| Lần tải đầu | Chậm hơn (phải tải runtime) | Nhanh |
| Bí mật (khóa API, chuỗi kết nối) | **Không giấu được gì cả** | Giấu được |

Chọn WebAssembly ở đây vì nó cùng triết lý với SoChi: **client tự chủ, server chỉ là nơi chứa dữ liệu**.

**Vì sao host từ `src/Api/SoChi.Api` chứ không chạy riêng.** Blazor WASM standalone chạy ở origin khác (ví dụ `localhost:5000`) trong khi API ở `localhost:7001` → mọi request bị chặn bởi CORS, phải cấu hình thêm, và cookie/token qua origin khác kéo theo một loạt vấn đề bảo mật. Cho `src/Api/SoChi.Api` phục vụ luôn file tĩnh của Web thì cả hai **cùng một origin**: không CORS, không cấu hình gì thêm, và deploy chỉ một thứ. Đây cũng là cách các dự án thực tế hay làm nhất.

**Điểm học lớn nhất của buổi này:** bạn sẽ nhận ra `AuthTokenHandler` viết cho Blazor **gần như giống hệt** cái đã viết cho MAUI ở Buổi 13. Cùng một `DelegatingHandler`, cùng một cách gắn `Bearer`. Khác biệt duy nhất là chỗ cất token: `SecureStorage` trên di động, `localStorage` trên web. Đó là bằng chứng cụ thể rằng bạn đang học **kiến trúc**, không phải học thuộc API của một framework.

## 3. Các bước thực hành chi tiết

### Bước 16.1: Tạo project và nối dây

```powershell
dotnet new blazorwasm -n SoChi.Web -o src/Web/SoChi.Web
dotnet sln SoChi.slnx add src/Web/SoChi.Web/SoChi.Web.csproj

# Web dùng chung DTO -> đây là project THỨ BA tham chiếu Shared
dotnet add src/Web/SoChi.Web/SoChi.Web.csproj reference src/Shared/SoChi.Shared/SoChi.Shared.csproj

# Api phục vụ file tĩnh của Web
dotnet add src/Api/SoChi.Api/SoChi.Api.csproj reference src/Web/SoChi.Web/SoChi.Web.csproj
dotnet add src/Api/SoChi.Api/SoChi.Api.csproj package Microsoft.AspNetCore.Components.WebAssembly.Server
```

Sơ đồ tham chiếu sau bước này:

```
Client (MAUI) ──┐
Web (Blazor)  ──┼──▶ Shared
Api           ──┘
      │
      └──▶ Web   (chỉ để lấy file tĩnh đã build, không gọi code)
```

Sửa `src/Api/SoChi.Api/Program.cs` — thứ tự các dòng này **có ý nghĩa**:

```csharp
var app = builder.Build();

app.UseBlazorFrameworkFiles();   // phục vụ _framework/*.wasm, *.dll
app.UseStaticFiles();            // phục vụ wwwroot của Web

app.UseAuthentication();
app.UseAuthorization();

app.MapAuthEndpoints(jwtKey);
app.MapSyncEndpoints();

app.MapFallbackToFile("index.html");   // BẮT BUỘC đặt CUỐI CÙNG

app.Run();
```

`MapFallbackToFile` nghĩa là "đường dẫn nào không khớp endpoint nào thì trả về `index.html`" — cần thiết để routing phía Blazor hoạt động khi người dùng F5 ở `/thong-ke`. Đặt nó **trước** các `Map...Endpoints` thì mọi lời gọi API sẽ nhận về trang HTML, và bạn sẽ nhận lỗi khó hiểu kiểu `'<' is an invalid start of a value` khi parse JSON.

Chạy thử:
```powershell
dotnet run --project src/Api/SoChi.Api
```
Mở `https://localhost:7xxx` — trang Blazor mặc định hiện lên, phục vụ bởi chính API. Không cần chạy hai tiến trình.

### Bước 16.2: Lưu token và gắn vào request

Tạo `src/Web/SoChi.Web/Services/TokenStore.cs`:

```csharp
using Microsoft.JSInterop;

namespace SoChi.Web.Services;

public class TokenStore(IJSRuntime js)
{
    private const string Key = "sochi_token";

    public ValueTask<string?> GetAsync() =>
        js.InvokeAsync<string?>("localStorage.getItem", Key);

    public ValueTask SetAsync(string token) =>
        js.InvokeVoidAsync("localStorage.setItem", Key, token);

    public ValueTask ClearAsync() =>
        js.InvokeVoidAsync("localStorage.removeItem", Key);
}
```

Tạo `src/Web/SoChi.Web/Services/AuthTokenHandler.cs` — **so nó với `AuthTokenHandler` của MAUI ở Buổi 13, gần như từng dòng một**:

```csharp
using System.Net.Http.Headers;

namespace SoChi.Web.Services;

public class AuthTokenHandler(TokenStore store) : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken ct)
    {
        var token = await store.GetAsync();

        if (!string.IsNullOrWhiteSpace(token))
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        return await base.SendAsync(request, ct);
    }
}
```

Đăng ký trong `src/Web/SoChi.Web/Program.cs`:

```csharp
builder.Services.AddScoped<TokenStore>();
builder.Services.AddTransient<AuthTokenHandler>();

builder.Services.AddHttpClient("SoChiApi", client =>
{
    // Cùng origin với trang web -> không cần biết địa chỉ API là gì,
    // và không dính CORS. Đây là phần thưởng của quyết định ở Bước 16.1.
    client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress);
})
.AddHttpMessageHandler<AuthTokenHandler>();

builder.Services.AddScoped(sp =>
    sp.GetRequiredService<IHttpClientFactory>().CreateClient("SoChiApi"));
```

### Bước 16.3: Trang đăng nhập

Tạo `src/Web/SoChi.Web/Pages/Login.razor`:

```razor
@page "/dang-nhap"
@using System.Net.Http.Json
@using SoChi.Shared
@using SoChi.Shared.Dtos
@using SoChi.Web.Services
@inject HttpClient Http
@inject TokenStore Tokens
@inject NavigationManager Nav

<h3>Đăng nhập SoChi</h3>

<div class="mb-2">
    <input class="form-control" placeholder="Email" @bind="email" />
</div>
<div class="mb-2">
    <input class="form-control" type="password" placeholder="Mật khẩu"
           @bind="password" @onkeyup="OnKeyUp" />
</div>

<button class="btn btn-primary" disabled="@busy" @onclick="LoginAsync">
    @(busy ? "Đang đăng nhập..." : "Đăng nhập")
</button>

@if (!string.IsNullOrEmpty(error))
{
    <div class="alert alert-danger mt-3">@error</div>
}

@code {
    private string email = "", password = "", error = "";
    private bool busy;

    private async Task OnKeyUp(KeyboardEventArgs e)
    {
        if (e.Key == "Enter") await LoginAsync();
    }

    private async Task LoginAsync()
    {
        busy = true;
        error = "";

        try
        {
            var res = await Http.PostAsJsonAsync($"/{ApiRoutes.Login}",
                new LoginRequest(email, password));

            if (!res.IsSuccessStatusCode)
            {
                error = "Email hoặc mật khẩu không đúng.";
                return;
            }

            var auth = await res.Content.ReadFromJsonAsync<AuthResponse>();
            await Tokens.SetAsync(auth!.Token);
            Nav.NavigateTo("/kiem-tra");
        }
        catch (HttpRequestException)
        {
            error = "Không kết nối được máy chủ.";
        }
        finally
        {
            busy = false;
        }
    }
}
```

Để ý: `ApiRoutes.Login`, `LoginRequest`, `AuthResponse` đều đến từ `src/Shared/SoChi.Shared` — **cùng đúng những kiểu mà MAUI đang dùng**. Đổi tên một trường trong `AuthResponse` bây giờ sẽ làm hỏng build của cả ba project cùng lúc, ngay lập tức. Đó chính xác là điều bạn muốn, và là lý do `Shared` tồn tại.

### Bước 16.4: Endpoint xem dữ liệu thô

Thêm vào `src/Api/SoChi.Api/Endpoints/SyncEndpoints.cs` (hoặc tạo `ReportEndpoints.cs`):

```csharp
group.MapGet("/raw", async (ClaimsPrincipal principal, AppDbContext db, bool includeDeleted = false) =>
{
    var userId = principal.GetUserId();

    var query = db.Transactions.Where(t => t.UserId == userId);
    if (!includeDeleted)
        query = query.Where(t => !t.IsDeleted);

    var rows = await query
        .OrderByDescending(t => t.UpdatedAt)
        .Take(500)
        .Select(t => new TransactionSyncDto(
            t.Id, t.CategoryId, t.Amount, t.OccurredOn, t.Note,
            t.ReceiptPath, t.UpdatedAt, t.IsDeleted))
        .ToListAsync();

    return Results.Ok(rows);
});
```

`.Take(500)` không phải chi tiết vặt: thiếu nó, một ngày nào đó trang này sẽ cố kéo về 50.000 dòng và treo trình duyệt. **Mọi endpoint trả về danh sách đều phải có giới hạn**, không có ngoại lệ.

### Bước 16.5: Trang Kiểm tra dữ liệu

Tạo `src/Web/SoChi.Web/Pages/DataInspector.razor`:

```razor
@page "/kiem-tra"
@using System.Net.Http.Json
@using SoChi.Shared.Dtos
@inject HttpClient Http

<h3>Kiểm tra dữ liệu trên server</h3>

<div class="form-check mb-3">
    <input class="form-check-input" type="checkbox" id="del"
           @bind="includeDeleted" @bind:after="LoadAsync" />
    <label class="form-check-label" for="del">Hiện cả bản ghi đã xóa mềm</label>
</div>

@if (loading)
{
    <p><em>Đang tải...</em></p>
}
else if (rows is null)
{
    <div class="alert alert-warning">
        Chưa đăng nhập hoặc token hết hạn. <a href="/dang-nhap">Đăng nhập lại</a>.
    </div>
}
else
{
    <p>@rows.Count bản ghi (tối đa 500, mới nhất trước).</p>

    <table class="table table-sm table-striped">
        <thead>
            <tr>
                <th>Số tiền</th>
                <th>Ngày</th>
                <th>Ghi chú</th>
                <th>UpdatedAt (UTC)</th>
                <th>Trạng thái</th>
                <th>Id</th>
            </tr>
        </thead>
        <tbody>
            @foreach (var r in rows)
            {
                <tr class="@(r.IsDeleted ? "text-decoration-line-through opacity-50" : "")">
                    <td class="text-end">@r.Amount.ToString("N0") đ</td>
                    <td>@r.OccurredOn.ToString("dd/MM/yyyy")</td>
                    <td>@r.Note</td>
                    <td><code>@r.UpdatedAt.ToString("O")</code></td>
                    <td>@(r.IsDeleted ? "Đã xóa" : "Hoạt động")</td>
                    <td><code>@r.Id.ToString()[..8]</code></td>
                </tr>
            }
        </tbody>
    </table>
}

@code {
    private List<TransactionSyncDto>? rows;
    private bool includeDeleted, loading = true;

    protected override Task OnInitializedAsync() => LoadAsync();

    private async Task LoadAsync()
    {
        loading = true;
        try
        {
            rows = await Http.GetFromJsonAsync<List<TransactionSyncDto>>(
                $"/api/sync/raw?includeDeleted={includeDeleted}");
        }
        catch (HttpRequestException)
        {
            rows = null;   // 401 hoặc mất mạng
        }
        finally
        {
            loading = false;
        }
    }
}
```

Trang này chính là **công cụ debug cho Buổi 14** mà bạn không có trước đây. Nó trả lời trực tiếp những câu hỏi mà trước kia phải mở pgAdmin gõ SQL mới biết:

- Bản ghi vừa tạo lúc offline đã thật sự lên tới server chưa?
- Xóa ở máy A rồi, trên server `IsDeleted` đã thành `true` chưa?
- Sửa ở hai máy, `UpdatedAt` nào lớn hơn — có đúng bản đó thắng không?

Hiển thị `UpdatedAt` ở định dạng `"O"` (ISO 8601 đầy đủ, có `Z` ở cuối) là cố ý: bạn cần thấy **chính xác đến từng phần nghìn giây và đúng là UTC** để đối chiếu last-write-wins, không phải một chuỗi ngày giờ đã bị làm tròn theo giờ địa phương.

## 4. Bẫy thường gặp (Pitfalls)
- **Lỗi:** Gọi API trả về HTML, JSON parse báo `'<' is an invalid start of a value`.
  - **Nguyên nhân:** `MapFallbackToFile("index.html")` đặt **trước** các endpoint API nên nó nuốt hết. Xem lại thứ tự ở Bước 16.1.
- **Lỗi:** `Access to fetch ... has been blocked by CORS policy`.
  - **Nguyên nhân:** Bạn đang chạy `src/Web/SoChi.Web` riêng (`dotnet run --project src/Web/SoChi.Web`) thay vì mở qua địa chỉ của API. Buổi này luôn chạy `dotnet run --project src/Api/SoChi.Api` rồi mở đúng cổng của nó.
- **Lỗi:** `IJSRuntime` ném `JavaScript interop calls cannot be issued at this time` khi đọc token.
  - **Nguyên nhân:** Gọi JS trong lúc prerender hoặc trong constructor. Chỉ gọi `localStorage` từ `OnAfterRenderAsync` hoặc từ event handler của người dùng.
- **Lỗi:** Đăng nhập xong F5 lại thì mất phiên.
  - **Nguyên nhân:** Token lưu ở `sessionStorage` thay vì `localStorage`, hoặc quên `await` khi ghi.
- **Cảnh báo bảo mật — đọc kỹ:** Blazor WASM chạy hoàn toàn trong trình duyệt, nên **mọi thứ trong `src/Web/SoChi.Web` đều công khai**: người dùng tải được toàn bộ DLL và đọc được mã nguồn của bạn. Tuyệt đối không đặt chuỗi kết nối database, khóa JWT, hay bất kỳ bí mật nào ở đây. Kiểm tra quyền **luôn phải nằm ở server** — `[Authorize]` và điều kiện `UserId` trong endpoint mới là thứ bảo vệ dữ liệu; ẩn/hiện nút bấm ở giao diện chỉ là tiện lợi cho người dùng, không phải bảo mật.
- **Về `localStorage`:** token nằm ở đó có thể bị đánh cắp nếu trang dính lỗ hổng XSS. Với dự án học tập thì chấp nhận được, nhưng cần biết đó là đánh đổi có ý thức. Giải pháp của ứng dụng thật là cookie `HttpOnly` + `SameSite`, và token sống ngắn kèm refresh token — chứ không phải token 30 ngày như Buổi 12 đang cấp.

## 5. Checklist nghiệm thu Buổi 16
- [ ] `dotnet run --project src/Api/SoChi.Api` rồi mở `https://localhost:7xxx` → thấy giao diện Blazor, **không** cần chạy tiến trình thứ hai.
- [ ] Đăng nhập bằng đúng tài khoản đã tạo ở Buổi 12 → chuyển sang trang Kiểm tra.
- [ ] Mở F12 → tab Network, thấy request `/api/sync/raw` mang header `Authorization: Bearer ...`.
- [ ] Xóa token trong `localStorage` rồi F5 → trang hiện cảnh báo chưa đăng nhập, **không** trắng màn hình.
- [ ] Thêm một giao dịch trên app MAUI rồi sync → F5 trang web thấy đúng bản ghi đó.
- [ ] Tick "Hiện cả bản ghi đã xóa mềm" → bản ghi đã xóa hiện lên với dòng gạch ngang.
- [ ] `dotnet build SoChi.slnx` xanh cả bốn project: Client, Api, Shared, Web.

---

# BUỔI 17: Endpoint Báo Cáo, Biểu Đồ SVG Tự Vẽ & Kiểm Chứng Sức Khỏe Dữ Liệu

**Thuộc chặng:** Chặng 9 (mở rộng) | **Thời lượng:** ~3 giờ

## 1. Mục tiêu buổi học (Definition of Done)
- Viết endpoint báo cáo **gộp dữ liệu tại server** bằng LINQ `GroupBy` được EF Core dịch sang SQL thật.
- Vẽ donut chart và bar chart bằng **inline SVG tự viết**, không dùng thư viện biểu đồ.
- Có trang kiểm tra sức khỏe dữ liệu: phát hiện bản ghi mồ côi, đếm bản đã xóa, xem mốc đồng bộ gần nhất.
- Kiểm chứng được kịch bản xung đột hai thiết bị của Buổi 15 bằng mắt, thay vì đoán.

## 2. Bản chất kiến trúc & Nguyên lý

**Gộp ở server hay ở client?** Ở Buổi 07–08, MAUI gộp dữ liệu ngay trên máy — đúng, vì SQLite nằm sẵn trong app và dữ liệu là của riêng một người. Trên web thì khác hẳn: kéo 20.000 giao dịch qua Internet về trình duyệt chỉ để cộng thành 8 con số là lãng phí băng thông, pin và thời gian chờ. **Nguyên tắc: gộp càng gần dữ liệu càng tốt.** PostgreSQL cộng 20.000 dòng trong vài mili-giây rồi trả về đúng 8 dòng kết quả.

**Vì sao vẫn tự vẽ biểu đồ.** Cùng lý do như Buổi 07: bạn học được cách biểu đồ thật sự hoạt động. Điều thú vị là **công thức toán không đổi một chút nào** so với `Microsoft.Maui.Graphics` — chỉ đổi đích vẽ:

| | MAUI (Buổi 07) | Blazor (buổi này) |
|---|---|---|
| Bề mặt vẽ | `GraphicsView` + `IDrawable` | Thẻ `<svg>` trong markup |
| Vẽ cung tròn | `canvas.DrawArc(...)` | `<circle>` + `stroke-dasharray` |
| Đơn vị góc | độ, 0° ở hướng 3 giờ | dùng chu vi, xoay `-90°` cho về đỉnh |
| Cập nhật khi đổi dữ liệu | `Invalidate()` | Blazor tự render lại |
| Tương tác (hover, click) | Phải tự bắt tọa độ chạm | Có sẵn qua `@onclick` trên từng phần tử |

Dòng cuối là điểm ăn tiền của SVG: mỗi cung là **một phần tử DOM thật**, nên gắn sự kiện, tooltip, hiệu ứng chuyển màu đều miễn phí.

## 3. Các bước thực hành chi tiết

### Bước 17.1: DTO báo cáo trong `src/Shared/SoChi.Shared`

Tạo `src/Shared/SoChi.Shared/Dtos/ReportDtos.cs`:

```csharp
namespace SoChi.Shared.Dtos;

public record CategoryReportItem(
    Guid CategoryId, string CategoryName, string ColorHex, long Total, int Count);

public record MonthlyReportItem(int Year, int Month, long Expense, long Income);

public record DataHealthReport(
    int TotalTransactions,
    int DeletedTransactions,
    int OrphanTransactions,
    int TotalCategories,
    DateTime? LastUpdatedAt);
```

Đặt ở `Shared` để sau này màn hình Thống kê của MAUI (Buổi 08) cũng có thể chuyển sang gọi cùng endpoint này — dùng lại đúng kiểu dữ liệu, không định nghĩa lần hai.

### Bước 17.2: Endpoint báo cáo

Tạo `src/Api/SoChi.Api/Endpoints/ReportEndpoints.cs`:

```csharp
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using SoChi.Api.Data;
using SoChi.Api.Security;
using SoChi.Shared.Dtos;

namespace SoChi.Api.Endpoints;

public static class ReportEndpoints
{
    public static void MapReportEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/reports").RequireAuthorization();

        // Tỉ trọng chi tiêu theo danh mục trong một tháng
        group.MapGet("/by-category", async (int year, int month,
            ClaimsPrincipal principal, AppDbContext db) =>
        {
            var userId = principal.GetUserId();
            var from = new DateTime(year, month, 1, 0, 0, 0, DateTimeKind.Utc);
            var to = from.AddMonths(1);

            var rows = await db.Transactions
                .Where(t => t.UserId == userId && !t.IsDeleted
                            && t.OccurredOn >= from && t.OccurredOn < to)
                .Join(db.Categories.Where(c => c.UserId == userId),
                      t => t.CategoryId, c => c.Id, (t, c) => new { t, c })
                .Where(x => x.c.Kind == 0)                  // 0 = Expense
                .GroupBy(x => new { x.c.Id, x.c.Name, x.c.ColorHex })
                .Select(g => new CategoryReportItem(
                    g.Key.Id, g.Key.Name, g.Key.ColorHex,
                    g.Sum(x => x.t.Amount), g.Count()))
                .OrderByDescending(r => r.Total)
                .ToListAsync();

            return Results.Ok(rows);
        });

        // Thu / chi 6 tháng gần nhất
        group.MapGet("/monthly", async (ClaimsPrincipal principal, AppDbContext db, int months = 6) =>
        {
            var userId = principal.GetUserId();
            var from = DateTime.UtcNow.Date.AddMonths(-months + 1);
            from = new DateTime(from.Year, from.Month, 1, 0, 0, 0, DateTimeKind.Utc);

            var rows = await db.Transactions
                .Where(t => t.UserId == userId && !t.IsDeleted && t.OccurredOn >= from)
                .Join(db.Categories.Where(c => c.UserId == userId),
                      t => t.CategoryId, c => c.Id, (t, c) => new { t, c })
                .GroupBy(x => new { x.t.OccurredOn.Year, x.t.OccurredOn.Month })
                .Select(g => new MonthlyReportItem(
                    g.Key.Year,
                    g.Key.Month,
                    g.Sum(x => x.c.Kind == 0 ? x.t.Amount : 0),
                    g.Sum(x => x.c.Kind == 1 ? x.t.Amount : 0)))
                .OrderBy(r => r.Year).ThenBy(r => r.Month)
                .ToListAsync();

            return Results.Ok(rows);
        });

        // Sức khỏe dữ liệu — công cụ kiểm tra
        group.MapGet("/health", async (ClaimsPrincipal principal, AppDbContext db) =>
        {
            var userId = principal.GetUserId();
            var mine = db.Transactions.Where(t => t.UserId == userId);

            var report = new DataHealthReport(
                TotalTransactions: await mine.CountAsync(),
                DeletedTransactions: await mine.CountAsync(t => t.IsDeleted),
                OrphanTransactions: await mine.CountAsync(t => !t.IsDeleted
                    && !db.Categories.Any(c => c.Id == t.CategoryId && c.UserId == userId)),
                TotalCategories: await db.Categories.CountAsync(c => c.UserId == userId && !c.IsDeleted),
                LastUpdatedAt: await mine.MaxAsync(t => (DateTime?)t.UpdatedAt));

            return Results.Ok(report);
        });
    }
}
```

Đăng ký trong `Program.cs`, **trước** `MapFallbackToFile`:

```csharp
app.MapReportEndpoints();
```

**Bản ghi mồ côi (orphan) là gì và vì sao phải đếm nó.** Đó là giao dịch trỏ tới một `CategoryId` không tồn tại trên server. Trong hệ thống offline-first, tình huống này xảy ra rất thật: máy A tạo danh mục mới rồi tạo giao dịch dùng nó, nhưng lúc sync chỉ đẩy được giao dịch còn danh mục thì thất bại giữa chừng. Kết quả là giao dịch hiển thị trống tên, trống màu — và nếu không có con số này thì bạn sẽ không bao giờ biết chuyện đó đã xảy ra.

> **Bài tập nâng cao:** làm cho `SyncService` ở Buổi 14 đẩy danh mục **trước**, giao dịch **sau**, trong cùng một lần gọi. Sau đó xem lại con số `OrphanTransactions` — nó phải luôn bằng 0.

### Bước 17.3: Donut chart bằng SVG

Tạo `src/Web/SoChi.Web/Components/DonutChart.razor`:

```razor
@using SoChi.Shared.Dtos

<svg viewBox="0 0 200 200" width="240" height="240">
    @{
        var total = Items.Sum(i => i.Total);
        double offset = 0;
    }

    @if (total == 0)
    {
        <circle cx="100" cy="100" r="70" fill="none" stroke="#E5E7EB" stroke-width="30" />
        <text x="100" y="105" text-anchor="middle" font-size="12" fill="#6B7280">Chưa có dữ liệu</text>
    }
    else
    {
        @foreach (var item in Items)
        {
            var fraction = (double)item.Total / total;
            var length = fraction * Circumference;
            var thisOffset = offset;
            offset += length;

            <circle cx="100" cy="100" r="@Radius"
                    fill="none"
                    stroke="@item.ColorHex"
                    stroke-width="30"
                    stroke-dasharray="@($"{length.ToString(Inv)} {(Circumference - length).ToString(Inv)}")"
                    stroke-dashoffset="@((-thisOffset).ToString(Inv))"
                    transform="rotate(-90 100 100)">
                <title>@item.CategoryName: @item.Total.ToString("N0") đ (@fraction.ToString("P1"))</title>
            </circle>
        }

        <text x="100" y="96"  text-anchor="middle" font-size="11" fill="#6B7280">Tổng chi</text>
        <text x="100" y="114" text-anchor="middle" font-size="16" font-weight="bold">
            @total.ToString("N0")
        </text>
    }
</svg>

@code {
    [Parameter, EditorRequired]
    public IReadOnlyList<CategoryReportItem> Items { get; set; } = [];

    private const double Radius = 70;
    private static readonly double Circumference = 2 * Math.PI * Radius;
    private static readonly System.Globalization.CultureInfo Inv =
        System.Globalization.CultureInfo.InvariantCulture;
}
```

Cơ chế `stroke-dasharray` đáng dừng lại để hiểu, vì nó là mẹo SVG kinh điển:

1. Vẽ **một đường tròn duy nhất** cho mỗi phần, bán kính giống nhau, viền dày 30.
2. `stroke-dasharray="A B"` biến viền thành nét đứt: vẽ `A` pixel, bỏ trống `B` pixel. Cho `A` = phần chu vi tương ứng với tỉ lệ, `B` = phần còn lại → chỉ hiện đúng một cung.
3. `stroke-dashoffset` đẩy cung đó tới vị trí bắt đầu của nó, bằng đúng tổng độ dài các cung trước.
4. `rotate(-90)` kéo điểm bắt đầu từ hướng 3 giờ về đỉnh — **cùng một chỉnh sửa bạn đã phải làm ở Buổi 07**, vì cả hai hệ đều lấy 0° ở hướng 3 giờ.

Thẻ `<title>` bên trong mỗi `<circle>` cho tooltip gốc của trình duyệt, không cần một dòng JavaScript nào.

> **Chi tiết dễ bỏ sót:** phải ép `InvariantCulture` khi đổi số sang chuỗi. Máy đặt tiếng Việt sẽ sinh ra `12,5` thay vì `12.5`, và SVG hiểu đó là hai giá trị khác nhau → biểu đồ vỡ. Đây đúng là loại lỗi "chỉ xảy ra trên máy của tôi" mà bạn nên gặp một lần cho nhớ.

### Bước 17.4: Bar chart 6 tháng

Tạo `src/Web/SoChi.Web/Components/BarChart.razor`:

```razor
@using SoChi.Shared.Dtos

<svg viewBox="0 0 360 200" width="100%" height="200">
    @{
        var max = Items.Count == 0 ? 1 : Items.Max(i => Math.Max(i.Expense, i.Income));
        if (max == 0) max = 1;

        var slot = 360.0 / Math.Max(Items.Count, 1);
        var barW = slot / 3;
    }

    <line x1="0" y1="170" x2="360" y2="170" stroke="#D1D5DB" stroke-width="1" />

    @for (var i = 0; i < Items.Count; i++)
    {
        var item = Items[i];
        var x = i * slot + slot / 2;

        var hExpense = item.Expense * 150.0 / max;
        var hIncome  = item.Income  * 150.0 / max;

        <rect x="@F(x - barW)" y="@F(170 - hExpense)" width="@F(barW - 2)" height="@F(hExpense)"
              fill="#EF4444" rx="2">
            <title>Tháng @item.Month: chi @item.Expense.ToString("N0") đ</title>
        </rect>

        <rect x="@F(x + 2)" y="@F(170 - hIncome)" width="@F(barW - 2)" height="@F(hIncome)"
              fill="#10B981" rx="2">
            <title>Tháng @item.Month: thu @item.Income.ToString("N0") đ</title>
        </rect>

        <text x="@F(x)" y="188" text-anchor="middle" font-size="10" fill="#6B7280">
            @item.Month/@(item.Year % 100)
        </text>
    }
</svg>

@code {
    [Parameter, EditorRequired]
    public IReadOnlyList<MonthlyReportItem> Items { get; set; } = [];

    private static string F(double v) =>
        v.ToString("0.##", System.Globalization.CultureInfo.InvariantCulture);
}
```

Phép tính `h = value * 150 / max` chính là **tỉ lệ trục tung** — đúng công thức bạn đã viết ở Buổi 08, chỉ khác là kết quả đi vào thuộc tính `height` của `<rect>` thay vì tham số của `canvas.FillRectangle`.

### Bước 17.5: Trang Thống kê

Tạo `src/Web/SoChi.Web/Pages/Statistics.razor`:

```razor
@page "/thong-ke"
@using System.Net.Http.Json
@using SoChi.Shared.Dtos
@using SoChi.Web.Components
@inject HttpClient Http

<h3>Thống kê</h3>

<div class="d-flex gap-2 mb-3">
    <select class="form-select w-auto" @bind="month" @bind:after="LoadAsync">
        @for (var m = 1; m <= 12; m++)
        {
            <option value="@m">Tháng @m</option>
        }
    </select>
    <input type="number" class="form-control w-auto" @bind="year" @bind:after="LoadAsync" />
</div>

@if (health is not null)
{
    <div class="row g-2 mb-4">
        <div class="col"><div class="border rounded p-2">
            <div class="text-muted small">Tổng giao dịch</div>
            <div class="fs-5">@health.TotalTransactions</div></div></div>
        <div class="col"><div class="border rounded p-2">
            <div class="text-muted small">Đã xóa mềm</div>
            <div class="fs-5">@health.DeletedTransactions</div></div></div>
        <div class="col"><div class="border rounded p-2 @(health.OrphanTransactions > 0 ? "border-danger" : "")">
            <div class="text-muted small">Mồ côi</div>
            <div class="fs-5 @(health.OrphanTransactions > 0 ? "text-danger" : "")">
                @health.OrphanTransactions</div></div></div>
        <div class="col"><div class="border rounded p-2">
            <div class="text-muted small">Sync gần nhất (UTC)</div>
            <div class="small">@(health.LastUpdatedAt?.ToString("dd/MM HH:mm:ss") ?? "—")</div></div></div>
    </div>
}

<div class="row">
    <div class="col-md-5"><DonutChart Items="byCategory" /></div>
    <div class="col-md-7"><BarChart Items="monthly" /></div>
</div>

<table class="table table-sm mt-4">
    @foreach (var item in byCategory)
    {
        <tr>
            <td style="width:24px">
                <span style="display:inline-block;width:14px;height:14px;border-radius:3px;
                             background:@item.ColorHex"></span>
            </td>
            <td>@item.CategoryName</td>
            <td class="text-muted">@item.Count giao dịch</td>
            <td class="text-end">@item.Total.ToString("N0") đ</td>
        </tr>
    }
</table>

@code {
    private List<CategoryReportItem> byCategory = [];
    private List<MonthlyReportItem> monthly = [];
    private DataHealthReport? health;

    private int year = DateTime.Now.Year;
    private int month = DateTime.Now.Month;

    protected override Task OnInitializedAsync() => LoadAsync();

    private async Task LoadAsync()
    {
        byCategory = await Http.GetFromJsonAsync<List<CategoryReportItem>>(
            $"/api/reports/by-category?year={year}&month={month}") ?? [];

        monthly = await Http.GetFromJsonAsync<List<MonthlyReportItem>>(
            "/api/reports/monthly?months=6") ?? [];

        health = await Http.GetFromJsonAsync<DataHealthReport>("/api/reports/health");
    }
}
```

### Bước 17.6: Dùng nó để kiểm chứng kịch bản sync của Buổi 15

Giờ mở lại kịch bản xung đột hai thiết bị ở Buổi 15 và làm lại, nhưng lần này **có thể nhìn thấy chuyện gì đang xảy ra**:

1. Máy A và máy B cùng offline, cùng sửa một giao dịch thành hai số tiền khác nhau.
2. Cho máy A online trước → mở trang **Kiểm tra**, ghi lại `UpdatedAt` của bản ghi đó.
3. Cho máy B online → F5 trang Kiểm tra.
4. Đối chiếu: `UpdatedAt` hiển thị phải là mốc **lớn hơn** trong hai mốc, và số tiền phải là của bản sửa sau. Nếu ngược lại, so sánh last-write-wins ở Bước 3.9 của Buổi 12 đang sai.
5. Kiểm tra thẻ **Mồ côi** trên trang Thống kê phải bằng 0. Khác 0 nghĩa là sync đang đẩy giao dịch trước danh mục.

Đây là điểm mà cả ba phần của dự án khớp lại: MAUI ghi dữ liệu, PostgreSQL lưu, và Blazor cho bạn **thấy** nó — bằng cùng một bộ DTO trong `src/Shared/SoChi.Shared`.

## 4. Bẫy thường gặp (Pitfalls)
- **Lỗi:** `The LINQ expression could not be translated` khi chạy endpoint báo cáo.
  - **Nguyên nhân:** Có phép tính mà PostgreSQL không hiểu nằm trong `GroupBy` hoặc `Select`. Cách sửa **sai** là chèn `.ToList()` trước `GroupBy` — làm vậy là kéo toàn bộ bảng về bộ nhớ rồi mới gộp, đúng cái điều buổi này muốn tránh. Cách đúng: đơn giản hóa biểu thức, hoặc dùng `.Select()` lấy trước các cột cần rồi mới nhóm.
- **Lỗi:** Gộp theo tháng cho kết quả lệch, giao dịch ngày cuối tháng nhảy sang tháng sau.
  - **Nguyên nhân:** `OccurredOn` bị ánh xạ thành `timestamptz` nên `Year`/`Month` được tính theo UTC — giao dịch lúc 23h ngày 31 giờ Việt Nam là 16h ngày 31 UTC, nhưng lúc 07h ngày 1 lại thành ngày 31 UTC. Khắc phục: giữ đúng ánh xạ `HasColumnType("date")` cho `OccurredOn` như đã ghi ở Buổi 12 — kiểu `date` không có múi giờ nên không thể lệch.
- **Lỗi:** Biểu đồ SVG trống trơn hoặc méo mó, F12 báo lỗi thuộc tính.
  - **Nguyên nhân:** Số thực bị định dạng theo culture tiếng Việt (`12,5`). Luôn `ToString(..., CultureInfo.InvariantCulture)` cho mọi giá trị đi vào thuộc tính SVG.
- **Lỗi:** Trang gọi API lặp vô hạn, tab trình duyệt đơ.
  - **Nguyên nhân:** Gọi API trong `OnParametersSet` hoặc `OnAfterRender` mà không có điều kiện dừng — mỗi lần render lại kích hoạt một lượt gọi mới. Tải dữ liệu trong `OnInitializedAsync`, hoặc trong `OnAfterRenderAsync(bool firstRender)` với kiểm tra `if (!firstRender) return;`.
- **Lỗi:** Số tiền lớn hiển thị sai ở web nhưng đúng ở MAUI.
  - **Nguyên nhân:** Đây là bẫy quen thuộc của JavaScript — số nguyên vượt 2^53 bị mất chính xác. **Blazor WASM không dính lỗi này** vì `long` vẫn là `long` trong .NET chạy trên WebAssembly. Nếu bạn thấy sai, nguyên nhân nằm ở chỗ khác (định dạng hoặc phép tính), không phải ở kiểu dữ liệu. Ghi nhớ điều này: đó là một lợi thế thật của Blazor so với SPA viết bằng JavaScript khi làm ứng dụng tài chính.

## 5. Checklist nghiệm thu Buổi 17
- [ ] `/api/reports/by-category?year=2026&month=9` trả về JSON đúng, và log của EF Core cho thấy một câu `SELECT ... GROUP BY ...` — **không** phải kéo hết bảng về rồi mới gộp.
- [ ] Donut chart hiện đúng tỉ lệ, tổng các cung khép kín thành vòng tròn, di chuột thấy tooltip tên danh mục và phần trăm.
- [ ] Bar chart hiện 6 tháng, cột chi (đỏ) và thu (xanh) đứng cạnh nhau, tháng cao nhất chạm gần đỉnh khung.
- [ ] Đổi tháng trong ô chọn → cả hai biểu đồ cập nhật, không phải F5.
- [ ] Thẻ **Mồ côi** hiển thị `0`. Nếu khác 0, tìm ra nguyên nhân trước khi kết thúc buổi.
- [ ] Chạy lại kịch bản xung đột hai thiết bị ở Bước 17.6 → bản ghi có `UpdatedAt` lớn hơn thắng, xác nhận bằng mắt trên trang Kiểm tra.
- [ ] Đặt máy sang ngôn ngữ tiếng Việt (dấu phẩy thập phân) → biểu đồ vẫn vẽ đúng.

---

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

# PHỤ LỤC B: Nhật Ký Học Tập

Tạo file `docs/NHAT-KY-HOC.md` và ghi vài dòng sau mỗi buổi, theo đúng bốn mục này:

```markdown
## Buổi 07 — Donut Chart (ngày 15/09/2026)

**Đã làm được:** Vẽ xong donut, chú thích hiển thị đúng, chạy ổn cả theme sáng và tối.

**Mắc ở đâu:** Cung vẽ ngược chiều kim đồng hồ và lệch 90 độ so với hình dung.

**Vì sao:** Trong `Microsoft.Maui.Graphics`, góc 0 độ nằm ở hướng 3 giờ và tăng theo
chiều ngược kim đồng hồ — ngược với trực giác. Phải bắt đầu từ 90 độ và trừ dần.

**Rút ra:** Trước khi code đồ họa, vẽ tay hệ trục ra giấy. Đoán hệ tọa độ tốn của mình 1 tiếng.
```

Ba lý do việc này đáng làm, dù cảm giác như mất thời gian:

- Mục **"Vì sao"** ép bạn thật sự hiểu nguyên nhân, thay vì sửa mò cho đến khi chạy được rồi quên sạch.
- Nó là bằng chứng cụ thể về năng lực khi phỏng vấn — kể được một lỗi và cách bạn truy ra gốc rễ có sức nặng hơn nhiều so với liệt kê "biết .NET MAUI".
- Ba tháng sau gặp lại đúng lỗi đó, bạn tra được trong 30 giây.

---

## 🎯 Lời Khuyên Hoàn Thành Khóa Học

1. **Tuân thủ đúng thứ tự 15 buổi:** Không nhảy cóc sang buổi sau khi buổi trước chưa tick đủ checklist nghiệm thu.
2. **Commit Git sau mỗi buổi:** Mỗi buổi hoàn thành tương ứng với một commit sạch sẽ có thông điệp rõ ràng (`feat(session-05): implement grouped collectionview with swipe actions`).
3. **Thực tế hóa dữ liệu:** Nhập chính chi tiêu hàng ngày của bạn vào ứng dụng SoChi để trải nghiệm những điểm bất tiện về UX và tự tay hoàn thiện nó. Chúc bạn làm chủ hoàn toàn .NET MAUI!
