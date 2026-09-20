using SoChi.Client.Data.Enums;

namespace SoChi.Dtos.Data;

public class CategoryDto
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    public string Name { get; set; } = string.Empty;

    public string IconGlyph { get; set; } = string.Empty;

    public string ColorHex { get; set; } = "#3B82F6";

    public TransactionKind Kind { get; set; } = TransactionKind.Expense;
    public long? MonthlyLimit { get; set; }

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public bool IsDeleted { get; set; } = false;
    public SyncState SyncState { get; set; } = SyncState.Local;
}