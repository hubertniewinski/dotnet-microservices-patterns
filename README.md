# dotnet-microservices-patterns

Practical reference implementation of microservices patterns in .NET.

## Architecture

┌─────────────────────────────────────────────────────┐
│                   docker-compose                    │
│                                                     │
│   ┌─────────────────┐     ┌─────────────────────┐  │
│   │   OrderService  │     │    WalletService    │  │
│   │   :5001         │────▶│    :5002            │  │
│   │                 │     │                     │  │
│   │  [Polly]        │◀────│  /chaos (demo only) │  │
│   │  circuit breaker│     │  + IdempotencyKey   │  │
│   │  retry + timeout│     │                     │  │
│   └────────┬────────┘     └──────────┬──────────┘  │
│            │    Outbox Relay          │             │
│            ▼                         ▼             │
│   ┌─────────────────┐     ┌─────────────────────┐  │
│   │   Orders DB     │     │     Wallets DB      │  │
│   │   + Outbox      │     │     + Outbox        │  │
│   └────────┬────────┘     └──────────┬──────────┘  │
│            └──────────┬──────────────┘             │
│                       ▼                            │
│            ┌─────────────────┐                    │
│            │    RabbitMQ     │                    │
│            │  OrderPlaced ───────▶ WalletService  │
│            │  WalletDebited ──────▶ OrderService  │
│            │  WalletFailed ───────▶ OrderService  │
│            └─────────────────┘                    │
│               Saga (choreography)                  │
└─────────────────────────────────────────────────────┘

## Patterns

| Pattern | Where | Why |
|---|---|---|
| Outbox | Both services | Atomic event publishing — state change and event in one transaction |
| Saga (choreography) | OrderService + WalletService | Distributed transaction with compensation on failure |
| Circuit Breaker | OrderService → WalletService | Stop calling a broken service, fail fast |
| Retry + Timeout | OrderService → WalletService | Handle transient failures safely |
| Idempotency Key | Wallet withdrawal | Prevent double charge on retry |
| CQRS + MediatR | Both services | Separate read/write concerns, pipeline behaviours |
| Clean Architecture | Both services | Domain independent of infrastructure |

## How to Run

Create a `.env` file in the root with the following variables:
```env
SA_PASSWORD=
RABBITMQ_USER=
RABBITMQ_PASSWORD=
```

Then:
```bash
docker-compose up --build
```

- OrderService: `http://localhost:5001/swagger`
- WalletService: `http://localhost:5002/swagger`
- RabbitMQ UI: `http://localhost:15672`

## Testing the Saga

```bash
# seed wallet
POST /api/wallets/seed
{ "userId": "3fa85f64-5717-4562-b3fc-2c963f66afa6", "initialBalance": 500 }

# happy path — order → Confirmed
POST /api/orders
{ "userId": "3fa85f64-5717-4562-b3fc-2c963f66afa6", "amount": 100 }

# compensation path — order → Failed
POST /api/orders
{ "userId": "3fa85f64-5717-4562-b3fc-2c963f66afa6", "amount": 999999 }
```

## Testing the Circuit Breaker

Place orders repeatedly — after 5 failures circuit opens. Watch OrderService logs: