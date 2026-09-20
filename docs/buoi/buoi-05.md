# BUỔI 05: Danh Sách Nâng Cao: Grouping Theo Ngày, SwipeView & RefreshView

**Thuộc chặng:** Chặng 3 (Phần 1) | **Thời lượng:** ~3 giờ

## 1. Mục tiêu buổi học (Definition of Done)
- Chuyển đổi `CollectionView` phẳng thành **Danh sách nhóm theo ngày (Grouped Collection)**.
- Header của mỗi nhóm hiển thị ngày giao dịch kèm **Tổng tiền thu/chi** của ngày đó.
- Thêm tính năng **Kéo xuống để làm mới (Pull-to-refresh)** với `RefreshView`.
- Thêm tính năng **Vuốt sang trái để xóa (`SwipeView`)** kèm hộp thoại xác nhận.
- Cấu hình giao diện trạng thái rỗng (`EmptyView`) với hình minh họa khi chưa có giao dịch nào.

## 2. Bản chất kiến trúc & Nguyên lý
- **Cấu trúc dữ liệu cho Grouping trong MAUI:** `CollectionView` không thể tự nhóm một danh sách phẳng. Bạn phải cung cấp một `ObservableCollection<TGroup>` trong đó `TGroup` kế thừa từ `List<TItem>` hoặc `ObservableCollection<TItem>`. Header của nhóm sẽ bind vào thuộc tính của `TGroup`, còn từng item con sẽ bind vào `TItem`.
- **Hiệu năng cuộn mượt:** Không dùng lồng nhiều tầng Layout (`StackLayout` lồng `StackLayout`) bên trong `ItemTemplate`. Luôn ưu tiên dùng `Grid` với kích thước cột rõ ràng để tránh đo đạc layout nhiều lần khi cuộn.

## 3. Các bước thực hành chi tiết

### Bước 5.1: Xây dựng Model Grouping
Tạo file `src/Client/SoChi.Client/Models/TransactionGroup.cs`:
```csharp
using System.Collections.ObjectModel;

namespace SoChi.Client.Models;

public class TransactionItemDisplay
{
    public Guid Id { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public string CategoryIcon { get; set; } = "💸";
    public string CategoryColor { get; set; } = "#3B82F6";
    public long Amount { get; set; }
    public string Note { get; set; } = string.Empty;
    public TransactionKind Kind { get; set; }
    public DateTime OccurredOn { get; set; }
}

public class TransactionGroup : ObservableCollection<TransactionItemDisplay>
{
    public DateTime Date { get; set; }
    public long TotalAmount => this.Sum(t => t.Kind == TransactionKind.Expense ? -t.Amount : t.Amount);

    public string FormattedDate => Date.Date == DateTime.Today
        ? $"Hôm nay ({Date:dd/MM})"
        : (Date.Date == DateTime.Today.AddDays(-1) ? $"Hôm qua ({Date:dd/MM})" : Date.ToString("dddd, dd/MM/yyyy"));

    public TransactionGroup(DateTime date, IEnumerable<TransactionItemDisplay> items) : base(items)
    {
        Date = date;
    }
}
```

### Bước 5.2: Cập nhật `TransactionsViewModel` xử lý nhóm
```csharp
[ObservableProperty] public partial bool IsRefreshing { get; set; }

public ObservableCollection<TransactionGroup> GroupedTransactions { get; } = new();

[RelayCommand]
public async Task RefreshDataAsync()
{
    IsRefreshing = true;
    try
    {
        var rawTransactions = await _repository.GetTransactionsAsync();
        var categories = (await _repository.GetCategoriesAsync()).ToDictionary(c => c.Id);

        var displayItems = rawTransactions.Select(t => new TransactionItemDisplay
        {
            Id = t.Id,
            Amount = t.Amount,
            Note = string.IsNullOrWhiteSpace(t.Note) ? (categories.ContainsKey(t.CategoryId) ? categories[t.CategoryId].Name : "Không tên") : t.Note,
            OccurredOn = t.OccurredOn,
            CategoryName = categories.ContainsKey(t.CategoryId) ? categories[t.CategoryId].Name : "Khác",
            CategoryIcon = categories.ContainsKey(t.CategoryId) ? categories[t.CategoryId].IconGlyph : "🏷️",
            CategoryColor = categories.ContainsKey(t.CategoryId) ? categories[t.CategoryId].ColorHex : "#6B7280",
            Kind = categories.ContainsKey(t.CategoryId) ? categories[t.CategoryId].Kind : TransactionKind.Expense
        });

        var groups = displayItems
            .GroupBy(t => t.OccurredOn.Date)
            .OrderByDescending(g => g.Key)
            .Select(g => new TransactionGroup(g.Key, g));

        GroupedTransactions.Clear();
        foreach (var g in groups)
        {
            GroupedTransactions.Add(g);
        }
    }
    finally
    {
        IsRefreshing = false;
    }
}

[RelayCommand]
private async Task DeleteTransactionAsync(TransactionItemDisplay item)
{
    bool confirm = await Shell.Current.DisplayAlert("Xác nhận", $"Bạn có chắc muốn xóa '{item.Note}'?", "Xóa", "Hủy");
    if (!confirm) return;

    await _repository.SoftDeleteTransactionAsync(item.Id);
    await RefreshDataAsync();
}
```

