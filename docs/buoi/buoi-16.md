# BUỔI 16: Blazor WebAssembly — Dựng Web Dashboard, JWT Auth & Công Cụ Kiểm Tra Dữ Liệu

**Thuộc chặng:** Chặng 9 (mở rộng) | **Thời lượng:** ~3 giờ

> **Buổi bổ sung.** Yêu cầu đã xong Buổi 12 (API + PostgreSQL). Đặt ở đây vì đến lúc này server đã có dữ liệu thật do client đồng bộ lên. Nếu muốn có công cụ debug sớm hơn để hỗ trợ Buổi 14, bạn hoàn toàn có thể làm buổi này ngay sau Buổi 12 — chỉ cần bơm vài bản ghi bằng Postman hoặc bằng chính bộ test ở Bước 3.10.

## 1. Mục tiêu buổi học (Definition of Done)
- Dựng project `src/Web/SoChi.Web` bằng Blazor WebAssembly, **được phục vụ trực tiếp từ `src/Api/SoChi.Api`** (cùng origin, không cần cấu hình CORS).
- Đăng nhập bằng chính endpoint `/api/auth/login` đã viết ở Buổi 12, token lưu ở `localStorage`.
- Có trang **Kiểm tra dữ liệu**: xem toàn bộ bản ghi thô trên server, gồm cả bản đã xóa mềm, kèm `UpdatedAt` và `UserId`.
- `src/Shared/SoChi.Shared` được **ba** project cùng tham chiếu: Client (MAUI), Api, Web.

## 2. Bản chất kiến trúc & Nguyên lý

**Blazor WebAssembly là gì.** Toàn bộ .NET runtime được biên dịch sang WebAssembly và tải về trình duyệt. Code C# của bạn chạy **trong trình duyệt người dùng**, không phải trên server. Nghĩa là: cùng ngôn ngữ, cùng DTO, cùng `HttpClient`, cùng mô hình DI mà bạn đã dùng suốt 15 buổi — chỉ đổi đích chạy.

Phân biệt với Blazor Server (dễ nhầm):

| | Blazor WebAssembly | Blazor Server |
|---|---|---|
| Code C# chạy ở | Trình duyệt | Server |
| Cần kết nối liên tục | Không — hoạt động cả khi mất mạng | Có — đứt SignalR là đứng hình |
| Lần tải đầu | Chậm hơn (phải tải runtime) | Nhanh |
| Bí mật (khóa API, chuỗi kết nối) | **Không giấu được gì cả** | Giấu được |

Chọn WebAssembly ở đây vì nó cùng triết lý với SoChi: **client tự chủ, server chỉ là nơi chứa dữ liệu**.

**Vì sao host từ `src/Api/SoChi.Api` chứ không chạy riêng.** Blazor WASM standalone chạy ở origin khác (ví dụ `localhost:5000`) trong khi API ở `localhost:7001` → mọi request bị chặn bởi CORS, phải cấu hình thêm, và cookie/token qua origin khác kéo theo một loạt vấn đề bảo mật. Cho `src/Api/SoChi.Api` phục vụ luôn file tĩnh của Web thì cả hai **cùng một origin**: không CORS, không cấu hình gì thêm, và deploy chỉ một thứ. Đây cũng là cách các dự án thực tế hay làm nhất.

**Điểm học lớn nhất của buổi này:** bạn sẽ nhận ra `AuthTokenHandler` viết cho Blazor **gần như giống hệt** cái đã viết cho MAUI ở Buổi 13. Cùng một `DelegatingHandler`, cùng một cách gắn `Bearer`. Khác biệt duy nhất là chỗ cất token: `SecureStorage` trên di động, `localStorage` trên web. Đó là bằng chứng cụ thể rằng bạn đang học **kiến trúc**, không phải học thuộc API của một framework.

## 3. Các bước thực hành chi tiết

### Bước 16.1: Tạo project và nối dây

```powershell
dotnet new blazorwasm -n SoChi.Web -o src/Web/SoChi.Web
dotnet sln SoChi.slnx add src/Web/SoChi.Web/SoChi.Web.csproj

# Web dùng chung DTO -> đây là project THỨ BA tham chiếu Shared
dotnet add src/Web/SoChi.Web/SoChi.Web.csproj reference src/Shared/SoChi.Shared/SoChi.Shared.csproj

# Api phục vụ file tĩnh của Web
dotnet add src/Api/SoChi.Api/SoChi.Api.csproj reference src/Web/SoChi.Web/SoChi.Web.csproj
dotnet add src/Api/SoChi.Api/SoChi.Api.csproj package Microsoft.AspNetCore.Components.WebAssembly.Server
```

Sơ đồ tham chiếu sau bước này:

