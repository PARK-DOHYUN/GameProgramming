using UnityEngine;

/// <summary>
/// 플레이어를 따라가는 카메라
/// </summary>
public class CameraFollow : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform target; // 플레이어

    [Header("Follow Settings")]
    [SerializeField] private Vector3 offset = new Vector3(0, 0, -10); // 카메라 오프셋
    [SerializeField] private float smoothSpeed = 5f; // 부드러움 (높을수록 빠름)
    [SerializeField] private bool smoothFollow = true; // 부드럽게 따라가기

    [Header("Boundaries (Optional)")]
    [SerializeField] private bool useBoundaries = false; // 맵 경계 사용
    [SerializeField] private Vector2 minBounds = new Vector2(-50, -50);
    [SerializeField] private Vector2 maxBounds = new Vector2(50, 50);

    private void Start()
    {
        // 타겟이 없으면 플레이어 찾기
        if (target == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                target = player.transform;
                Debug.Log("CameraFollow: Target found (Player)");
            }
            else
            {
                Debug.LogError("CameraFollow: Player not found! Make sure Player has 'Player' tag.");
            }
        }
    }

    private void LateUpdate()
    {
        if (target == null) return;

        // 목표 위치 계산
        Vector3 desiredPosition = target.position + offset;

        // 경계 제한 (사용 시)
        if (useBoundaries)
        {
            desiredPosition.x = Mathf.Clamp(desiredPosition.x, minBounds.x, maxBounds.x);
            desiredPosition.y = Mathf.Clamp(desiredPosition.y, minBounds.y, maxBounds.y);
        }

        // 부드럽게 이동 또는 즉시 이동
        if (smoothFollow)
        {
            // Lerp로 부드럽게
            transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
        }
        else
        {
            // 즉시 이동
            transform.position = desiredPosition;
        }
    }

    /// <summary>
    /// 타겟 설정
    /// </summary>
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    /// <summary>
    /// 부드러움 설정
    /// </summary>
    public void SetSmoothSpeed(float speed)
    {
        smoothSpeed = speed;
    }
}