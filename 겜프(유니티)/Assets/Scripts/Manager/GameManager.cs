using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 게임 전체 관리 (타이머, 승리/패배)
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Game Settings")]
    [SerializeField] private float gameDuration = 600f; // 10분 (초 단위)

    [Header("UI")]
    [SerializeField] private VictoryUI victoryScreen;
    [SerializeField] private DefeatUI defeatScreen;

    private float remainingTime;
    private float startTime; // 시작 시간 기록
    private bool gameEnded = false;

    // 이벤트
    public delegate void TimeUpdated(float remaining, float total);
    public event TimeUpdated OnTimeUpdated;

    public delegate void TimerChanged(float remaining);
    public event TimerChanged OnTimerChanged; // 기존 TimerUI 호환

    public delegate void GameVictory();
    public event GameVictory OnGameVictory;

    public delegate void GameDefeat();
    public event GameDefeat OnGameDefeat;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        // Scene 로드 이벤트 구독
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void Start()
    {
        remainingTime = gameDuration;
        startTime = Time.time;

        // UI 초기화
        if (victoryScreen != null) victoryScreen.gameObject.SetActive(false);
        if (defeatScreen != null) defeatScreen.gameObject.SetActive(false);

        // 배경 음악 재생 (Restart 후에도 재생되도록)
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayBackgroundMusic();
            Debug.Log("Background music started");
        }

        Debug.Log($"Game started! Duration: {gameDuration}s ({gameDuration / 60f:F1} minutes)");
    }

    /// <summary>
    /// Scene 로드 완료 시 호출
    /// </summary>
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"Scene loaded: {scene.name}");

        // 게임 상태 초기화
        gameEnded = false;
        remainingTime = gameDuration;
        startTime = Time.time;

        // Time.timeScale 복원 (혹시 모를 경우 대비)
        Time.timeScale = 1f;

        // 커서 제어는 CursorManager에 맡김!
    }

    private void OnDestroy()
    {
        // 이벤트 구독 해제
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Update()
    {
        if (gameEnded) return;

        // 타이머 카운트다운
        remainingTime -= Time.deltaTime;

        // UI 업데이트 (두 이벤트 모두 발생)
        OnTimeUpdated?.Invoke(remainingTime, gameDuration);
        OnTimerChanged?.Invoke(remainingTime);

        // 시간 초과
        if (remainingTime <= 0f)
        {
            remainingTime = 0f;
            TimeOut();
        }
    }

    /// <summary>
    /// 승리 처리
    /// </summary>
    public void Victory()
    {
        if (gameEnded) return;

        gameEnded = true;

        float survivedTime = gameDuration - remainingTime;
        Debug.Log($"Victory! You escaped! Survived for {survivedTime:F1}s");

        // 승리 음악
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayVictoryMusic();
        }

        OnGameVictory?.Invoke();

        // 승리 화면 표시
        if (victoryScreen != null)
        {
            victoryScreen.Show(survivedTime);
        }

        // 게임 일시정지
        Time.timeScale = 0f;
    }

    /// <summary>
    /// 패배 처리 (시간 초과)
    /// </summary>
    private void TimeOut()
    {
        if (gameEnded) return;

        gameEnded = true;
        Debug.Log("Defeat! Time's up!");

        // 패배 음악
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayDefeatMusic();
        }

        OnGameDefeat?.Invoke();

        // 패배 화면 표시 (시간 초과)
        if (defeatScreen != null)
        {
            defeatScreen.ShowTimeUp();
        }

        // 게임 일시정지
        Time.timeScale = 0f;
    }

    /// <summary>
    /// 패배 처리 (플레이어 사망) - 기존 GameOver 호환
    /// </summary>
    public void GameOver()
    {
        PlayerDied();
    }

    /// <summary>
    /// 패배 처리 (플레이어 사망)
    /// </summary>
    public void PlayerDied()
    {
        if (gameEnded) return;

        gameEnded = true;
        Debug.Log("Defeat! You died!");

        // 패배 음악
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayDefeatMusic();
        }

        OnGameDefeat?.Invoke();

        // 패배 화면 표시 (사망)
        if (defeatScreen != null)
        {
            defeatScreen.ShowDeath();
        }

        // 게임 일시정지
        Time.timeScale = 0f;
    }

    /// <summary>
    /// 게임 재시작
    /// </summary>
    public void RestartGame()
    {
        Debug.Log("Restarting game...");

        // 1. Time.timeScale 복원
        Time.timeScale = 1f;

        // 2. 음악 정지
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.StopMusic();
            Debug.Log("Music stopped for restart");
        }

        // 3. 인벤토리 초기화
        if (InventorySystem.Instance != null)
        {
            InventorySystem.Instance.ClearInventory();
            Debug.Log("Inventory cleared");
        }

        // 4. Scene 재시작
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    /// <summary>
    /// 메인 메뉴로
    /// </summary>
    public void LoadMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    /// <summary>
    /// 게임 종료
    /// </summary>
    public void QuitGame()
    {
        Debug.Log("Quitting game...");

#if UNITY_EDITOR
        // Unity Editor에서는 Play 모드 종료
        UnityEditor.EditorApplication.isPlaying = false;
#else
        // 빌드된 게임에서는 게임 종료
        Application.Quit();
#endif
    }

    /// <summary>
    /// 남은 시간 (기존 GetTimeRemaining 호환)
    /// </summary>
    public float GetTimeRemaining()
    {
        return remainingTime;
    }

    /// <summary>
    /// 남은 시간
    /// </summary>
    public float GetRemainingTime()
    {
        return remainingTime;
    }

    /// <summary>
    /// 게임 종료 여부
    /// </summary>
    public bool IsGameEnded()
    {
        return gameEnded;
    }

    /// <summary>
    /// 시간 포맷 (MM:SS) - 기존 FormatTime 호환
    /// </summary>
    public static string FormatTime(float timeInSeconds)
    {
        int minutes = Mathf.FloorToInt(timeInSeconds / 60f);
        int seconds = Mathf.FloorToInt(timeInSeconds % 60f);
        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}