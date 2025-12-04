using UnityEngine;
using TMPro;
using UnityEngine.UI;

/// <summary>
/// 승리 화면 UI
/// </summary>
public class VictoryUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI victoryText;
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button quitButton;

    private void Start()
    {
        // 버튼 이벤트 연결
        if (restartButton != null)
        {
            restartButton.onClick.AddListener(OnRestartClicked);
        }

        if (quitButton != null)
        {
            quitButton.onClick.AddListener(OnQuitClicked);
        }
    }

    /// <summary>
    /// 승리 화면 표시
    /// </summary>
    public void Show(float survivedTime)
    {
        gameObject.SetActive(true);

        // 생존 시간 표시
        if (timeText != null)
        {
            int minutes = Mathf.FloorToInt(survivedTime / 60f);
            int seconds = Mathf.FloorToInt(survivedTime % 60f);
            timeText.text = $"Survived for: {minutes:00}:{seconds:00}";
        }

        // 커서 표시
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    /// <summary>
    /// Restart 버튼
    /// </summary>
    private void OnRestartClicked()
    {
        Debug.Log("Restart button clicked");

        // 버튼 클릭 소리
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayButtonClick();
        }

        if (GameManager.Instance != null)
        {
            GameManager.Instance.RestartGame();
        }
    }

    /// <summary>
    /// Quit 버튼
    /// </summary>
    private void OnQuitClicked()
    {
        Debug.Log("Quit button clicked");

        // 버튼 클릭 소리
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayButtonClick();
        }

        if (GameManager.Instance != null)
        {
            GameManager.Instance.QuitGame();
        }
    }

    private void OnDestroy()
    {
        // 이벤트 구독 해제
        if (restartButton != null)
        {
            restartButton.onClick.RemoveListener(OnRestartClicked);
        }

        if (quitButton != null)
        {
            quitButton.onClick.RemoveListener(OnQuitClicked);
        }
    }
}