```
Client (MAUI) ──┐
Web (Blazor)  ──┼──▶ Shared
Api           ──┘
      │
      └──▶ Web   (chỉ để lấy file tĩnh đã build, không gọi code)
```

Sửa `src/Api/SoChi.Api/Program.cs` — thứ tự các dòng này **có ý nghĩa**:

```csharp
var app = builder.Build();

app.UseBlazorFrameworkFiles();   // phục vụ _framework/*.wasm, *.dll
app.UseStaticFiles();            // phục vụ wwwroot của Web

app.UseAuthentication();
app.UseAuthorization();

app.MapAuthEndpoints(jwtKey);
app.MapSyncEndpoints();

app.MapFallbackToFile("index.html");   // BẮT BUỘC đặt CUỐI CÙNG

app.Run();
```

`MapFallbackToFile` nghĩa là "đường dẫn nào không khớp endpoint nào thì trả về `index.html`" — cần thiết để routing phía Blazor hoạt động khi người dùng F5 ở `/thong-ke`. Đặt nó **trước** các `Map...Endpoints` thì mọi lời gọi API sẽ nhận về trang HTML, và bạn sẽ nhận lỗi khó hiểu kiểu `'<' is an invalid start of a value` khi parse JSON.

Chạy thử:
```powershell
dotnet run --project src/Api/SoChi.Api
```
Mở `https://localhost:7xxx` — trang Blazor mặc định hiện lên, phục vụ bởi chính API. Không cần chạy hai tiến trình.

### Bước 16.2: Lưu token và gắn vào request

Tạo `src/Web/SoChi.Web/Services/TokenStore.cs`:

```csharp
using Microsoft.JSInterop;

namespace SoChi.Web.Services;

public class TokenStore(IJSRuntime js)
{
    private const string Key = "sochi_token";

    public ValueTask<string?> GetAsync() =>
        js.InvokeAsync<string?>("localStorage.getItem", Key);

    public ValueTask SetAsync(string token) =>
        js.InvokeVoidAsync("localStorage.setItem", Key, token);

    public ValueTask ClearAsync() =>
        js.InvokeVoidAsync("localStorage.removeItem", Key);
}
```

Tạo `src/Web/SoChi.Web/Services/AuthTokenHandler.cs` — **so nó với `AuthTokenHandler` của MAUI ở Buổi 13, gần như từng dòng một**:

```csharp
using System.Net.Http.Headers;

namespace SoChi.Web.Services;

public class AuthTokenHandler(TokenStore store) : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken ct)
    {
        var token = await store.GetAsync();

        if (!string.IsNullOrWhiteSpace(token))
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        return await base.SendAsync(request, ct);
    }
}
```

Đăng ký trong `src/Web/SoChi.Web/Program.cs`:

```csharp
builder.Services.AddScoped<TokenStore>();
builder.Services.AddTransient<AuthTokenHandler>();

builder.Services.AddHttpClient("SoChiApi", client =>
{
    // Cùng origin với trang web -> không cần biết địa chỉ API là gì,
    // và không dính CORS. Đây là phần thưởng của quyết định ở Bước 16.1.
    client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress);
})
.AddHttpMessageHandler<AuthTokenHandler>();

builder.Services.AddScoped(sp =>
    sp.GetRequiredService<IHttpClientFactory>().CreateClient("SoChiApi"));
```

### Bước 16.3: Trang đăng nhập

Tạo `src/Web/SoChi.Web/Pages/Login.razor`:

```razor
@page "/dang-nhap"
@using System.Net.Http.Json
@using SoChi.Shared
@using SoChi.Shared.Dtos
@using SoChi.Web.Services
@inject HttpClient Http
@inject TokenStore Tokens
@inject NavigationManager Nav

<h3>Đăng nhập SoChi</h3>

<div class="mb-2">
    <input class="form-control" placeholder="Email" @bind="email" />
</div>
<div class="mb-2">
    <input class="form-control" type="password" placeholder="Mật khẩu"
           @bind="password" @onkeyup="OnKeyUp" />
</div>

<button class="btn btn-primary" disabled="@busy" @onclick="LoginAsync">
    @(busy ? "Đang đăng nhập..." : "Đăng nhập")
</button>

@if (!string.IsNullOrEmpty(error))
{
    <div class="alert alert-danger mt-3">@error</div>
}

@code {
    private string email = "", password = "", error = "";
    private bool busy;

    private async Task OnKeyUp(KeyboardEventArgs e)
    {
        if (e.Key == "Enter") await LoginAsync();
    }

    private async Task LoginAsync()
    {
        busy = true;
        error = "";

        try
        {
            var res = await Http.PostAsJsonAsync($"/{ApiRoutes.Login}",
                new LoginRequest(email, password));

            if (!res.IsSuccessStatusCode)
            {
                error = "Email hoặc mật khẩu không đúng.";
                return;
            }

            var auth = await res.Content.ReadFromJsonAsync<AuthResponse>();
            await Tokens.SetAsync(auth!.Token);
            Nav.NavigateTo("/kiem-tra");
        }
        catch (HttpRequestException)
        {
            error = "Không kết nối được máy chủ.";
        }
        finally
        {
            busy = false;
        }
    }
}
```

