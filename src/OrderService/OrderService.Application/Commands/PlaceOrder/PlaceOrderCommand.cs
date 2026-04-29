using MediatR;

namespace OrderService.Application.Commands.PlaceOrder;

public record PlaceOrderCommand(Guid UserId, decimal Amount) : IRequest<Guid>;