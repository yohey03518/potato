# Implementation Plan: [FEATURE]

**Branch**: `[###-feature-name]` | **Date**: [DATE] | **Spec**: [link]
**Input**: Feature specification from `/specs/[###-feature-name]/spec.md`

**Note**: This template is filled in by the `/speckit.plan` command. See `.specify/templates/commands/plan.md` for the execution workflow.
**LANGUAGE CONSTRAINT**: All content generated using this template MUST be written in Traditional Chinese.

## Summary

本專案旨在開發一個「台股自動交易模擬系統」，透過 CLI 介面提供使用者即時的市場監控與模擬交易功能。系統將接收即時台股行情（Tick/Bid/Ask），依據技術指標（5分K SMA 20）執行自動化交易策略，並模擬真實市場的撮合邏輯。技術架構採 .NET 8 (LTS) 開發，使用 EF Core 搭配 MySQL 進行資料儲存，並透過 Docker 容器化部署。遵循憲法原則，所有外部 API（如行情數據）均直接透過 `HttpClient` 實作，不使用第三方 SDK。

## Technical Context

**Language/Version**: .NET 8 (LTS)
**Primary Dependencies**: 
- Microsoft.EntityFrameworkCore (EF Core)
- NUnit (Testing)
- NSubstitute (Mocking)
- Spectre.Console (CLI UI)
- Microsoft.Extensions.Hosting / DependencyInjection
**Storage**: MySQL (via EF Core)
**Testing**: NUnit
**Target Platform**: Linux (Docker container)
**Project Type**: Console Application (CLI) / Background Service
**Performance Goals**: Tick 處理延遲 < 100ms
**Constraints**: 
- **NO SDKs**: 外部 API 必須直接透過 `HttpClient` 介接。
- **Precision**: 金額與價格計算必須使用 `decimal` 類型。
- **Persistence**: 交易紀錄必須持久化至 MySQL。
**Scale/Scope**: 單一策略模擬，支援多檔股票監控，適用於台股交易時段。

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

- **Principle I (Financial Integrity)**: 使用 `decimal` 處理所有金額與價格計算。 ✅
- **Principle II (TDD)**: 採用 NUnit 與 NSubstitute 支援 TDD 流程。 ✅
- **Principle VII (Dependency Minimalism)**: **嚴格執行**，所有行情 API Client 均直接使用 `HttpClient` 實作，禁止引入第三方 SDK。 ✅
- **Principle IX (Language Localization)**: 所有產出物（含此計畫書、程式碼註解、UI）皆使用繁體中文。 ✅
- **Technology Standards**: 使用 .NET 8 LTS 與 C#。 ✅

**Gate Status**: PASS

## Project Structure

### Documentation (this feature)

```text
specs/001-stock-trading-sim/
├── plan.md              # This file
├── research.md          # Phase 0 output
├── data-model.md        # Phase 1 output
├── quickstart.md        # Phase 1 output
├── contracts/           # Phase 1 output
└── tasks.md             # Phase 2 output
```

### Source Code (repository root)

```text
src/
├── Potato.Trading.Core/           # 核心層：實體、介面、業務邏輯 (Domain)
│   ├── Entities/
│   ├── Interfaces/
│   ├── Services/
│   └── ValueObjects/
├── Potato.Trading.Infrastructure/ # 基礎設施層：資料庫、API Client (Infra)
│   ├── Data/
│   ├── MarketData/                # 直接使用 HttpClient 實作的 API Clients
│   └── Repositories/
└── Potato.Trading.Cli/            # 展示層：Console UI、程式進入點 (Presentation)
    ├── Commands/
    └── UI/

tests/
├── Potato.Trading.UnitTests/      # 單元測試
└── Potato.Trading.IntegrationTests/ # 整合測試
```

**Structure Decision**: 採用 Clean Architecture 分層架構（Core, Infrastructure, Cli），確保業務邏輯與外部依賴（MySQL, Market Data API）解耦。基礎設施層將直接負責 HTTP 通訊。

## Complexity Tracking

> **Fill ONLY if Constitution Check has violations that must be justified**

| Violation | Why Needed | Simpler Alternative Rejected Because |
|-----------|------------|-------------------------------------|
| [e.g., 4th project] | [current need] | [why 3 projects insufficient] |
| [e.g., Repository pattern] | [specific problem] | [why direct DB access insufficient] |
