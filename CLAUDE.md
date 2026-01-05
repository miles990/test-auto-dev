# Claude Code 專案設定

## 語言
請使用繁體中文回覆，除非是專有名詞（如 HTML、JavaScript、CSS 等技術名詞）。

## Memory 系統

完成任務後，請更新 `.github/memory/` 中的相關檔案：

### 學習記錄 (`learnings/`)
記錄在此專案學到的技術知識或最佳實踐：
```markdown
# [日期] [標題]
- 學到的內容
- 適用情境
```

### 失敗經驗 (`failures/`)
記錄遇到的問題和解決方法：
```markdown
# [日期] [問題描述]
- 原因分析
- 解決方案
```

### 決策記錄 (`decisions/`)
記錄重要的技術決策：
```markdown
# [日期] [決策標題]
- 背景
- 選項
- 決定
- 原因
```

### 模式 (`patterns/`)
記錄發現的程式碼模式：
```markdown
# [模式名稱]
- 使用情境
- 程式碼範例
```

### 更新 index.md
每次新增記錄後，更新 `.github/memory/index.md` 的索引。

## 專案結構

### 網頁遊戲（HTML5）
- 網頁遊戲/應用放在根目錄
- 每個 HTML 檔案應為獨立可執行的單一檔案
- 適用於輕量級小遊戲（如 mario.html、calculator.html）

### Unity 遊戲專案
- Unity 專案放在 `unity-games/` 目錄下
- 每個 Unity 專案有獨立的子目錄（如 `unity-games/mario-platformer/`）
- 包含完整的 Unity 專案結構：
  - `Assets/` - 遊戲資源與腳本
  - `ProjectSettings/` - Unity 專案設定
  - `Packages/` - Package Manager 套件
  - `.gitignore` - Unity 專用 Git 忽略規則
  - `README.md` - 專案說明文件
- 需要在本地使用 Unity Editor 開發
- 可建置為 WebGL 版本部署

## 完成任務報告格式

### 🛠️ 使用的工具與技能
- 列出使用的程式語言/框架
- 使用的技術或方法

### 📝 任務摘要
- 完成了什麼
- 建立/修改了哪些檔案

### 📚 Memory 更新
- 列出更新的 memory 檔案
