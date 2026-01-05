# 2026-01-05 Unity 與 GitHub Actions 整合

## 學習內容

### Unity WebGL 自動建置流程

使用 GitHub Actions 可以自動建置 Unity WebGL 專案，無需在本地手動建置。

### 關鍵步驟

1. **使用 game-ci/unity-builder action**
   - 專門為 Unity CI/CD 設計的 GitHub Action
   - 支援多平台建置（WebGL、Windows、Mac、Linux、iOS、Android 等）
   - 自動處理 Unity 授權啟動

2. **必要的 GitHub Secrets**
   ```
   UNITY_LICENSE  - Unity 授權檔案內容
   UNITY_EMAIL    - Unity 帳號 Email
   UNITY_PASSWORD - Unity 帳號密碼
   ```

3. **快取策略**
   - 快取 `Library/` 資料夾可大幅減少建置時間
   - 使用 `actions/cache@v3` 並以 Assets 變更作為快取鍵

4. **建置產物處理**
   - 使用 `actions/upload-artifact@v3` 保存建置結果
   - 可選：自動部署到 GitHub Pages

### 範例工作流程結構

```yaml
name: Unity WebGL Build
on:
  push:
    paths:
      - 'unity-games/**'
jobs:
  build:
    runs-on: ubuntu-latest
    steps:
      - Checkout 程式碼
      - 快取 Library
      - 建置 Unity 專案
      - 上傳產物
      - 部署（可選）
```

### Unity 授權取得方式

兩種方法：

1. **手動啟動**（適合個人專案）
   ```bash
   unity-editor -quit -batchmode \
     -serial SB-XXXX-XXXX-XXXX \
     -username 'you@example.com' \
     -password 'YourPassword'
   ```

2. **Unity Activation**（適合 CI/CD）
   - 訪問：https://license.unity3d.com/manual
   - 上傳 .alf 檔案取得 .ulf 授權檔

## 適用情境

- 多人協作的 Unity 專案
- 需要自動化建置流程的專案
- 想要在 GitHub Pages 部署遊戲的專案
- 需要跨平台建置的專案

## 最佳實踐

1. **不要提交建置檔案到版控**
   - 使用 .gitignore 排除 Build/、Library/ 等資料夾
   - 建置檔案應由 CI/CD 自動產生

2. **使用 LFS 管理大型資源**
   - Unity 專案常有大型圖片、音訊、模型檔案
   - 使用 Git LFS 可避免儲存庫肥大

3. **設定路徑觸發**
   - 只在 Unity 專案檔案變更時觸發建置
   - 節省 CI/CD 額外用量

4. **使用快取加速**
   - 適當的快取策略可將建置時間從 20 分鐘降到 5 分鐘以內

## 注意事項

- Unity 建置需要較長時間（通常 10-30 分鐘）
- 免費的 GitHub Actions 有每月使用額度限制
- WebGL 建置檔案較大（通常 10-50 MB），注意 GitHub Pages 容量限制
- Unity 授權資訊為敏感資料，務必使用 GitHub Secrets 保護

## 參考資源

- GameCI 文件：https://game.ci/
- Unity Builder Action：https://github.com/game-ci/unity-builder
- Unity 官方 CI/CD 指南：https://docs.unity3d.com/Manual/CommandLineArguments.html
