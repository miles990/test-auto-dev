# 2026-01-05 支援 Unity 專案架構

## 背景
使用者明確要求使用 Unity 建立類超級瑪利歐遊戲，經過多次討論後，使用者選擇修改專案架構以支援 Unity WebGL 專案。

## 問題
原專案設定要求「每個 HTML 檔案應為獨立可執行的單一檔案」，但 Unity WebGL 建置必然產生多個檔案和資料夾，兩者無法相容。

## 選項

### 選項 1：維持原架構，拒絕 Unity
- ✅ 保持專案簡潔性
- ✅ 符合原始設計理念
- ❌ 無法滿足使用者需求
- ❌ 限制專案發展可能性

### 選項 2：完全改用 Unity（移除單一檔案限制）
- ✅ 滿足使用者需求
- ❌ 失去輕量級網頁遊戲的優勢
- ❌ 需要重構整個專案
- ❌ 增加維護複雜度

### 選項 3：雙軌架構（推薦）✓
- ✅ 保留原有的單一 HTML 檔案遊戲
- ✅ 同時支援 Unity 多檔案專案
- ✅ 各自獨立，互不干擾
- ✅ 提供更多開發選擇
- ⚠️ 需要清楚的目錄結構規範

## 決定
採用**選項 3：雙軌架構**

## 實作方式

### 目錄結構
```
test-auto-dev/
├── mario.html              # 輕量級網頁遊戲（原有）
├── calculator.html
├── countdown.html
└── unity-games/            # Unity 專案目錄（新增）
    └── mario-platformer/   # 個別 Unity 遊戲
        ├── Assets/
        ├── ProjectSettings/
        ├── README.md
        └── .gitignore
```

### CLAUDE.md 更新
新增「Unity 遊戲專案」章節，明確規範：
- Unity 專案放在 `unity-games/` 目錄
- 每個遊戲使用獨立子目錄
- 建置輸出不納入版控
- 可部署至 GitHub Pages 或其他靜態託管

### 優勢

1. **相容性**
   - 原有的 HTML 遊戲繼續運作
   - 新的 Unity 專案有獨立空間

2. **彈性**
   - 簡單遊戲用 HTML5
   - 複雜遊戲用 Unity
   - 開發者可自由選擇

3. **可維護性**
   - 清楚的目錄分隔
   - 各自的 .gitignore 設定
   - 獨立的建置流程

4. **擴展性**
   - 未來可加入其他遊戲引擎
   - 例如：`godot-games/`、`unreal-games/` 等

## 技術實作

### Unity 專案模板
提供完整的 Unity 專案架構：
- C# 腳本範例（PlayerController、EnemyAI、GameManager）
- README 說明文件
- .gitignore 設定
- GitHub Actions 工作流程範例

### CI/CD 整合
參考 [akiojin/skills](https://github.com/akiojin/skills) 提供的 Unity 建置工具，建立自動化建置流程。

## 權衡

### 優點
- 滿足使用者需求
- 保留原有優勢
- 提供更多可能性

### 缺點
- 專案結構更複雜
- 需要維護兩種不同的開發流程
- Unity 專案需要額外的建置工具和 CI/CD 設定

### 風險與緩解
| 風險 | 影響 | 緩解措施 |
|------|------|----------|
| 開發者混淆兩種架構 | 中 | 在 CLAUDE.md 明確文件化 |
| Unity 建置失敗 | 低 | 提供詳細的設定指南和範例 |
| 儲存庫大小增加 | 中 | Unity Library 不納入版控 |

## 後續行動
1. ✅ 更新 CLAUDE.md
2. ✅ 建立 unity-games/ 目錄結構
3. ✅ 提供 Unity 專案範本
4. ✅ 撰寫詳細的 README
5. ✅ 建立 GitHub Actions 範例
6. ⏳ 使用者在本地使用 Unity Editor 建立實際遊戲內容

## 參考資源
- Unity 官方文件：https://docs.unity3d.com/
- Unity CI/CD 工具：https://github.com/akiojin/skills
- Unity WebGL 建置指南：https://docs.unity3d.com/Manual/webgl-building.html
