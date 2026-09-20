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

🏠 [Mục lục toàn khóa](../HUONG-DAN-TUNG-BUOI.md)

⬅️ [BUỔI 11: Hạn Mức Chi Tiêu (ProgressBar), Lọc, Debounce & xUnit Tests](buoi-11.md)

[BUỔI 13: Client Tích Hợp API, DelegatingHandler, SecureStorage & 10.0.2.2](buoi-13.md) ➡️
