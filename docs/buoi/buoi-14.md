# BUỔI 14: Offline-First Sync Hai Chiều, Connectivity & Last-Write-Wins

**Thuộc chặng:** Chặng 7 (Phần 3) | **Thời lượng:** ~3 giờ

## 1. Mục tiêu buổi học (Definition of Done)
- Hoàn thiện cơ chế **Đồng bộ hai chiều (Two-way Synchronization)**:
  - **Push:** Đẩy toàn bộ các bản ghi cục bộ có `SyncState = Local` lên máy chủ.
  - **Pull:** Kéo các bản ghi trên máy chủ có `UpdatedAt > Lần sync gần nhất` về cập nhật lại SQLite.
- Áp dụng thuật toán giải quyết xung đột **Last-Write-Wins (LWW)** dựa trên mốc thời gian UTC `UpdatedAt`.
- Xử lý đồng bộ **Xóa mềm (`IsDeleted = true`)**: Khi một bên xóa giao dịch, bên kia khi sync về cũng đánh dấu xóa mềm, không làm xuất hiện lại dữ liệu đã xóa.
- Giám sát trạng thái mạng tự động bằng `Connectivity.Current`: Mất mạng thì tiếp tục dùng bình thường; khi có mạng trở lại thì app tự động kích hoạt đồng bộ ngầm.

## 2. Bản chất kiến trúc & Nguyên lý
- **Triết lý Offline-First:** Ứng dụng luôn đọc và ghi trực tiếp vào SQLite cục bộ trước, sau đó mới đồng bộ với Server trong background. Người dùng không bao giờ phải nhìn thấy vòng xoay tải trang (loading spinner) khi thêm một giao dịch.
- **Thuật toán Last-Write-Wins (LWW):** Khi cùng một giao dịch bị sửa ở cả Client và Server trong lúc ngắt mạng:
  - Nếu `Client.UpdatedAt > Server.UpdatedAt`: Bản ghi từ Client ghi đè lên Server.
  - Nếu `Server.UpdatedAt > Client.UpdatedAt`: Bản ghi từ Server ghi đè lên Client.
  - *Hạn chế cần nhớ:* Phụ thuộc vào đồng hồ thiết bị. Cần luôn dùng chuẩn `DateTime.UtcNow`.

## 3. Các bước thực hành chi tiết

### Bước 3.1: Xây dựng Sync Engine tại Client
Tạo file `src/Client/SoChi.Client/Services/ISyncService.cs`:
```csharp
using System.Net.Http.Json;
using SoChi.Client.Models;
using SoChi.Shared;
using SoChi.Shared.Dtos;

namespace SoChi.Client.Services;

public class SyncService
{
    private readonly ITransactionRepository _repository;
    private readonly IHttpClientFactory _httpClientFactory;
    private const string LastSyncKey = "last_sync_timestamp_utc";

    public SyncService(ITransactionRepository repository, IHttpClientFactory httpClientFactory)
    {
        _repository = repository;
        _httpClientFactory = httpClientFactory;
    }

    public async Task<bool> SynchronizeAsync()
    {
        if (Connectivity.Current.NetworkAccess != NetworkAccess.Internet)
            return false;

        try
        {
            var client = _httpClientFactory.CreateClient("SoChiApi");

            // 1. PUSH: Lấy các bản ghi local chưa sync
            var localTransactions = await _repository.GetTransactionsAsync();
            var dirtyTransactions = localTransactions.Where(t => t.SyncState == SyncState.Local).ToList();

            var pushRequest = new SyncPushRequest(
                new List<CategorySyncDto>(),
                dirtyTransactions.Select(t => new TransactionSyncDto(
                    t.Id, t.CategoryId, t.Amount, t.OccurredOn, t.Note, t.ReceiptPath, t.UpdatedAt, t.IsDeleted
                )).ToList()
            );

            var pushResponse = await client.PostAsJsonAsync(ApiRoutes.SyncPush, pushRequest);
            if (!pushResponse.IsSuccessStatusCode) return false;

            // Đánh dấu các bản ghi đã push thành Synced
            foreach (var t in dirtyTransactions)
            {
                t.SyncState = SyncState.Synced;
                await _repository.SaveTransactionAsync(t);
            }

            // 2. PULL: Kéo dữ liệu mới từ Server
            DateTime lastSync = Preferences.Get(LastSyncKey, DateTime.MinValue);
            var pullResponse = await client.GetFromJsonAsync<SyncPullResponse>($"{ApiRoutes.SyncPull}?sinceUtc={lastSync:O}");

            if (pullResponse != null)
            {
                foreach (var serverItem in pullResponse.Transactions)
                {
                    var localItem = await _repository.GetTransactionByIdAsync(serverItem.Id);
                    // Áp dụng Last-Write-Wins
                    if (localItem == null || serverItem.UpdatedAt > localItem.UpdatedAt)
                    {
                        var updated = new Transaction
                        {
                            Id = serverItem.Id,
                            CategoryId = serverItem.CategoryId,
                            Amount = serverItem.Amount,
                            OccurredOn = serverItem.OccurredOn,
                            Note = serverItem.Note,
                            ReceiptPath = serverItem.ReceiptPath,
                            UpdatedAt = serverItem.UpdatedAt,
                            IsDeleted = serverItem.IsDeleted,
                            SyncState = SyncState.Synced
                        };
                        await _repository.SaveTransactionAsync(updated);
                    }
                }

                Preferences.Set(LastSyncKey, pullResponse.ServerTimeUtc);
            }

            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }
}
```

