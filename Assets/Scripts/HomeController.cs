using UnityEngine;
using UnityEngine.UI;

public class HomeController : MonoBehaviour
{
    [SerializeField] private Text highScoreText;

    [SerializeField] private Menu mainMenu;
    [SerializeField] private Menu continueMenu;
    [SerializeField] private Menu tutorialMenu;

    private int tutorialProgress;
    [SerializeField] private GameObject[] tutorialElements;

    /// <summary>
    /// Unity Event function.
    /// Initialize before first frame update.
    /// </summary>
    private void Start()
    {
        highScoreText.text = "High Score: " + PlayerPrefs.GetInt("HighScore", 0);
        EffectsController.Instance.SetDepthOfFieldEnabled(false);
    }

    /// <summary>
    /// Play main game.
    /// </summary>
    public void Play()
    {
        EffectsController.Instance.SetDepthOfFieldEnabled(true);

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

    /// <summary>
    /// Play a sequence of tutorial messages.
    /// </summary>
    public void EnterTutorialMode()
    {
        EffectsController.Instance.SetDepthOfFieldEnabled(true);
        mainMenu.gameObject.SetActive(false);
        tutorialMenu.gameObject.SetActive(true);
    }

    /// <summary>
    /// Advance to the next stage of the tutorial.
    /// </summary>
    public void AdvanceTutorial()
    {
        tutorialElements[tutorialProgress].SetActive(false);
        tutorialProgress++;
        tutorialElements[tutorialProgress].SetActive(true);
    }
}
