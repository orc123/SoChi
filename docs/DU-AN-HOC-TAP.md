# Dự án học tập: **SoChi** — Sổ chi tiêu cá nhân (offline-first)

> Tài liệu này là **đề bài**, không phải lời giải. Không có code mẫu — chỉ có yêu cầu, tiêu chí hoàn thành, và từ khóa để bạn tự tra cứu. Mỗi chặng làm xong phải **chạy được trên máy thật** rồi mới sang chặng sau.

---

## 1. Tại sao là dự án này?

Bạn muốn học MAUI là chính. Một app học tập tốt phải ép bạn chạm vào **đủ các mảng của MAUI**, chứ không chỉ là màn hình danh sách.

| Mảng của MAUI | Nơi dự án này bắt bạn dùng |
|---|---|
| Danh sách hiệu năng cao | Danh sách giao dịch dài, có nhóm theo ngày, vuốt để xóa |
| Form + validation | Màn hình nhập giao dịch |
| Điều hướng Shell | Tab bar, truyền tham số, modal |
| Dữ liệu cục bộ | SQLite trên máy, app phải chạy khi mất mạng |
| Vẽ đồ họa | Biểu đồ donut theo danh mục, tự vẽ bằng `Microsoft.Maui.Graphics` |
| API thiết bị | Chụp ảnh hóa đơn, đọc trạng thái mạng, rung phản hồi, chia sẻ file |
| Theme & style | Sáng/tối, đổi màu chủ đạo |
| Custom control | Bàn phím số nhập tiền (không dùng `Entry` mặc định) |
| Đồng bộ với backend | Sync SQLite ↔ Web API, xử lý xung đột |
| Đóng gói | Xuất APK Android, MSIX Windows |

Ngoài ra nó có **dữ liệu thật của chính bạn** → bạn sẽ thực sự dùng app, và tự phát hiện ra thiếu tính năng gì. Đó là động lực học tốt hơn nhiều so với app demo.

**Hai ý tưởng dự phòng** nếu bạn không thích chủ đề chi tiêu (spec bên dưới vẫn áp dụng được gần như nguyên vẹn, chỉ đổi domain):

- **Sổ tập gym** — buổi tập, bài tập, set/rep/tạ, biểu đồ tiến bộ, hẹn giờ nghỉ giữa set (thêm được phần background timer).
- **Nhật ký đọc sách** — sách, tiến độ trang, ghi chú theo trang, quét ISBN bằng camera.

---

## 2. Phạm vi — app làm được gì

**Bản thân người dùng ghi lại thu/chi hằng ngày, xem được tiền đi đâu, và dữ liệu không mất khi đổi máy.**

Có:
- Ghi giao dịch (chi hoặc thu): số tiền, danh mục, ngày, ghi chú, ảnh hóa đơn (tùy chọn)
- Quản lý danh mục (tên, icon, màu)
- Xem theo tháng: tổng chi, tổng thu, còn lại
- Biểu đồ tỉ trọng theo danh mục
- Đặt hạn mức chi cho từng danh mục, cảnh báo khi vượt
- Tìm kiếm và lọc
- Hoạt động **hoàn toàn offline**; khi có mạng thì đồng bộ lên server
- Xuất CSV để chia sẻ

Không có (đừng để scope phình ra):
- Nhiều người dùng chung một sổ
- Đa tiền tệ, tỉ giá
- Kết nối ngân hàng
- Giao dịch định kỳ tự động (có thể thêm ở chặng mở rộng)

---

## 3. Các màn hình

| # | Màn hình | Nội dung chính |
|---|---|---|
| 1 | **Tổng quan** (tab) | Thẻ tóm tắt tháng hiện tại (chi/thu/còn lại), biểu đồ donut theo danh mục, 5 giao dịch gần nhất, nút thêm nổi |
| 2 | **Giao dịch** (tab) | Danh sách toàn bộ, **nhóm theo ngày**, header nhóm hiện tổng của ngày; vuốt trái để xóa, chạm để sửa; ô tìm kiếm; bộ lọc theo tháng/danh mục/loại |
| 3 | **Thêm/Sửa giao dịch** (modal) | Bàn phím số tự làm, chọn loại chi/thu, chọn danh mục dạng lưới icon, chọn ngày, ghi chú, đính ảnh hóa đơn |
| 4 | **Danh mục** | Danh sách danh mục, thêm/sửa/xóa, chọn icon + màu, đặt hạn mức tháng |
| 5 | **Thống kê** (tab) | Chọn khoảng thời gian, biểu đồ cột 6 tháng gần nhất, xếp hạng danh mục chi nhiều nhất, so sánh với tháng trước |
| 6 | **Cài đặt** (tab) | Theme sáng/tối/theo hệ thống, màu chủ đạo, khóa app bằng vân tay, xuất CSV, trạng thái đồng bộ, xóa toàn bộ dữ liệu |