### Bước 3.2: Lắng nghe sự kiện thay đổi mạng
Trong `App.xaml.cs`:
```csharp
protected override void OnStart()
{
    base.OnStart();
    Connectivity.Current.ConnectivityChanged += OnConnectivityChanged;
}

private async void OnConnectivityChanged(object? sender, ConnectivityChangedEventArgs e)
{
    if (e.NetworkAccess == NetworkAccess.Internet)
    {
        var syncService = Handler?.MauiContext?.Services.GetService<SyncService>();
        if (syncService != null)
        {
            await syncService.SynchronizeAsync();
        }
    }
}
```

### Bước 3.3: Cập nhật UI từ luồng nền — `MainThread`

Đây là lỗi sẽ xuất hiện **ngay khi bạn chạy trên Android**, và hầu như không xuất hiện trên Windows — nên rất dễ tưởng là code đã đúng.

Sự kiện `Connectivity.ConnectivityChanged` ở Bước 3.2 được hệ điều hành phát đi từ **luồng nền**. Mọi thứ chạy tiếp sau nó cũng ở luồng nền. Nếu trong đó bạn sửa `ObservableCollection` đang được `CollectionView` hiển thị, Android sẽ ném exception:

> `Only the original thread that created a view hierarchy can touch its views.`

Nguyên tắc: **mọi thay đổi chạm tới UI phải nằm trên UI thread.** Sửa hàm xử lý sự kiện thành:

```csharp
private async void OnConnectivityChanged(object? sender, ConnectivityChangedEventArgs e)
{
    if (e.NetworkAccess != NetworkAccess.Internet) return;

    var changed = await _syncService.SynchronizeAsync();
    if (!changed) return;

    // Nạp lại dữ liệu ở luồng nền là an toàn...
    var items = await _repository.GetTransactionsAsync();

    // ...nhưng đổ vào collection đang bind thì bắt buộc phải qua UI thread
    MainThread.BeginInvokeOnMainThread(() =>
    {
        Transactions.Clear();
        foreach (var item in items)
            Transactions.Add(item);

        SyncStatusText = $"Đồng bộ lúc {DateTime.Now:HH:mm}";
    });
}
```

Ba tình huống **bắt buộc** dùng `MainThread` trong dự án này:

| Tình huống | Vì sao |
|---|---|
| `ConnectivityChanged` (Buổi 14) | Sự kiện platform, luôn ở luồng nền |
| Callback từ `Timer` / `Task.Run` | Không giữ context của UI |
| Sync tự động chạy nền khi app khởi động | Không bắt nguồn từ tương tác người dùng |

Và tình huống **không cần**: khi bạn `await` bên trong một `[RelayCommand]` do người dùng bấm. Lệnh đó bắt đầu trên UI thread, nên sau mỗi `await` mã sẽ tự quay lại đúng luồng đó (`SynchronizationContext` lo việc này). Đây chính là lý do các buổi trước chưa cần đến `MainThread`.

Nếu cần trả về giá trị, dùng bản `async`:

```csharp
var confirmed = await MainThread.InvokeOnMainThreadAsync(
    () => Shell.Current.DisplayAlert("Xung đột", "Dữ liệu đã đổi ở máy khác.", "OK", "Hủy"));
```

> **Mẹo debug:** thêm `System.Diagnostics.Debug.WriteLine($"UI thread? {MainThread.IsMainThread}");` vào đầu hàm đang nghi ngờ. Đây là cách nhanh nhất để biết mình đang đứng ở luồng nào.

### Bước 3.4: Chống mạng chập chờn — Retry, Timeout & tránh sync chồng nhau

Sync là thao tác mạng dài nhất trong app, lại thường chạy đúng lúc mạng vừa mới có lại — tức là lúc kết nối yếu nhất. Không xử lý thì một lần 4G phập phù là sync thất bại và dữ liệu nằm lại máy vô thời hạn.

Cài gói khả năng chịu lỗi (đây là Polly được đóng gói sẵn cho `HttpClient`, cách dùng chuẩn từ .NET 8 trở lên):

```powershell
dotnet add src/Client/SoChi.Client/SoChi.Client.csproj package Microsoft.Extensions.Http.Resilience
```

Trong `MauiProgram.cs`, gắn vào đúng `HttpClient` đã đăng ký ở Buổi 13:

```csharp
builder.Services.AddHttpClient("SoChiApi", client =>
{
    client.BaseAddress = new Uri(ApiConfig.BaseUrl);
    client.Timeout = TimeSpan.FromSeconds(60);   // trần tổng thể cho cả chuỗi retry
})
.AddHttpMessageHandler<AuthTokenHandler>()
.AddStandardResilienceHandler(options =>
{
    options.Retry.MaxRetryAttempts = 3;
    options.Retry.Delay = TimeSpan.FromSeconds(2);
    options.Retry.BackoffType = Polly.DelayBackoffType.Exponential;

    options.AttemptTimeout.Timeout = TimeSpan.FromSeconds(15);   // cho MỖI lần thử
    options.TotalRequestTimeout.Timeout = TimeSpan.FromSeconds(55);
});
```

Bốn cơ chế bạn vừa bật, và ý nghĩa của từng cái:

| Cơ chế | Làm gì | Vì sao cần |
|---|---|---|
| **Retry** | Thử lại khi gặp lỗi mạng hoặc HTTP 5xx | Mạng di động rớt gói là chuyện bình thường, không phải lỗi thật |
| **Exponential backoff** | Giãn dần 2s → 4s → 8s | Thử lại dồn dập chỉ làm nghẽn thêm mạng đang yếu |
| **Attempt timeout** | Cắt mỗi lần thử ở 15s | Không để một request treo chiếm hết ngân sách thời gian |
| **Circuit breaker** | Tự ngắt khi server hỏng liên tục | Ngừng gọi vô ích, tiết kiệm pin và dữ liệu di động |

Quan trọng: `AddStandardResilienceHandler` **chỉ thử lại các phương thức idempotent một cách an toàn** khi lỗi là timeout hoặc 5xx. Endpoint `/api/sync/push` ở Buổi 12 an toàn với việc thử lại vì nó dùng **upsert theo `Id` do client sinh** — gửi hai lần cùng một bản ghi cho ra đúng một dòng trong database, không nhân bản. Đó không phải may mắn: đó là hệ quả trực tiếp của quyết định "khóa chính là `Guid` do client sinh" từ Buổi 03.

Cuối cùng, **chặn sync chồng nhau**. Người dùng có thể vừa kéo-để-làm-mới vừa đúng lúc mạng bật lại, khiến hai lượt sync chạy song song và ghi đè lẫn nhau. Thêm khóa vào `SyncService`:

