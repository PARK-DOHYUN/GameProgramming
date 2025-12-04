using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 좀비의 체력을 관리하는 스크립트
/// </summary>
public class ZombieHealth : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private float maxHealth = 50f;
    private float currentHealth;

    [Header("Drop Settings")]
    [SerializeField] private GameObject[] itemDrops; // 드랍 가능한 아이템들
    [SerializeField] private float dropChance = 0.3f; // 30% 확률로 아이템 드랍

    [Header("Events")]
    public UnityEvent OnZombieDeath;

    [Header("Animation")]
    [SerializeField] private Animator animator;

    private bool isDead = false;

    private void Awake()
    {
        currentHealth = maxHealth;

        // Animator 자동 찾기
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
    }

    /// <summary>
    /// 데미지 받기
    /// </summary>
    public void TakeDamage(float damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        currentHealth = Mathf.Max(0, currentHealth);

        // 좀비 피격음
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayZombieHit();
        }

        // 피격 애니메이션 재생 (추후 구현)
        // animator.SetTrigger("Hit");

        // 사망 체크
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    /// <summary>
    /// 사망 처리
    /// </summary>
    private void Die()
    {
        if (isDead) return;

        isDead = true;

        // 좀비 죽는 소리
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayZombieDeath();
        }

        // 사망 이벤트 발생
        OnZombieDeath?.Invoke();

        // 사망 애니메이션 재생 (먼저!)
        PlayDeathAnimation();

        // AI 비활성화
        ZombieAI zombieAI = GetComponent<ZombieAI>();
        if (zombieAI != null)
        {
            zombieAI.enabled = false;
        }

        // Rigidbody 정지 (simulated는 끄지 않음!)
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.isKinematic = true; // 물리 영향 받지 않지만 활성화 상태 유지
        }

        // Collider 비활성화 (다른 오브젝트와 충돌 방지)
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
        {
            col.enabled = false;
        }

        // 아이템 드랍
        DropItem();

        // 일정 시간 후 오브젝트 제거 (애니메이션 재생 시간 고려)
        Destroy(gameObject, 2f); // 2초 후 삭제

        Debug.Log("Zombie died!");
    }

    /// <summary>
    /// 아이템 드랍
    /// </summary>
    private void DropItem()
    {
        if (itemDrops.Length == 0) return;

        // 드랍 확률 체크
        if (Random.value <= dropChance)
        {
            // 랜덤 아이템 선택
            int randomIndex = Random.Range(0, itemDrops.Length);
            GameObject itemToDrop = itemDrops[randomIndex];

            if (itemToDrop != null)
            {
                Instantiate(itemToDrop, transform.position, Quaternion.identity);
                Debug.Log($"Zombie dropped {itemToDrop.name}");
            }
        }
    }

    /// <summary>
    /// 사망 애니메이션 재생
    /// </summary>
    private void PlayDeathAnimation()
    {
        if (animator != null)
        {
            // ZombieAI에서 마지막 방향 가져오기
            ZombieAI zombieAI = GetComponent<ZombieAI>();
            if (zombieAI != null)
            {
                Vector2 direction = zombieAI.GetMovementDirection();

                // 좌우 방향만 사용 (Left/Right Death만 있음)
                float horizontal = direction.x;

                // 방향이 거의 없으면 기본값
                if (Mathf.Abs(horizontal) < 0.1f)
                {
                    horizontal = 1f; // 기본 오른쪽
                }

                animator.SetFloat("Horizontal", horizontal);
            }

            // Death 트리거 발동
            animator.SetTrigger("Death");

            Debug.Log("Playing death animation");
        }
    }

    /// <summary>
    /// 현재 체력 반환
    /// </summary>
    public float GetCurrentHealth()
    {
        return currentHealth;
    }

    /// <summary>
    /// 최대 체력 반환
    /// </summary>
    public float GetMaxHealth()
    {
        return maxHealth;
    }

    /// <summary>
    /// 체력 백분율 반환
    /// </summary>
    public float GetHealthPercentage()
    {
        return currentHealth / maxHealth;
    }

    /// <summary>
    /// 사망 여부 반환
    /// </summary>
    public bool IsDead()
    {
        return isDead;
    }
}