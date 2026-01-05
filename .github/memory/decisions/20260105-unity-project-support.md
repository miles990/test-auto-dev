# 2026-01-05 新增 Unity 專案架構支援

## 背景

使用者要求使用 Unity 建立類超級瑪利歐遊戲。經過多次討論後，使用者明確選擇建立 Unity 專案架構，儘管這與原有的「單一 HTML 檔案」專案規範衝突。

## 問題

- 原有專案規範要求每個遊戲為獨立的單一 HTML 檔案
- Unity 專案需要複雜的目錄結構和多個檔案
- Unity WebGL 建置無法壓縮成單一檔案
- 需要 Unity Editor 才能開發和建置

## 考慮的選項

### 選項 1：提供 Unity 腳本範本（使用者本地建置）
**優點**：
- 提供完整的 C# 腳本和專案說明
- 使用者可以完全控制專案
- 不改變現有專案架構

**缺點**：
- 使用者需要手動建立 Unity 專案
- 腳本不在 repository 中版本控制
- 無法直接執行

### 選項 2：修改專案架構，支援 Unity 多檔案專案
**優點**：
- 所有程式碼納入版本控制
- 可以整合 GitHub Actions 自動建置
- 提供完整的開發體驗

**缺點**：
- 改變專案的核心設計理念
- 增加專案複雜度
- 需要更新 CLAUDE.md 規範

### 選項 3：使用進階 HTML5 框架（Phaser 3）模擬 Unity 功能
**優點**：
- 保持單一 HTML 檔案架構
- 立即可用，無需額外工具
- 功能接近 Unity 2D

**缺點**：
- 不是真正的 Unity
- 學習曲線與 Unity 不同

## 決定

**選擇選項 2：修改專案架構，支援 Unity 多檔案專案**

使用者明確回覆 "2" 和 "@claude A"，表示希望：
1. 修改專案架構允許 Unity
2. 建立完整的 Unity 專案結構

## 實作方式

### 1. 更新 CLAUDE.md

新增 Unity 專案結構說明：
- 保留原有的 HTML5 單一檔案規範
- 新增 Unity 專案規範（`unity-games/` 目錄）
- 建立雙軌架構：輕量級 HTML5 + 進階 Unity

### 2. 建立 Unity 專案結構

```
unity-games/mario-platformer/
├── Assets/
│   └── Scripts/
│       ├── PlayerController.cs
│       ├── EnemyAI.cs
│       ├── GameManager.cs
│       ├── CameraFollow.cs
│       └── Collectible.cs
├── ProjectSettings/
├── Packages/
├── .gitignore
└── README.md
```

### 3. 提供完整的開發文件

- Unity 安裝指南
- 場景設定步驟
- 腳本使用說明
- WebGL 建置流程
- GitHub Actions 整合範例

### 4. 使用 akiojin/skills 整合

使用者提供的資源：https://github.com/akiojin/skills
- 整合 Unity Build Action
- 自動化 WebGL 建置流程
- 部署到 GitHub Pages（可選）

## 技術細節

### C# 腳本設計原則

1. **使用 RequireComponent**：確保必要組件存在
2. **AnimatorHash 優化**：使用 StringToHash 提升效能
3. **單例模式**：GameManager 使用 Singleton 設計
4. **事件系統**：支援 UI 更新和擴展
5. **Gizmos 視覺化**：方便在編輯器中調整參數

### GitHub Actions 整合

使用 GameCI 或 akiojin/skills：
- Unity License 管理
- 自動建置 WebGL
- Artifact 上傳
- GitHub Pages 部署

## 風險與緩解

### 風險 1：專案複雜度增加
**緩解**：
- 詳細的 README 說明
- 清楚的目錄結構
- 完整的範例程式碼

### 風險 2：使用者沒有 Unity 經驗
**緩解**：
- 提供逐步教學
- 包含所有設定步驟
- 提供疑難排解章節

### 風險 3：建置時間過長
**緩解**：
- 使用 GitHub Actions Cache
- 提供本地建置指南
- 優化建置設定

## 後續影響

- 專案可以同時支援兩種遊戲開發方式
- 未來可以新增更多 Unity 專案
- 需要維護兩套開發文件

## 參考資料

- Unity 官方文件：https://docs.unity3d.com/
- GameCI：https://game.ci/
- akiojin/skills：https://github.com/akiojin/skills
