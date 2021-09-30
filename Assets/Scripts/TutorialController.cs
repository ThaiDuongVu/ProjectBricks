using UnityEngine;

public class TutorialController : MonoBehaviour
{
    private int tutorialProgress;
    private int maxTutorialProgress;
    [SerializeField] private GameObject[] tutorialElements;

    /// <summary>
    /// Unity Event function.
    /// Initialize before first frame update.
    /// </summary>
    private void Start()
    {
        gameObject.SetActive(false);
        tutorialProgress = 0;
        maxTutorialProgress = tutorialElements.Length - 1;
        foreach (var element in tutorialElements) element.SetActive(false);
        tutorialElements[0].SetActive(true);
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
