using UnityEngine;

namespace MarioPlatformer.Enemies
{
    /// <summary>
    /// 基礎敵人 AI
    /// 實現簡單的左右巡邏行為
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Collider2D))]
    public class EnemyAI : MonoBehaviour
    {
        [Header("移動設定")]
        [SerializeField] private float moveSpeed = 2f;
        [SerializeField] private float patrolDistance = 5f;

        [Header("牆壁檢測")]
        [SerializeField] private Transform wallCheck;
        [SerializeField] private float wallCheckDistance = 0.5f;
        [SerializeField] private LayerMask obstacleLayer;

        [Header("生命值")]
        [SerializeField] private int maxHealth = 1;
        [SerializeField] private int currentHealth;

        // 組件參考
        private Rigidbody2D rb;
        private SpriteRenderer spriteRenderer;

        // 狀態變數
        private Vector2 startPosition;
        private int moveDirection = 1; // 1 = 右, -1 = 左
        private bool isDead = false;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            currentHealth = maxHealth;
            startPosition = transform.position;
        }

        private void FixedUpdate()
        {
            if (isDead) return;

            // 檢查是否碰到牆壁或到達巡邏邊界
            if (ShouldTurnAround())
            {
                TurnAround();
            }

            // 移動
            Move();
        }

        private void Move()
        {
            rb.velocity = new Vector2(moveDirection * moveSpeed, rb.velocity.y);

            // 更新朝向
            spriteRenderer.flipX = moveDirection < 0;
        }

        private bool ShouldTurnAround()
        {
            // 檢查是否碰到障礙物
            bool hitWall = Physics2D.Raycast(
                wallCheck.position,
                Vector2.right * moveDirection,
                wallCheckDistance,
                obstacleLayer
            );

            // 檢查是否超出巡邏範圍
            float distanceFromStart = transform.position.x - startPosition.x;
            bool reachedPatrolLimit = Mathf.Abs(distanceFromStart) >= patrolDistance;

            return hitWall || reachedPatrolLimit;
        }

        private void TurnAround()
        {
            moveDirection *= -1;
        }

        public void TakeDamage(int damage)
        {
            if (isDead) return;

            currentHealth -= damage;

            if (currentHealth <= 0)
            {
                Die();
            }
        }

        private void Die()
        {
            isDead = true;
            rb.velocity = Vector2.zero;

            // 死亡動畫或效果
            StartCoroutine(DeathSequence());
        }

        private System.Collections.IEnumerator DeathSequence()
        {
            // 可以在這裡加入死亡動畫
            // 例如：播放音效、粒子效果、翻轉精靈等

            // 向上彈跳
            rb.velocity = new Vector2(0, 5f);

            // 等待一段時間
            yield return new WaitForSeconds(1f);

            // 銷毀物件
            Destroy(gameObject);
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            // 檢查是否被玩家踩踏
            if (collision.gameObject.CompareTag("Player"))
            {
                // 計算碰撞方向
                Vector2 contactNormal = collision.contacts[0].normal;

                // 如果玩家從上方踩下來（法線向上）
                if (contactNormal.y > 0.5f)
                {
                    TakeDamage(1);

                    // 給玩家一個小跳躍
                    Rigidbody2D playerRb = collision.gameObject.GetComponent<Rigidbody2D>();
                    if (playerRb != null)
                    {
                        playerRb.velocity = new Vector2(playerRb.velocity.x, 8f);
                    }
                }
            }
        }

        // Gizmos 用於視覺化檢測範圍
        private void OnDrawGizmosSelected()
        {
            if (wallCheck == null) return;

            // 繪製牆壁檢測射線
            Gizmos.color = Color.blue;
            Vector2 direction = Vector2.right * moveDirection;
            Gizmos.DrawRay(wallCheck.position, direction * wallCheckDistance);

            // 繪製巡邏範圍
            Gizmos.color = Color.yellow;
            Vector2 start = Application.isPlaying ? startPosition : (Vector2)transform.position;
            Gizmos.DrawLine(start + Vector2.left * patrolDistance, start + Vector2.right * patrolDistance);
        }
    }
}
