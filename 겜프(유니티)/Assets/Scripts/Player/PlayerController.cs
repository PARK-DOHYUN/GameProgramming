using UnityEngine;

/// <summary>
/// 플레이어 이동 및 애니메이션 제어
/// WASD로 이동, 마우스 방향으로 바라봄
/// </summary>
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;

    [Header("Animators")]
    [SerializeField] private Animator bodyAnimator;
    [SerializeField] private Animator handsAnimator;

    private Rigidbody2D rb;
    private Camera mainCamera;
    private Vector2 moveInput;
    private Vector2 mousePosition;
    private Vector2 lookDirection = Vector2.down; // 바라보는 방향

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        mainCamera = Camera.main;

        // Animator 자동 찾기
        if (bodyAnimator == null || handsAnimator == null)
        {
            Animator[] animators = GetComponentsInChildren<Animator>();
            if (animators.Length >= 2)
            {
                foreach (var anim in animators)
                {
                    if (anim.gameObject.name == "Body")
                        bodyAnimator = anim;
                    else if (anim.gameObject.name == "Hands")
                        handsAnimator = anim;
                }
            }
        }

        // Rigidbody2D 설정
        if (rb != null)
        {
            rb.gravityScale = 0f;
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        }
    }

    private void Update()
    {
        GetInput();
        UpdateLookDirection(); // 마우스 방향 계산
        UpdateAnimation();
    }

    private void FixedUpdate()
    {
        Move();
    }

    /// <summary>
    /// WASD 입력 및 마우스 위치 받기
    /// </summary>
    private void GetInput()
    {
        // WASD 이동 입력
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        moveInput = new Vector2(horizontal, vertical).normalized;

        // 마우스 월드 좌표
        if (mainCamera != null)
        {
            mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        }
    }

    /// <summary>
    /// 이동
    /// </summary>
    private void Move()
    {
        if (rb != null)
        {
            rb.velocity = moveInput * moveSpeed;
        }
    }

    /// <summary>
    /// 마우스 방향으로 바라보는 방향 업데이트
    /// </summary>
    private void UpdateLookDirection()
    {
        // 플레이어 → 마우스 방향 벡터
        Vector2 direction = (mousePosition - (Vector2)transform.position).normalized;

        if (direction.magnitude > 0.1f)
        {
            lookDirection = direction;
        }
    }

    /// <summary>
    /// 애니메이션 업데이트
    /// </summary>
    private void UpdateAnimation()
    {
        bool isMoving = moveInput.magnitude > 0.1f;

        // Body Animator 업데이트
        if (bodyAnimator != null)
        {
            bodyAnimator.SetFloat("Horizontal", lookDirection.x);
            bodyAnimator.SetFloat("Vertical", lookDirection.y);
            bodyAnimator.SetBool("isMoving", isMoving);
        }

        // Hands Animator 업데이트
        if (handsAnimator != null)
        {
            handsAnimator.SetFloat("Horizontal", lookDirection.x);
            handsAnimator.SetFloat("Vertical", lookDirection.y);
            handsAnimator.SetBool("isMoving", isMoving);
        }
    }

    /// <summary>
    /// 현재 이동 중인지 반환
    /// </summary>
    public bool IsMoving()
    {
        return moveInput.magnitude > 0.1f;
    }

    /// <summary>
    /// 현재 바라보는 방향 반환 (공격 방향으로 사용)
    /// </summary>
    public Vector2 GetLookDirection()
    {
        return lookDirection;
    }

    /// <summary>
    /// 마우스 방향 반환 (GetLookDirection과 동일)
    /// </summary>
    public Vector2 GetMouseDirection()
    {
        return lookDirection;
    }

    /// <summary>
    /// 현재 속도 반환
    /// </summary>
    public Vector2 GetVelocity()
    {
        return rb != null ? rb.velocity : Vector2.zero;
    }

    /// <summary>
    /// 이동 속도 설정
    /// </summary>
    public void SetMoveSpeed(float newSpeed)
    {
        moveSpeed = newSpeed;
    }

    /// <summary>
    /// Hands Animator 가져오기
    /// </summary>
    public Animator GetHandsAnimator()
    {
        return handsAnimator;
    }

    /// <summary>
    /// Body Animator 가져오기
    /// </summary>
    public Animator GetBodyAnimator()
    {
        return bodyAnimator;
    }
}