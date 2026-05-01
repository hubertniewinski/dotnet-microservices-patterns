using OrderService.Domain.Aggregates;
using OrderService.Domain.Interfaces;
using OrderService.Infrastructure.Outbox;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Shared.Contracts.Events;
using OrderService.Domain.Events;

namespace OrderService.Infrastructure.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly AppDbContext _db;

    public OrderRepository(AppDbContext db) => _db = db;

    public async Task<Order?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _db.Orders.FirstOrDefaultAsync(o => o.Id == id, ct);

    public async Task SaveAsync(Order order, CancellationToken ct)
    {
        _db.Orders.Update(order);

        foreach (var evt in order.DomainEvents)
        {
            var contractEvent = evt switch
            {
                OrderPlaced e => (object)new OrderPlacedEvent(e.OrderId, e.UserId, e.Amount),
                _ => null
            };

            if (contractEvent is null) {
                continue;
            }

            await _db.OutboxMessages.AddAsync(new OutboxMessage
            {
                EventType = contractEvent.GetType().AssemblyQualifiedName!,
                Payload = JsonConvert.SerializeObject(contractEvent)
            }, ct);
        }

        order.ClearDomainEvents();

        await _db.SaveChangesAsync(ct);
    }
}