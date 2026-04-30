namespace WalletService.Infrastructure.Outbox;

public class OutboxMessage
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string EventType { get; set; } = string.Empty;
    public string Payload { get; set; } = string.Empty;
    public bool Published { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}