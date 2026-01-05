# WebSocket 遊戲大廳模式

## 使用情境
多人即時遊戲需要玩家在遊戲開始前的等待和協調機制。

## 程式碼範例

### 伺服器端狀態管理
```javascript
// 全域狀態
let gameState = 'lobby'; // 'lobby' | 'playing'
let roomHost = null;

// 玩家加入時
players[playerId] = {
    id: playerId,
    ready: false,
    snake: null // 大廳中尚未建立遊戲物件
};

// 設定房主
if (!roomHost) {
    roomHost = playerId;
}

// 廣播大廳狀態
function broadcastLobbyState() {
    broadcast({
        type: 'lobbyUpdate',
        players: Object.entries(players).map(([id, p]) => ({
            id,
            ready: p.ready,
            isHost: id === roomHost
        })),
        gameState: gameState
    });
}
```

### 準備機制
```javascript
// 處理準備訊息
if (data.type === 'ready') {
    players[playerId].ready = !players[playerId].ready;
    broadcastLobbyState();

    // 檢查自動開始條件
    const allReady = Object.values(players).every(p => p.ready);
    if (allReady && Object.keys(players).length >= 2) {
        setTimeout(() => {
            if (gameState === 'lobby') startGame();
        }, 3000);
    }
}
```

### 遊戲開始
```javascript
function startGame() {
    gameState = 'playing';

    // 為所有玩家建立遊戲物件
    Object.values(players).forEach(player => {
        player.snake = createSnake(player.color);
        player.ready = false;
    });

    broadcast({ type: 'gameStart' });
    startGameLoop();
}
```

### 返回大廳
```javascript
function endGame() {
    gameState = 'lobby';

    // 清理遊戲物件
    Object.values(players).forEach(player => {
        player.snake = null;
        player.ready = false;
    });

    stopGameLoop();
    broadcast({ type: 'gameEnd' });
    broadcastLobbyState();
}
```

### 客戶端 UI 切換
```javascript
function handleServerMessage(data) {
    switch (data.type) {
        case 'init':
            isHost = data.isHost;
            showLobby();
            break;

        case 'lobbyUpdate':
            updateLobbyDisplay(data.players);
            break;

        case 'gameStart':
            showGame();
            startGameLoop();
            break;

        case 'gameEnd':
            showLobby();
            stopGameLoop();
            break;
    }
}

function showLobby() {
    document.getElementById('lobbyContainer').classList.add('active');
    document.getElementById('gameContainer').classList.remove('active');
}

function showGame() {
    document.getElementById('lobbyContainer').classList.remove('active');
    document.getElementById('gameContainer').classList.add('active');
}
```

## 關鍵要點
1. **狀態分離**：大廳狀態和遊戲狀態分開管理
2. **延遲建立**：遊戲物件在遊戲開始時才建立，避免資源浪費
3. **雙向控制**：支援自動開始和手動開始兩種模式
4. **權限管理**：房主有特殊權限但會自動轉移
5. **可重複性**：遊戲結束後返回大廳而非斷線
