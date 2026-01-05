using UnityEngine;

/// <summary>
/// 玩家角色控制器
/// 處理玩家的移動、跳躍、碰撞偵測等核心遊戲機制
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
public class PlayerController : MonoBehaviour
{
    [Header("移動設定")]
    [SerializeField] private float moveSpeed = 7f;
    [SerializeField] private float acceleration = 20f;
    [SerializeField] private float deceleration = 20f;

    [Header("跳躍設定")]
    [SerializeField] private float jumpForce = 15f;
    [SerializeField] private float gravity = 3f;
    [SerializeField] private float fallMultiplier = 2.5f;
    [SerializeField] private int maxJumps = 2; // 允許二段跳

    [Header("地面檢測")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private Vector2 groundCheckSize = new Vector2(0.8f, 0.1f);
    [SerializeField] private LayerMask groundLayer;

    [Header("敵人互動")]
    [SerializeField] private Transform enemyCheck;
    [SerializeField] private float enemyCheckRadius = 0.3f;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private float bounceForce = 10f;

    [Header("音效")]
    [SerializeField] private AudioClip jumpSound;
    [SerializeField] private AudioClip landSound;
    [SerializeField] private AudioClip hurtSound;

    // 組件引用
    private Rigidbody2D rb;
    private Animator animator;
    private AudioSource audioSource;
    private SpriteRenderer spriteRenderer;

    // 狀態變數
    private float horizontalInput;
    private float currentVelocity;
    private int jumpsRemaining;
    private bool isGrounded;
    private bool wasGrounded;
    private bool facingRight = true;

    // 動畫參數哈希 (效能優化)
    private static readonly int SpeedHash = Animator.StringToHash("Speed");
    private static readonly int IsGroundedHash = Animator.StringToHash("IsGrounded");
    private static readonly int VerticalVelocityHash = Animator.StringToHash("VerticalVelocity");
    private static readonly int JumpHash = Animator.StringToHash("Jump");
    private static readonly int HurtHash = Animator.StringToHash("Hurt");

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        // 設定 Rigidbody2D
        rb.gravityScale = gravity;
        rb.freezeRotation = true;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
    }

    private void Update()
    {
        // 處理玩家輸入
        HandleInput();

        // 更新地面檢測
        CheckGround();

        // 檢測敵人踩踏
        CheckEnemyStomp();

        // 更新動畫
        UpdateAnimation();
    }

    private void FixedUpdate()
    {
        // 處理移動
        HandleMovement();

        // 處理跳躍物理
        HandleJumpPhysics();
    }

    private void HandleInput()
    {
        // 獲取水平輸入
        horizontalInput = Input.GetAxisRaw("Horizontal");

        // 跳躍輸入
        if (Input.GetButtonDown("Jump") && jumpsRemaining > 0)
        {
            Jump();
        }
    }

    private void CheckGround()
    {
        wasGrounded = isGrounded;
        isGrounded = Physics2D.OverlapBox(
            groundCheck.position,
            groundCheckSize,
            0f,
            groundLayer
        );

        // 著地時重置跳躍次數
        if (isGrounded && !wasGrounded)
        {
            jumpsRemaining = maxJumps;
            PlaySound(landSound);
        }
    }

    private void HandleMovement()
    {
        // 計算目標速度
        float targetSpeed = horizontalInput * moveSpeed;

        // 平滑加減速
        float speedDiff = targetSpeed - currentVelocity;
        float accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? acceleration : deceleration;
        float movement = speedDiff * accelRate * Time.fixedDeltaTime;

        currentVelocity += movement;

        // 應用速度
        rb.velocity = new Vector2(currentVelocity, rb.velocity.y);

        // 翻轉角色
        if (horizontalInput > 0 && !facingRight)
            Flip();
        else if (horizontalInput < 0 && facingRight)
            Flip();
    }

    private void HandleJumpPhysics()
    {
        // 改善跳躍手感：下落時增加重力
        if (rb.velocity.y < 0)
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.fixedDeltaTime;
        }
        // 短按跳躍時減少跳躍高度
        else if (rb.velocity.y > 0 && !Input.GetButton("Jump"))
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.fixedDeltaTime;
        }
    }

    private void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        jumpsRemaining--;
        animator.SetTrigger(JumpHash);
        PlaySound(jumpSound);
    }

    private void CheckEnemyStomp()
    {
        if (!isGrounded && rb.velocity.y < 0)
        {
            Collider2D[] enemies = Physics2D.OverlapCircleAll(
                enemyCheck.position,
                enemyCheckRadius,
                enemyLayer
            );

            foreach (Collider2D enemy in enemies)
            {
                EnemyAI enemyAI = enemy.GetComponent<EnemyAI>();
                if (enemyAI != null)
                {
                    // 踩踏敵人
                    enemyAI.TakeDamage(1);

                    // 彈跳效果
                    rb.velocity = new Vector2(rb.velocity.x, bounceForce);
                    jumpsRemaining = maxJumps; // 重置跳躍次數
                }
            }
        }
    }

    private void Flip()
    {
        facingRight = !facingRight;
        transform.Rotate(0f, 180f, 0f);
    }

    private void UpdateAnimation()
    {
        animator.SetFloat(SpeedHash, Mathf.Abs(currentVelocity));
        animator.SetBool(IsGroundedHash, isGrounded);
        animator.SetFloat(VerticalVelocityHash, rb.velocity.y);
    }

    public void TakeDamage(int damage)
    {
        animator.SetTrigger(HurtHash);
        PlaySound(hurtSound);

        // 通知 GameManager
        GameManager gameManager = FindObjectOfType<GameManager>();
        if (gameManager != null)
        {
            gameManager.PlayerTakeDamage(damage);
        }

        // 受傷後短暫無敵時間（可選實作）
        StartCoroutine(InvincibilityCoroutine());
    }

    private System.Collections.IEnumerator InvincibilityCoroutine()
    {
        // 0.5 秒無敵時間
        Physics2D.IgnoreLayerCollision(gameObject.layer, LayerMask.NameToLayer("Enemy"), true);

        // 閃爍效果
        for (int i = 0; i < 5; i++)
        {
            spriteRenderer.enabled = false;
            yield return new WaitForSeconds(0.05f);
            spriteRenderer.enabled = true;
            yield return new WaitForSeconds(0.05f);
        }

        Physics2D.IgnoreLayerCollision(gameObject.layer, LayerMask.NameToLayer("Enemy"), false);
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
            Gizmos.DrawWireCube(groundCheck.position, groundCheckSize);
        }

        if (enemyCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(enemyCheck.position, enemyCheckRadius);
        }
    }
}
