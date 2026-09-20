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

🏠 [Mục lục toàn khóa](../HUONG-DAN-TUNG-BUOI.md)

⬅️ [BUỔI 07: Đồ Họa 2D Thuần: Donut Chart Tự Vẽ Với Microsoft.Maui.Graphics](buoi-07.md)

[BUỔI 09: API Thiết Bị: Chụp/Chọn Ảnh Hóa Đơn, FileSystem & Phân Quyền](buoi-09.md) ➡️
