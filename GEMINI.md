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

## Project Structure (Clean Architecture)
- **src/Potato.Trading.Core/**: Core Layer (Domain) - Entities, Interfaces, Business Logic, ValueObjects.
- **src/Potato.Trading.Infrastructure/**: Infrastructure Layer (Infra) - Database (EF Core), MarketData (HttpClient API Clients), Repositories.
- **src/Potato.Trading.Cli/**: Presentation Layer (Presentation) - Console UI (Spectre.Console), Entry Point.
- **tests/**: Unit and Integration Tests.
