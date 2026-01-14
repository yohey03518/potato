# Quickstart: 台股自動交易模擬系統

## 1. 環境準備
- **.NET SDK**: 8.0+
- **Database**: MySQL 8.0+
- **API Key**: 需準備 Fugle Market Data API Key。

## 2. 設定檔 (appsettings.json)
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=trading_sim;User=root;Password=yourpassword;"
  },
  "Trading": {
    "FugleApiKey": "YOUR_API_KEY",
    "TotalCapital": 1000000,
    "FeeRate": 0.001425,
    "TaxRate": 0.003
  }
}
```

## 3. 執行步驟
1. **資料庫遷移**:
   ```bash
   dotnet ef database update --project src/Potato.Trading.Infrastructure --startup-project src/Potato.Trading.Cli
   ```
2. **啟動模擬系統**:
   ```bash
   dotnet run --project src/Potato.Trading.Cli
   ```

## 4. 測試
- **單元測試**: `dotnet test tests/Potato.Trading.UnitTests`
- **整合測試**: `dotnet test tests/Potato.Trading.IntegrationTests` (需資料庫)

## 5. Docker 執行
```bash
docker build -t potato-trading-sim .
docker run --env-file .env potato-trading-sim
```
