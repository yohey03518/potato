# Research: 台股自動交易模擬系統技術調查

## 1. 行情數據來源 (Market Data Source)

### 決策：優先使用 Fugle (富果) RealTime API
- **原因**：提供完整的 REST API 與 WebSocket 接口，且文件詳盡。雖然有提供 SDK，但其核心功能可直接透過 HTTP Request 與 WebSocket 達成，符合本專案「不使用 SDK」的限制。
- **替代方案**：
  - **Fubon (富邦) API**：同樣提供 Web API 與 WebSocket，但需開戶且權限申請較嚴格。
  - **Twelve Data**：國際級提供商，有台股數據，但即時性與本地 provider 相比可能稍有延遲。

### 實作重點 (No-SDK)
- **Authentication**: 使用 HTTP Header 帶入 `X-API-KEY`。
- **WebSocket**: 使用 .NET 內建的 `ClientWebSocket` 直接連線至 `wss://api.fugle.tw/marketdata/v1.0/stock/streaming`。

## 2. 資料庫與 EF Core 最佳實踐

### 決策：使用 Pomelo.EntityFrameworkCore.MySql
- **原因**：.NET 社群中 MySQL 的事實標準。
- **財務精準度 (Financial Precision)**：
  - **C# 類型**：一律使用 `decimal`。
  - **資料庫類型**：使用 `DECIMAL(18, 4)` 或 `DECIMAL(19, 6)`。
  - **配置方式**：在 `DbContext` 中使用 Fluent API：
    ```csharp
    modelBuilder.Entity<Order>()
        .Property(o => o.Price)
        .HasPrecision(18, 4);
    ```

## 3. 容器化與 CI/CD

### 決策：GitHub Actions + Docker + GHCR
- **Docker 基礎鏡像**：`mcr.microsoft.com/dotnet/runtime:8.0` (執行環境) 與 `mcr.microsoft.com/dotnet/sdk:8.0` (編譯環境)。
- **GitHub Actions 流程**：
  1. `dotnet restore` & `dotnet build`
  2. `dotnet test` (執行 NUnit 測試)
  3. `docker build` 封裝 Console App。
  4. `docker push` 至 GitHub Container Registry (GHCR)。

## 4. 外部排程啟動 (External Scheduler)

### 決策：使用 GitHub Actions Schedule 或外部 Cron Job 觸發 Docker Run
- **方案**：由於系統需在台股交易時段運行（09:00 - 13:30），可透過排程在 08:30 啟動 Docker 容器，並在 14:00 停止。
- **內部邏輯**：系統啟動後應先執行「開盤前選股策略」，隨後進入監控循環。
