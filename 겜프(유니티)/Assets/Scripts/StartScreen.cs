using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 시작 화면 - PLAY/QUIT만
/// </summary>
public class StartScreen : MonoBehaviour
{
    [Header("UI Buttons")]
    [SerializeField] private Button playButton;
    [SerializeField] private Button quitButton;

    [Header("Title")]
    [SerializeField] private TextMeshProUGUI titleText;

    [Header("Settings")]
    [SerializeField] private string gameSceneName = "SampleScene"; // 게임 씬 이름

    [Header("Audio")]
    [SerializeField] private bool playBackgroundMusic = true;

    [Header("Cursor")]
    [SerializeField] private Texture2D cursorTexture;
    [SerializeField] private Vector2 cursorHotspot = Vector2.zero;

    private void Start()
    {
        // 버튼 이벤트 연결
        if (playButton != null)
        {
            playButton.onClick.AddListener(OnPlayClicked);
        }

        if (quitButton != null)
        {
            quitButton.onClick.AddListener(OnQuitClicked);
        }

        // 타이틀 텍스트 설정
        if (titleText != null && string.IsNullOrEmpty(titleText.text))
        {
            titleText.text = "LAST NIGHT\nIN THE\nVILLAGE";
        }

        // 커서 표시
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        // 커스텀 커서 설정
        if (cursorTexture != null)
        {
            Cursor.SetCursor(cursorTexture, cursorHotspot, CursorMode.Auto);
            Debug.Log("Custom cursor set");
        }

        // 시간 정상화
        Time.timeScale = 1f;

        // 배경 음악 재생
        if (playBackgroundMusic && AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayBackgroundMusic();
        }

        Debug.Log("Start Screen loaded");
    }

    /// <summary>
    /// PLAY 버튼
    /// </summary>
    private void OnPlayClicked()
    {
        Debug.Log("Play button clicked");

        // 버튼 클릭 소리
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayButtonClick();
        }

        // 게임 씬 로드
        SceneManager.LoadScene(gameSceneName);
    }

    /// <summary>
    /// QUIT 버튼
    /// </summary>
    private void OnQuitClicked()
    {
        Debug.Log("Quit button clicked");

        // 버튼 클릭 소리
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayButtonClick();
        }

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private void OnDestroy()
    {
        // 이벤트 구독 해제
        if (playButton != null)
        {
            playButton.onClick.RemoveListener(OnPlayClicked);
        }

        if (quitButton != null)
        {
            quitButton.onClick.RemoveListener(OnQuitClicked);
        }
    }
}