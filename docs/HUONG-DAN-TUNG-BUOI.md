# Hướng Dẫn Chi Tiết Từng Buổi Học: Dự Án SoChi (19 Buổi + 2 Phụ Lục)

> Tài liệu thực hành chi tiết (Lab Guide & Syllabus) đồng hành cùng [DU-AN-HOC-TAP.md](DU-AN-HOC-TAP.md) và [HUONG-DAN-PROJECT.md](HUONG-DAN-PROJECT.md).  
> Thiết kế theo mô hình **15 buổi thực hành (~3h/buổi)**, chia làm **8 chặng**, cộng thêm **2 buổi bổ sung** (Buổi 00 chuẩn bị môi trường, Buổi 04B quản lý danh mục), **Chặng 9 mở rộng** gồm 2 buổi Blazor WebAssembly (Buổi 16–17), và **2 phụ lục** áp dụng xuyên suốt ở cuối tài liệu. Mỗi buổi học cung cấp mục tiêu rõ ràng, nguyên lý kiến trúc, hướng dẫn từng bước (code mẫu thực tế), các bẫy thường gặp và checklist nghiệm thu trên máy thật.

---

## 📑 Bảng Lộ Trình 19 Buổi

| Buổi | Chặng | Tên buổi học & Trọng tâm kỹ thuật | Sản phẩm đạt được sau 3h |
|:---:|:---:|---|---|
| [**00**](buoi/buoi-00.md) | **Chuẩn bị** | Chuẩn bị môi trường, chuẩn hóa cấu trúc thư mục & cấu hình toàn cục | Toolchain sẵn sàng, emulator chạy được, layout `src/` chuẩn, commit Git đầu tiên |
| [**01**](buoi/buoi-01.md) | **Chặng 0 & 1** | Khởi tạo Solution, Git, Clean Architecture & Khởi động MVVM | Solution chuẩn 3 tầng, build xanh, chạy app rỗng trên Windows |
| [**02**](buoi/buoi-02.md) | **Chặng 1** | AppShell 5 Tabs, Modal Navigation & Dependency Injection | Điều hướng 5 tabs, mở modal Thêm giao dịch, DI tự động |
| [**03**](buoi/buoi-03.md) | **Chặng 2** | Mô hình dữ liệu SQLite, Repository Pattern & Lazy Init | SQLite sẵn sàng, Data Model chuẩn Guid + VND + Sync State |
| [**04**](buoi/buoi-04.md) | **Chặng 2** | Seed Data, Form Thêm/Sửa & Hiển thị CollectionView | Nhập giao dịch -> Lưu SQLite -> Tắt mở lại app vẫn còn |
| [**04B**](buoi/buoi-04b.md) | **Chặng 2** | Quản lý Danh mục: CRUD, chọn icon/màu, đặt hạn mức tháng | Tab Danh mục hoạt động đầy đủ, có nguồn nhập `MonthlyLimit` cho Buổi 11 |
| [**05**](buoi/buoi-05.md) | **Chặng 3** | Danh sách nâng cao: Grouping theo ngày, SwipeView & RefreshView | Danh sách nhóm ngày, tính tổng ngày, vuốt xóa, kéo làm mới |
| [**06**](buoi/buoi-06.md) | **Chặng 3** | Custom Numeric Keypad, Format VND Realtime & Snackbar Hoàn tác | Bàn phím số tự chế không dùng Entry, format tiền, Undo xóa |
| [**07**](buoi/buoi-07.md) | **Chặng 4** | Đồ họa 2D thuần: Donut Chart tự vẽ với Microsoft.Maui.Graphics | Biểu đồ tròn tỉ trọng chi tiêu tự vẽ, hỗ trợ Dark/Light mode |
| [**08**](buoi/buoi-08.md) | **Chặng 4** | Biểu đồ cột 6 tháng (Bar Chart), Responsive & Xoay màn hình | Biểu đồ cột doanh thu/chi tiêu, tính toán trục tọa độ tự động |
| [**09**](buoi/buoi-09.md) | **Chặng 5** | API thiết bị: Chụp/Chọn ảnh hóa đơn, FileSystem & Phân quyền | Chụp ảnh hóa đơn, lưu AppData, thumbnail, xin quyền Android |
| [**10**](buoi/buoi-10.md) | **Chặng 5** | Theme sáng/tối, Sinh trắc học vân tay, Haptic & Xuất file CSV | Đổi theme tức thì, bảo mật vân tay, rung phím, chia sẻ CSV |
| [**11**](buoi/buoi-11.md) | **Chặng 6** | Hạn mức chi tiêu (ProgressBar), Lọc, Debounce & xUnit Tests | Đặt hạn mức đổi màu, tìm kiếm debounce, tối thiểu 8 unit test xanh |
| [**12**](buoi/buoi-12.md) | **Chặng 7** | Backend ASP.NET Core Web API, Shared DTOs & JWT Auth | Backend Web API chạy ngon lành, cấp JWT token, PostgreSQL + EF Core |
| [**13**](buoi/buoi-13.md) | **Chặng 7** | Client tích hợp API, DelegatingHandler, SecureStorage & 10.0.2.2 | Client đăng nhập/đăng ký thành công từ cả Windows & Android Emulator |
| [**14**](buoi/buoi-14.md) | **Chặng 7** | Offline-First Sync hai chiều, Connectivity & Last-Write-Wins | Chế độ máy bay ghi nhận bình thường, có mạng tự sync không đè mất |
| [**15**](buoi/buoi-15.md) | **Chặng 7 & 8** | Kiểm thử kịch bản Sync thực tế & Đóng gói Release (APK/MSIX) | File APK cài máy thật, MSIX Windows, tối ưu Trim/AOT |
| [**16**](buoi/buoi-16.md) | **Chặng 9** | Blazor WebAssembly: dựng Web Dashboard, JWT Auth & công cụ kiểm tra dữ liệu | Web app phục vụ từ chính API, đăng nhập được, xem dữ liệu thô trên server |
| [**17**](buoi/buoi-17.md) | **Chặng 9** | Endpoint báo cáo, biểu đồ SVG tự vẽ & kiểm chứng sức khỏe dữ liệu | Trang thống kê có donut + bar chart, phát hiện bản ghi mồ côi, soi được xung đột sync |

