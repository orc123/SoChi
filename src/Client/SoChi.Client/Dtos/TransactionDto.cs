using SoChi.Client.Data.Enums;

namespace SoChi.Dtos.Data;

public class TransactionDto
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid CategoryId { get; set; }
    public long Amount { get; set; } // Đơn vị đồng

    public DateTime OccurredOn { get; set; } = DateTime.Today;

    public string Note { get; set; } = string.Empty;
    public string? ReceiptPath { get; set; }
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public bool IsDeleted { get; set; } = false;
    public SyncState SyncState { get; set; } = SyncState.Local;
}
