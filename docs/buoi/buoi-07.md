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

🏠 [Mục lục toàn khóa](../HUONG-DAN-TUNG-BUOI.md)

⬅️ [BUỔI 06: Custom Numeric Keypad, Format VND Realtime & Snackbar Hoàn Tác](buoi-06.md)

[BUỔI 08: Biểu Đồ Cột 6 Tháng (Bar Chart), Responsive & Xoay Màn Hình](buoi-08.md) ➡️
