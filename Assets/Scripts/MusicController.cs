using UnityEngine;

public class MusicController : MonoBehaviour
{
    // Use a singleton pattern to make the class globally accessible

    #region Singleton

    private static MusicController MusicControllerInstance;

    public static MusicController Instance
    {
        get
        {
            if (MusicControllerInstance == null) MusicControllerInstance = FindObjectOfType<MusicController>();
            return MusicControllerInstance;
        }
    }

    #endregion
}
