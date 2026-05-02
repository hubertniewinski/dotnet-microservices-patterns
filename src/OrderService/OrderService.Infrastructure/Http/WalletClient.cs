using Microsoft.Extensions.Logging;
using OrderService.Application.Interfaces;

namespace OrderService.Infrastructure.Http;

public class WalletClient : IWalletClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<WalletClient> _logger;

    public WalletClient(HttpClient httpClient, ILogger<WalletClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<bool> CheckBalanceAsync(Guid userId, decimal amount, CancellationToken ct = default)
    {
        var response = await _httpClient.GetAsync($"/api/wallets/balance/{userId}/{amount}", ct);

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogWarning("Balance check failed for user {UserId} with status {StatusCode}", userId, response.StatusCode);
            return false;
        }

        return true;
    }
}