namespace WalletService.Infrastructure.Persistence.Idempotency;

public class IdempotencyRecord
{
    public Guid IdempotencyKey { get; set; }
    public string Result { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}