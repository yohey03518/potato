# Domain Knowledge: Stock Trading Simulator

## Core Domain
- **Project**: Stock Trading Simulator 
- **Primary Entity**: `StockSnapshot` (represents stock data at a point in time).

## Business Rules & Constraints
- **Financial Integrity**: All monetary and price calculations MUST use the `decimal` type to prevent floating-point errors.
- **Market Data Source**: Fugle Market Data (implemented via direct HTTP/WebSocket, not SDKs).
