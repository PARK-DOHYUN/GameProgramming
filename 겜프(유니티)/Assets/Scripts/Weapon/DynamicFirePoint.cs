using UnityEngine;

/// <summary>
/// FirePoint를 마우스 방향으로 정확히 배치 및 회전
/// Hand의 위치 변화도 반영
/// </summary>
public class DynamicFirePoint : MonoBehaviour
{
    [Header("Direction Offsets (총구 위치)")]
    [SerializeField] private Vector2 upOffset = new Vector2(0.2f, 0.5f);
    [SerializeField] private Vector2 downOffset = new Vector2(0.2f, -0.3f);
    [SerializeField] private Vector2 rightOffset = new Vector2(0.5f, 0.2f);
    [SerializeField] private Vector2 leftOffset = new Vector2(-0.5f, 0.2f);

    [Header("Settings")]
    [SerializeField] private bool rotateFirePoint = true; // FirePoint 회전 여부
    [SerializeField] private float rotationOffset = -90f; // 회전 보정값

    private Camera mainCamera;
    private Transform playerTransform;
    private Transform handTransform;

    private void Awake()
    {
        mainCamera = Camera.main;

        // Rifle의 부모가 Weapons, Weapons의 부모가 Hand 또는 Player
        // 구조: Player -> Hand -> Weapons -> Rifle -> FirePoint
        // 또는: Player -> Weapons -> Rifle -> FirePoint

        handTransform = transform.parent?.parent?.parent; // Hand
        if (handTransform == null || handTransform.name != "Hand")
        {
            // 대체: Player 찾기
            handTransform = transform.parent?.parent;
        }

        playerTransform = FindPlayerTransform();
    }

    private Transform FindPlayerTransform()
    {
        Transform current = transform;
        while (current != null)
        {
            if (current.name == "Player" || current.GetComponent<PlayerController>() != null)
            {
                return current;
            }
            current = current.parent;
        }
        return null;
    }

    private void Update()
    {
        UpdateFirePointPosition();

        if (rotateFirePoint)
        {
            RotateTowardsMouse();
        }
    }

    /// <summary>
    /// 마우스 방향에 따라 FirePoint 위치 조정
    /// Hand의 위치도 고려
    /// </summary>
    private void UpdateFirePointPosition()
    {
        if (mainCamera == null || playerTransform == null) return;

        // 마우스 월드 좌표
        Vector2 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);

        // Player → 마우스 방향
        Vector2 direction = (mousePosition - (Vector2)playerTransform.position).normalized;

        // 각도 계산
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // 4방향 중 어느 방향인지 판단하여 오프셋 결정
        Vector2 offset = GetOffsetForDirection(angle);

        // 로컬 위치 업데이트 (Rifle 기준)
        // Hand의 위치 변화는 이미 월드 좌표에 반영되므로
        // 로컬 오프셋만 설정하면 됨
        transform.localPosition = offset;
    }

    /// <summary>
    /// 각도에 따른 오프셋 반환
    /// </summary>
    private Vector2 GetOffsetForDirection(float angle)
    {
        // 각도 범위에 따라 오프셋 결정
        if (angle >= -45f && angle < 45f)
        {
            // 오른쪽 (0도)
            return rightOffset;
        }
        else if (angle >= 45f && angle < 135f)
        {
            // 위 (90도)
            return upOffset;
        }
        else if (angle >= -135f && angle < -45f)
        {
            // 아래 (-90도)
            return downOffset;
        }
        else
        {
            // 왼쪽 (180도 / -180도)
            return leftOffset;
        }
    }

    /// <summary>
    /// FirePoint를 마우스 방향으로 회전
    /// </summary>
    private void RotateTowardsMouse()
    {
        if (mainCamera == null || playerTransform == null) return;

        // 마우스 월드 좌표
        Vector2 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);

        // FirePoint → 마우스 방향
        Vector2 direction = mousePosition - (Vector2)transform.position;

        if (direction.magnitude > 0.1f)
        {
            // 목표 각도 계산
            float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + rotationOffset;

            // 회전 적용
            transform.rotation = Quaternion.Euler(0f, 0f, targetAngle);
        }
    }

    /// <summary>
    /// 에디터에서 FirePoint 위치 시각화
    /// </summary>
    private void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;

        // FirePoint 위치 표시
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 0.1f);

        // 발사 방향 표시
        Gizmos.color = Color.yellow;
        Vector2 direction = transform.right; // 회전 고려
        Gizmos.DrawRay(transform.position, direction * 2f);
    }
}