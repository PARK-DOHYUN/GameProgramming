using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 플레이어 체력바 UI 관리
/// </summary>
public class HealthBarUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Image fillImage;
    [SerializeField] private TextMeshProUGUI healthText;

    [Header("Colors")]
    [SerializeField] private Color fullHealthColor = Color.green;
    [SerializeField] private Color lowHealthColor = Color.red;
    [SerializeField] private float lowHealthThreshold = 0.3f; // 30% 이하면 빨간색

    [Header("Animation")]
    [SerializeField] private float smoothSpeed = 5f; // 부드러운 변화 속도

    private float targetFillAmount = 1f;
    private PlayerHealth playerHealth;

    private void Start()
    {
        // PlayerHealth 찾기
        playerHealth = FindObjectOfType<PlayerHealth>();

        if (playerHealth == null)
        {
            Debug.LogError("PlayerHealth not found!");
            return;
        }

        // 초기 체력 설정
        UpdateHealthBar(playerHealth.GetCurrentHealth(), playerHealth.GetMaxHealth());
    }

    private void Update()
    {
        // 부드러운 Fill 애니메이션
        if (fillImage != null)
        {
            fillImage.fillAmount = Mathf.Lerp(
                fillImage.fillAmount,
                targetFillAmount,
                smoothSpeed * Time.deltaTime
            );
        }
    }

    /// <summary>
    /// 체력바 업데이트
    /// </summary>
    public void UpdateHealthBar(float currentHealth, float maxHealth)
    {
        // Fill Amount 계산
        targetFillAmount = Mathf.Clamp01(currentHealth / maxHealth);

        // 색상 변경 (체력에 따라)
        if (fillImage != null)
        {
            if (targetFillAmount <= lowHealthThreshold)
            {
                fillImage.color = lowHealthColor;
            }
            else
            {
                fillImage.color = Color.Lerp(lowHealthColor, fullHealthColor,
                    (targetFillAmount - lowHealthThreshold) / (1f - lowHealthThreshold));
            }
        }

        // 텍스트 업데이트
        if (healthText != null)
        {
            healthText.text = $"{Mathf.CeilToInt(currentHealth)} / {Mathf.CeilToInt(maxHealth)}";
        }
    }
}