using UnityEngine;

namespace MarioPlatformer.Player
{
    /// <summary>
    /// 玩家角色控制器
    /// 處理玩家輸入、移動和跳躍邏輯
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Collider2D))]
    public class PlayerController : MonoBehaviour
    {
        [Header("移動設定")]
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float acceleration = 10f;
        [SerializeField] private float deceleration = 10f;

        [Header("跳躍設定")]
        [SerializeField] private float jumpForce = 12f;
        [SerializeField] private float fallMultiplier = 2.5f;
        [SerializeField] private float lowJumpMultiplier = 2f;

        [Header("地面檢測")]
        [SerializeField] private Transform groundCheck;
        [SerializeField] private float groundCheckRadius = 0.2f;
        [SerializeField] private LayerMask groundLayer;

        // 組件參考
        private Rigidbody2D rb;
        private SpriteRenderer spriteRenderer;
        private Animator animator;

        // 狀態變數
        private float horizontalInput;
        private bool isGrounded;
        private bool jumpPressed;
        private float currentSpeed;

        // 動畫參數名稱
        private static readonly int IsRunning = Animator.StringToHash("IsRunning");
        private static readonly int IsGrounded = Animator.StringToHash("IsGrounded");
        private static readonly int VerticalVelocity = Animator.StringToHash("VerticalVelocity");

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            animator = GetComponent<Animator>();
        }

        private void Update()
        {
            // 讀取輸入
            horizontalInput = Input.GetAxisRaw("Horizontal");

            if (Input.GetButtonDown("Jump") && isGrounded)
            {
                jumpPressed = true;
            }

            // 更新動畫
            UpdateAnimations();
        }

        private void FixedUpdate()
        {
            // 地面檢測
            CheckGround();

            // 處理移動
            HandleMovement();

            // 處理跳躍
            if (jumpPressed)
            {
                Jump();
                jumpPressed = false;
            }

            // 改善跳躍手感
            ImproveJumpPhysics();
        }

        private void CheckGround()
        {
            isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        }

        private void HandleMovement()
        {
            // 計算目標速度
            float targetSpeed = horizontalInput * moveSpeed;

            // 使用加速度平滑移動
            float speedDiff = targetSpeed - currentSpeed;
            float accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? acceleration : deceleration;
            float movement = speedDiff * accelRate;

            currentSpeed += movement * Time.fixedDeltaTime;
            currentSpeed = Mathf.Clamp(currentSpeed, -moveSpeed, moveSpeed);

            // 套用速度
            rb.velocity = new Vector2(currentSpeed, rb.velocity.y);

            // 翻轉角色朝向
            if (horizontalInput > 0.01f)
            {
                spriteRenderer.flipX = false;
            }
            else if (horizontalInput < -0.01f)
            {
                spriteRenderer.flipX = true;
            }
        }

        private void Jump()
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }

        private void ImproveJumpPhysics()
        {
            // 下落時增加重力，讓跳躍更有重量感
            if (rb.velocity.y < 0)
            {
                rb.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.fixedDeltaTime;
            }
            // 放開跳躍鍵時，如果還在上升則減少上升速度
            else if (rb.velocity.y > 0 && !Input.GetButton("Jump"))
            {
                rb.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.fixedDeltaTime;
            }
        }

        private void UpdateAnimations()
        {
            if (animator == null) return;

            animator.SetBool(IsRunning, Mathf.Abs(horizontalInput) > 0.01f);
            animator.SetBool(IsGrounded, isGrounded);
            animator.SetFloat(VerticalVelocity, rb.velocity.y);
        }

        // Gizmos 用於視覺化地面檢測範圍
        private void OnDrawGizmosSelected()
        {
            if (groundCheck == null) return;

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }

        // 公開方法供其他腳本使用
        public bool IsGrounded() => isGrounded;
        public float GetCurrentSpeed() => currentSpeed;
        public void SetCanMove(bool canMove) => enabled = canMove;
    }
}
