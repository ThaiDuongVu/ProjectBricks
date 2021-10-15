using System.Collections.Generic;
using UnityEngine;

public class CameraShaker : MonoBehaviour
{
    // Use the singleton pattern to make the class globally accessible

    #region Singleton

    private static CameraShaker CameraShakerInstance;

    public static CameraShaker Instance
    {
        get
        {
            if (CameraShakerInstance == null) CameraShakerInstance = FindObjectOfType<CameraShaker>();
            return CameraShakerInstance;
        }
    }

    #endregion

    private float shakeDuration; // How long to shake the camera
    private float shakeIntensity; // How hard to shake the camera
    private float decreaseFactor; // How steep should the shake decrease

    private Vector3 originalPosition;

    /// <summary>
    /// Unity Event function.
    /// Initialize before first frame update.
    /// </summary>
    private void Start()
    {
        originalPosition = transform.position;
    }

    /// <summary>
    /// Unity Event function.
    /// Update at consistent time.
    /// </summary>
    private void FixedUpdate()
    {
        Randomize();
    }

    /// <summary>
    /// Randomize camera position by shake intensity if is shaking.
    /// </summary>
    private void Randomize()
    {
        // While shake duration is greater than 0
        if (shakeDuration > 0)
        {
            // Randomize position
            transform.localPosition = originalPosition + Random.insideUnitSphere * shakeIntensity;
            // Decrease shake duration
            shakeDuration -= Time.fixedDeltaTime * decreaseFactor * Time.timeScale;
        }
        // When shake duration reaches 0
        else
        {
            // Reset everything
            shakeDuration = 0f;
            transform.localPosition = originalPosition;
        }
    }

    #region Shake Method

    /// <summary>
    /// Start shaking camera.
    /// </summary>
    /// <param name="cameraShakeMode">Mode at which to shake</param>
    public void Shake(CameraShakeMode cameraShakeMode)
    {
        originalPosition = new Vector3(0f, 0f, -10f);

        switch (cameraShakeMode)
        {
            case CameraShakeMode.Micro:
                shakeDuration = 0.05f;
                shakeIntensity = 0.05f;
                break;

            case CameraShakeMode.Light:
                shakeDuration = 0.1f;
                shakeIntensity = 0.1f;
                break;

            case CameraShakeMode.Normal:
                shakeDuration = 0.15f;
                shakeIntensity = 0.15f;
                break;

            case CameraShakeMode.Hard:
                shakeDuration = 0.3f;
                shakeIntensity = 0.3f;
                break;

            default:
                return;
        }

        decreaseFactor = 2f;
    }

    #endregion
}