using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    // Use a singleton pattern to make the class globally accessible

    #region Singleton

    private static SceneLoader SceneLoaderInstance;

    public static SceneLoader Instance
    {
        get
        {
            if (SceneLoaderInstance == null) SceneLoaderInstance = FindObjectOfType<SceneLoader>();
            return SceneLoaderInstance;
        }
    }

    #endregion

    private Animator cameraAnimator;
    private static readonly int OutroTrigger = Animator.StringToHash("outro");
    [SerializeField] private AnimationClip cameraOutroAnimationClip;
    private Canvas[] canvases;

    private string sceneToLoad = "";

    /// <summary>
    /// Unity Event function.
    /// Get component references.
    /// </summary>
    private void Awake()
    {
        if (Camera.main is { }) cameraAnimator = Camera.main.GetComponent<Animator>();
        canvases = FindObjectsOfType<Canvas>();
    }

    /// <summary>
    /// Load a scene in the background.
    /// </summary>
    private IEnumerator Load()
    {
        // Load scene in background but don't allow transition
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneToLoad, LoadSceneMode.Single);
        asyncOperation.allowSceneActivation = false;

        // Play camera animation
        cameraAnimator.SetTrigger(OutroTrigger);

        // Wait for camera animation to complete
        yield return new WaitForSeconds(cameraOutroAnimationClip.averageDuration);

        // Allow transition to new scene
        asyncOperation.allowSceneActivation = true;
    }

    /// <summary>
    /// Load a scene.
    /// </summary>
    /// <param name="scene">Scene to load</param>
    public void Load(string scene)
    {
        Time.timeScale = 1f;
        sceneToLoad = scene;

        AudioController.Instance.Play(AudioType.UIClick);
        StartCoroutine(Load());
    }

    /// <summary>
    /// Restart scene.
    /// </summary>
    public void Restart()
    {
        // Reload current active scene
        Load(SceneManager.GetActiveScene().name);
    }

    /// <summary>
    /// Quit game.
    /// </summary>
    public static void Quit()
    {
        AudioController.Instance.Play(AudioType.UIClick);
        Application.Quit();
    }
}