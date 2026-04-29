using OrderService.Domain.Aggregates;

namespace OrderService.Domain.Interfaces;

public interface IOrderRepository
{
    Task SaveAsync(Order order, CancellationToken ct = default);
}