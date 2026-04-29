using OrderService.Domain.Common;
using OrderService.Domain.Events;

namespace OrderService.Domain.Aggregates;

public class Order
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public decimal Amount { get; private set; }
    public OrderStatus Status { get; private set; }
    private readonly List<IDomainEvent> _events = new();
    public IReadOnlyList<IDomainEvent> DomainEvents => _events.AsReadOnly();

    public static Order Create(Guid userId, decimal amount)
    {
        var order = new Order
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Amount = amount,
            Status = OrderStatus.Pending
        };
        
        order._events.Add(new OrderPlaced(order.Id, userId, amount));
        return order;
    }

    public void Confirm() { 
        Status = OrderStatus.Confirmed; 
        _events.Add(new OrderConfirmed(Id)); 
    }

    public void Fail() { 
        Status = OrderStatus.Failed; 
        _events.Add(new OrderFailed(Id)); 
    }
}