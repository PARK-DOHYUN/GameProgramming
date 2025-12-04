using UnityEngine;
using TMPro;

/// <summary>
/// 타이머 UI 표시 (카운트다운)
/// </summary>
public class TimerUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI timerText;

    [Header("Color Settings")]
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color warningColor = Color.yellow;
    [SerializeField] private Color criticalColor = Color.red;
    [SerializeField] private float warningTime = 60f; // 1분 이하 경고
    [SerializeField] private float criticalTime = 30f; // 30초 이하 위험

    private void Start()
    {
        // GameManager의 이벤트 구독
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnTimerChanged += UpdateTimer;

            // 초기 타이머 표시
            UpdateTimer(GameManager.Instance.GetTimeRemaining());
        }
        else
        {
            Debug.LogWarning("GameManager not found!");
        }
    }

    private void OnDestroy()
    {
        // 이벤트 구독 해제
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnTimerChanged -= UpdateTimer;
        }
    }

    /// <summary>
    /// 타이머 UI 업데이트
    /// </summary>
    private void UpdateTimer(float timeRemaining)
    {
        if (timerText != null)
        {
            // 시간 포맷 (MM:SS)
            timerText.text = GameManager.FormatTime(timeRemaining);

            // 색상 변경
            if (timeRemaining <= criticalTime)
            {
                timerText.color = criticalColor;
            }
            else if (timeRemaining <= warningTime)
            {
                timerText.color = warningColor;
            }
            else
            {
                timerText.color = normalColor;
            }
        }
    }
}