using MediatR;

namespace OrderService.Application.Commands.FailOrder;

public record FailOrderCommand(Guid OrderId) : IRequest;