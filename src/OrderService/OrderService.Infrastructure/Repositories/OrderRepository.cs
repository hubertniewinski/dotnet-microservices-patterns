using OrderService.Domain.Aggregates;
using OrderService.Domain.Interfaces;
using OrderService.Infrastructure.Outbox;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

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
            await _db.OutboxMessages.AddAsync(new OutboxMessage
            {
                EventType = evt.GetType().Name,
                Payload = JsonConvert.SerializeObject(evt, new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.All
                })
            }, ct);
        }

        order.ClearDomainEvents();

        await _db.SaveChangesAsync(ct);
    }
}