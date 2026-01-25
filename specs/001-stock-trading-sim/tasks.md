# Tasks: 001-stock-trading-sim

**Feature**: 台股自動交易模擬系統 (Taiwan Stock Automated Trading System and Simulation)
**Status**: Pending
**Spec**: [spec.md](./spec.md) | **Plan**: [plan.md](./plan.md)

## Phase 1: Setup (環境建置)
*Goal: 初始化專案結構、Docker 環境與基礎設定。*

- [ ] T001 初始化 .NET 8 Solution `Potato.Trading.sln`，包含專案：`Core` (Class Lib), `Infrastructure` (Class Lib), `Cli` (Console), `UnitTests` (NUnit), `IntegrationTests` (NUnit) 於 `src/` 與 `tests/` 目錄中。
- [ ] T002 在專案根目錄建立 `Dockerfile` 與 `.dockerignore` 以支援容器化部署。
- [ ] T003 設定 `appsettings.json` (ConnectionStrings, Fugle Config) 並於 `src/Potato.Trading.Cli/Program.cs` 設定 Dependency Injection 與 Logging。
- [ ] T004 建立 `README.md`，說明建置與執行方式。

## Phase 2: Foundational (基礎設施)
*Goal: 實作核心實體、資料庫基礎設施與市場行情連線 (No-SDK)。*

- [ ] T005 [P] 定義 Domain Entities (`Watchlist`, `MarketData`, `KLine`, `Order`, `Execution`) 於 `src/Potato.Trading.Core/Entities/`。
- [ ] T006 [P] 定義 Repository Interfaces (`IOrderRepository`, `IWatchlistRepository`) 於 `src/Potato.Trading.Core/Interfaces/`。
- [ ] T007 設定 `TradingDbContext` 使用 `Pomelo.EntityFrameworkCore.MySql` 並設定 Entity Configurations (Precision) 於 `src/Potato.Trading.Infrastructure/Data/TradingDbContext.cs`。
- [ ] T008 使用 `HttpClient` 實作 `FugleMarketDataApiClient` (REST Snapshot) 於 `src/Potato.Trading.Infrastructure/MarketData/FugleMarketDataApiClient.cs`。
- [ ] T009 使用 `ClientWebSocket` 實作 `FugleWebSocketClient` (Streaming) 於 `src/Potato.Trading.Infrastructure/MarketData/FugleWebSocketClient.cs`。
- [ ] T010 實作 Repositories (`OrderRepository`, `WatchlistRepository`) 於 `src/Potato.Trading.Infrastructure/Repositories/`。
- [ ] T011 建立 EF Core Migrations 與 `init.sql` 腳本以初始化 MySQL schema。

## Phase 3: User Story 1 - 策略監控與訊號觸發 (Priority: P1)
*Goal: 實作資料接收、KLine 聚合、SMA 計算與訊號產生。*

- [ ] T012 [US1] 實作 `MarketDataService` 以接收 Ticks 並聚合為 5分鐘 `KLine` 於 `src/Potato.Trading.Core/Services/MarketDataService.cs`。
- [ ] T013 [US1] 實作 `SmaIndicator` 邏輯 (計算 SMA 20) 於 `src/Potato.Trading.Core/Indicators/SmaIndicator.cs`。
- [ ] T014 [US1] 實作 `StrategyEvaluator` 以依據 KLine 與 SMA 邏輯產生 Buy/Sell 訊號於 `src/Potato.Trading.Core/Services/StrategyEvaluator.cs`。
- [ ] T015 [US1] 實作 `WatchlistService` 以依據前日數據篩選每日觀察股於 `src/Potato.Trading.Core/Services/WatchlistService.cs`。
- [ ] T016 [US1] 建立 `StrategyEvaluator` (訊號邏輯) 的單元測試於 `tests/Potato.Trading.UnitTests/Services/StrategyEvaluatorTests.cs`。

## Phase 4: User Story 2 - 擬真撮合模擬 (Priority: P1)
*Goal: 模擬下單執行，包含排隊撮合與真實市場 Tick 判定。*

- [ ] T017 [US2] 實作 `OrderBookTracker` 以管理限價單的排隊順序於 `src/Potato.Trading.Core/Domain/OrderBookTracker.cs`。
- [ ] T018 [US2] 實作 `MatchingEngine` 以依據 Tick 資料與佇列位置模擬成交於 `src/Potato.Trading.Core/Services/MatchingEngine.cs`。
- [ ] T019 [US2] 實作 `TradingService` 以協調訊號、委託單與撮合邏輯於 `src/Potato.Trading.Core/Services/TradingService.cs`。
- [ ] T020 [US2] 建立 `MatchingEngine` (確定性重播) 的單元測試於 `tests/Potato.Trading.UnitTests/Services/MatchingEngineTests.cs`。

## Phase 5: User Story 3 - 當沖強迫平倉 (Priority: P2)
*Goal: 確保所有部位於收盤前 (13:25) 平倉。*

- [ ] T021 [US3] 實作 `TimeScheduler` 或 `MarketClock` 以觸發時間驅動事件於 `src/Potato.Trading.Core/Services/MarketClock.cs`。
- [ ] T022 [US3] 實作 `LiquidationService` 以於 13:25 取消未成交單並強制平倉於 `src/Potato.Trading.Core/Services/LiquidationService.cs`。
- [ ] T023 [US3] 針對 `LiquidationService` 邏輯進行單元測試於 `tests/Potato.Trading.UnitTests/Services/LiquidationServiceTests.cs`。

## Phase 6: User Story 4 - 交易紀錄保存 (Priority: P3)
*Goal: 確保所有交易事件皆持久化至資料庫。*

- [ ] T024 [US4] 驗證並確保 `TradingService` 透過 Repositories 儲存 `Order` 與 `Execution` 狀態於 `src/Potato.Trading.Core/Services/TradingService.cs`。
- [ ] T025 [US4] 建立完整交易生命週期持久化的整合測試於 `tests/Potato.Trading.IntegrationTests/Repositories/OrderRepositoryTests.cs`。

## Phase 7: Polish & UI (優化與介面)
*Goal: 建置 CLI Dashboard 並完成應用程式。*

- [ ] T026 實作 Spectre.Console Dashboard (即時報價、部位、訊號) 於 `src/Potato.Trading.Cli/UI/Dashboard.cs`。
- [ ] T027 實作 `Worker` (BackgroundService) 以執行主要交易迴圈於 `src/Potato.Trading.Cli/Worker.cs`。
- [ ] T028 執行最終手動測試並依據 Success Criteria 進行驗證。

## Dependencies (依賴關係)

- **US1** 依賴 **Foundational** (市場數據, 實體)。
- **US2** 依賴 **US1** (需先產生訊號才能進行撮合)。
- **US3** 依賴 **US2** (平倉需依賴執行引擎)。
- **US4** 依賴 **US2** (持久化需依賴產生的事件)。

## Implementation Strategy (實作策略)

1. **MVP Scope**: 完成 Phase 1, 2, 3, 4 (Setup, Infra, Strategy, Matching)。這允許執行核心模擬邏輯。
2. **Safety**: Phase 5 (Liquidation) 對於「模擬」的真實性至關重要 (避免留倉風險)。
3. **Refinement**: Phase 7 將透過 UI 呈現結果給使用者。
