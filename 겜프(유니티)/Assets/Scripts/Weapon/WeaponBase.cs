using UnityEngine;

/// <summary>
/// 모든 무기의 기본이 되는 추상 클래스
/// </summary>
public abstract class WeaponBase : MonoBehaviour
{
    [Header("Weapon Info")]
    [SerializeField] protected string weaponName;
    [SerializeField] protected float damage = 10f;
    [SerializeField] protected float attackRange = 1.5f;
    [SerializeField] protected float attackCooldown = 0.5f;

    [Header("Layers")]
    [SerializeField] protected LayerMask enemyLayer;

    protected float lastAttackTime = 0f;
    protected bool canAttack = true;

    /// <summary>
    /// 공격 실행
    /// </summary>
    public virtual void Attack(Vector2 direction)
    {
        if (!CanAttack()) return;

        PerformAttack(direction);
        lastAttackTime = Time.time;
    }

    /// <summary>
    /// 실제 공격 로직 (자식 클래스에서 구현)
    /// </summary>
    protected abstract void PerformAttack(Vector2 direction);

    /// <summary>
    /// 공격 가능 여부 확인
    /// </summary>
    protected virtual bool CanAttack()
    {
        return canAttack && (Time.time - lastAttackTime >= attackCooldown);
    }

    /// <summary>
    /// 데미지 반환
    /// </summary>
    public float GetDamage()
    {
        return damage;
    }

    /// <summary>
    /// 무기 이름 반환
    /// </summary>
    public string GetWeaponName()
    {
        return weaponName;
    }

    /// <summary>
    /// 무기 활성화/비활성화
    /// </summary>
    public virtual void SetActive(bool active)
    {
        gameObject.SetActive(active);
    }
}
