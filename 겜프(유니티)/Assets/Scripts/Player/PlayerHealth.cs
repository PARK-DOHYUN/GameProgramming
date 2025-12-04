using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 플레이어 체력 관리
/// </summary>
public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float currentHealth;

    [Header("Damage Settings")]
    [SerializeField] private float invincibilityTime = 0.5f; // 무적 시간
    private float lastDamageTime = -999f;

    [Header("UI")]
    [SerializeField] private HealthBarUI healthBarUI;

    [Header("Events")]
    public UnityEvent<float, float> OnHealthChanged; // (current, max)
    public UnityEvent OnDeath;

    [Header("Animation")]
    [SerializeField] private Animator bodyAnimator;
    [SerializeField] private Animator handAnimator;

    private bool isDead = false;
    private PlayerController playerController;

    private void Awake()
    {
        currentHealth = maxHealth;

        // PlayerController 찾기
        playerController = GetComponent<PlayerController>();

        // Animator 자동 찾기
        if (bodyAnimator == null || handAnimator == null)
        {
            Animator[] animators = GetComponentsInChildren<Animator>();
            foreach (var anim in animators)
            {
                if (anim.gameObject.name == "Body")
                    bodyAnimator = anim;
                else if (anim.gameObject.name == "Hand")
                    handAnimator = anim;
            }
        }

        // HealthBarUI 자동 찾기
        if (healthBarUI == null)
        {
            healthBarUI = FindObjectOfType<HealthBarUI>();
        }
    }

    /// <summary>
    /// 데미지 받기
    /// </summary>
    public void TakeDamage(float damage)
    {
        if (isDead) return;

        // 무적 시간 체크
        if (Time.time - lastDamageTime < invincibilityTime)
        {
            return;
        }

        currentHealth -= damage;
        currentHealth = Mathf.Max(0f, currentHealth);
        lastDamageTime = Time.time;

        // 피격음
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayPlayerHurt();
        }

        Debug.Log($"Player took {damage} damage. Health: {currentHealth}/{maxHealth}");

        // UI 업데이트
        if (healthBarUI != null)
        {
            healthBarUI.UpdateHealthBar(currentHealth, maxHealth);
        }

        // 이벤트 호출
        OnHealthChanged?.Invoke(currentHealth, maxHealth);

        // 사망 체크
        if (currentHealth <= 0f && !isDead)
        {
            Die();
        }
    }

    /// <summary>
    /// 체력 회복
    /// </summary>
    public void Heal(float amount)
    {
        if (isDead) return;

        currentHealth += amount;
        currentHealth = Mathf.Min(currentHealth, maxHealth);

        // 회복음
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayHeal();
        }

        Debug.Log($"Player healed {amount}. Health: {currentHealth}/{maxHealth}");

        // UI 업데이트
        if (healthBarUI != null)
        {
            healthBarUI.UpdateHealthBar(currentHealth, maxHealth);
        }

        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    /// <summary>
    /// 사망 처리
    /// </summary>
    private void Die()
    {
        isDead = true;

        // 죽는 소리
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayPlayerDeath();
        }

        Debug.Log("Player died!");

        // WeaponManager에서 맨손으로 변경
        WeaponManager weaponManager = GetComponent<WeaponManager>();
        if (weaponManager != null)
        {
            weaponManager.SwitchWeapon(WeaponManager.WeaponType.BareHands);
            weaponManager.enabled = false;
        }

        // PlayerController 비활성화
        if (playerController != null)
        {
            playerController.enabled = false;
        }

        // Rigidbody 정지
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
        }

        // Death 애니메이션 재생
        PlayDeathAnimation();

        OnDeath?.Invoke();

        // GameManager에 게임오버 알림
        if (GameManager.Instance != null)
        {
            GameManager.Instance.GameOver();
        }

        // 게임 오버 처리 (추후 구현)
        // GameManager.Instance.GameOver();
    }

    /// <summary>
    /// Death 애니메이션 재생
    /// </summary>
    private void PlayDeathAnimation()
    {
        if (playerController == null) return;

        // 플레이어의 마지막 방향 가져오기
        Vector2 direction = playerController.GetLookDirection();

        // 좌우 방향만 사용
        float horizontal = direction.x;

        // 방향이 거의 없으면 기본값
        if (Mathf.Abs(horizontal) < 0.1f)
        {
            horizontal = 1f; // 기본 오른쪽
        }

        // Body Death 애니메이션
        if (bodyAnimator != null)
        {
            bodyAnimator.SetFloat("Horizontal", horizontal);
            bodyAnimator.SetTrigger("Death");
        }

        // Hand Death 애니메이션
        if (handAnimator != null)
        {
            handAnimator.SetFloat("Horizontal", horizontal);
            handAnimator.SetTrigger("Death");
        }

        Debug.Log("Playing player death animation");
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
    /// 체력 비율 반환 (0~1)
    /// </summary>
    public float GetHealthPercentage()
    {
        return currentHealth / maxHealth;
    }

    /// <summary>
    /// 사망 여부
    /// </summary>
    public bool IsDead()
    {
        return isDead;
    }

    /// <summary>
    /// 체력 완전 회복
    /// </summary>
    public void FullHeal()
    {
        currentHealth = maxHealth;
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }
}