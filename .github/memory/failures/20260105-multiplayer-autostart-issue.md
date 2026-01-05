# 2026-01-05 多人連線模式自動開始問題

## 問題描述
多人連線貪食蛇遊戲在玩家連線後，蛇不會自動顯示，必須手動點擊「開始遊戲」按鈕才能看到遊戲畫面。

## 原因分析
1. **缺少自動啟動機制**：客戶端在收到伺服器 `init` 訊息後，只建立了玩家物件，但沒有啟動遊戲循環
2. **繪圖邏輯問題**：`updateGame()` 函式中的 `!mySnake` 檢查導致多人模式下無法繪製畫面

## 解決方案

### 1. 自動啟動遊戲 (snake-game.html:330-331)
在收到 `init` 訊息時自動呼叫 `startGame()` 和 `updatePlayerList()`：

```javascript
case 'init':
    playerId = data.playerId;
    mySnake = createSnake(data.color);
    players[playerId] = {
        id: playerId,
        snake: mySnake,
        color: data.color,
        score: 0
    };
    updatePlayerList();  // 新增
    startGame();        // 新增
    break;
```

### 2. 改善繪圖邏輯 (snake-game.html:478-486)
修改 `updateGame()` 函式，確保多人模式持續繪製：

```javascript
function updateGame() {
    if (!gameRunning || gamePaused) return;  // 移除 !mySnake 檢查

    if (!connected && mySnake) {  // 改為檢查是否在單機模式且有蛇
        updateLocalGame();
    }

    draw();
}
```

## 適用情境
- 多人連線遊戲需要連線後立即開始
- WebSocket 遊戲客戶端的初始化流程
- 需要區分單機模式和多人模式的遊戲邏輯

## 學到的教訓
1. 伺服器端和客戶端的遊戲狀態需要同步
2. 客戶端繪圖循環應該獨立於遊戲邏輯
3. 條件檢查要考慮不同模式（單機/多人）的需求