Ràng buộc thiết kế bắt buộc: **thêm một giao dịch phải xong trong dưới 5 giây và không quá 4 lần chạm.** Đây là ràng buộc thật của app tài chính, và nó sẽ ép bạn suy nghĩ về UX chứ không chỉ về code.

---

## 4. Mô hình dữ liệu

Tự thiết kế bảng, nhưng phải có đủ các trường sau (tên tùy bạn):

**Category**
- `Id` (Guid — xem ghi chú bên dưới), `Name`, `IconGlyph`, `ColorHex`, `Kind` (Expense/Income), `MonthlyLimit` (nullable)

**Transaction**
- `Id` (Guid), `CategoryId`, `Amount` (số nguyên, đơn vị **đồng** — xem ghi chú), `OccurredOn` (ngày), `Note`, `ReceiptPath` (nullable)

**Trường phục vụ đồng bộ** (thêm vào cả hai bảng ngay từ đầu, đừng để sau mới nhét vào):
- `UpdatedAt` (UTC), `IsDeleted` (bool), `SyncState` (Local/Synced)

Ba quyết định thiết kế bạn phải hiểu **lý do**, không chép máy móc:

1. **Khóa chính là `Guid`, không phải `int` tự tăng.** Vì client tạo bản ghi khi offline — nếu để server cấp id, hai máy sẽ đụng id nhau. Guid cho phép client tự sinh id an toàn.
2. **Tiền lưu dạng số nguyên (đồng), không dùng `double`.** `double` là số thực nhị phân, `0.1 + 0.2 != 0.3`. Tiền bạc dùng `double` là sai nguyên tắc. `decimal` cũng được, nhưng số nguyên đơn giản hơn cho VND vốn không có phần lẻ.
3. **Xóa mềm (`IsDeleted`) thay vì xóa thật.** Xóa thật khi offline thì lúc sync server không biết bản ghi đó "đã bị xóa" hay "chưa từng tồn tại".

---

## 5. Lộ trình 8 chặng

Mỗi chặng có **Mục tiêu → Việc phải làm → Tiêu chí hoàn thành → Từ khóa tự tra**. Đừng nhảy cóc; mỗi chặng đều dựa vào chặng trước.

---

### Chặng 0 — Dựng khung (nửa buổi)

**Mục tiêu:** có solution build xanh, chạy được app rỗng trên Windows.

**Việc phải làm**
- Tạo solution theo cấu trúc trong [HUONG-DAN-PROJECT.md](HUONG-DAN-PROJECT.md) §2 (tạm thời chỉ cần `Client` và `Shared`, `Api` để đến Chặng 7)
- Thêm `global.json`, `Directory.Build.props`, `.gitignore`
- `git init` và commit đầu tiên

**Tiêu chí hoàn thành**
- `dotnet build` xanh, không warning
- App chạy trên Windows, hiện một `Label` "SoChi"

---

### Chặng 1 — Shell, điều hướng, MVVM (1–2 buổi)

**Mục tiêu:** đủ 5 tab, chuyển qua lại được, mỗi tab một ViewModel rỗng đã được DI resolve.

**Việc phải làm**
- Cài `CommunityToolkit.Mvvm`
- Dựng `AppShell` với `TabBar` 5 tab
- Mỗi tab: một `ContentPage` + một ViewModel, **cả hai đăng ký trong `MauiProgram`**
- ViewModel nhận dữ liệu qua constructor, Page nhận ViewModel qua constructor, gán vào `BindingContext`
- Đăng ký route cho màn hình Thêm/Sửa, mở nó dạng **modal**

**Tiêu chí hoàn thành**
- Không có chỗ nào `new ViewModel()` bằng tay trong code-behind
- Chuyển tab không mất trạng thái
- Mở được modal Thêm giao dịch và đóng lại