Để ý: `ApiRoutes.Login`, `LoginRequest`, `AuthResponse` đều đến từ `src/Shared/SoChi.Shared` — **cùng đúng những kiểu mà MAUI đang dùng**. Đổi tên một trường trong `AuthResponse` bây giờ sẽ làm hỏng build của cả ba project cùng lúc, ngay lập tức. Đó chính xác là điều bạn muốn, và là lý do `Shared` tồn tại.

### Bước 16.4: Endpoint xem dữ liệu thô

Thêm vào `src/Api/SoChi.Api/Endpoints/SyncEndpoints.cs` (hoặc tạo `ReportEndpoints.cs`):

```csharp
group.MapGet("/raw", async (ClaimsPrincipal principal, AppDbContext db, bool includeDeleted = false) =>
{
    var userId = principal.GetUserId();

    var query = db.Transactions.Where(t => t.UserId == userId);
    if (!includeDeleted)
        query = query.Where(t => !t.IsDeleted);

    var rows = await query
        .OrderByDescending(t => t.UpdatedAt)
        .Take(500)
        .Select(t => new TransactionSyncDto(
            t.Id, t.CategoryId, t.Amount, t.OccurredOn, t.Note,
            t.ReceiptPath, t.UpdatedAt, t.IsDeleted))
        .ToListAsync();

    return Results.Ok(rows);
});
```

`.Take(500)` không phải chi tiết vặt: thiếu nó, một ngày nào đó trang này sẽ cố kéo về 50.000 dòng và treo trình duyệt. **Mọi endpoint trả về danh sách đều phải có giới hạn**, không có ngoại lệ.

### Bước 16.5: Trang Kiểm tra dữ liệu

Tạo `src/Web/SoChi.Web/Pages/DataInspector.razor`:

```razor
@page "/kiem-tra"
@using System.Net.Http.Json
@using SoChi.Shared.Dtos
@inject HttpClient Http

<h3>Kiểm tra dữ liệu trên server</h3>

<div class="form-check mb-3">
    <input class="form-check-input" type="checkbox" id="del"
           @bind="includeDeleted" @bind:after="LoadAsync" />
    <label class="form-check-label" for="del">Hiện cả bản ghi đã xóa mềm</label>
</div>

@if (loading)
{
    <p><em>Đang tải...</em></p>
}
else if (rows is null)
{
    <div class="alert alert-warning">
        Chưa đăng nhập hoặc token hết hạn. <a href="/dang-nhap">Đăng nhập lại</a>.
    </div>
}
else
{
    <p>@rows.Count bản ghi (tối đa 500, mới nhất trước).</p>

    <table class="table table-sm table-striped">
        <thead>
            <tr>
                <th>Số tiền</th>
                <th>Ngày</th>
                <th>Ghi chú</th>
                <th>UpdatedAt (UTC)</th>
                <th>Trạng thái</th>
                <th>Id</th>
            </tr>
        </thead>
        <tbody>
            @foreach (var r in rows)
            {
                <tr class="@(r.IsDeleted ? "text-decoration-line-through opacity-50" : "")">
                    <td class="text-end">@r.Amount.ToString("N0") đ</td>
                    <td>@r.OccurredOn.ToString("dd/MM/yyyy")</td>
                    <td>@r.Note</td>
                    <td><code>@r.UpdatedAt.ToString("O")</code></td>
                    <td>@(r.IsDeleted ? "Đã xóa" : "Hoạt động")</td>
                    <td><code>@r.Id.ToString()[..8]</code></td>
                </tr>
            }
        </tbody>
    </table>
}

@code {
    private List<TransactionSyncDto>? rows;
    private bool includeDeleted, loading = true;

    protected override Task OnInitializedAsync() => LoadAsync();

    private async Task LoadAsync()
    {
        loading = true;
        try
        {
            rows = await Http.GetFromJsonAsync<List<TransactionSyncDto>>(
                $"/api/sync/raw?includeDeleted={includeDeleted}");
        }
        catch (HttpRequestException)
        {
            rows = null;   // 401 hoặc mất mạng
        }
        finally
        {
            loading = false;
        }
    }
}
```

