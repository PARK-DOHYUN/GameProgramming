using UnityEngine;

/// <summary>
/// Hand(손/무기)를 마우스 방향으로 회전
/// 애니메이션 방향에 따라 회전 오프셋 자동 조정
/// 무기별, 방향별 위치 오프셋 적용
/// </summary>
public class HandRotation : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private bool enableRotation = true;
    [SerializeField] private float rotationSpeed = 10f; // 부드러운 회전 (0 = 즉시)

    [Header("Rotation Offsets (방향별)")]
    [SerializeField] private float upOffset = 0f;      // Up 애니메이션일 때
    [SerializeField] private float downOffset = 0f;    // Down 애니메이션일 때
    [SerializeField] private float rightOffset = -90f; // Right 애니메이션일 때
    [SerializeField] private float leftOffset = 90f;   // Left 애니메이션일 때

    [Header("맨손 Position Offsets")]
    [SerializeField] private Vector2 bareHandsUpPos = new Vector2(0f, 0f);
    [SerializeField] private Vector2 bareHandsDownPos = new Vector2(0f, 0f);
    [SerializeField] private Vector2 bareHandsRightPos = new Vector2(0f, 0f);
    [SerializeField] private Vector2 bareHandsLeftPos = new Vector2(0f, 0f);

    [Header("배트 Position Offsets")]
    [SerializeField] private Vector2 batUpPos = new Vector2(0f, 0f);
    [SerializeField] private Vector2 batDownPos = new Vector2(0f, 0f);
    [SerializeField] private Vector2 batRightPos = new Vector2(0f, 0f);
    [SerializeField] private Vector2 batLeftPos = new Vector2(0f, 0f);

    [Header("라이플 Position Offsets")]
    [SerializeField] private Vector2 rifleUpPos = new Vector2(0f, 0.2f);
    [SerializeField] private Vector2 rifleDownPos = new Vector2(0f, -0.1f);
    [SerializeField] private Vector2 rifleRightPos = new Vector2(0.2f, 0f);
    [SerializeField] private Vector2 rifleLeftPos = new Vector2(-0.2f, 0f);

    [Header("Sorting Order Settings")]
    [SerializeField] private int upSortingOrder = 2;      // 위쪽 볼 때
    [SerializeField] private int defaultSortingOrder = 4; // 다른 방향

    private Camera mainCamera;
    private PlayerController playerController;
    private WeaponManager weaponManager;
    private Vector3 originalLocalPosition; // 원래 로컬 위치
    private SpriteRenderer spriteRenderer; // 스프라이트 렌더러
    private int lastDirection = -1; // 마지막 방향 저장

    private void Awake()
    {
        mainCamera = Camera.main;
        playerController = GetComponentInParent<PlayerController>();
        weaponManager = GetComponentInParent<WeaponManager>();

        // SpriteRenderer 찾기 (자식 오브젝트에서)
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        if (spriteRenderer == null)
        {
            Debug.LogWarning($"{gameObject.name}: SpriteRenderer not found in children!");
        }
        else
        {
            // 초기 Sorting Order 설정
            spriteRenderer.sortingOrder = defaultSortingOrder;
        }

        // 원래 위치 저장
        originalLocalPosition = transform.localPosition;
    }

    private void Update()
    {
        if (enableRotation)
        {
            RotateAndPositionTowardsMouse();
        }
        else
        {
            // 회전은 안 하지만 위치는 조정
            PositionTowardsMouse();
        }
    }

    private void RotateAndPositionTowardsMouse()
    {
        if (mainCamera == null) return;

        // 마우스 월드 좌표
        Vector2 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);

        // 플레이어 위치 (부모의 부모)
        Vector2 playerPosition = transform.parent != null ?
            (Vector2)transform.parent.position : (Vector2)transform.position;

        // Player → 마우스 방향
        Vector2 direction = mousePosition - playerPosition;

        if (direction.magnitude > 0.1f)
        {
            // 마우스 각도 계산
            float mouseAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            // 방향 인덱스 가져오기
            int directionIndex = GetDirectionIndex(mouseAngle);

            // 현재 애니메이션 방향에 맞는 오프셋 선택
            float currentRotOffset = GetRotationOffsetForDirection(mouseAngle);
            Vector2 currentPosOffset = GetPositionOffsetForDirection(mouseAngle);

            // Sorting Order 업데이트 (방향이 바뀔 때만)
            UpdateSortingOrder(directionIndex);

            // 목표 각도 = 마우스 각도 + 회전 오프셋
            float targetAngle = mouseAngle + currentRotOffset;

            // 회전 적용
            if (rotationSpeed > 0f)
            {
                // 부드러운 회전
                float currentAngle = transform.rotation.eulerAngles.z;
                float newAngle = Mathf.LerpAngle(currentAngle, targetAngle, rotationSpeed * Time.deltaTime);
                transform.rotation = Quaternion.Euler(0f, 0f, newAngle);
            }
            else
            {
                // 즉시 회전
                transform.rotation = Quaternion.Euler(0f, 0f, targetAngle);
            }

            // 위치 적용 (원래 위치 + 오프셋)
            transform.localPosition = originalLocalPosition + (Vector3)currentPosOffset;
        }
    }

    /// <summary>
    /// 회전 없이 위치만 조정 (맨손, 배트용)
    /// </summary>
    private void PositionTowardsMouse()
    {
        if (mainCamera == null) return;

        // 마우스 월드 좌표
        Vector2 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);

        // 플레이어 위치
        Vector2 playerPosition = transform.parent != null ?
            (Vector2)transform.parent.position : (Vector2)transform.position;

        // Player → 마우스 방향
        Vector2 direction = mousePosition - playerPosition;

        if (direction.magnitude > 0.1f)
        {
            // 마우스 각도 계산
            float mouseAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            // 방향 인덱스 가져오기
            int directionIndex = GetDirectionIndex(mouseAngle);

            // Sorting Order 업데이트
            UpdateSortingOrder(directionIndex);

            // 현재 무기에 맞는 위치 오프셋만 적용
            Vector2 currentPosOffset = GetPositionOffsetForDirection(mouseAngle);

            // 위치만 적용 (회전은 안 함)
            transform.localPosition = originalLocalPosition + (Vector3)currentPosOffset;
        }
    }

    /// <summary>
    /// 마우스 방향에 따라 적절한 회전 오프셋 반환
    /// </summary>
    private float GetRotationOffsetForDirection(float angle)
    {
        // 각도 범위로 4방향 판단
        if (angle >= -45f && angle < 45f)
        {
            // 오른쪽 (Right 애니메이션)
            return rightOffset;
        }
        else if (angle >= 45f && angle < 135f)
        {
            // 위 (Up 애니메이션)
            return upOffset;
        }
        else if (angle >= -135f && angle < -45f)
        {
            // 아래 (Down 애니메이션)
            return downOffset;
        }
        else
        {
            // 왼쪽 (Left 애니메이션)
            return leftOffset;
        }
    }

    /// <summary>
    /// 마우스 방향과 현재 무기에 따라 적절한 위치 오프셋 반환
    /// </summary>
    private Vector2 GetPositionOffsetForDirection(float angle)
    {
        // 현재 무기 확인
        WeaponManager.WeaponType currentWeapon = WeaponManager.WeaponType.BareHands;

        if (weaponManager != null)
        {
            currentWeapon = weaponManager.GetCurrentWeaponType();
        }

        // 방향 판단
        int direction = GetDirectionIndex(angle); // 0=Right, 1=Up, 2=Down, 3=Left

        // 무기별 오프셋 반환
        switch (currentWeapon)
        {
            case WeaponManager.WeaponType.BareHands:
                return GetBareHandsOffset(direction);

            case WeaponManager.WeaponType.Bat:
                return GetBatOffset(direction);

            case WeaponManager.WeaponType.Rifle:
                return GetRifleOffset(direction);

            default:
                return Vector2.zero;
        }
    }

    private int GetDirectionIndex(float angle)
    {
        if (angle >= -45f && angle < 45f)
            return 0; // Right
        else if (angle >= 45f && angle < 135f)
            return 1; // Up
        else if (angle >= -135f && angle < -45f)
            return 2; // Down
        else
            return 3; // Left
    }

    private Vector2 GetBareHandsOffset(int direction)
    {
        switch (direction)
        {
            case 0: return bareHandsRightPos;
            case 1: return bareHandsUpPos;
            case 2: return bareHandsDownPos;
            case 3: return bareHandsLeftPos;
            default: return Vector2.zero;
        }
    }

    private Vector2 GetBatOffset(int direction)
    {
        switch (direction)
        {
            case 0: return batRightPos;
            case 1: return batUpPos;
            case 2: return batDownPos;
            case 3: return batLeftPos;
            default: return Vector2.zero;
        }
    }

    private Vector2 GetRifleOffset(int direction)
    {
        switch (direction)
        {
            case 0: return rifleRightPos;
            case 1: return rifleUpPos;
            case 2: return rifleDownPos;
            case 3: return rifleLeftPos;
            default: return Vector2.zero;
        }
    }

    /// <summary>
    /// Sorting Order 업데이트 (위쪽 볼 때만 2, 나머지 4)
    /// </summary>
    private void UpdateSortingOrder(int directionIndex)
    {
        if (spriteRenderer == null) return;

        // 방향이 바뀔 때만 업데이트 (최적화)
        if (directionIndex == lastDirection) return;

        lastDirection = directionIndex;

        // 1 = Up 방향
        if (directionIndex == 1)
        {
            spriteRenderer.sortingOrder = upSortingOrder; // 2
        }
        else
        {
            spriteRenderer.sortingOrder = defaultSortingOrder; // 4
        }
    }

    /// <summary>
    /// 회전 켜기/끄기
    /// </summary>
    public void SetRotationEnabled(bool enabled)
    {
        enableRotation = enabled;

        if (!enabled)
        {
            // 회전 해제 시 원래대로
            transform.rotation = Quaternion.identity;
            transform.localPosition = originalLocalPosition;
        }
    }
}