using MediatR;
using WalletService.Domain.Interfaces;

namespace WalletService.Application.Queries.CheckBalance;

public class CheckBalanceHandler : IRequestHandler<CheckBalanceQuery, bool>
{
    private readonly IWalletRepository _repository;

    public CheckBalanceHandler(IWalletRepository repository) => _repository = repository;

    public async Task<bool> Handle(CheckBalanceQuery query, CancellationToken ct)
    {
        var wallet = await _repository.GetByUserIdAsync(query.UserId, ct);
        return wallet is not null && wallet.Balance >= query.Amount;
    }
}