Trang này chính là **công cụ debug cho Buổi 14** mà bạn không có trước đây. Nó trả lời trực tiếp những câu hỏi mà trước kia phải mở pgAdmin gõ SQL mới biết:

- Bản ghi vừa tạo lúc offline đã thật sự lên tới server chưa?
- Xóa ở máy A rồi, trên server `IsDeleted` đã thành `true` chưa?
- Sửa ở hai máy, `UpdatedAt` nào lớn hơn — có đúng bản đó thắng không?

Hiển thị `UpdatedAt` ở định dạng `"O"` (ISO 8601 đầy đủ, có `Z` ở cuối) là cố ý: bạn cần thấy **chính xác đến từng phần nghìn giây và đúng là UTC** để đối chiếu last-write-wins, không phải một chuỗi ngày giờ đã bị làm tròn theo giờ địa phương.

## 4. Bẫy thường gặp (Pitfalls)
- **Lỗi:** Gọi API trả về HTML, JSON parse báo `'<' is an invalid start of a value`.
  - **Nguyên nhân:** `MapFallbackToFile("index.html")` đặt **trước** các endpoint API nên nó nuốt hết. Xem lại thứ tự ở Bước 16.1.
- **Lỗi:** `Access to fetch ... has been blocked by CORS policy`.
  - **Nguyên nhân:** Bạn đang chạy `src/Web/SoChi.Web` riêng (`dotnet run --project src/Web/SoChi.Web`) thay vì mở qua địa chỉ của API. Buổi này luôn chạy `dotnet run --project src/Api/SoChi.Api` rồi mở đúng cổng của nó.
- **Lỗi:** `IJSRuntime` ném `JavaScript interop calls cannot be issued at this time` khi đọc token.
  - **Nguyên nhân:** Gọi JS trong lúc prerender hoặc trong constructor. Chỉ gọi `localStorage` từ `OnAfterRenderAsync` hoặc từ event handler của người dùng.
- **Lỗi:** Đăng nhập xong F5 lại thì mất phiên.
  - **Nguyên nhân:** Token lưu ở `sessionStorage` thay vì `localStorage`, hoặc quên `await` khi ghi.
- **Cảnh báo bảo mật — đọc kỹ:** Blazor WASM chạy hoàn toàn trong trình duyệt, nên **mọi thứ trong `src/Web/SoChi.Web` đều công khai**: người dùng tải được toàn bộ DLL và đọc được mã nguồn của bạn. Tuyệt đối không đặt chuỗi kết nối database, khóa JWT, hay bất kỳ bí mật nào ở đây. Kiểm tra quyền **luôn phải nằm ở server** — `[Authorize]` và điều kiện `UserId` trong endpoint mới là thứ bảo vệ dữ liệu; ẩn/hiện nút bấm ở giao diện chỉ là tiện lợi cho người dùng, không phải bảo mật.
- **Về `localStorage`:** token nằm ở đó có thể bị đánh cắp nếu trang dính lỗ hổng XSS. Với dự án học tập thì chấp nhận được, nhưng cần biết đó là đánh đổi có ý thức. Giải pháp của ứng dụng thật là cookie `HttpOnly` + `SameSite`, và token sống ngắn kèm refresh token — chứ không phải token 30 ngày như Buổi 12 đang cấp.

## 5. Checklist nghiệm thu Buổi 16
- [ ] `dotnet run --project src/Api/SoChi.Api` rồi mở `https://localhost:7xxx` → thấy giao diện Blazor, **không** cần chạy tiến trình thứ hai.
- [ ] Đăng nhập bằng đúng tài khoản đã tạo ở Buổi 12 → chuyển sang trang Kiểm tra.
- [ ] Mở F12 → tab Network, thấy request `/api/sync/raw` mang header `Authorization: Bearer ...`.
- [ ] Xóa token trong `localStorage` rồi F5 → trang hiện cảnh báo chưa đăng nhập, **không** trắng màn hình.
- [ ] Thêm một giao dịch trên app MAUI rồi sync → F5 trang web thấy đúng bản ghi đó.
- [ ] Tick "Hiện cả bản ghi đã xóa mềm" → bản ghi đã xóa hiện lên với dòng gạch ngang.
- [ ] `dotnet build SoChi.slnx` xanh cả bốn project: Client, Api, Shared, Web.

---

🏠 [Mục lục toàn khóa](../HUONG-DAN-TUNG-BUOI.md)

⬅️ [BUỔI 15: Kiểm Thử Kịch Bản Sync Thực Tế & Đóng Gói Release (APK/MSIX)](buoi-15.md)

[BUỔI 17: Endpoint Báo Cáo, Biểu Đồ SVG Tự Vẽ & Kiểm Chứng Sức Khỏe Dữ Liệu](buoi-17.md) ➡️
