# Contract: Fugle Market Data API (No-SDK)

## 1. REST API: 快照 (Snapshot)
**Endpoint**: `GET /marketdata/v1.0/stock/snapshot/{symbol}`

### Response Schema (JSON)
```json
{
  "symbol": "2330",
  "lastTrade": {
    "price": 585.0,
    "size": 10,
    "time": "2026-01-14T13:30:00.000Z"
  },
  "bids": [
    {"price": 584.0, "size": 100},
    {"price": 583.0, "size": 150}
  ],
  "asks": [
    {"price": 585.0, "size": 80},
    {"price": 586.0, "size": 200}
  ]
}
```

## 2. WebSocket: 即時成交 (Ticking)
**Endpoint**: `wss://api.fugle.tw/marketdata/v1.0/stock/streaming`

### Message Schema
```json
{
  "event": "data",
  "data": {
    "symbol": "2330",
    "type": "TRADE",
    "price": 585.0,
    "size": 5,
    "time": "2026-01-14T13:31:05.000Z"
  }
}
```

## 3. WebSocket: 五檔更新 (OrderBook)
### Message Schema
```json
{
  "event": "data",
  "data": {
    "symbol": "2330",
    "type": "QUOTE",
    "bids": [ ... ],
    "asks": [ ... ],
    "time": "2026-01-14T13:31:05.000Z"
  }
}
```
