using System.IO;
using UnityEngine;
using UnityEngine.InputSystem;

public class ScreenshotController : MonoBehaviour
{
    private int screenshotCount;

    private InputManager inputManager;

    /// <summary>
    /// Unity Event function.
    /// On current object enabled.
    /// </summary>
    private void OnEnable()
    {
        screenshotCount = PlayerPrefs.GetInt("ScreenshotCount", 0);
        inputManager = new InputManager();

        // Handle screenshot taking
        inputManager.Debug.Screenshot.started += (InputAction.CallbackContext context) =>
        {
            var path = "Promotion/screenshot" + screenshotCount.ToString() + ".png";
            if (File.Exists(path)) File.Delete(path);

            ScreenCapture.CaptureScreenshot(path);
            screenshotCount++;
            PlayerPrefs.SetInt("ScreenshotCount", screenshotCount);
        };

        inputManager.Enable();
    }

    /// <summary>
    /// Unity Event function.
    /// On current object disabled.
    /// </summary>
    private void OnDisable()
    {
        inputManager.Disable();
    }
}
