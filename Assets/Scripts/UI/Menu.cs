using UnityEngine;

public class Menu : MonoBehaviour
{
    [SerializeField] private bool disableOnStartup;

    /// <summary>
    /// Unity Event function.
    /// Initialize before first frame update.
    /// </summary>
    private void Start()
    {
        gameObject.SetActive(!disableOnStartup);
    }
    
}
