using UnityEngine;

/// <summary>
/// 可收集物品基類
/// 處理金幣、道具、生命值等收集物品的邏輯
/// </summary>
public class Collectible : MonoBehaviour
{
    [Header("物品類型")]
    [SerializeField] private CollectibleType type = CollectibleType.Coin;

    [Header("數值設定")]
    [SerializeField] private int scoreValue = 10;
    [SerializeField] private int coinValue = 1;

    [Header("視覺效果")]
    [SerializeField] private bool rotateItem = true;
    [SerializeField] private float rotationSpeed = 100f;
    [SerializeField] private bool bobUpDown = false;
    [SerializeField] private float bobSpeed = 2f;
    [SerializeField] private float bobHeight = 0.3f;

    [Header("音效與特效")]
    [SerializeField] private AudioClip collectSound;
    [SerializeField] private GameObject collectParticle;

    // 內部變數
    private Vector3 startPosition;
    private float bobTimer = 0f;

    public enum CollectibleType
    {
        Coin,           // 金幣
        PowerUp,        // 能力提升道具
        Life,           // 生命值
        Star,           // 無敵星星
        HealthRestore   // 恢復生命值
    }

    private void Start()
    {
        startPosition = transform.position;

        // 隨機初始旋轉角度（讓多個物品看起來不同步）
        if (rotateItem)
        {
            transform.Rotate(Vector3.forward, Random.Range(0f, 360f));
        }

        // 隨機初始浮動相位
        if (bobUpDown)
        {
            bobTimer = Random.Range(0f, Mathf.PI * 2f);
        }
    }

    private void Update()
    {
        // 旋轉動畫
        if (rotateItem)
        {
            transform.Rotate(Vector3.forward, rotationSpeed * Time.deltaTime);
        }

        // 上下浮動動畫
        if (bobUpDown)
        {
            bobTimer += Time.deltaTime * bobSpeed;
            float newY = startPosition.y + Mathf.Sin(bobTimer) * bobHeight;
            transform.position = new Vector3(transform.position.x, newY, transform.position.z);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 檢查是否為玩家收集
        if (other.CompareTag("Player"))
        {
            Collect(other.gameObject);
        }
    }

    private void Collect(GameObject player)
    {
        // 根據類型執行不同的收集邏輯
        switch (type)
        {
            case CollectibleType.Coin:
                CollectCoin();
                break;

            case CollectibleType.PowerUp:
                CollectPowerUp();
                break;

            case CollectibleType.Life:
                CollectLife();
                break;

            case CollectibleType.Star:
                CollectStar(player);
                break;

            case CollectibleType.HealthRestore:
                CollectHealthRestore();
                break;
        }

        // 播放收集音效
        PlayCollectSound();

        // 生成收集特效
        SpawnParticle();

        // 銷毀物品
        Destroy(gameObject);
    }

    private void CollectCoin()
    {
        GameManager gameManager = FindObjectOfType<GameManager>();
        if (gameManager != null)
        {
            gameManager.CollectCoin(scoreValue);
        }
    }

    private void CollectPowerUp()
    {
        GameManager gameManager = FindObjectOfType<GameManager>();
        if (gameManager != null)
        {
            gameManager.AddScore(scoreValue);
            gameManager.CollectPowerUp();
        }
    }

    private void CollectLife()
    {
        GameManager gameManager = FindObjectOfType<GameManager>();
        if (gameManager != null)
        {
            gameManager.AddLife();
            gameManager.AddScore(scoreValue);
        }
    }

    private void CollectStar(GameObject player)
    {
        GameManager gameManager = FindObjectOfType<GameManager>();
        if (gameManager != null)
        {
            gameManager.AddScore(scoreValue);
        }

        // 啟動無敵模式
        PlayerController playerController = player.GetComponent<PlayerController>();
        if (playerController != null)
        {
            // 可以在 PlayerController 中實作 ActivateInvincibility 方法
            // playerController.ActivateInvincibility(10f);
        }
    }

    private void CollectHealthRestore()
    {
        GameManager gameManager = FindObjectOfType<GameManager>();
        if (gameManager != null)
        {
            gameManager.AddLife();
        }
    }

    private void PlayCollectSound()
    {
        if (collectSound != null)
        {
            // 在世界空間播放音效（即使物品被銷毀也能聽到）
            AudioSource.PlayClipAtPoint(collectSound, transform.position);
        }
    }

    private void SpawnParticle()
    {
        if (collectParticle != null)
        {
            Instantiate(collectParticle, transform.position, Quaternion.identity);
        }
    }

    // Gizmos 用於在編輯器中視覺化物品類型
    private void OnDrawGizmos()
    {
        Color gizmoColor = type switch
        {
            CollectibleType.Coin => Color.yellow,
            CollectibleType.PowerUp => Color.blue,
            CollectibleType.Life => Color.green,
            CollectibleType.Star => Color.magenta,
            CollectibleType.HealthRestore => Color.red,
            _ => Color.white
        };

        Gizmos.color = gizmoColor;
        Gizmos.DrawWireSphere(transform.position, 0.5f);
    }
}
