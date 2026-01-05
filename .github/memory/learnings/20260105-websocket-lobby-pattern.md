# 2026-01-05 WebSocket 多人遊戲大廳模式

## 學到的內容

### 1. 狀態管理架構
在 WebSocket 多人遊戲中，使用明確的狀態機管理遊戲流程：
- 伺服器端維護全域 `gameState`（lobby/playing）
- 客戶端同步維護 `currentGameState` 並據此切換 UI
- 使用廣播機制確保所有客戶端狀態同步

### 2. 房主轉移機制
房主系統需要考慮動態變化：
```javascript
// 房主離開時重新指定
if (playerId === roomHost) {
    const remainingPlayers = Object.keys(players);
    if (remainingPlayers.length > 0) {
        roomHost = remainingPlayers[0];
    } else {
        roomHost = null;
    }
}
```

### 3. 準備狀態協調
實作準備機制時的關鍵點：
- 使用布林值儲存每位玩家的準備狀態
- 檢查所有玩家是否都準備好時使用 `Array.every()`
- 自動開始前設定延遲（3 秒）給玩家反應時間
- 使用 `setTimeout` 時檢查狀態避免重複開始

### 4. UI 狀態切換
使用 CSS class 控制大廳和遊戲畫面切換：
```css
.lobby-container { display: none; }
.lobby-container.active { display: block; }
.game-container { display: none; }
.game-container.active { display: block; }
```

### 5. WebSocket 訊息設計
清晰的訊息類型定義：
- `init` - 玩家初始化（包含 isHost 資訊）
- `lobbyUpdate` - 大廳狀態更新
- `ready` - 切換準備狀態
- `startGame` - 房主開始遊戲
- `gameStart` - 通知所有人遊戲開始
- `gameEnd` - 通知所有人返回大廳

## 適用情境
- 需要玩家協調的多人即時遊戲
- 房間制遊戲系統
- 需要等待機制的線上對戰遊戲
- 支援多局遊戲的應用（遊戲後返回大廳）

## 最佳實踐
1. 始終驗證玩家權限（如房主操作）
2. 廣播狀態更新時包含完整必要資訊
3. 客戶端 UI 應即時反映準備狀態變化
4. 使用視覺提示（顏色、圖示）區分不同狀態
5. 提供多種開始遊戲的方式增加彈性
