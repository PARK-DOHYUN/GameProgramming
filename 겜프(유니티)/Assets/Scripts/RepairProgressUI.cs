using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 원형 수리 진행바
/// </summary>
public class RepairProgressUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Image fillImage; // 채워지는 원형 이미지
    [SerializeField] private Image backgroundImage; // 배경 원

    [Header("Colors")]
    [SerializeField] private Color fillColor = Color.green;
    [SerializeField] private Color backgroundColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);

    [Header("Animation")]
    [SerializeField] private bool animateScale = true;
    [SerializeField] private float scaleSpeed = 2f;
    [SerializeField] private float scaleAmount = 0.1f;

    private float targetProgress = 0f;
    private float currentProgress = 0f;

    private void Start()
    {
        // 초기 설정
        if (fillImage != null)
        {
            fillImage.type = Image.Type.Filled;
            fillImage.fillMethod = Image.FillMethod.Radial360;
            fillImage.fillOrigin = (int)Image.Origin360.Top;
            fillImage.fillAmount = 0f;
            fillImage.color = fillColor;
        }

        if (backgroundImage != null)
        {
            backgroundImage.color = backgroundColor;
        }

        // 시작할 때 숨김
        gameObject.SetActive(false);
    }

    private void Update()
    {
        // 부드러운 진행
        if (currentProgress < targetProgress)
        {
            currentProgress = Mathf.MoveTowards(currentProgress, targetProgress, Time.unscaledDeltaTime * 2f);

            if (fillImage != null)
            {
                fillImage.fillAmount = currentProgress;
            }
        }

        // 펄스 애니메이션 (선택사항)
        if (animateScale && gameObject.activeSelf)
        {
            float scale = 1f + Mathf.Sin(Time.unscaledTime * scaleSpeed) * scaleAmount;
            transform.localScale = Vector3.one * scale;
        }
    }

    /// <summary>
    /// 진행바 표시 시작
    /// </summary>
    public void Show()
    {
        gameObject.SetActive(true);
        currentProgress = 0f;
        targetProgress = 0f;

        if (fillImage != null)
        {
            fillImage.fillAmount = 0f;
        }

        transform.localScale = Vector3.one;
    }

    /// <summary>
    /// 진행바 숨기기
    /// </summary>
    public void Hide()
    {
        gameObject.SetActive(false);
        currentProgress = 0f;
        targetProgress = 0f;
        transform.localScale = Vector3.one;
    }

    /// <summary>
    /// 진행도 업데이트 (0~1)
    /// </summary>
    public void UpdateProgress(float progress)
    {
        targetProgress = Mathf.Clamp01(progress);

        // 진행도에 따라 색상 변경 (선택사항)
        if (fillImage != null)
        {
            if (progress < 0.33f)
            {
                fillImage.color = Color.red; // 빨간색
            }
            else if (progress < 0.66f)
            {
                fillImage.color = Color.yellow; // 노란색
            }
            else
            {
                fillImage.color = fillColor; // 녹색
            }
        }
    }

    /// <summary>
    /// 색상 변경
    /// </summary>
    public void SetColor(Color color)
    {
        fillColor = color;
        if (fillImage != null)
        {
            fillImage.color = color;
        }
    }
}