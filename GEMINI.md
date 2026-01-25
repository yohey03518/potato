## Active Technologies
- .NET 8 (LTS) (001-stock-trading-sim)
- MySQL (via EF Core) (001-stock-trading-sim)

## Technical Standards
- **Language/Version**: .NET 8 (LTS) C#
- **Architecture**: Clean Architecture (Core, Infrastructure, Cli)
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
- **Constraints & Principles**:
  - **NO SDKs**: External APIs (e.g., Market Data) MUST be implemented directly via `HttpClient` or `WebSocket`. No third-party SDKs.
  - **Financial Integrity**: All monetary and price calculations MUST use the `decimal` type.
  - **Verification**: Always run all tests after modifying code to ensure stability.

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
└── Potato.Cli/            # Presentation Layer: Console UI, Entry Point (Presentation)
    ├── Commands/
    └── UI/
```

### Tests

```text
tests/
├── Potato.Core.Tests/           # Unit tests for Core layer
├── Potato.Infrastructure.Tests/ # Unit tests for Infrastructure layer
└── Potato.Cli.Tests/            # Tests for CLI layer
```

**Structure Decision**: Adopts Clean Architecture (Core, Infrastructure, Cli) to decouple business logic from external dependencies (MySQL, Market Data API). The Infrastructure layer is responsible for HTTP communication.