---

---

## 📂 Tài Liệu Từng Buổi

Nội dung chi tiết của mỗi buổi được tách thành file riêng trong thư mục [`docs/buoi/`](buoi/):

- [BUỔI 00: Chuẩn Bị Môi Trường & Chuẩn Hóa Cấu Trúc Thư Mục](buoi/buoi-00.md)
- [BUỔI 01: Khởi Tạo Solution, Git, Clean Architecture & Khởi Động MVVM](buoi/buoi-01.md)
- [BUỔI 02: AppShell 5 Tabs, Modal Navigation & Dependency Injection](buoi/buoi-02.md)
- [BUỔI 03: Mô Hình Dữ Liệu SQLite, Repository Pattern & Lazy Init](buoi/buoi-03.md)
- [BUỔI 04: Seed Data, Form Thêm/Sửa & Hiển Thị CollectionView](buoi/buoi-04.md)
- [BUỔI 04B: Quản Lý Danh Mục — CRUD, Chọn Icon/Màu & Đặt Hạn Mức](buoi/buoi-04b.md)
- [BUỔI 05: Danh Sách Nâng Cao: Grouping Theo Ngày, SwipeView & RefreshView](buoi/buoi-05.md)
- [BUỔI 06: Custom Numeric Keypad, Format VND Realtime & Snackbar Hoàn Tác](buoi/buoi-06.md)
- [BUỔI 07: Đồ Họa 2D Thuần: Donut Chart Tự Vẽ Với Microsoft.Maui.Graphics](buoi/buoi-07.md)
- [BUỔI 08: Biểu Đồ Cột 6 Tháng (Bar Chart), Responsive & Xoay Màn Hình](buoi/buoi-08.md)
- [BUỔI 09: API Thiết Bị: Chụp/Chọn Ảnh Hóa Đơn, FileSystem & Phân Quyền](buoi/buoi-09.md)
- [BUỔI 10: Theme Sáng/Tối, Sinh Trắc Học Vân Tay, Haptic & Xuất File CSV](buoi/buoi-10.md)
- [BUỔI 11: Hạn Mức Chi Tiêu (ProgressBar), Lọc, Debounce & xUnit Tests](buoi/buoi-11.md)
- [BUỔI 12: Backend ASP.NET Core Web API, PostgreSQL, Shared DTOs & JWT Auth](buoi/buoi-12.md)
- [BUỔI 13: Client Tích Hợp API, DelegatingHandler, SecureStorage & 10.0.2.2](buoi/buoi-13.md)
- [BUỔI 14: Offline-First Sync Hai Chiều, Connectivity & Last-Write-Wins](buoi/buoi-14.md)
- [BUỔI 15: Kiểm Thử Kịch Bản Sync Thực Tế & Đóng Gói Release (APK/MSIX)](buoi/buoi-15.md)
- [BUỔI 16: Blazor WebAssembly — Dựng Web Dashboard, JWT Auth & Công Cụ Kiểm Tra Dữ Liệu](buoi/buoi-16.md)
- [BUỔI 17: Endpoint Báo Cáo, Biểu Đồ SVG Tự Vẽ & Kiểm Chứng Sức Khỏe Dữ Liệu](buoi/buoi-17.md)
- [PHỤ LỤC A: Trạng Thái Loading, Lỗi & Offline (áp dụng xuyên suốt)](buoi/phu-luc-a.md)
- [PHỤ LỤC B: Nhật Ký Học Tập](buoi/phu-luc-b.md)

---

## 🎯 Lời Khuyên Hoàn Thành Khóa Học

1. **Tuân thủ đúng thứ tự 15 buổi:** Không nhảy cóc sang buổi sau khi buổi trước chưa tick đủ checklist nghiệm thu.
2. **Commit Git sau mỗi buổi:** Mỗi buổi hoàn thành tương ứng với một commit sạch sẽ có thông điệp rõ ràng (`feat(session-05): implement grouped collectionview with swipe actions`).
3. **Thực tế hóa dữ liệu:** Nhập chính chi tiêu hàng ngày của bạn vào ứng dụng SoChi để trải nghiệm những điểm bất tiện về UX và tự tay hoàn thiện nó. Chúc bạn làm chủ hoàn toàn .NET MAUI!
