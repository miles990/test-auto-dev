# 單一檔案遊戲結構模式

## 使用情境
建立可獨立執行的 HTML5 遊戲，所有代碼（HTML、CSS、JavaScript）包含在單一檔案中。

## 程式碼範例

### 基本結構
```html
<!DOCTYPE html>
<html>
<head>
    <style>
        /* 內嵌 CSS 樣式 */
    </style>
</head>
<body>
    <canvas id="gameCanvas"></canvas>
    <script>
        // 遊戲物件定義
        const gameState = { /* 狀態 */ };
        const player = { /* 玩家屬性 */ };

        // 遊戲邏輯函數
        function update() { /* 更新邏輯 */ }
        function draw() { /* 繪製邏輯 */ }

        // 遊戲迴圈
        function gameLoop() {
            update();
            draw();
            requestAnimationFrame(gameLoop);
        }

        gameLoop();
    </script>
</body>
</html>
```

### 核心組件
1. **遊戲狀態管理**：使用物件儲存遊戲狀態
2. **輸入處理**：addEventListener 監聽鍵盤/滑鼠事件
3. **物理系統**：重力、速度、碰撞偵測
4. **遊戲迴圈**：使用 requestAnimationFrame 確保流暢性
5. **繪製系統**：Canvas 2D Context API

### 優點
- 易於分享和部署（只需開啟 HTML 檔案）
- 無需建置工具或伺服器
- 適合快速原型開發
- 便於版本控制和追蹤變更
