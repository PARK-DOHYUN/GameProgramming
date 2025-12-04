using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 원거리 무기 시스템 (라이플)
/// </summary>
public class RangedWeapon : WeaponBase
{
    [Header("Ammo Settings")]
    [SerializeField] private int startingAmmo = 30; // 시작 탄약
    [SerializeField] private int maxAmmo = 999; // 최대 보유 탄약
    private int currentAmmo;

    [Header("Bullet Settings")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float bulletSpeed = 20f;

    [Header("Events")]
    public UnityEvent<int, int> OnAmmoChanged; // (current, max)
    public UnityEvent OnOutOfAmmo;

    private void Awake()
    {
        currentAmmo = startingAmmo; // 시작 탄약으로 초기화
        OnAmmoChanged?.Invoke(currentAmmo, maxAmmo); // UI 즉시 업데이트
    }

    protected override void PerformAttack(Vector2 direction)
    {
        // 탄약 체크
        if (currentAmmo <= 0)
        {
            Debug.Log("Out of ammo!");
            OnOutOfAmmo?.Invoke();
            return;
        }

        // 총소리 재생
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayRifleShoot();
        }

        // 총알 발사
        FireBullet(direction);

        // 탄약 감소
        ReduceAmmo(1);

        // 발사 애니메이션 재생
        PlayShootAnimation();
    }

    /// <summary>
    /// 총알 발사
    /// </summary>
    private void FireBullet(Vector2 direction)
    {
        if (bulletPrefab == null || firePoint == null)
        {
            Debug.LogWarning("Bullet prefab or fire point is not assigned!");
            return;
        }

        // 총알 생성
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);

        // 총알에 방향과 속도 전달
        Bullet bulletScript = bullet.GetComponent<Bullet>();
        if (bulletScript != null)
        {
            bulletScript.Initialize(direction, bulletSpeed, damage, enemyLayer);
        }
        else
        {
            // Bullet 스크립트가 없으면 기본 물리 적용
            Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
            if (bulletRb != null)
            {
                bulletRb.velocity = direction * bulletSpeed;
            }
        }
    }

    /// <summary>
    /// 탄약 감소
    /// </summary>
    private void ReduceAmmo(int amount)
    {
        currentAmmo -= amount;
        currentAmmo = Mathf.Max(0, currentAmmo);

        OnAmmoChanged?.Invoke(currentAmmo, maxAmmo);

        if (currentAmmo <= 0)
        {
            OnOutOfAmmo?.Invoke();
        }
    }

    /// <summary>
    /// 탄약 추가
    /// </summary>
    public void AddAmmo(int amount)
    {
        currentAmmo += amount;
        currentAmmo = Mathf.Min(currentAmmo, maxAmmo);

        OnAmmoChanged?.Invoke(currentAmmo, maxAmmo);
    }

    /// <summary>
    /// 발사 애니메이션 재생
    /// </summary>
    private void PlayShootAnimation()
    {
        // 애니메이션 시스템과 연동 (추후 구현)
        // animator.SetTrigger("Shoot");
    }

    /// <summary>
    /// 현재 탄약 반환
    /// </summary>
    public int GetCurrentAmmo()
    {
        return currentAmmo;
    }

    /// <summary>
    /// 최대 탄약 반환
    /// </summary>
    public int GetMaxAmmo()
    {
        return maxAmmo;
    }

    /// <summary>
    /// 탄약 백분율 반환
    /// </summary>
    public float GetAmmoPercentage()
    {
        return (float)currentAmmo / maxAmmo;
    }

    /// <summary>
    /// 탄약이 있는지 확인
    /// </summary>
    public bool HasAmmo()
    {
        return currentAmmo > 0;
    }

    protected override bool CanAttack()
    {
        return base.CanAttack() && currentAmmo > 0;
    }
}