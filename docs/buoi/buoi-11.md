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

🏠 [Mục lục toàn khóa](../HUONG-DAN-TUNG-BUOI.md)

⬅️ [BUỔI 10: Theme Sáng/Tối, Sinh Trắc Học Vân Tay, Haptic & Xuất File CSV](buoi-10.md)

[BUỔI 12: Backend ASP.NET Core Web API, PostgreSQL, Shared DTOs & JWT Auth](buoi-12.md) ➡️
