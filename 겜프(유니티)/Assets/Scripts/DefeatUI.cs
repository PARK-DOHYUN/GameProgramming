using UnityEngine;
using TMPro;
using UnityEngine.UI;

/// <summary>
/// 패배 화면 UI
/// </summary>
public class DefeatUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI defeatText;
    [SerializeField] private TextMeshProUGUI reasonText;
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
    /// 패배 화면 표시 (플레이어 사망)
    /// </summary>
    public void ShowDeath()
    {
        gameObject.SetActive(true);

        if (defeatText != null)
        {
            defeatText.text = "YOU DIED";
            defeatText.color = new Color(1f, 0f, 0f); // 빨간색
        }

        if (reasonText != null)
        {
            reasonText.text = "Killed by Zombies";
        }

        // 커서 표시
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    /// <summary>
    /// 패배 화면 표시 (시간 초과)
    /// </summary>
    public void ShowTimeUp()
    {
        gameObject.SetActive(true);

        if (defeatText != null)
        {
            defeatText.text = "TIME'S UP";
            defeatText.color = new Color(1f, 0.5f, 0f); // 주황색
        }

        if (reasonText != null)
        {
            reasonText.text = "You ran out of time!";
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