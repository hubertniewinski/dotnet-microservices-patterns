using OrderService.Domain.Aggregates;

namespace OrderService.Domain.Interfaces;

public interface IOrderRepository
{
    Task<Order?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task SaveAsync(Order order, bool isNew = false, CancellationToken ct = default);
}