# 2026-01-05 Unity 遊戲開發最佳實踐

## 學到的內容

### Unity C# 腳本開發

#### 1. RequireComponent 屬性

確保腳本所需的組件自動加入：

```csharp
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
public class PlayerController : MonoBehaviour
{
    // Unity 會自動加入 Rigidbody2D 和 Animator 組件
}
```

**適用情境**：
- 避免 null reference 錯誤
- 確保組件依賴關係
- 提升開發效率

#### 2. Animator 參數優化

使用 `StringToHash` 提升效能：

```csharp
// 不佳做法：每次都轉換字串
animator.SetFloat("Speed", speed);

// 最佳做法：使用哈希值
private static readonly int SpeedHash = Animator.StringToHash("Speed");
animator.SetFloat(SpeedHash, speed);
```

**效能提升**：
- 字串比對 → 整數比對
- 減少 GC 壓力
- 提升約 10-20% 效能

#### 3. 單例模式（Singleton）

全域管理器實作：

```csharp
public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    public static GameManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<GameManager>();
                if (instance == null)
                {
                    GameObject go = new GameObject("GameManager");
                    instance = go.AddComponent<GameManager>();
                }
            }
            return instance;
        }
    }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
```

**適用情境**：
- GameManager
- AudioManager
- SceneManager
- 任何需要全域存取的管理器

#### 4. 物理檢測優化

使用 `OverlapBox` 而非 `Raycast` 進行地面檢測：

```csharp
bool isGrounded = Physics2D.OverlapBox(
    groundCheck.position,
    groundCheckSize,
    0f,
    groundLayer
);
```

**優點**：
- 更穩定的檢測（不會因速度過快而穿過）
- 可以檢測較大區域
- 視覺化更直觀（使用 Gizmos）

#### 5. 平滑移動實作

使用加速度而非直接設定速度：

```csharp
float targetSpeed = input * moveSpeed;
float speedDiff = targetSpeed - currentVelocity;
float accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? acceleration : deceleration;
float movement = speedDiff * accelRate * Time.fixedDeltaTime;
currentVelocity += movement;
```

**效果**：
- 更自然的移動感
- 類似真實物理的加減速
- 更好的遊戲手感

#### 6. 可變跳躍高度

短按/長按實作不同跳躍高度：

```csharp
if (rb.velocity.y > 0 && !Input.GetButton("Jump"))
{
    rb.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.fixedDeltaTime;
}
```

**原理**：
- 放開跳躍鍵時增加下落速度
- 模擬瑪利歐系列的跳躍手感

#### 7. Gizmos 視覺化偵錯

在編輯器中顯示檢測區域：

```csharp
private void OnDrawGizmosSelected()
{
    if (groundCheck != null)
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(groundCheck.position, groundCheckSize);
    }
}
```

**優點**：
- 即時視覺化調整
- 不需要執行遊戲就能看到範圍
- 提升開發效率

### Unity 專案架構

#### 推薦目錄結構

```
Assets/
├── Scenes/           # 場景檔案
├── Scripts/          # C# 腳本
│   ├── Player/
│   ├── Enemies/
│   ├── Managers/
│   └── UI/
├── Sprites/          # 精靈圖
├── Audio/            # 音效與音樂
├── Prefabs/          # 預製物件
├── Animations/       # 動畫控制器
└── Materials/        # 材質
```

#### Layer 設定

建議使用的 Layers：
- Layer 6: Player
- Layer 7: Ground
- Layer 8: Enemy
- Layer 9: Collectible
- Layer 10: Obstacle

#### Tag 設定

常用 Tags：
- Player
- Enemy
- Ground
- Collectible
- Checkpoint

### GitHub Actions Unity 建置

#### 1. Unity License 取得

三種方式：

**方式 1：個人授權（免費）**
```bash
unity-editor -quit -batchmode -nographics -createManualActivationFile
# 上傳 .alf 檔案到 Unity 網站取得 .ulf
```

**方式 2：Professional/Plus 授權**
使用授權序號直接啟動

**方式 3：使用 Unity Personal**
使用 Email/Password 認證

#### 2. GitHub Secrets 設定

必要的 Secrets：
- `UNITY_LICENSE`: 授權檔案內容
- `UNITY_EMAIL`: Unity 帳號
- `UNITY_PASSWORD`: Unity 密碼

#### 3. Cache 策略

快取 `Library/` 資料夾可以大幅減少建置時間：

```yaml
- uses: actions/cache@v4
  with:
    path: unity-games/mario-platformer/Library
    key: Library-${{ hashFiles('Assets/**', 'Packages/**', 'ProjectSettings/**') }}
```

**效果**：
- 首次建置：15-20 分鐘
- 快取後建置：3-5 分鐘

### Unity WebGL 最佳化

#### 減少檔案大小

1. **Compression Format**: Gzip
2. **Code Optimization**: Size
3. **Managed Stripping Level**: High
4. **Enable Exceptions**: None

**結果**：
- 一般專案：20-30 MB → 5-10 MB
- 簡單專案：10-15 MB → 2-5 MB

#### 提升載入速度

1. 使用 Addressables 系統（進階專案）
2. 延遲載入音效與圖片
3. 使用 Sprite Atlas
4. 壓縮音效檔案

## 適用情境

### 何時使用 Unity

✅ **適合**：
- 複雜的 2D/3D 遊戲
- 需要物理引擎的遊戲
- 需要動畫系統的遊戲
- 多平台部署（PC、Mobile、WebGL）
- 團隊協作開發

❌ **不適合**：
- 簡單的網頁小遊戲
- 需要極小檔案大小（<500KB）
- 快速原型開發
- 純文字/邏輯遊戲

### 何時使用 HTML5 Canvas

✅ **適合**：
- 輕量級小遊戲
- 單一檔案需求
- 不需要複雜物理
- 快速部署

❌ **不適合**：
- 大型複雜遊戲
- 需要進階物理引擎
- 需要大量動畫
- 多平台原生部署

## 參考資料

- Unity Manual: https://docs.unity3d.com/Manual/
- Unity Scripting API: https://docs.unity3d.com/ScriptReference/
- GameCI Documentation: https://game.ci/docs/
- Unity Optimization Tips: https://docs.unity3d.com/Manual/MobileOptimizationPracticalGuide.html
