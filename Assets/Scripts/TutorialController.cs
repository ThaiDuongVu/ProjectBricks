using UnityEngine;

public class TutorialController : MonoBehaviour
{
    private int tutorialProgress;
    private int maxTutorialProgress;
    [SerializeField] private GameObject[] tutorialElements;

    /// <summary>
    /// Unity Event function.
    /// On current object enabled.
    /// </summary>
    private void OnEnable()
    {
        tutorialProgress = 0;
        foreach (var element in tutorialElements) element.SetActive(false);
        tutorialElements[0].SetActive(true);
    }

    /// <summary>
    /// Unity Event function.
    /// Initialize before first frame update.
    /// </summary>
    private void Start()
    {
        gameObject.SetActive(false);
        maxTutorialProgress = tutorialElements.Length - 1;
    }

    /// <summary>
    /// Advance to the next stage of the tutorial.
    /// </summary>
    public void AdvanceTutorial()
    {
        if (tutorialProgress < maxTutorialProgress)
        {
            tutorialElements[tutorialProgress].SetActive(false);
            tutorialProgress++;
            tutorialElements[tutorialProgress].SetActive(true);
        }
        else
        {
            HomeController.Instance.EndTutorial();
        }
    }
}
