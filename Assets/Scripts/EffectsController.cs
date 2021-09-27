using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class EffectsController : MonoBehaviour
{
    // Use a singleton pattern to make the class globally accessible

    #region Singleton

    private static EffectsController EffectsControllerInstance;

    public static EffectsController Instance
    {
        get
        {
            if (EffectsControllerInstance == null) EffectsControllerInstance = FindObjectOfType<EffectsController>();
            return EffectsControllerInstance;
        }
    }

    #endregion

    [SerializeField] private VolumeProfile volumeProfile;
    private DepthOfField depthOfField;

    public const float DefaultVignetteIntensity = 0.4f;
    public const float DefaultChromaticAberrationIntensity = 0.1f;

    /// <summary>
    /// Unity Event function.
    /// Get component references.
    /// </summary>
    private void Awake()
    {
        volumeProfile.TryGet(out depthOfField);
    }

    /// <summary>
    /// Enable/disable depth of field effect.
    /// </summary>
    /// <param name="value">Enable state</param>
    public void SetDepthOfFieldEnabled(bool value)
    {
        depthOfField.active = value;
    }
}
