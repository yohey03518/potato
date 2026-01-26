# Feature Specification: 台股自動交易模擬系統 (Taiwan Stock Trading Sim)

**Feature Branch**: `001-stock-trading-sim`  
**Created**: 2026-01-13  
**Status**: Draft  
**Input**: User description: "Taiwan Stock Automated Trading System and Simulation" (from requirement-prompt.md) & "Stable Profit Architecture Update" (2026-01-27)
**LANGUAGE CONSTRAINT**: All content generated using this template MUST be written in Traditional Chinese.

## User Scenarios & Testing *(mandatory)*

## Clarifications

### Session 2026-01-13
- Q: 系統的行情數據來源為何？ → A: 使用真實的 Live 交易資料（具體 API 於規劃階段確認）。
- Q: 使用者如何監控系統運行狀態與查看訊號？ → A: 使用 CLI / Terminal UI（終端機介面）。
- Q: 交易紀錄與行情資料的儲存方式為何？ → A: 推遲至規劃階段決定（Deferred to Planning）。
- Q: 策略的停損與停利邏輯為何？ → A: 複合式邏輯：(1) 進場前檢查停損距離若 >2% 則放棄；(2) 獲利達 5% 時先平倉 50%；(3) 其餘部位依策略訊號（跌破20MA、收盤強平、漲跌停）出場。
- Q: 如何決定每日的「觀察標的清單」？ → A: 自動選股策略：系統每日開盤前依據前日數據自動篩選符合條件的股票。
- Q: 交易成本（手續費與稅金）是否需要納入模擬計算？ → A: 是，且需支援使用者自訂手續費率與證交稅率。
- Q: 進場前檢查的「停損距離」具體如何定義？ → A: 預計下單金額與當前 5 分K線 20MA 的價格差距不可超過 2%。
- Q: 歷史模擬結果是否需要保存以便跨策略比較？ → A: 是，每次交易紀錄皆需持久化，以便未來比較不同策略（雖本次僅實作單一策略）的績效。
- Q: 盤中是否會動態調整觀察標的名單？ → A: 否，每日觀察名單依前日數據固定，盤中不變更，盤中變化僅作為策略下單之依據。
- Q: 若模擬的「限價委託單」在收盤強平時間（13:25）尚未成交，該如何處理？ → A: 先取消（Cancel）所有未成交的委託單，再針對現有部位執行強制平倉。

### Session 2026-01-14
- Q: 實作程式語言為何？ → A: 推遲至規劃階段決定（Deferred to Planning）。
- Q: 策略使用的 20MA 與選股用的月線具體類型為何？ → A: 簡單移動平均 (SMA)。
- Q: 交易資料的持久化儲存技術為何？ → A: 推遲至規劃階段決定（Deferred to Planning）。
- Q: 資金管理與下單規模為何？ → A: 每筆交易以 20 萬元為基準，且必須能買入至少 2 張（2000股）才觸發交易；若股價過高（單價 > 100 元）則忽略該標的。
- Q: 資金限制處理與總資金設定為何？ → A: 採「第一訊號優先」；系統需支援透過參數設定「總投入資金」，當可用資金不足 20 萬時忽略後續訊號。
- Q: 系統是否支援歷史數據回測 (Backtest)？ → A: 否，僅支援於真實交易時段進行即時模擬交易 (Live Paper Trading)。

### Session 2026-01-27 (Architecture Update)
- Q: 系統整體架構流程為何？ → A: Background Job (初階篩選) -> Strategy Scanner (策略適配) -> Order Execution (模擬下單) -> Position Monitor (監控池/出場管理)。
- Q: 初階篩選 (Background Job) 的規則為何？ → A: 排除 ETF、排除昨日成交量 < 5000 張的股票。
- Q: 策略掃描與執行的關係？ → A: 針對通過初篩的股票，系統會即時掃描是否符合「準備好的多種策略」中的任一種；若符合則執行下單。
- Q: 成交後的處理流程？ → A: 成交後將該股票移入「監控池 (Monitoring Pool)」，由監控池專責處理該策略對應的停損、停利與收盤強平邏輯。

### User Story 1 - 初階篩選與策略掃描 (Priority: P1)

系統需有一個 Background Job 在盤前或盤中不斷過濾股票，移除不適合的標的（如 ETF、流動性不足），並針對留下的股票進行即時掃描，判斷是否符合特定策略的進場條件。

**Why this priority**: 這是交易漏斗的最上層，決定了後續要監控哪些股票以及何時觸發交易。

**Independent Test**: 提供包含 ETF 與低量股票的清單，驗證系統過濾後是否僅剩符合條件的股票；並輸入特定 K 線形態驗證是否能匹配到對應策略。

**Acceptance Scenarios**:

1. **Given** 股票清單包含 ETF (如 0050) 與 昨日成交量 4000 張的股票, **When** 執行初階篩選, **Then** 這些股票應被移除，不進入策略掃描清單。
2. **Given** 通過篩選的股票, **When** 盤中走勢符合「策略 A (如 SMA 20 突破)」的進場條件, **Then** 系統應判定該股票「適配策略 A」並發出買入訊號。
3. **Given** 股票同時符合多個策略, **When** 掃描時, **Then** 系統應依據預設優先權或特定規則選擇一種策略執行（本次實作先採 First-Match）。
4. **Given** 股票不符合任何策略, **When** 盤中掃描, **Then** 系統繼續監控，不執行任何動作。

