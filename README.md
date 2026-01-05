# Test Auto-Dev

這是一個測試 Auto-Dev workflow 的專案。

## 使用方式

1. 建立 Issue
2. 加上 `auto-dev` label
3. 等待 Bot 自動建立 PR

## 或使用命令

在任何 Issue 留言：
```
/evolve 建立一個簡單的 Hello World 程式
```

## 專案內容

### 多人連線貪食蛇遊戲

一個支援多人即時連線的網頁貪食蛇遊戲。

#### 功能特色
- 即時多人連線對戰
- 單機模式（無需伺服器也可遊玩）
- 響應式設計
- 流暢的遊戲體驗
- 玩家排行榜

#### 快速開始

**單機模式：**
直接在瀏覽器中開啟 `snake-game.html` 即可開始遊玩。

**多人模式：**

1. 安裝依賴：
```bash
npm install
```

2. （可選）配置伺服器設定：
```bash
cp .env.example .env
# 編輯 .env 檔案來自訂伺服器設定
```

預設設定：
- `WS_PORT=8080` - WebSocket 伺服器埠號
- `WS_HOST=localhost` - WebSocket 伺服器主機（安全起見預設為 localhost）
- `GAME_SPEED=100` - 遊戲更新速度（毫秒）
- `TILE_COUNT=30` - 遊戲格子數量
- `MAX_FOOD_SPAWN_RETRIES=100` - 食物生成最大重試次數

3. 啟動 WebSocket 伺服器：
```bash
npm start
```

4. 在瀏覽器開啟 `snake-game.html`

5. 點擊「連線」按鈕連接到伺服器

6. 點擊「開始遊戲」開始遊玩

#### 遊戲操作
- 方向鍵 ↑↓←→ 或 WASD 控制蛇的移動
- 空白鍵暫停遊戲
- 吃到食物增加分數和蛇的長度
- 避免撞到牆壁或其他玩家

#### 技術架構
- 前端：純 HTML5 Canvas + JavaScript
- 後端：Node.js + WebSocket (ws)
- 即時通訊：WebSocket 協議

### 簡易計算機

一個功能完整的網頁計算機，支援基本運算。開啟 `calculator.html` 使用。
