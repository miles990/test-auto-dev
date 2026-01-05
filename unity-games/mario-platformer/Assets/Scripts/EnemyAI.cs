using UnityEngine;

/// <summary>
/// 敵人 AI 控制器
/// 實作基本的巡邏、轉向、碰撞偵測等行為
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class EnemyAI : MonoBehaviour
{
    [Header("移動設定")]
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private bool startMovingRight = true;

    [Header("地面檢測")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckDistance = 0.5f;
    [SerializeField] private LayerMask groundLayer;

    [Header("牆壁檢測")]
    [SerializeField] private Transform wallCheck;
    [SerializeField] private float wallCheckDistance = 0.5f;

    [Header("生命值設定")]
    [SerializeField] private int maxHealth = 1;
    [SerializeField] private float deathDelay = 0.5f;

    [Header("音效")]
    [SerializeField] private AudioClip hitSound;
    [SerializeField] private AudioClip deathSound;

    [Header("特效")]
    [SerializeField] private GameObject deathParticle;

    // 組件引用
    private Rigidbody2D rb;
    private Animator animator;
    private AudioSource audioSource;
    private SpriteRenderer spriteRenderer;

    // 狀態變數
    private int currentHealth;
    private bool facingRight;
    private bool isDead = false;

    // 動畫參數哈希
    private static readonly int DeathHash = Animator.StringToHash("Death");
    private static readonly int HitHash = Animator.StringToHash("Hit");

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        currentHealth = maxHealth;
        facingRight = startMovingRight;

        // 確保敵人朝向正確
        if (!facingRight)
        {
            transform.Rotate(0f, 180f, 0f);
        }
    }

    private void FixedUpdate()
    {
        if (!isDead)
        {
            Patrol();
            CheckTurnaround();
        }
    }

    private void Patrol()
    {
        // 移動
        float direction = facingRight ? 1f : -1f;
        rb.velocity = new Vector2(direction * moveSpeed, rb.velocity.y);
    }

    private void CheckTurnaround()
    {
        // 檢查前方是否有地面
        bool hasGround = Physics2D.Raycast(
            groundCheck.position,
            Vector2.down,
            groundCheckDistance,
            groundLayer
        );

        // 檢查前方是否有牆壁
        bool hasWall = Physics2D.Raycast(
            wallCheck.position,
            facingRight ? Vector2.right : Vector2.left,
            wallCheckDistance,
            groundLayer
        );

        // 如果前方沒有地面或遇到牆壁，就轉向
        if (!hasGround || hasWall)
        {
            Flip();
        }
    }

    private void Flip()
    {
        facingRight = !facingRight;
        transform.Rotate(0f, 180f, 0f);
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            // 播放受傷動畫和音效
            if (animator != null)
                animator.SetTrigger(HitHash);

            PlaySound(hitSound);
        }
    }

    private void Die()
    {
        isDead = true;

        // 停止移動
        rb.velocity = Vector2.zero;
        rb.isKinematic = true;

        // 播放死亡動畫和音效
        if (animator != null)
            animator.SetTrigger(DeathHash);

        PlaySound(deathSound);

        // 生成死亡特效
        if (deathParticle != null)
        {
            Instantiate(deathParticle, transform.position, Quaternion.identity);
        }

        // 通知 GameManager 加分
        GameManager gameManager = FindObjectOfType<GameManager>();
        if (gameManager != null)
        {
            gameManager.AddScore(100);
        }

        // 延遲銷毀
        Destroy(gameObject, deathDelay);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // 檢查是否碰到玩家
        if (collision.gameObject.CompareTag("Player") && !isDead)
        {
            // 檢查玩家是否從上方踩下來
            Vector2 normal = collision.contacts[0].normal;
            bool stompedFromAbove = normal.y > 0.7f;

            if (!stompedFromAbove)
            {
                // 從側面碰撞，傷害玩家
                PlayerController player = collision.gameObject.GetComponent<PlayerController>();
                if (player != null)
                {
                    player.TakeDamage(1);
                }
            }
            // 從上方踩踏的情況在 PlayerController 中處理
        }
    }

    private void PlaySound(AudioClip clip)
    {
        if (clip != null && audioSource != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    // Gizmos 用於在編輯器中視覺化檢測區域
    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(
                groundCheck.position,
                groundCheck.position + Vector3.down * groundCheckDistance
            );
        }

        if (wallCheck != null)
        {
            Gizmos.color = Color.blue;
            Vector3 direction = facingRight ? Vector3.right : Vector3.left;
            Gizmos.DrawLine(
                wallCheck.position,
                wallCheck.position + direction * wallCheckDistance
            );
        }
    }
}
