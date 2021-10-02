using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    // Use a singleton pattern to make the class globally accessible

    #region Singleton

    private static GameController GameControllerInstance;

    public static GameController Instance
    {
        get
        {
            if (GameControllerInstance == null) GameControllerInstance = FindObjectOfType<GameController>();
            return GameControllerInstance;
        }
    }

    #endregion

    public GameState State { get; set; } = GameState.Started;

    [SerializeField] private Button pauseButton;
    [SerializeField] private Button forfeitButton;
    [SerializeField] private Menu pauseMenu;
    [SerializeField] private Menu gameOverMenu;

    private int score;
    private int highScore;
    [SerializeField] private Text scoreText;
    private bool newHighScore;
    private const int MaxScore = 999999;
    public int Score
    {
        get => score;
        set
        {
            if (value > MaxScore)
            {
                score = MaxScore;
                return;
            }

            score = value;
            scoreText.text = "Score: " + score;

            if (score > highScore)
            {
                highScore = score;
                PlayerPrefs.SetInt("HighScore", highScore);

                if (!newHighScore)
                {
                    SendUIMessage("New high score!");
                    newHighScore = true;
                }
            }
        }
    }

    [SerializeField] private GameObject uiMessage;

    /// <summary>
    /// Unity Event function.
    /// Initialize before first frame update.
    /// </summary>
    private void Start()
    {
        Application.targetFrameRate = 60;
        EffectsController.Instance.SetDepthOfFieldEnabled(false);

        highScore = PlayerPrefs.GetInt("HighScore", 0);
        uiMessage.SetActive(false);

        AdsController.Instance.ShowBanner();
    }

    /// <summary>
    /// Pause current game.
    /// </summary>
    public void Pause()
    {
        Time.timeScale = 0f;
        
        pauseButton.gameObject.SetActive(false);
        forfeitButton.gameObject.SetActive(false);
        State = GameState.Paused;

        EffectsController.Instance.SetDepthOfFieldEnabled(true);
        pauseMenu.gameObject.SetActive(true);
    }

    /// <summary>
    /// Resume current game.
    /// </summary>
    public void Resume()
    {
        Time.timeScale = 1f;

        pauseButton.gameObject.SetActive(true);
        forfeitButton.gameObject.SetActive(true);
        State = GameState.Started;

        EffectsController.Instance.SetDepthOfFieldEnabled(false);
        pauseMenu.gameObject.SetActive(false);
    }

    /// <summary>
    /// Handle game over.
    /// </summary>
    public void GameOver()
    {
        Time.timeScale = 0f;

        pauseButton.gameObject.SetActive(false);
        forfeitButton.gameObject.SetActive(false);
        State = GameState.Over;

        EffectsController.Instance.SetDepthOfFieldEnabled(true);
        gameOverMenu.gameObject.SetActive(true);

        PlayerPrefs.SetInt("LastGameOver", 0);
    }

    /// <summary>
    /// Handle game over.
    /// </summary>
    public void SendUIMessage(string message)
    {
        uiMessage.SetActive(false);
        uiMessage.GetComponentInChildren<Text>().text = message;
        uiMessage.SetActive(true);
    }
}
