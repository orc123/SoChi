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
```

**Chỉ một package duy nhất.** Nhiều hướng dẫn trên mạng bảo cài kèm `SQLitePCLRaw.bundle_green` — **đừng làm theo**. Bản mới của `sqlite-net-pcl` (1.11.285 trở lên) đã tự kéo về `SQLitePCLRaw.core` 3.0.3, `SQLitePCLRaw.provider.e_sqlite3` 3.0.3 và `SourceGear.sqlite3` 3.53.3. Thêm `bundle_green` vào chỉ tổ kéo ngược một nhánh phụ thuộc cũ kỹ, và nhánh đó đang dính lỗ hổng bảo mật (xem phần Bẫy bên dưới).

Kiểm tra cây phụ thuộc thực tế:
```powershell
dotnet list src/Client/SoChi.Client/SoChi.Client.csproj package --include-transitive
```
Phải thấy `SQLitePCLRaw.*` ở nhánh **3.0.x**, không phải 2.1.x.

**Về ba mảnh ghép của SQLite trong .NET** — hiểu để không cài thừa:

| Thành phần | Vai trò |
|---|---|
| `sqlite-net-pcl` | ORM — biến class C# thành bảng, sinh câu SQL. Đây là thứ bạn gọi trong code. |
| `SQLitePCLRaw.*` | Lớp trung gian gọi xuống thư viện native |
| `SourceGear.sqlite3` | Bản biên dịch native của SQLite cho từng nền tảng (`.so`, `.dylib`, `.dll`) |

Các gói tên `bundle_*` chỉ là những **combo đóng gói sẵn** của hai tầng dưới. Khi ORM đã khai báo đủ phụ thuộc rồi thì bạn không cần combo nào cả.

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

- **Lỗi:** Restore thất bại với `NU1903: Warning As Error: Package 'SQLitePCLRaw.lib.e_sqlite3' 2.1.11 has a known high severity vulnerability`, kèm dòng `Rolling back package changes`.
  - **Nguyên nhân:** Bạn đã cài `SQLitePCLRaw.bundle_green`. Gói này dừng lại ở bản 2.1.11 và kéo theo `SQLitePCLRaw.lib.e_sqlite3` 2.1.11 đang có cảnh báo bảo mật. Bản vá nằm ở 2.1.13, nhưng `bundle_green` không được cập nhật để dùng nó.
  - **Vì sao nó là *error* chứ không phải *warning*:** từ .NET 8, **NuGet Audit** bật sẵn — mỗi lần restore, NuGet đối chiếu danh sách package với cơ sở dữ liệu lỗ hổng của GitHub Advisory và phát cảnh báo `NU1903`. Cấu hình `TreatWarningsAsErrors=true` từ Buổi 00 biến cảnh báo đó thành lỗi, và restore bị **rollback** — nghĩa là package không được thêm vào `.csproj` chút nào.
  - **Khắc phục:** gỡ `bundle_green`, chỉ giữ `sqlite-net-pcl`:
    ```powershell
    dotnet remove src/Client/SoChi.Client/SoChi.Client.csproj package SQLitePCLRaw.bundle_green
    ```
  - **Đừng tắt cảnh báo này.** Sẽ có người khuyên bạn thêm `<NoWarn>$(NoWarn);NU1903</NoWarn>` hoặc `<NuGetAudit>false</NuGetAudit>`. Làm vậy là bịt mắt mình trước một lỗ hổng thật, chứ không phải sửa nó. Cách xử lý đúng với `NU1903` luôn là **nâng cấp hoặc bỏ package có vấn đề** — ở đây may mắn là bỏ được hẳn vì nó vốn thừa.
  - **Bài học rộng hơn:** `TreatWarningsAsErrors` bật từ Buổi 00 vừa chặn bạn đúng lúc. Nếu tắt nó, package dính lỗ hổng đã lặng lẽ đi vào dự án và bạn sẽ không bao giờ biết.
- **Lỗi:** `UpdatedAt` đọc từ SQLite ra có `Kind = Unspecified` thay vì `Utc`, dù lúc ghi vào là `DateTime.UtcNow`.
  - **Nguyên nhân:** `sqlite-net-pcl` mặc định lưu `DateTime` dưới dạng **ticks** (`storeDateTimeAsTicks: true`). Lúc đọc ra nó dựng lại bằng `new DateTime(ticks)` — mà constructor này luôn cho `Kind = Unspecified`. Thông tin "đây là giờ UTC" bị mất sạch.
  - **Vì sao nguy hiểm:** ở Buổi 13–14, DTO gửi lên server sẽ được `System.Text.Json` tuần tự hóa **không có hậu tố `Z`**. Server PostgreSQL nhận vào rồi diễn giải theo kiểu khác, khiến so sánh last-write-wins sai lệch đúng bằng chênh lệch múi giờ (7 tiếng ở Việt Nam). Triệu chứng sẽ là "bản sửa cũ hơn lại thắng" — cực khó truy vì không có lỗi nào được ném ra.
  - **Khắc phục:** đừng dùng thẳng giá trị đọc từ database. Chuẩn hóa ngay tại tầng Repository, mỗi khi đọc bản ghi lên:
    ```csharp
    private static DateTime AsUtc(DateTime value) =>
        value.Kind == DateTimeKind.Utc ? value : DateTime.SpecifyKind(value, DateTimeKind.Utc);
    ```
    Áp dụng cho `UpdatedAt` sau mỗi lần `GetAsync`/`Table<T>()`. Cách gọn hơn là bọc nó vào chính property của model:
    ```csharp
    private DateTime _updatedAt = DateTime.UtcNow;
    public DateTime UpdatedAt
    {
        get => DateTime.SpecifyKind(_updatedAt, DateTimeKind.Utc);
        set => _updatedAt = value;
    }
    ```
  - **Lưu ý:** `OccurredOn` **không** cần xử lý này. Nó là một ngày trên lịch (`DateTime.Today`), không phải mốc thời gian tuyệt đối — đúng như cách nó được ánh xạ sang kiểu `date` ở phía server tại Buổi 12.
- **Lỗi:** Build đỏ với `CS8618: Non-nullable property must contain a non-null value when exiting constructor`.
  - **Nguyên nhân:** `Nullable` và `TreatWarningsAsErrors` đều bật từ Buổi 00. Mọi property kiểu `string` trong model phải có giá trị khởi tạo (`= string.Empty`) hoặc khai báo là `string?`. Code mẫu ở Bước 3.2 đã làm đúng — nếu bạn tự gõ lại thì đừng bỏ sót phần khởi tạo.

## 5. Checklist nghiệm thu Buổi 03
- [ ] Database file được cấu hình tạo đúng tại `FileSystem.AppDataDirectory`.
- [ ] Hai class `Category` và `Transaction` định nghĩa đủ các thuộc tính (Guid, long, IsDeleted, SyncState).
- [ ] Gọi thử `SaveTransactionAsync()` và `GetTransactionsAsync()` trong hàm kiểm tra không gặp deadlock.
- [ ] Ghi một bản ghi rồi đọc lại: `UpdatedAt.Kind` phải là `Utc`, không phải `Unspecified`.
- [ ] `dotnet list ... package --include-transitive` cho thấy `SQLitePCLRaw.*` ở nhánh **3.0.x**, không còn dấu vết 2.1.x.
- [ ] Build xanh cả 4 target, 0 warning (nhớ `Nullable` và `TreatWarningsAsErrors` đang bật).

---

🏠 [Mục lục toàn khóa](../HUONG-DAN-TUNG-BUOI.md)

⬅️ [BUỔI 02: AppShell 5 Tabs, Modal Navigation & Dependency Injection](buoi-02.md)

[BUỔI 04: Seed Data, Form Thêm/Sửa & Hiển Thị CollectionView](buoi-04.md) ➡️
