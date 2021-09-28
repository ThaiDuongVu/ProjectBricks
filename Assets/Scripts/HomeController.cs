using UnityEngine;
using UnityEngine.UI;

public class HomeController : MonoBehaviour
{
    [SerializeField] private Text highScoreText;

    [SerializeField] private Menu mainMenu;
    [SerializeField] private Menu continueMenu;

    /// <summary>
    /// Unity Event function.
    /// Initialize before first frame update.
    /// </summary>
    private void Start()
    {
        highScoreText.text = "High Score: " + PlayerPrefs.GetInt("HighScore", 0);
        EffectsController.Instance.SetDepthOfFieldEnabled(true);
    }

    /// <summary>
    /// Play main game.
    /// </summary>
    public void Play()
    {
        // Check if player has a game saved
        if (PlayerPrefs.GetInt("LastGameOver", 0) == 0)
        {
            SceneLoader.Instance.Load("MainGame");
        }
        else
        {
            // Show a menu asking player if they want to continue their last game or start a new one
            mainMenu.gameObject.SetActive(false);
            continueMenu.gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// Reset game over save state.
    /// </summary>
    public void ResetSaveState()
    {
        PlayerPrefs.SetInt("LastGameOver", 0);
    }
}
