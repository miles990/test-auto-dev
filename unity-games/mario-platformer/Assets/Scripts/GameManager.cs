using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// 遊戲管理器
/// 使用單例模式管理遊戲狀態、分數、生命值等
/// </summary>
public class GameManager : MonoBehaviour
{
    #region Singleton
    private static GameManager instance;
    public static GameManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<GameManager>();
                if (instance == null)
                {
                    GameObject go = new GameObject("GameManager");
                    instance = go.AddComponent<GameManager>();
                }
            }
            return instance;
        }
    }
    #endregion

    [Header("玩家設定")]
    [SerializeField] private int startingLives = 3;
    [SerializeField] private int maxLives = 5;

    [Header("關卡設定")]
    [SerializeField] private float levelTime = 300f; // 5 分鐘

    [Header("UI 引用")]
    [SerializeField] private Text scoreText;
    [SerializeField] private Text livesText;
    [SerializeField] private Text timeText;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject levelCompletePanel;
    [SerializeField] private GameObject pausePanel;

    [Header("音效")]
    [SerializeField] private AudioClip coinSound;
    [SerializeField] private AudioClip powerUpSound;
    [SerializeField] private AudioClip gameOverSound;
    [SerializeField] private AudioClip levelCompleteSound;

    // 遊戲狀態
    private int currentScore = 0;
    private int currentLives;
    private float remainingTime;
    private bool isGameOver = false;
    private bool isPaused = false;
    private bool isLevelComplete = false;

    // 組件引用
    private AudioSource audioSource;

    // 事件系統（可選）
    public delegate void ScoreChanged(int newScore);
    public event ScoreChanged OnScoreChanged;

    public delegate void LivesChanged(int newLives);
    public event LivesChanged OnLivesChanged;

    private void Awake()
    {
        // 確保只有一個實例
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject); // 跨場景保留

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    private void Start()
    {
        InitializeGame();
    }

    private void Update()
    {
        // 暫停功能
        if (Input.GetKeyDown(KeyCode.Escape) && !isGameOver && !isLevelComplete)
        {
            TogglePause();
        }

        // 更新倒數計時
        if (!isPaused && !isGameOver && !isLevelComplete)
        {
            remainingTime -= Time.deltaTime;
            UpdateTimeUI();

            if (remainingTime <= 0)
            {
                GameOver();
            }
        }
    }

    private void InitializeGame()
    {
        currentLives = startingLives;
        currentScore = 0;
        remainingTime = levelTime;
        isGameOver = false;
        isLevelComplete = false;

        UpdateScoreUI();
        UpdateLivesUI();
        UpdateTimeUI();

        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        if (levelCompletePanel != null)
            levelCompletePanel.SetActive(false);

        if (pausePanel != null)
            pausePanel.SetActive(false);
    }

    public void AddScore(int points)
    {
        currentScore += points;
        UpdateScoreUI();
        OnScoreChanged?.Invoke(currentScore);
    }

    public void PlayerTakeDamage(int damage)
    {
        if (isGameOver) return;

        currentLives -= damage;
        UpdateLivesUI();
        OnLivesChanged?.Invoke(currentLives);

        if (currentLives <= 0)
        {
            GameOver();
        }
    }

    public void AddLife()
    {
        if (currentLives < maxLives)
        {
            currentLives++;
            UpdateLivesUI();
            OnLivesChanged?.Invoke(currentLives);
        }
    }

    public void CollectCoin(int value = 10)
    {
        AddScore(value);
        PlaySound(coinSound);
    }

    public void CollectPowerUp()
    {
        PlaySound(powerUpSound);
        // 實作能力提升邏輯（例如：速度提升、無敵等）
    }

    public void LevelComplete()
    {
        if (isLevelComplete) return;

        isLevelComplete = true;

        // 剩餘時間加分
        int timeBonus = Mathf.RoundToInt(remainingTime * 10);
        AddScore(timeBonus);

        PlaySound(levelCompleteSound);

        if (levelCompletePanel != null)
        {
            levelCompletePanel.SetActive(true);
        }

        // 可選：自動載入下一關
        // Invoke("LoadNextLevel", 3f);
    }

    private void GameOver()
    {
        if (isGameOver) return;

        isGameOver = true;
        Time.timeScale = 0f; // 暫停遊戲

        PlaySound(gameOverSound);

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void LoadNextLevel()
    {
        Time.timeScale = 1f;
        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;

        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextSceneIndex);
        }
        else
        {
            // 沒有下一關，返回主選單
            LoadMainMenu();
        }
    }

    public void LoadMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(0); // 假設主選單是第一個場景
    }

    public void TogglePause()
    {
        isPaused = !isPaused;

        if (isPaused)
        {
            Time.timeScale = 0f;
            if (pausePanel != null)
                pausePanel.SetActive(true);
        }
        else
        {
            Time.timeScale = 1f;
            if (pausePanel != null)
                pausePanel.SetActive(false);
        }
    }

    private void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = $"分數: {currentScore:D6}";
        }
    }

    private void UpdateLivesUI()
    {
        if (livesText != null)
        {
            livesText.text = $"生命: {currentLives}";
        }
    }

    private void UpdateTimeUI()
    {
        if (timeText != null)
        {
            int minutes = Mathf.FloorToInt(remainingTime / 60f);
            int seconds = Mathf.FloorToInt(remainingTime % 60f);
            timeText.text = $"時間: {minutes:D2}:{seconds:D2}";
        }
    }

    private void PlaySound(AudioClip clip)
    {
        if (clip != null && audioSource != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    // 公開屬性用於查詢遊戲狀態
    public int CurrentScore => currentScore;
    public int CurrentLives => currentLives;
    public float RemainingTime => remainingTime;
    public bool IsGameOver => isGameOver;
    public bool IsPaused => isPaused;
    public bool IsLevelComplete => isLevelComplete;
}
