using OrderService.Domain.Common;

namespace OrderService.Domain.Events;

public record OrderFailed(Guid OrderId) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredAt { get; } = DateTime.UtcNow;
}