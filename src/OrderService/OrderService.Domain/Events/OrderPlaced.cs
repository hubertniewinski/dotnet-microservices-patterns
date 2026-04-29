using OrderService.Domain.Common;

namespace OrderService.Domain.Events;

public record OrderPlaced(Guid OrderId, Guid UserId, decimal Amount) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredAt { get; } = DateTime.UtcNow;
}