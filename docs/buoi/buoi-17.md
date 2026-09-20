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

🏠 [Mục lục toàn khóa](../HUONG-DAN-TUNG-BUOI.md)

⬅️ [BUỔI 16: Blazor WebAssembly — Dựng Web Dashboard, JWT Auth & Công Cụ Kiểm Tra Dữ Liệu](buoi-16.md)

[PHỤ LỤC A: Trạng Thái Loading, Lỗi & Offline (áp dụng xuyên suốt)](phu-luc-a.md) ➡️
