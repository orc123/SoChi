using SoChi.Client.Data.Enums;

using SQLite;

namespace SoChi.Client.Data;

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
    private DateTime _updatedAt = DateTime.UtcNow;
    public DateTime UpdatedAt
    {
        get => DateTime.SpecifyKind(_updatedAt, DateTimeKind.Utc);
        set => _updatedAt = value;
    }
    public bool IsDeleted { get; set; } = false;
    public SyncState SyncState { get; set; } = SyncState.Local;
}