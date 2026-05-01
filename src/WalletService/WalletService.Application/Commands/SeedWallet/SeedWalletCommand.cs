using MediatR;

namespace WalletService.Application.Commands.SeedWallet;

public record SeedWalletCommand(Guid UserId, decimal InitialBalance) : IRequest;