using UnityEngine;

/// <summary>
/// 좀비의 AI를 담당하는 스크립트
/// 배회, 추적, 공격 상태를 가짐
/// 4방향 애니메이션 사용
/// </summary>
public class ZombieAI : MonoBehaviour
{
    // AI States
    public enum State { Wandering, Chasing, Attacking }
    private State currentState = State.Wandering;

    [Header("Movement")]
    [SerializeField] private float wanderSpeed = 1.5f;
    [SerializeField] private float chaseSpeed = 3f;

    [Header("Detection")]
    [SerializeField] private float detectionRange = 8f;
    [SerializeField] private LayerMask playerLayer;

    [Header("Attack")]
    [SerializeField] private float attackRange = 1.5f;
    [SerializeField] private float attackDamage = 10f;
    [SerializeField] private float attackCooldown = 1.5f;
    private float lastAttackTime = 0f;

    [Header("Wandering")]
    [SerializeField] private float wanderRadius = 10f;
    [SerializeField] private float wanderChangeInterval = 3f;
    private Vector2 wanderTarget;
    private float wanderTimer = 0f;

    [Header("Animation")]
    [SerializeField] private Animator animator;

    private Transform player;
    private Rigidbody2D rb;
    private Vector2 movement;
    private Vector2 lastDirection = Vector2.down; // 마지막 방향 (애니메이션용)

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.gravityScale = 0f;
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        }

        // Animator 자동 찾기
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }

        // 첫 배회 목표 설정
        SetNewWanderTarget();
    }

    private void Update()
    {
        // 플레이어 탐지
        DetectPlayer();

        // 상태에 따른 행동
        switch (currentState)
        {
            case State.Wandering:
                Wander();
                break;
            case State.Chasing:
                ChasePlayer();
                break;
            case State.Attacking:
                AttackPlayer();
                break;
        }

        // 애니메이션 업데이트
        UpdateAnimation();
    }

    private void FixedUpdate()
    {
        // 이동 적용
        if (rb != null)
        {
            rb.velocity = movement;
        }
    }

    /// <summary>
    /// 플레이어 탐지
    /// </summary>
    private void DetectPlayer()
    {
        // 플레이어 찾기
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
            }
            else
            {
                return;
            }
        }

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        // 공격 범위 안에 있으면 공격 상태
        if (distanceToPlayer <= attackRange)
        {
            currentState = State.Attacking;
            movement = Vector2.zero;
        }
        // 탐지 범위 안에 있으면 추적 상태
        else if (distanceToPlayer <= detectionRange)
        {
            currentState = State.Chasing;
        }
        // 범위 밖이면 배회 상태
        else
        {
            currentState = State.Wandering;
        }
    }

    /// <summary>
    /// 배회 행동
    /// </summary>
    private void Wander()
    {
        wanderTimer += Time.deltaTime;

        // 목표 지점까지의 거리
        float distance = Vector2.Distance(transform.position, wanderTarget);

        // 목표에 도달하거나 시간이 지나면 새 목표 설정
        if (distance < 0.5f || wanderTimer >= wanderChangeInterval)
        {
            SetNewWanderTarget();
            wanderTimer = 0f;
        }

        // 목표를 향해 이동
        Vector2 direction = (wanderTarget - (Vector2)transform.position).normalized;
        movement = direction * wanderSpeed;

        // 방향 저장 (애니메이션용)
        if (direction.magnitude > 0.1f)
        {
            lastDirection = direction;
        }
    }

    /// <summary>
    /// 새로운 배회 목표 설정
    /// </summary>
    private void SetNewWanderTarget()
    {
        Vector2 randomDirection = Random.insideUnitCircle * wanderRadius;
        wanderTarget = (Vector2)transform.position + randomDirection;
    }

    /// <summary>
    /// 플레이어 추적
    /// </summary>
    private void ChasePlayer()
    {
        if (player == null) return;

        Vector2 direction = (player.position - transform.position).normalized;
        movement = direction * chaseSpeed;

        // 방향 저장 (애니메이션용)
        if (direction.magnitude > 0.1f)
        {
            lastDirection = direction;
        }
    }

    /// <summary>
    /// 플레이어 공격
    /// </summary>
    private void AttackPlayer()
    {
        if (player == null) return;

        // 플레이어 방향 보기 (애니메이션용)
        Vector2 direction = (player.position - transform.position).normalized;
        if (direction.magnitude > 0.1f)
        {
            lastDirection = direction;
        }

        // 공격 쿨다운 체크
        if (Time.time - lastAttackTime >= attackCooldown)
        {
            PerformAttack();
            lastAttackTime = Time.time;
        }
    }

    /// <summary>
    /// 실제 공격 수행
    /// </summary>
    private void PerformAttack()
    {
        if (player == null) return;

        // 좀비 공격 소리
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayZombieAttack();
        }

        // 플레이어 데미지
        PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(attackDamage);
            Debug.Log($"Zombie attacked player for {attackDamage} damage");
        }

        // 공격 애니메이션 트리거
        if (animator != null)
        {
            animator.SetTrigger("Attack");
        }
    }

    /// <summary>
    /// 애니메이션 업데이트
    /// </summary>
    private void UpdateAnimation()
    {
        if (animator == null) return;

        // 이동 여부
        bool isMoving = movement.magnitude > 0.1f;
        animator.SetBool("isMoving", isMoving);

        // 방향 설정 (Blend Tree용)
        animator.SetFloat("Horizontal", lastDirection.x);
        animator.SetFloat("Vertical", lastDirection.y);
    }

    /// <summary>
    /// 현재 상태 반환
    /// </summary>
    public State GetCurrentState()
    {
        return currentState;
    }

    /// <summary>
    /// 이동 방향 반환
    /// </summary>
    public Vector2 GetMovementDirection()
    {
        return lastDirection;
    }

    // 디버그용 기즈모
    private void OnDrawGizmosSelected()
    {
        // 탐지 범위
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        // 공격 범위
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        // 배회 범위
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, wanderRadius);

        // 배회 목표
        if (Application.isPlaying)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(wanderTarget, 0.3f);
        }
    }
}