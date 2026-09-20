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

🏠 [Mục lục toàn khóa](../HUONG-DAN-TUNG-BUOI.md)

[BUỔI 01: Khởi Tạo Solution, Git, Clean Architecture & Khởi Động MVVM](buoi-01.md) ➡️
