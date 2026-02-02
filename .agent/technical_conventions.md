# Technical Conventions

## AI Instructions
- If there is any ambiguous or unclear command from human, ask before doing.

## Technical Standards
- **Language/Version**: .NET 8 (LTS) C#
- **Architecture**: Clean Architecture (Core, Infrastructure, Client)
- **Primary Dependencies**:
  - Microsoft.EntityFrameworkCore (EF Core)
  - NUnit (Testing)
  - FluentAssertions (Assertions)
  - NSubstitute (Mocking)
  - Spectre.Console (CLI UI)
  - Serilog (Logging)
  - Microsoft.Extensions.Hosting / DependencyInjection
- **Storage**: MySQL (via EF Core)
- **Testing**: NUnit, FluentAssertions, TDD approach

## Constraints & Principles
- **NO SDKs**: External APIs (e.g., Market Data) MUST be implemented directly via `HttpClient` or `WebSocket`. No third-party SDKs.
- **Clean Architecture**:
    - **Core**: Entities and Interfaces (e.g., `IMarketDataProxy`, `StockSnapshot`) reside here. NO dependencies on Infrastructure or Client.
    - **Infrastructure**: Implementation of Interfaces (e.g., `FugleMarketDataProxy`, HTTP Clients) resides here. Depends on Core.
    - **Client**: Entry point and UI. Depends on Core and Infrastructure (for DI registration only).
- **Abstraction**: External APIs MUST be wrapped in a Proxy Layer (e.g., `IMarketDataProxy`) using Domain Models. DO NOT leak external API models (DTOs) into the Core/Service layers.
- **Verification**: Always run all tests after modifying code to ensure stability.
- **No API Client Tests**: Do not write unit tests for infrastructure API clients (e.g. `FugleTechnicalApiClient`). Only integration or manual tests are required for these.

## Project Structure

### Source Code

```text
src/
├── Potato.Core/           # Core Layer: Entities, Interfaces, Business Logic (Domain)
│   ├── Entities/
│   ├── Interfaces/
│   ├── Services/
│   └── ValueObjects/
├── Potato.Infrastructure/ # Infrastructure Layer: Database, API Clients (Infra)
│   ├── Data/
│   ├── MarketData/        # API Clients implemented directly with HttpClient
│   └── Repositories/
└── Potato.Client/         # Presentation Layer: Console UI, Entry Point (Presentation)
    ├── Commands/
    └── UI/
```

### Tests

```text
tests/
├── Potato.Core.Tests/           # Unit tests for Core layer
├── Potato.Infrastructure.Tests/ # Unit tests for Infrastructure layer
└── Potato.Client.Tests/         # Tests for Client layer
```

**Structure Decision**: Adopts Clean Architecture (Core, Infrastructure, Client) to decouple business logic from external dependencies (MySQL, Market Data API). The Infrastructure layer is responsible for HTTP communication.