```csharp
private readonly SemaphoreSlim _syncLock = new(1, 1);

public async Task<bool> SynchronizeAsync(CancellationToken ct = default)
{
    if (Connectivity.Current.NetworkAccess != NetworkAccess.Internet)
        return false;

    // Đang có lượt sync chạy -> bỏ qua lượt này, không xếp hàng chờ
    if (!await _syncLock.WaitAsync(0, ct))
        return false;

    try
    {
        // ... toàn bộ logic push/pull ...
        return true;
    }
    catch (HttpRequestException)
    {
        return false;   // mất mạng giữa chừng: im lặng bỏ qua, lần sau sync lại
    }
    catch (TaskCanceledException)
    {
        return false;   // hết timeout sau khi đã retry đủ
    }
    finally
    {
        _syncLock.Release();
    }
}
```

`WaitAsync(0)` là điểm tinh tế: nó **không chờ** — chiếm được khóa thì làm, không thì trả `false` ngay. Đúng với ngữ nghĩa của sync: chạy lượt thứ hai ngay sau lượt thứ nhất là vô nghĩa.


## 4. Bẫy thường gặp (Pitfalls)
- **Lỗi:** Xóa mềm ở máy A, sau khi sync máy B thì dữ liệu ở máy B bị nhân bản hoặc hiển thị lại.
  - **Nguyên nhân:** Khi kéo dữ liệu từ server về máy B, nếu không cập nhật cờ `IsDeleted = true` vào SQLite máy B thì hàm `GetTransactionsAsync()` vẫn sẽ đọc nó ra.
- **Lỗi:** App crash ngay khi mạng bật lại, log ghi `Only the original thread that created a view hierarchy can touch its views.`
  - **Nguyên nhân:** `ConnectivityChanged` chạy ở luồng nền, còn code trong đó lại sửa `ObservableCollection` đang được `CollectionView` hiển thị. Xem Bước 3.3.
- **Lỗi:** Sync trên 4G thất bại liên tục dù server vẫn sống.
  - **Nguyên nhân:** Chưa có retry/timeout. Xem Bước 3.4.
- **Lỗi:** Giao dịch bị nhân đôi sau khi mạng chập chờn.
  - **Nguyên nhân:** Hai lượt sync chạy song song. Dùng `SemaphoreSlim` với `WaitAsync(0)` như Bước 3.4. (Nếu bạn đã làm đúng phần `Guid` do client sinh từ Buổi 03 thì việc đẩy trùng **không** tạo bản ghi mới — nhân đôi ở đây là do logic client tạo `Id` mới khi retry.)

## 5. Checklist nghiệm thu Buổi 14
- [ ] Bật chế độ máy bay (ngắt hoàn toàn WiFi/4G trên điện thoại) -> Nhập thêm 3 giao dịch mới -> App hoạt động trơn tru không báo lỗi.
- [ ] Tắt chế độ máy bay (bật mạng lại) -> App tự động đẩy 3 giao dịch lên Server database.
- [ ] Mở app ở thiết bị thứ hai (hoặc mở trên Windows), đăng nhập cùng tài khoản -> Toàn bộ 3 giao dịch xuất hiện đầy đủ.
- [ ] Sync chạy nền xong, danh sách tự cập nhật mà **không crash trên Android** (đã bọc `MainThread`).
- [ ] Tắt hẳn server rồi bấm đồng bộ -> App hiện thông báo lỗi tử tế, không văng, dữ liệu cục bộ còn nguyên.
- [ ] Bấm đồng bộ liên tục 5 lần thật nhanh -> Chỉ một lượt thực sự chạy, không nhân bản dữ liệu.

---

🏠 [Mục lục toàn khóa](../HUONG-DAN-TUNG-BUOI.md)

⬅️ [BUỔI 13: Client Tích Hợp API, DelegatingHandler, SecureStorage & 10.0.2.2](buoi-13.md)

[BUỔI 15: Kiểm Thử Kịch Bản Sync Thực Tế & Đóng Gói Release (APK/MSIX)](buoi-15.md) ➡️
