using WalletService.Domain.Common;

namespace WalletService.Domain.Events;

public record WalletDebited(Guid UserId, Guid OrderId, decimal Amount) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredAt { get; } = DateTime.UtcNow;
}