---

### User Story 2 - 模擬下單與入池監控 (Priority: P1)

當策略掃描器發出訊號後，系統需進行模擬下單（參考原擬真撮合邏輯）。**成交後**，該股票必須被放入「監控池」，之後的出場邏輯（停損、停利、強平）全權由監控池負責，與掃描器脫鉤。

**Why this priority**: 確保「進場」與「出場」職責分離，掃描器專注找機會，監控池專注守護獲利與風險。

**Independent Test**: 模擬一筆買單成交，驗證該股票是否立即出現在「監控池」清單中，且監控池能獨立觸發停損賣單。

**Acceptance Scenarios**:

1. **Given** 策略訊號觸發並完成模擬成交, **When** 成交確認, **Then** 該股票應立即被移入「Active Position Monitoring Pool」。
2. **Given** 股票在監控池中, **When** 市場價格觸及該策略設定的停損點 (如 < 2%) 或 停利點 (如 > 5%), **Then** 監控池應立即發出平倉訊號。
3. **Given** 股票在監控池中, **When** 時間到達 13:25, **Then** 監控池應強制執行平倉（收盤強平）。
4. **Given** 模擬單尚未成交（排隊中）, **When** 價格遠離導致策略失效, **Then** 應取消委託，且該股票**不**進入監控池。

---

### User Story 3 - 擬真撮合模擬 (Priority: P1)

(沿用原設計) 收到交易訊號後，系統需模擬下單行為。根據當下市場的五檔掛單（最佳五檔買賣價量）記錄排隊順序，並依據後續真實成交的價格與單量，判定模擬單是否成交。

**Why this priority**: 驗證策略有效性的基礎。

**Independent Test**: 模擬掛出一張限價單，接著輸入一系列市場成交資料（Tick Data），驗證系統是否在市場成交量累積超過排隊量後才標記為「成交」。

**Acceptance Scenarios**:

1. **Given** 發出限價買單, **When** 當下市場賣一價格高於委託價（需排隊）, **Then** 系統記錄當前委託單在買盤中的排隊順序。
2. **Given** 委託單處於排隊狀態, **When** 市場後續成交價格等於委託價且累積成交量超過排隊順序, **Then** 系統判定該模擬單成交，並記錄成交時間與價格。

---

### User Story 4 - 交易紀錄保存 (Priority: P3)

(沿用原設計) 系統需完整保存每一筆交易的生命週期。

**Acceptance Scenarios**:

1. **Given** 一筆委託單經歷下單、排隊、成交、入池監控、平倉, **When** 完整交易結束, **Then** 資料庫應存有完整的紀錄。

---

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: 系統必須包含一個「背景過濾服務 (Background Filter Service)」，持續或定期依據規則（如：排除 ETF、排除昨日成交量 < 5000 張）篩選出合格的觀察標的。
- **FR-002**: 系統必須實作「策略掃描器 (Strategy Scanner)」，針對合格標的即時運算多種策略條件（本次以 SMA 20 策略為首個實作範例）。
- **FR-003**: 系統必須維護一個「監控池 (Monitoring Pool)」，專門管理已成交的持倉部位，並負責執行該部位對應策略的出場邏輯（停損、停利、強平）。
- **FR-004**: 系統必須能透過 API 接收台股即時行情數據（Live Market Data），包含 Tick、Bid/Ask 與 KLine。
- **FR-005**: 系統必須實作「擬真撮合引擎」，依據五檔掛單量計算排隊順序，判定模擬單的成交狀態。
- **FR-006**: 系統必須在每日 13:25（收盤前5分鐘）由監控池觸發「強制平倉機制」。
- **FR-007**: 系統必須支援複合式風控：(1) 單筆 20 萬；(2) 股價 > 100 元不交易；(3) 停損距離 > 2% 不進場；(4) 獲利 5% 半數停利。
- **FR-008**: 系統必須將所有交易事件（訊號、委託、成交、入池、出場）寫入持久化儲存。
- **FR-009**: 系統必須提供 CLI 介面，顯示：(1) 目前觀察清單；(2) 監控池中的持倉狀態；(3) 當日已結算損益。

### Key Entities *(include if feature involves data)*

- **StockFilter**: 篩選規則定義（如 IsETF, MinVolume）。
- **Strategy**: 策略介面，定義 EntryCondition (進場) 與 ExitCondition (出場)。
- **MonitoringPool**: 管理 ActivePosition 的容器。
- **ActivePosition**: 目前持倉，包含成本、數量、對應策略 ID、即時損益。
- **MarketTick, KLine, Order, Execution**: (同原設計)

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: 篩選正確性：系統能正確過濾掉所有 ETF 與成交量不足的股票，誤判率 0%。
- **SC-002**: 狀態流轉正確性：從 掃描 -> 下單 -> 成交 -> 入池 -> 平倉 的狀態流轉無死鎖或狀態遺失。
- **SC-003**: 系統穩定性：同時監控池內有 5 檔股票且觀察清單有 50 檔股票時，Tick 處理延遲仍 < 100ms。
- **SC-004**: 停損停利觸發延遲：當行情觸發監控池內的停損/停利點時，系統需在 1 秒內產生平倉委託。