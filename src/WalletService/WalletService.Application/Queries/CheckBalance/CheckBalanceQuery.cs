using MediatR;

namespace WalletService.Application.Queries.CheckBalance;

public record CheckBalanceQuery(Guid UserId, decimal Amount) : IRequest<bool>;