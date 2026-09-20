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

🏠 [Mục lục toàn khóa](../HUONG-DAN-TUNG-BUOI.md)

⬅️ [PHỤ LỤC A: Trạng Thái Loading, Lỗi & Offline (áp dụng xuyên suốt)](phu-luc-a.md)
