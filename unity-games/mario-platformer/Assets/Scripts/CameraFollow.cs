using UnityEngine;

/// <summary>
/// 相機跟隨控制器
/// 實作平滑的相機跟隨、死區、邊界限制等功能
/// </summary>
public class CameraFollow : MonoBehaviour
{
    [Header("跟隨目標")]
    [SerializeField] private Transform target;

    [Header("平滑設定")]
    [SerializeField] private float smoothSpeed = 10f;
    [SerializeField] private Vector3 offset = new Vector3(0f, 2f, -10f);

    [Header("死區設定")]
    [Tooltip("死區減少相機抖動，玩家在死區內移動時相機不會跟隨")]
    [SerializeField] private bool useDeadZone = true;
    [SerializeField] private Vector2 deadZoneSize = new Vector2(4f, 2f);

    [Header("邊界限制")]
    [Tooltip("限制相機移動範圍，防止顯示關卡外的空白區域")]
    [SerializeField] private bool useBounds = true;
    [SerializeField] private Vector2 minBounds = new Vector2(-50f, 0f);
    [SerializeField] private Vector2 maxBounds = new Vector2(50f, 20f);

    [Header("預測移動")]
    [Tooltip("根據玩家移動方向預測相機位置")]
    [SerializeField] private bool useLookAhead = false;
    [SerializeField] private float lookAheadDistance = 3f;
    [SerializeField] private float lookAheadSpeed = 2f;

    // 內部變數
    private Vector3 velocity = Vector3.zero;
    private Vector3 targetPosition;
    private Vector3 currentLookAhead = Vector3.zero;
    private Camera cam;

    private void Start()
    {
        cam = GetComponent<Camera>();

        // 自動尋找玩家作為跟隨目標
        if (target == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                target = player.transform;
            }
            else
            {
                Debug.LogWarning("CameraFollow: 找不到玩家物件！");
            }
        }

        // 立即移動到目標位置（避免遊戲開始時的相機跳躍）
        if (target != null)
        {
            transform.position = target.position + offset;
        }
    }

    private void LateUpdate()
    {
        if (target == null) return;

        // 計算目標位置
        CalculateTargetPosition();

        // 平滑移動相機
        SmoothMove();

        // 應用邊界限制
        if (useBounds)
        {
            ClampToBounds();
        }
    }

    private void CalculateTargetPosition()
    {
        Vector3 desiredPosition = target.position + offset;

        // 死區檢測
        if (useDeadZone)
        {
            Vector3 currentPosition = transform.position;
            Vector3 difference = desiredPosition - currentPosition;

            // 只在目標離開死區時更新位置
            if (Mathf.Abs(difference.x) > deadZoneSize.x / 2f)
            {
                desiredPosition.x = currentPosition.x + (difference.x - Mathf.Sign(difference.x) * deadZoneSize.x / 2f);
            }
            else
            {
                desiredPosition.x = currentPosition.x;
            }

            if (Mathf.Abs(difference.y) > deadZoneSize.y / 2f)
            {
                desiredPosition.y = currentPosition.y + (difference.y - Mathf.Sign(difference.y) * deadZoneSize.y / 2f);
            }
            else
            {
                desiredPosition.y = currentPosition.y;
            }
        }

        // 預測移動
        if (useLookAhead)
        {
            Rigidbody2D targetRb = target.GetComponent<Rigidbody2D>();
            if (targetRb != null)
            {
                Vector3 lookAheadTarget = new Vector3(
                    Mathf.Sign(targetRb.velocity.x) * lookAheadDistance,
                    0f,
                    0f
                );
                currentLookAhead = Vector3.Lerp(currentLookAhead, lookAheadTarget, lookAheadSpeed * Time.deltaTime);
                desiredPosition += currentLookAhead;
            }
        }

        targetPosition = desiredPosition;
    }

    private void SmoothMove()
    {
        // 使用 SmoothDamp 實現平滑跟隨
        Vector3 smoothPosition = Vector3.SmoothDamp(
            transform.position,
            targetPosition,
            ref velocity,
            1f / smoothSpeed
        );

        transform.position = smoothPosition;
    }

    private void ClampToBounds()
    {
        Vector3 pos = transform.position;

        // 計算相機視野範圍
        float verticalSize = cam.orthographicSize;
        float horizontalSize = verticalSize * cam.aspect;

        // 限制相機位置
        pos.x = Mathf.Clamp(pos.x, minBounds.x + horizontalSize, maxBounds.x - horizontalSize);
        pos.y = Mathf.Clamp(pos.y, minBounds.y + verticalSize, maxBounds.y - verticalSize);

        transform.position = pos;
    }

    /// <summary>
    /// 設定相機邊界（可由關卡管理器呼叫）
    /// </summary>
    public void SetBounds(Vector2 min, Vector2 max)
    {
        minBounds = min;
        maxBounds = max;
        useBounds = true;
    }

    /// <summary>
    /// 設定跟隨目標
    /// </summary>
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    /// <summary>
    /// 立即移動到目標位置（無平滑）
    /// </summary>
    public void SnapToTarget()
    {
        if (target != null)
        {
            transform.position = target.position + offset;
            velocity = Vector3.zero;
            currentLookAhead = Vector3.zero;
        }
    }

    /// <summary>
    /// 相機震動效果
    /// </summary>
    public void Shake(float duration = 0.5f, float magnitude = 0.3f)
    {
        StartCoroutine(ShakeCoroutine(duration, magnitude));
    }

    private System.Collections.IEnumerator ShakeCoroutine(float duration, float magnitude)
    {
        Vector3 originalPosition = transform.localPosition;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            transform.localPosition = new Vector3(
                originalPosition.x + x,
                originalPosition.y + y,
                originalPosition.z
            );

            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = originalPosition;
    }

    // Gizmos 用於在編輯器中視覺化設定
    private void OnDrawGizmosSelected()
    {
        // 繪製死區
        if (useDeadZone)
        {
            Gizmos.color = new Color(0f, 1f, 0f, 0.3f);
            Gizmos.DrawWireCube(transform.position, new Vector3(deadZoneSize.x, deadZoneSize.y, 0f));
        }

        // 繪製邊界
        if (useBounds)
        {
            Gizmos.color = new Color(1f, 0f, 0f, 0.5f);
            Vector3 center = new Vector3(
                (minBounds.x + maxBounds.x) / 2f,
                (minBounds.y + maxBounds.y) / 2f,
                0f
            );
            Vector3 size = new Vector3(
                maxBounds.x - minBounds.x,
                maxBounds.y - minBounds.y,
                0f
            );
            Gizmos.DrawWireCube(center, size);
        }
    }
}
