using UnityEngine;
using UnityEngine.SceneManagement;

namespace MarioPlatformer.Managers
{
    /// <summary>
    /// 遊戲管理器
    /// 管理遊戲狀態、分數、生命值等全域資料
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        // 單例模式
        public static GameManager Instance { get; private set; }

        [Header("玩家狀態")]
        [SerializeField] private int playerLives = 3;
        [SerializeField] private int currentScore = 0;

        [Header("關卡設定")]
        [SerializeField] private float levelTimeLimit = 300f; // 5 分鐘

        // 遊戲狀態
        private bool isGamePaused = false;
        private float currentLevelTime;

        // 事件
        public event System.Action<int> OnScoreChanged;
        public event System.Action<int> OnLivesChanged;
        public event System.Action OnGameOver;
        public event System.Action OnLevelComplete;

        private void Awake()
        {
            // 單例模式實作
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
                return;
            }

            currentLevelTime = levelTimeLimit;
        }

        private void Update()
        {
            if (isGamePaused) return;

            // 倒數計時
            if (currentLevelTime > 0)
            {
                currentLevelTime -= Time.deltaTime;

                if (currentLevelTime <= 0)
                {
                    currentLevelTime = 0;
                    TimeUp();
                }
            }
        }

        #region 分數管理

        public void AddScore(int points)
        {
            currentScore += points;
            OnScoreChanged?.Invoke(currentScore);
        }

        public int GetScore()
        {
            return currentScore;
        }

        #endregion

        #region 生命值管理

        public void LoseLife()
        {
            playerLives--;
            OnLivesChanged?.Invoke(playerLives);

            if (playerLives <= 0)
            {
                GameOver();
            }
            else
            {
                RestartLevel();
            }
        }

        public void AddLife()
        {
            playerLives++;
            OnLivesChanged?.Invoke(playerLives);
        }

        public int GetLives()
        {
            return playerLives;
        }

        #endregion

        #region 遊戲流程控制

        public void PauseGame()
        {
            isGamePaused = true;
            Time.timeScale = 0f;
        }

        public void ResumeGame()
        {
            isGamePaused = false;
            Time.timeScale = 1f;
        }

        public bool IsGamePaused()
        {
            return isGamePaused;
        }

        private void TimeUp()
        {
            Debug.Log("時間到！");
            LoseLife();
        }

        public void LevelComplete()
        {
            Debug.Log("關卡完成！");

            // 時間獎勵分數
            int timeBonus = Mathf.RoundToInt(currentLevelTime * 10);
            AddScore(timeBonus);

            OnLevelComplete?.Invoke();

            // 可以在這裡載入下一關
            // LoadNextLevel();
        }

        private void GameOver()
        {
            Debug.Log("遊戲結束！");
            OnGameOver?.Invoke();

            // 可以在這裡顯示遊戲結束畫面
            // 或重新載入主選單
        }

        public void RestartLevel()
        {
            Time.timeScale = 1f;
            isGamePaused = false;
            currentLevelTime = levelTimeLimit;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        public void LoadNextLevel()
        {
            int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;

            if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
            {
                currentLevelTime = levelTimeLimit;
                SceneManager.LoadScene(nextSceneIndex);
            }
            else
            {
                Debug.Log("已經是最後一關！");
                // 可以返回主選單或顯示勝利畫面
            }
        }

        public void LoadMainMenu()
        {
            Time.timeScale = 1f;
            isGamePaused = false;
            SceneManager.LoadScene(0); // 假設主選單是第一個場景
        }

        #endregion

        #region 時間管理

        public float GetRemainingTime()
        {
            return currentLevelTime;
        }

        public void AddTime(float seconds)
        {
            currentLevelTime += seconds;
        }

        #endregion

        // 重置遊戲資料（新遊戲時使用）
        public void ResetGameData()
        {
            currentScore = 0;
            playerLives = 3;
            currentLevelTime = levelTimeLimit;
            isGamePaused = false;

            OnScoreChanged?.Invoke(currentScore);
            OnLivesChanged?.Invoke(playerLives);
        }
    }
}
