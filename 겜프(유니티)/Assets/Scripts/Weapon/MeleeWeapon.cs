using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 근접 무기 (맨손, 배트)
/// </summary>
public class MeleeWeapon : WeaponBase
{
    [Header("Durability")]
    [SerializeField] private bool hasDurability = false;
    [SerializeField] private int maxDurability = 50;
    private int currentDurability;

    [Header("Events")]
    public UnityEvent<int, int> OnDurabilityChanged; // (current, max)
    public UnityEvent OnWeaponBroken;

    private bool isBroken = false;

    private void Awake()
    {
        if (hasDurability)
        {
            currentDurability = maxDurability;
        }
    }

    public override void Attack(Vector2 direction)
    {
        if (isBroken)
        {
            Debug.Log("Weapon is broken!");
            return;
        }

        if (!CanAttack()) return;

        // 휘두르는 소리
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayMeleeSwing();
        }

        PerformAttack(direction);
        lastAttackTime = Time.time;

        // 내구도 감소
        if (hasDurability)
        {
            DecreaseDurability(1);
        }
    }

    protected override void PerformAttack(Vector2 direction)
    {
        // 플레이어 위치 가져오기
        Vector2 attackPosition = transform.position;

        // 범위 내 적 탐지
        Collider2D[] hits = Physics2D.OverlapCircleAll(
            attackPosition,
            attackRange,
            enemyLayer
        );

        foreach (Collider2D hit in hits)
        {
            // 방향 체크 (공격 방향과 적의 방향이 비슷한지)
            Vector2 toEnemy = (hit.transform.position - (Vector3)attackPosition).normalized;
            float angle = Vector2.Dot(direction.normalized, toEnemy);

            // 전방 120도 범위 내에 있는 적만 공격
            if (angle > 0.5f) // cos(60°) = 0.5
            {
                ZombieHealth zombieHealth = hit.GetComponent<ZombieHealth>();
                if (zombieHealth != null)
                {
                    zombieHealth.TakeDamage(damage);

                    // 타격음
                    if (AudioManager.Instance != null)
                    {
                        AudioManager.Instance.PlayMeleeHit();
                    }

                    Debug.Log($"Melee attack dealt {damage} damage to {hit.name}");
                }
            }
        }
    }

    private void DecreaseDurability(int amount)
    {
        currentDurability -= amount;
        currentDurability = Mathf.Max(0, currentDurability);

        OnDurabilityChanged?.Invoke(currentDurability, maxDurability);

        if (currentDurability <= 0)
        {
            BreakWeapon();
        }
    }

    private void BreakWeapon()
    {
        isBroken = true;
        OnWeaponBroken?.Invoke();
        Debug.Log($"{gameObject.name} is broken!");
    }

    public bool IsBroken()
    {
        return isBroken;
    }

    public int GetCurrentDurability()
    {
        return currentDurability;
    }

    public int GetMaxDurability()
    {
        return maxDurability;
    }

    public bool HasDurability()
    {
        return hasDurability;
    }

    // Gizmo로 공격 범위 표시
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        // 공격 범위 (원)
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}