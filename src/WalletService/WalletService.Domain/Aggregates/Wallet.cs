using WalletService.Domain.Common;
using WalletService.Domain.Events;

namespace WalletService.Domain.Aggregates;

public class Wallet
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public decimal Balance { get; private set; }
    private readonly List<IDomainEvent> _events = new();
    public IReadOnlyList<IDomainEvent> DomainEvents => _events.AsReadOnly();
    
    public void ClearDomainEvents() => _events.Clear();

    public static Wallet Create(Guid userId, decimal initialBalance) 
        => new Wallet { Id = Guid.NewGuid(), UserId = userId, Balance = initialBalance };

    public void Debit(decimal amount, Guid orderId)
    {
        if (amount <= 0)
            throw new DomainException("Amount must be positive");

        if (Balance < amount)
        {
            _events.Add(new WalletDebitFailed(UserId, orderId, amount));
            return;
        }

        Balance -= amount;
        _events.Add(new WalletDebited(UserId, orderId, amount));
    }
}