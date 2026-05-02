using MediatR;

namespace WalletService.Application.Commands.DebitWallet;

public record DebitWalletCommand(Guid IdempotencyKey, Guid UserId, Guid OrderId, decimal Amount) : IRequest;