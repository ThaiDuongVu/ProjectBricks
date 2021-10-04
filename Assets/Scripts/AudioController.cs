using UnityEngine;

public class AudioController : MonoBehaviour
{
    // Use a singleton pattern to make the class globally accessible

    #region Singleton

    private static AudioController AudioControllerInstance;

    public static AudioController Instance
    {
        get
        {
            if (AudioControllerInstance == null) AudioControllerInstance = FindObjectOfType<AudioController>();
            return AudioControllerInstance;
        }
    }

    #endregion

    private AudioSource[] audioSources;

    /// <summary>
    /// Unity Event function.
    /// Get component references.
    /// </summary>
    private void Awake()
    {
        audioSources = GetComponentsInChildren<AudioSource>();
    }

    /// <summary>
    /// Play a selected audio clip.
    /// </summary>
    /// <param name="audioType">Which audio to play</param>
    public void Play(AudioType audioType)
    {
        audioSources[((int)audioType)].Play();
    }
}
