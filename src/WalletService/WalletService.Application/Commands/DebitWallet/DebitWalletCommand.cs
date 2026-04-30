using MediatR;

namespace WalletService.Application.Commands.DebitWallet;

public record DebitWalletCommand(Guid UserId, Guid OrderId, decimal Amount) : IRequest;