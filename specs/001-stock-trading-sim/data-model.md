# Data Model: 台股自動交易模擬系統

## 實體定義 (Entities)

### 1. Watchlist (每日觀察名單)
- `Id`: Guid
- `TradeDate`: DateTime (日期)
- `Symbol`: string (股票代碼)
- `Direction`: enum (Long/Short, 依昨日收盤價與月線關係決定)
- `BasePrice`: decimal (昨日收盤價)
- `MA20_Day`: decimal (昨日月線值)

### 2. MarketData (即時行情快照)
- `Symbol`: string
- `Price`: decimal (成交價)
- `Volume`: long (單筆成交量)
- `BidPrices`: decimal[] (五檔買價)
- `BidVolumes`: long[] (五檔買量)
- `AskPrices`: decimal[] (五檔賣價)
- `AskVolumes`: long[] (五檔賣量)
- `Timestamp`: DateTime

### 3. KLine (5分鐘K線)
- `Symbol`: string
- `StartTime`: DateTime
- `Open`: decimal
- `High`: decimal
- `Low`: decimal
- `Close`: decimal
- `Volume`: long
- `SMA20`: decimal (5分K之20期均線)

### 4. Order (委託單)
- `Id`: Guid
- `Symbol`: string
- `Side`: enum (Buy/Sell)
- `Type`: enum (Market/Limit)
- `Price`: decimal (委託價格)
- `Quantity`: int (股數)
- `Status`: enum (Pending/PartialFilled/Filled/Cancelled/Rejected)
- `QueuePositionVolume`: long (下單時該價位的排隊總量)
- `FilledVolume`: int (已成交量)
- `CreatedAt`: DateTime

### 5. Execution (成交紀錄)
- `Id`: Guid
- `OrderId`: Guid
- `Price`: decimal
- `Quantity`: int
- `ExecutedAt`: DateTime
- `Fee`: decimal (手續費)
- `Tax`: decimal (交易稅)

## 狀態轉換與校驗規則

### 委託狀態轉換
- `Pending` -> `Filled` (全數成交)
- `Pending` -> `PartialFilled` -> `Filled`
- `Pending` -> `Cancelled` (收盤強平前的撤單)

### 資金校驗 (FR-011)
- 每筆投入：200,000 TWD
- 最小張數：2 張 (2,000 股)
- 單價限制：Price <= 100 TWD
- 停損距離：(Price - SMA20) / Price <= 2%