**Từ khóa tự tra:** `Shell TabBar`, `Routing.RegisterRoute`, `GoToAsync modal`, `ObservableObject`, `ObservableProperty`, `RelayCommand`, `AddTransient vs AddSingleton MAUI`

**Điểm dễ sai:** quên đăng ký Page trong DI → Shell ném exception lúc navigate chứ không phải lúc build.

---

### Chặng 2 — Dữ liệu cục bộ với SQLite (2 buổi)

**Mục tiêu:** thêm/sửa/xóa giao dịch thật, dữ liệu còn nguyên sau khi tắt app.

**Việc phải làm**
- Cài `sqlite-net-pcl` (hoặc EF Core SQLite — chọn một, `sqlite-net-pcl` nhẹ và hợp cho học)
- Đặt file db ở `FileSystem.AppDataDirectory`
- Viết `IRepository` cho Category và Transaction, đăng ký DI dạng `Singleton`
- Seed sẵn ~8 danh mục mặc định lần chạy đầu
- Màn hình Giao dịch hiển thị dữ liệu thật bằng `CollectionView`
- Màn hình Thêm/Sửa lưu được vào db

**Tiêu chí hoàn thành**
- Thêm giao dịch → tắt app → mở lại → giao dịch vẫn còn
- Không có `Thread.Sleep`, không có `.Result` / `.Wait()` (deadlock chờ bạn ở đó)

**Từ khóa tự tra:** `sqlite-net-pcl CreateTableAsync`, `FileSystem.AppDataDirectory`, `CollectionView ItemsSource`, `ObservableCollection`, `async void vs async Task`

**Điểm dễ sai:** khởi tạo db trong constructor của repository (constructor không `async` được). Tra cách làm "lazy async initialization".

---

### Chặng 3 — Danh sách nâng cao & bàn phím số (2 buổi)

**Mục tiêu:** giao diện bắt đầu giống app thật.

**Việc phải làm**
- `CollectionView` **nhóm theo ngày**, header nhóm hiện ngày + tổng tiền của ngày đó
- Vuốt trái để xóa (`SwipeView`), có xác nhận
- Kéo xuống để làm mới
- Trạng thái rỗng ("Chưa có giao dịch nào") thay vì màn hình trắng
- **Tự làm bàn phím số** cho ô nhập tiền: `Grid` các nút 0–9, nút xóa, nút "000". Không dùng `Entry` với `Keyboard.Numeric`
- Số tiền hiển thị có phân cách nghìn ngay khi gõ (`1.250.000`)
- Converter: số → chuỗi tiền tệ, `Kind` → màu (chi đỏ, thu xanh)

**Tiêu chí hoàn thành**
- Cuộn danh sách 500 bản ghi vẫn mượt
- Nhập 250000 hiện ra `250.000 ₫`
- Xóa nhầm có thể hoàn tác (snackbar "Hoàn tác") — tra `CommunityToolkit.Maui` Snackbar

**Từ khóa tự tra:** `CollectionView IsGrouped`, `GroupHeaderTemplate`, `SwipeView`, `RefreshView`, `EmptyView`, `IValueConverter`, `CommunityToolkit.Maui Snackbar`

**Điểm dễ sai:** nhóm dữ liệu cần một collection lồng (`ObservableCollection<Grouping<...>>`), không phải list phẳng. Đây là chỗ tốn thời gian nhất chặng này.

---

### Chặng 4 — Vẽ biểu đồ bằng tay (1–2 buổi)

**Mục tiêu:** hiểu `Microsoft.Maui.Graphics` — thứ nằm dưới mọi thư viện chart.

**Việc phải làm**
- **Không cài thư viện chart.** Tự vẽ.
- Biểu đồ donut trên màn hình Tổng quan: mỗi danh mục một cung, màu lấy từ `ColorHex`, giữa donut ghi tổng chi
- Biểu đồ cột 6 tháng trên màn hình Thống kê
- Cả hai phải đúng trong cả theme sáng và tối

**Tiêu chí hoàn thành**
- Biểu đồ vẽ đúng khi dữ liệu thay đổi (thêm giao dịch → cập nhật ngay)
- Xoay ngang màn hình không vỡ layout

**Từ khóa tự tra:** `GraphicsView`, `IDrawable`, `ICanvas DrawArc`, `Invalidate()`, `BindableProperty` (để truyền dữ liệu từ XAML vào drawable)