### Bước 5.3: Xây dựng XAML với GroupHeaderTemplate, SwipeView & EmptyView
Cập nhật `TransactionsPage.xaml`:
```xml
<RefreshView IsRefreshing="{Binding IsRefreshing}" Command="{Binding RefreshDataCommand}">
    <CollectionView ItemsSource="{Binding GroupedTransactions}"
                    IsGrouped="True">
        
        <!-- Header của từng nhóm ngày -->
        <CollectionView.GroupHeaderTemplate>
            <DataTemplate x:DataType="models:TransactionGroup">
                <Grid ColumnDefinitions="*, Auto" Padding="16,10" BackgroundColor="#F3F4F6">
                    <Label Text="{Binding FormattedDate}" FontAttributes="Bold" TextColor="#374151" />
                    <Label Grid.Column="1" Text="{Binding TotalAmount, StringFormat='{0:N0} đ'}"
                           FontAttributes="Bold" TextColor="#4B5563" />
                </Grid>
            </DataTemplate>
        </CollectionView.GroupHeaderTemplate>

        <!-- Item chi tiết trong nhóm -->
        <CollectionView.ItemTemplate>
            <DataTemplate x:DataType="models:TransactionItemDisplay">
                <SwipeView>
                    <SwipeView.RightItems>
                        <SwipeItems Mode="Reveal">
                            <SwipeItem Text="Xóa" BackgroundColor="#EF4444"
                                       Command="{Binding Source={RelativeSource AncestorType={x:Type vm:TransactionsViewModel}}, Path=DeleteTransactionCommand}"
                                       CommandParameter="{Binding .}" />
                        </SwipeItems>
                    </SwipeView.RightItems>

                    <Grid ColumnDefinitions="48, *, Auto" Padding="16,12" ColumnSpacing="12">
                        <!-- Icon Danh mục -->
                        <Border Grid.Column="0" WidthRequest="44" HeightRequest="44"
                                StrokeShape="RoundRectangle 22" BackgroundColor="#F3F4F6" StrokeThickness="0">
                            <Label Text="{Binding CategoryIcon}" FontSize="20" HorizontalOptions="Center" VerticalOptions="Center" />
                        </Border>

                        <!-- Tên & Danh mục -->
                        <VerticalStackLayout Grid.Column="1" VerticalOptions="Center" Spacing="2">
                            <Label Text="{Binding Note}" FontSize="15" FontAttributes="Bold" TextColor="#111827" />
                            <Label Text="{Binding CategoryName}" FontSize="12" TextColor="#6B7280" />
                        </VerticalStackLayout>

                        <!-- Số tiền -->
                        <Label Grid.Column="2" Text="{Binding Amount, StringFormat='{0:N0} đ'}"
                               FontAttributes="Bold" FontSize="15" VerticalOptions="Center" TextColor="#EF4444" />
                    </Grid>
                </SwipeView>
            </DataTemplate>
        </CollectionView.ItemTemplate>

        <!-- Trạng thái rỗng -->
        <CollectionView.EmptyView>
            <VerticalStackLayout VerticalOptions="Center" HorizontalOptions="Center" Spacing="12" Padding="40">
                <Label Text="🍃" FontSize="60" HorizontalOptions="Center" />
                <Label Text="Chưa có giao dịch nào" FontAttributes="Bold" FontSize="18" HorizontalOptions="Center" TextColor="#6B7280" />
                <Label Text="Nhấn nút '+' để thêm khoản chi tiêu đầu tiên" TextColor="#9CA3AF" HorizontalOptions="Center" HorizontalTextAlignment="Center" />
            </VerticalStackLayout>
        </CollectionView.EmptyView>
    </CollectionView>
</RefreshView>
```

## 4. Bẫy thường gặp (Pitfalls)
- **Lỗi:** Danh sách bị trắng trơn dù database có dữ liệu.
  - **Nguyên nhân:** Quên khai báo `IsGrouped="True"` trên thẻ `CollectionView`.
- **Lỗi:** Nút xóa trong `SwipeItem` không kích hoạt lệnh `DeleteTransactionCommand`.
  - **Nguyên nhân:** Do `SwipeItem` có context là `TransactionItemDisplay`, không phải `TransactionsViewModel`. Cần dùng cú pháp `{RelativeSource AncestorType={x:Type vm:TransactionsViewModel}}` để trỏ về ViewModel cha.

## 5. Checklist nghiệm thu Buổi 05
- [ ] Giao dịch được phân tách thành từng ngày rõ ràng ("Hôm nay", "Hôm qua", hoặc định dạng thứ ngày).
- [ ] Header hiển thị chuẩn tổng thu/chi của cả ngày đó.
- [ ] Vuốt sang trái trên một dòng -> Nút "Xóa" màu đỏ lộ ra. Bấm xóa -> Xuất hiện popup xác nhận -> Xóa thành công.
- [ ] Kéo từ đỉnh màn hình xuống -> Vòng xoay Refresh xuất hiện và tự tắt khi cập nhật xong.
- [ ] Khi xóa sạch dữ liệu -> Giao diện EmptyView hiển thị biểu tượng chiếc lá.

---

🏠 [Mục lục toàn khóa](../HUONG-DAN-TUNG-BUOI.md)

⬅️ [BUỔI 04B: Quản Lý Danh Mục — CRUD, Chọn Icon/Màu & Đặt Hạn Mức](buoi-04b.md)

[BUỔI 06: Custom Numeric Keypad, Format VND Realtime & Snackbar Hoàn Tác](buoi-06.md) ➡️
