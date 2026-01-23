# Potato Trading Simulator

台股自動交易模擬系統 (Taiwan Stock Automated Trading System and Simulation)

## 專案簡介

本系統為一個基於 .NET 8 (LTS) 開發的 CLI 應用程式，旨在模擬台股自動化交易策略。系統透過接收即時市場行情 (Tick/Bid/Ask)，計算技術指標 (SMA)，並依據預設策略進行模擬撮合與交易。

## 功能特色

- **即時行情監控**: 串接 Fugle (富果) API 接收即時報價。
- **自動化策略**: 支援 5分K 進出與 SMA 20 均線策略。
- **擬真撮合**: 模擬真實市場的排隊與成交邏輯。
- **風險控制**: 包含資金管理、停損停利與收盤強制平倉機制。
- **CLI 儀表板**: 使用 Spectre.Console 提供終端機視覺化介面。

## 環境需求

- .NET 8.0 SDK
- Docker & Docker Compose (選用，用於資料庫)
- MySQL 8.0+

## 快速開始

### 1. 設定環境

請複製 `src/Potato.Trading.Cli/appsettings.json` 並設定您的 Fugle API Key 與資料庫連線字串。

### 2. 建置專案

```bash
dotnet build
```

### 3. 執行測試

```bash
dotnet test
```

### 4. 啟動模擬器

```bash
dotnet run --project src/Potato.Trading.Cli
```

## 專案結構

- `src/Potato.Trading.Core`: 核心業務邏輯、實體與介面。
- `src/Potato.Trading.Infrastructure`: 資料庫存取與外部 API 實作。
- `src/Potato.Trading.Cli`: 應用程式進入點與使用者介面。
- `tests/`: 單元測試與整合測試。

## 授權

MIT License