**Ghi chú:** chặng này khó hơn vẻ ngoài — tính toán góc cung và chú thích tốn thời gian. Nhưng làm xong bạn sẽ tự tin viết được bất kỳ custom control nào.

---

### Chặng 5 — API thiết bị & giao diện hoàn thiện (2 buổi)

**Mục tiêu:** chạm vào phần "native" của MAUI.

**Việc phải làm**
- Chụp / chọn ảnh hóa đơn, lưu vào thư mục app, hiện thumbnail trong chi tiết giao dịch
- Theme sáng/tối/theo hệ thống, lưu lựa chọn bằng `Preferences`
- Khóa app bằng vân tay/khuôn mặt (tùy chọn bật trong Cài đặt)
- Xuất CSV rồi mở hộp thoại chia sẻ
- Rung nhẹ khi bấm nút bàn phím số
- Định dạng tiền và ngày theo `CultureInfo` `vi-VN`

**Tiêu chí hoàn thành**
- Chạy trên **Android** (không chỉ Windows) — đây là lần đầu bạn bắt buộc phải mở emulator
- Xin quyền camera đúng cách: từ chối quyền thì app hiện thông báo tử tế, không crash

**Từ khóa tự tra:** `MediaPicker.CapturePhotoAsync`, `Permissions.RequestAsync`, `AppThemeBinding`, `Application.Current.UserAppTheme`, `Preferences`, `SecureStorage`, `Share.RequestAsync`, `HapticFeedback`, `AndroidManifest permission`

**Điểm dễ sai:** quyền phải khai báo **cả** trong `AndroidManifest.xml` **và** xin lúc runtime. Thiếu một trong hai là hỏng.

---

### Chặng 6 — Hạn mức, lọc, tìm kiếm (1 buổi)

**Mục tiêu:** logic nghiệp vụ thật, và test được nó.

**Việc phải làm**
- Đặt hạn mức tháng cho danh mục; thanh tiến trình đổi màu khi vượt 80% / 100%
- Lọc theo khoảng ngày, danh mục, loại; kết hợp nhiều điều kiện
- Tìm kiếm theo ghi chú, có debounce (đừng query mỗi lần gõ một ký tự)
- Tạo project `tests/Client.Tests`, viết test cho phần tính toán tổng/hạn mức và cho ViewModel

**Tiêu chí hoàn thành**
- `dotnet test` xanh, tối thiểu 8 test có ý nghĩa
- Test ViewModel chạy được mà **không cần dựng UI** (nếu không được → ViewModel của bạn đang dính vào UI, phải tách ra)

**Từ khóa tự tra:** `xUnit MAUI viewmodel test`, `debounce CancellationToken`, `ProgressBar`, `IQueryAttributable` (nhận tham số điều hướng)

---

### Chặng 7 — Backend & đồng bộ (3–4 buổi, khó nhất)

**Mục tiêu:** dữ liệu sống sót khi đổi máy. Đây là chặng dạy nhiều nhất về kiến trúc.

**Việc phải làm**
- Tạo project `src/Api`: đăng nhập đơn giản (email + mật khẩu → JWT), CRUD giao dịch/danh mục, SQLite hoặc PostgreSQL
- Chuyển toàn bộ DTO sang `src/Shared`, cả hai phía tham chiếu
- Client: màn hình đăng nhập, lưu token trong `SecureStorage`
- **Đồng bộ hai chiều**: đẩy bản ghi `SyncState = Local` lên, kéo về bản ghi có `UpdatedAt` mới hơn lần sync trước
- Xử lý xung đột theo quy tắc **last-write-wins dựa trên `UpdatedAt`** (đơn giản, và bạn phải hiểu điểm yếu của nó)
- Theo dõi `Connectivity` — mất mạng thì app vẫn dùng bình thường, có mạng lại thì tự sync
- Chỉ báo trạng thái sync trên màn hình Cài đặt

**Tiêu chí hoàn thành** — kịch bản này phải chạy đúng:
1. Bật chế độ máy bay, thêm 3 giao dịch → app hoạt động bình thường
2. Tắt chế độ máy bay → 3 giao dịch tự lên server
3. Cài app trên máy/emulator thứ hai, đăng nhập cùng tài khoản → thấy đủ 3 giao dịch
4. Sửa cùng một giao dịch ở hai máy khi cả hai offline → sau khi cả hai online, bản sửa sau thắng, **không mất dữ liệu, không nhân bản**

