# 獨立 HTML 應用程式模式

## 使用情境
- 建立簡單的網頁工具或小遊戲
- 需要單一檔案即可執行的應用程式
- 不需要外部依賴的互動式網頁

## 設計原則
1. 所有資源（CSS、JavaScript）內嵌在同一個 HTML 檔案中
2. 使用現代 CSS 特性（漸層、backdrop-filter、陰影）創造視覺效果
3. 使用 JavaScript 實現動態功能
4. 確保響應式設計，支援各種螢幕尺寸

## 程式碼範例

### 基本結構
```html
<!DOCTYPE html>
<html lang="zh-TW">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>應用程式標題</title>
    <style>
        /* 內嵌 CSS */
        body {
            margin: 0;
            padding: 0;
        }

        /* 響應式設計 */
        @media (max-width: 768px) {
            /* 平板樣式 */
        }

        @media (max-width: 480px) {
            /* 手機樣式 */
        }
    </style>
</head>
<body>
    <!-- HTML 內容 -->

    <script>
        // JavaScript 功能
        function init() {
            // 初始化程式
        }

        init();
    </script>
</body>
</html>
```

### 玻璃擬態效果
```css
.glass-container {
    background: rgba(255, 255, 255, 0.1);
    backdrop-filter: blur(10px);
    border-radius: 30px;
    box-shadow: 0 8px 32px 0 rgba(31, 38, 135, 0.37);
    border: 1px solid rgba(255, 255, 255, 0.18);
}
```

### 即時更新功能
```javascript
function updateContent() {
    // 更新內容邏輯
}

// 初始化
updateContent();

// 定期更新（例如每秒）
setInterval(updateContent, 1000);
```

## 最佳實踐
- 使用語意化的 HTML 標籤
- CSS 使用 Flexbox 或 Grid 進行布局
- JavaScript 使用現代語法（ES6+）
- 確保跨瀏覽器相容性
- 適當使用動畫效果增強使用者體驗
