using UnityEngine;

/// <summary>
/// 커스텀 마우스 커서 관리
/// </summary>
public class CursorManager : MonoBehaviour
{
    [Header("Cursor Settings")]
    [SerializeField] private Texture2D cursorTexture;
    [SerializeField] private Vector2 hotspot = Vector2.zero; // 클릭 포인트 (보통 왼쪽 위)
    [SerializeField] private CursorMode cursorMode = CursorMode.Auto;

    [Header("Game Cursor")]
    [SerializeField] private bool hideInGame = true; // 게임 중에는 숨기기
    [SerializeField] private bool showInUI = true; // UI에서는 표시

    private bool isUIActive = false;

    private void Awake()
    {
        // Scene 로드 이벤트 구독
        UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void Start()
    {
        InitializeCursor();
    }

    /// <summary>
    /// Scene 로드 시 커서 재설정
    /// </summary>
    private void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode mode)
    {
        Debug.Log($"CursorManager: Scene loaded - {scene.name}");
        isUIActive = false;
        InitializeCursor();
    }

    /// <summary>
    /// 커서 초기화
    /// </summary>
    private void InitializeCursor()
    {
        // 커스텀 커서 설정
        if (cursorTexture != null)
        {
            SetCustomCursor();
        }

        // 게임 중 커서 상태 설정
        if (hideInGame && !isUIActive)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Confined;
        }
        else
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        // GameManager 이벤트 구독 (재구독 방지)
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnGameVictory -= ShowCursor;
            GameManager.Instance.OnGameDefeat -= ShowCursor;

            GameManager.Instance.OnGameVictory += ShowCursor;
            GameManager.Instance.OnGameDefeat += ShowCursor;
        }
    }

    /// <summary>
    /// 커스텀 커서 설정
    /// </summary>
    private void SetCustomCursor()
    {
        Cursor.SetCursor(cursorTexture, hotspot, cursorMode);
        Debug.Log("Custom cursor set!");
    }

    /// <summary>
    /// 기본 커서로 복원
    /// </summary>
    private void SetDefaultCursor()
    {
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }

    /// <summary>
    /// 커서 표시 (승리/패배 시)
    /// </summary>
    private void ShowCursor()
    {
        if (showInUI)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            isUIActive = true;
        }
    }

    /// <summary>
    /// 커서 숨기기 (게임 중)
    /// </summary>
    public void HideCursor()
    {
        if (hideInGame && !isUIActive)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Confined;
        }
    }

    private void OnDestroy()
    {
        // Scene 로드 이벤트 구독 해제
        UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;

        // GameManager 이벤트 구독 해제
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnGameVictory -= ShowCursor;
            GameManager.Instance.OnGameDefeat -= ShowCursor;
        }

        // 기본 커서로 복원
        SetDefaultCursor();
    }
}