**Từ khóa tự tra:** `JWT bearer ASP.NET Core`, `WebApplicationFactory integration test`, `Connectivity.NetworkAccess`, `DelegatingHandler token`, `Polly retry`, `last-write-wins sync`, `10.0.2.2 android emulator` (xem [HUONG-DAN-PROJECT.md](HUONG-DAN-PROJECT.md) §9)

**Cảnh báo:** đây là chặng dễ bỏ cuộc. Nếu bí, hãy làm sync **một chiều** trước (chỉ đẩy lên), chạy được rồi mới làm chiều kéo về.

---

### Chặng 8 — Đóng gói (1 buổi)

**Mục tiêu:** app rời khỏi máy dev.

**Việc phải làm**
- Icon và splash screen thật (không dùng mặc định)
- Tạo keystore, build APK/AAB Release cho Android, cài lên điện thoại thật
- Build MSIX cho Windows
- Bật `TrimMode` / AOT cho Release, đo lại kích thước và tốc độ khởi động
- Kiểm tra: chắc chắn đoạn bỏ qua chứng chỉ HTTPS **không** có trong bản Release

**Tiêu chí hoàn thành**
- APK cài được lên điện thoại thật và gọi API thật thành công
- Khởi động lạnh dưới 3 giây

**Từ khóa tự tra:** `dotnet publish -f net10.0-android -c Release`, `AndroidKeyStore`, `MauiIcon MauiSplashScreen`, `PublishTrimmed`

---

## 6. Quy tắc tự đặt cho mình khi làm

Những quy tắc này quan trọng ngang với code — chúng quyết định bạn học được gì.

1. **Chạy được rồi mới đẹp.** Mỗi chặng làm cho nó *hoạt động* trước, tinh chỉnh giao diện sau.
2. **Commit theo chặng**, message rõ ràng. Nhìn lại lịch sử git chính là nhìn lại quá trình học.
3. **Bí quá 30 phút thì tra tài liệu chính thức trước**, blog/StackOverflow sau. Docs của MAUI tốt hơn bạn nghĩ.
4. **Đừng cài thư viện để né việc học.** Chart tự vẽ (Chặng 4). Sau khi tự vẽ xong, cài `LiveCharts` cũng được — nhưng phải sau.
5. **Test trên Android sớm**, đừng để đến cuối. Nhiều thứ chạy ngon trên Windows nhưng vỡ trên Android (permission, layout, font, HTTPS).
6. **Ghi nhật ký học tập.** Mỗi chặng viết vài dòng vào một file: cái gì khó, tại sao, giải quyết ra sao. Ba tháng sau bạn sẽ cảm ơn chính mình.
7. **Dùng app thật ít nhất một tuần** trước khi coi là xong. Bạn sẽ tự thấy chỗ nào khó dùng.

---

## 7. Mở rộng khi đã xong (chọn cái bạn thích)

- **Web dashboard bằng Blazor WebAssembly** — thống kê và soi dữ liệu trên server, dùng lại đúng DTO trong `Shared`. Phần này đã được viết thành hướng dẫn đầy đủ ở Buổi 16–17.
- Widget màn hình chính Android hiển thị chi tiêu hôm nay
- Thông báo nhắc ghi chi tiêu lúc 21h hằng ngày
- Giao dịch định kỳ tự sinh (tiền nhà, cước mạng)
- Nhập sao kê CSV từ ngân hàng
- Tìm kiếm toàn văn bằng SQLite FTS5
- Sync dùng CRDT thay cho last-write-wins (nâng cao thật sự)
- Đa ngôn ngữ Việt/Anh bằng `.resx`

---

## 8. Ước lượng thời gian

| Chặng | Buổi (~3h) |
|---|---|
| 0 — Khung | 0.5 |
| 1 — Shell + MVVM | 1.5 |
| 2 — SQLite | 2 |
| 3 — Danh sách + bàn phím số | 2 |
| 4 — Biểu đồ tự vẽ | 1.5 |
| 5 — API thiết bị | 2 |
| 6 — Nghiệp vụ + test | 1 |
| 7 — Backend + sync | 3.5 |
| 8 — Đóng gói | 1 |
| **Tổng** | **~15 buổi** |

Học đều 3 buổi/tuần thì khoảng **5 tuần**. Chậm hơn cũng không sao — làm kỹ từng chặng quan trọng hơn về đích sớm.
