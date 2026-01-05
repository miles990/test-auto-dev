# Unity 瑪利歐平台遊戲專案

這是一個使用 Unity 引擎開發的類超級瑪利歐平台遊戲專案。本專案提供完整的 C# 腳本、詳細的設定說明，以及 GitHub Actions CI/CD 整合範例。

## 📋 目錄

- [專案概述](#專案概述)
- [環境需求](#環境需求)
- [快速開始](#快速開始)
- [專案結構](#專案結構)
- [核心腳本說明](#核心腳本說明)
- [Unity 編輯器設定](#unity-編輯器設定)
- [WebGL 建置流程](#webgl-建置流程)
- [GitHub Actions 整合](#github-actions-整合)
- [遊戲設計參數](#遊戲設計參數)
- [疑難排解](#疑難排解)

## 專案概述

本專案實作了平台遊戲的核心機制：

- ✅ **玩家控制**：流暢的移動、可變跳躍高度、二段跳
- ✅ **敵人 AI**：自動巡邏、轉向、碰撞偵測
- ✅ **遊戲管理**：分數系統、生命值、倒數計時、暫停功能
- ✅ **相機系統**：平滑跟隨、死區、邊界限制、震動效果
- ✅ **收集系統**：金幣、道具、生命值等多種收集物品

## 環境需求

### 必要軟體

- **Unity Editor**: 2022.3 LTS 或更新版本（推薦）
  - 下載：[Unity Hub](https://unity.com/download)
  - 必須安裝 **WebGL Build Support** 模組

- **作業系統**：
  - Windows 10/11 (64-bit)
  - macOS 10.13 或更新版本
  - Ubuntu 18.04 或更新版本

### 選用軟體

- **Visual Studio Code** 或 **Visual Studio 2022**（C# 開發）
- **Git**（版本控制）

## 快速開始

### 步驟 1：安裝 Unity

1. 下載並安裝 [Unity Hub](https://unity.com/download)
2. 在 Unity Hub 中安裝 **Unity 2022.3 LTS**
3. 確認勾選以下模組：
   - WebGL Build Support
   - Visual Studio Community（如果沒有其他 IDE）

### 步驟 2：建立 Unity 專案

1. 開啟 Unity Hub
2. 點選 **New Project**
3. 選擇 **2D** 範本
4. 專案名稱：`mario-platformer`
5. 專案位置：選擇本 repository 的 `unity-games/mario-platformer/` 目錄
6. 點選 **Create Project**

### 步驟 3：匯入腳本

Unity 專案建立完成後，本目錄中的 `Assets/Scripts/` 資料夾內的所有 C# 腳本會自動被 Unity 識別。

### 步驟 4：設定場景

1. 建立新場景：`File > New Scene`，選擇 **2D** 模式
2. 儲存場景：`File > Save As`，命名為 `Level01.unity`

#### 4.1 建立玩家

1. 建立空物件：`GameObject > Create Empty`，命名為 `Player`
2. 新增組件：
   - `Add Component > Sprite Renderer`（設定玩家精靈圖）
   - `Add Component > Rigidbody 2D`
     - Gravity Scale: 3
     - Freeze Rotation: Z 勾選
   - `Add Component > Box Collider 2D`
   - `Add Component > Scripts > PlayerController`
   - `Add Component > Animator`（可選，用於動畫）
3. 建立子物件 `GroundCheck`：
   - Position: (0, -0.5, 0)
   - 在 PlayerController 中拖曳此物件到 Ground Check 欄位
4. 建立子物件 `EnemyCheck`：
   - Position: (0, -0.5, 0)
   - 在 PlayerController 中拖曳此物件到 Enemy Check 欄位
5. 設定 Layer：將 Player 設為 `Player` layer
6. 設定 Tag：將 Player Tag 設為 `Player`

#### 4.2 建立地面

1. 建立精靈圖：`GameObject > 2D Object > Sprite`，命名為 `Ground`
2. 新增組件：`Add Component > Box Collider 2D`
3. 設定 Layer 為 `Ground`

#### 4.3 建立敵人

1. 建立空物件：`GameObject > Create Empty`，命名為 `Enemy`
2. 新增組件：
   - `Add Component > Sprite Renderer`
   - `Add Component > Rigidbody 2D`
     - Freeze Rotation: Z 勾選
   - `Add Component > Box Collider 2D`
   - `Add Component > Scripts > EnemyAI`
3. 建立子物件 `GroundCheck` 和 `WallCheck`
4. 設定 Layer 為 `Enemy`
5. 設定 Tag 為 `Enemy`

#### 4.4 建立相機

1. 選擇 Main Camera
2. 新增組件：`Add Component > Scripts > CameraFollow`
3. 拖曳 Player 物件到 Target 欄位

#### 4.5 建立遊戲管理器

1. 建立空物件：`GameObject > Create Empty`，命名為 `GameManager`
2. 新增組件：`Add Component > Scripts > GameManager`
3. 建立 UI Canvas（`GameObject > UI > Canvas`）
4. 在 Canvas 下建立 Text 元素顯示分數、生命值、時間
5. 拖曳 UI 元素到 GameManager 對應欄位

#### 4.6 建立收集物品

1. 建立空物件：`GameObject > Create Empty`，命名為 `Coin`
2. 新增組件：
   - `Add Component > Sprite Renderer`
   - `Add Component > Circle Collider 2D`
     - Is Trigger: 勾選
   - `Add Component > Scripts > Collectible`
3. 在 Collectible 設定物品類型為 `Coin`

### 步驟 5：設定 Layers

1. 點選 `Edit > Project Settings > Tags and Layers`
2. 新增以下 Layers：
   - Layer 6: `Player`
   - Layer 7: `Ground`
   - Layer 8: `Enemy`
   - Layer 9: `Collectible`

### 步驟 6：設定 Physics 2D

1. 點選 `Edit > Project Settings > Physics 2D`
2. 在 Layer Collision Matrix 中設定碰撞規則

### 步驟 7：測試遊戲

1. 點選 Unity 編輯器上方的 **Play** 按鈕
2. 使用方向鍵或 WASD 移動
3. 按空白鍵跳躍
4. 按 ESC 暫停

## 專案結構

```
unity-games/mario-platformer/
├── Assets/
│   ├── Scenes/              # 遊戲場景
│   │   └── Level01.unity
│   ├── Scripts/             # C# 腳本
│   │   ├── PlayerController.cs
│   │   ├── EnemyAI.cs
│   │   ├── GameManager.cs
│   │   ├── CameraFollow.cs
│   │   └── Collectible.cs
│   ├── Sprites/             # 精靈圖資源（需自行新增）
│   ├── Audio/               # 音效資源（需自行新增）
│   ├── Prefabs/             # 預製物件
│   └── Animations/          # 動畫控制器
├── ProjectSettings/         # Unity 專案設定
├── Packages/                # Package Manager 套件
├── .gitignore              # Git 忽略規則
└── README.md               # 本文件
```

## 核心腳本說明

### PlayerController.cs

**功能**：
- 水平移動（加速/減速系統）
- 可變高度跳躍（短按/長按）
- 二段跳支援
- 地面檢測
- 敵人踩踏判定
- 受傷與無敵時間
- 動畫控制

**重要參數**：
- `moveSpeed`: 移動速度（預設 7）
- `jumpForce`: 跳躍力度（預設 15）
- `maxJumps`: 最大跳躍次數（預設 2）

### EnemyAI.cs

**功能**：
- 自動巡邏移動
- 地面邊緣偵測（自動轉向）
- 牆壁偵測（自動轉向）
- 生命值系統
- 死亡處理與特效
- 玩家碰撞傷害

**重要參數**：
- `moveSpeed`: 移動速度（預設 3）
- `maxHealth`: 最大生命值（預設 1）

### GameManager.cs

**功能**：
- 單例模式（全域存取）
- 分數管理
- 生命值系統
- 關卡倒數計時
- 遊戲暫停/繼續
- 遊戲結束/關卡完成
- UI 更新
- 場景切換

**重要方法**：
- `AddScore(int points)`: 加分
- `PlayerTakeDamage(int damage)`: 玩家受傷
- `CollectCoin(int value)`: 收集金幣
- `LevelComplete()`: 完成關卡

### CameraFollow.cs

**功能**：
- 平滑相機跟隨
- 死區系統（減少抖動）
- 邊界限制
- 移動預測
- 相機震動效果

**重要參數**：
- `smoothSpeed`: 平滑速度（預設 10）
- `deadZoneSize`: 死區大小（預設 4x2）
- `minBounds/maxBounds`: 相機邊界

### Collectible.cs

**功能**：
- 多種物品類型（金幣、道具、生命、星星等）
- 旋轉動畫
- 上下浮動動畫
- 收集音效與特效

**物品類型**：
- `Coin`: 金幣（加分）
- `PowerUp`: 能力提升
- `Life`: 額外生命
- `Star`: 無敵星星
- `HealthRestore`: 恢復生命值

## Unity 編輯器設定

### Input Manager 設定

1. `Edit > Project Settings > Input Manager`
2. 確認以下輸入軸存在：
   - `Horizontal`（方向鍵左右 / A D）
   - `Vertical`（方向鍵上下 / W S）
   - `Jump`（空白鍵）

### Quality Settings

1. `Edit > Project Settings > Quality`
2. WebGL 平台建議使用 **Medium** 品質等級

### Player Settings (WebGL)

1. `Edit > Project Settings > Player`
2. 切換到 **WebGL** 平台
3. 設定：
   - Company Name: 您的名稱
   - Product Name: Mario Platformer
   - Resolution:
     - Default Canvas Width: 1280
     - Default Canvas Height: 720
   - Publishing Settings:
     - Compression Format: Gzip（較小檔案）
     - Enable Exceptions: None（較快載入）

## WebGL 建置流程

### 本地建置

1. 點選 `File > Build Settings`
2. 選擇 **WebGL** 平台，點選 **Switch Platform**
3. 點選 **Add Open Scenes** 加入當前場景
4. 點選 **Build**
5. 選擇輸出目錄（建議：`Build/WebGL`）
6. 等待建置完成（可能需要 5-15 分鐘）

### 測試 WebGL 建置

建置完成後，**不能直接開啟 index.html**（因為瀏覽器安全限制）。

#### 方法 1：使用 Python HTTP Server

```bash
cd Build/WebGL
python3 -m http.server 8000
```

開啟瀏覽器訪問 `http://localhost:8000`

#### 方法 2：使用 Node.js HTTP Server

```bash
cd Build/WebGL
npx http-server
```

#### 方法 3：使用 Unity 內建伺服器

在 Build Settings 中勾選 **Build And Run**，Unity 會自動啟動本地伺服器。

## GitHub Actions 整合

本專案可與 [akiojin/skills](https://github.com/akiojin/skills) 整合，實現自動化建置。

### 前置準備

#### 1. 取得 Unity 授權

Unity 需要授權檔案才能在 CI/CD 環境中建置。

**個人授權（免費）方式**：

1. 安裝 [Unity License Activator](https://github.com/game-ci/unity-request-activation-file)
2. 執行以下指令：

```bash
# 產生啟動請求檔案
unity-editor -quit -batchmode -nographics -logFile - -createManualActivationFile

# 會產生 Unity_v20XX.X.XXXX.alf 檔案
```

3. 前往 [Unity 手動授權頁面](https://license.unity3d.com/manual)
4. 上傳 `.alf` 檔案
5. 下載授權檔案 `.ulf`

#### 2. 設定 GitHub Secrets

1. 前往 GitHub repository 的 `Settings > Secrets and variables > Actions`
2. 點選 **New repository secret**
3. 新增以下 Secrets：

| Secret 名稱 | 說明 |
|------------|------|
| `UNITY_LICENSE` | Unity 授權檔案內容（複製 .ulf 檔案的完整內容） |
| `UNITY_EMAIL` | Unity 帳號 Email |
| `UNITY_PASSWORD` | Unity 帳號密碼 |

### GitHub Actions 工作流程範例

在 repository 根目錄建立 `.github/workflows/unity-build.yml`：

```yaml
name: Unity WebGL Build

on:
  push:
    branches:
      - main
    paths:
      - 'unity-games/mario-platformer/**'
  pull_request:
    branches:
      - main
    paths:
      - 'unity-games/mario-platformer/**'

jobs:
  build:
    name: Build Unity WebGL
    runs-on: ubuntu-latest

    steps:
      # Checkout Repository
      - name: Checkout repository
        uses: actions/checkout@v4
        with:
          lfs: true  # 如果使用 Git LFS 儲存大型資源

      # Cache Unity Library（加速建置）
      - name: Cache Unity Library
        uses: actions/cache@v4
        with:
          path: unity-games/mario-platformer/Library
          key: Library-mario-platformer-WebGL-${{ hashFiles('unity-games/mario-platformer/Assets/**', 'unity-games/mario-platformer/Packages/**', 'unity-games/mario-platformer/ProjectSettings/**') }}
          restore-keys: |
            Library-mario-platformer-WebGL-
            Library-mario-platformer-

      # Build Unity Project
      - name: Build Unity Project
        uses: game-ci/unity-builder@v4
        env:
          UNITY_LICENSE: ${{ secrets.UNITY_LICENSE }}
          UNITY_EMAIL: ${{ secrets.UNITY_EMAIL }}
          UNITY_PASSWORD: ${{ secrets.UNITY_PASSWORD }}
        with:
          projectPath: unity-games/mario-platformer
          targetPlatform: WebGL
          unityVersion: 2022.3.10f1  # 替換為您使用的 Unity 版本
          buildsPath: unity-games/mario-platformer/Build

      # Upload Build Artifact
      - name: Upload WebGL Build
        uses: actions/upload-artifact@v4
        with:
          name: mario-platformer-webgl
          path: unity-games/mario-platformer/Build/WebGL

      # Optional: Deploy to GitHub Pages
      - name: Deploy to GitHub Pages
        if: github.ref == 'refs/heads/main'
        uses: peaceiris/actions-gh-pages@v4
        with:
          github_token: ${{ secrets.GITHUB_TOKEN }}
          publish_dir: unity-games/mario-platformer/Build/WebGL
          destination_dir: unity-games/mario-platformer
```

### 使用 akiojin/skills 的整合範例

如果使用 [akiojin/skills](https://github.com/akiojin/skills) 中的 Unity 建置 Action：

```yaml
name: Unity Build with Skills

on:
  push:
    branches: [main]

jobs:
  build:
    runs-on: ubuntu-latest

    steps:
      - uses: actions/checkout@v4

      - name: Build Unity Project
        uses: akiojin/unity-build-action@v1  # 使用 skills repository 中的 action
        with:
          project-path: unity-games/mario-platformer
          build-target: WebGL
          unity-version: 2022.3.10f1
        env:
          UNITY_LICENSE: ${{ secrets.UNITY_LICENSE }}
```

### 觸發建置

設定完成後，每次推送程式碼到 `main` 分支時，GitHub Actions 會自動：

1. 檢出程式碼
2. 快取 Unity Library（加速後續建置）
3. 使用 Unity 建置 WebGL 版本
4. 上傳建置結果到 Artifacts
5. （可選）部署到 GitHub Pages

## 遊戲設計參數

### 推薦數值

以下是經過調整的遊戲參數，提供良好的遊戲手感：

#### 玩家移動
- Move Speed: 7-8
- Jump Force: 14-16
- Gravity Scale: 3
- Fall Multiplier: 2.5

#### 敵人
- Move Speed: 2-4
- Ground Check Distance: 0.5
- Wall Check Distance: 0.5

#### 相機
- Smooth Speed: 8-12
- Dead Zone Size: (3-5, 1.5-2.5)
- Offset: (0, 2, -10)

### 調整建議

- **移動太滑**：增加 `deceleration` 數值
- **跳躍太低**：增加 `jumpForce` 或減少 `gravity`
- **相機抖動**：增加 `deadZoneSize`
- **敵人太快**：減少 `moveSpeed`

## 疑難排解

### 常見問題

#### Q: 玩家無法移動

**解決方法**：
1. 檢查 Rigidbody2D 的 Body Type 是否為 `Dynamic`
2. 確認 Freeze Position 的 X 軸沒有勾選
3. 檢查 PlayerController 腳本是否正確附加

#### Q: 地面檢測不正常

**解決方法**：
1. 確認 GroundCheck 物件位置正確（在玩家腳下）
2. 檢查 Ground Layer 是否正確設定
3. 調整 Ground Check Size 參數

#### Q: 敵人掉落懸崖

**解決方法**：
1. 確認 GroundCheck 物件位置（應在敵人前方）
2. 調整 Ground Check Distance
3. 檢查地面 Layer 設定

#### Q: 相機不跟隨玩家

**解決方法**：
1. 確認 CameraFollow 腳本中的 Target 欄位有拖曳 Player 物件
2. 檢查 Player 的 Tag 是否為 `Player`

#### Q: WebGL 建置失敗

**解決方法**：
1. 確認安裝了 WebGL Build Support 模組
2. 檢查 Project Settings > Player > WebGL 設定
3. 清除快取：刪除 `Library` 資料夾後重新開啟專案

#### Q: GitHub Actions 建置失敗

**解決方法**：
1. 確認 `UNITY_LICENSE` Secret 設定正確
2. 檢查 Unity 版本號是否一致
3. 查看 Actions 日誌中的錯誤訊息

### 效能優化

#### 減少建置檔案大小

1. `Edit > Project Settings > Player > WebGL`
2. 設定：
   - Enable Exceptions: None
   - Compression Format: Gzip
   - Code Optimization: Size
   - Managed Stripping Level: High

#### 加快建置速度

1. 使用快取（GitHub Actions 已設定）
2. 移除未使用的資源
3. 使用較低的 Quality Level

## 資源連結

- [Unity 官方文件](https://docs.unity3d.com/)
- [Unity Learn 平台](https://learn.unity.com/)
- [GameCI 文件](https://game.ci/)（Unity CI/CD）
- [akiojin/skills](https://github.com/akiojin/skills)（GitHub Actions 整合）

## 授權

本專案遵循 MIT 授權條款。

---

**專案維護者**: Claude Code
**最後更新**: 2026-01-05
