using SoChi.Client.Data.Enums;

using SQLite;

namespace SoChi.Client.Data;

[Table("Transactions")]
public class Transaction
{
    [PrimaryKey]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Indexed]
    public Guid CategoryId { get; set; }
    public long Amount { get; set; } // Đơn vị đồng

    [Indexed]
    public DateTime OccurredOn { get; set; } = DateTime.Today;

    public string Note { get; set; } = string.Empty;
    public string? ReceiptPath { get; set; }
    private DateTime _updatedAt = DateTime.UtcNow;
    public DateTime UpdatedAt
    {
        get => DateTime.SpecifyKind(_updatedAt, DateTimeKind.Utc);
        set => _updatedAt = value;
    }
    public bool IsDeleted { get; set; } = false;
    public SyncState SyncState { get; set; } = SyncState.Local;
}
