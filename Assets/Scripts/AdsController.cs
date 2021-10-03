using UnityEngine;
using UnityEngine.Advertisements;

public class AdsController : MonoBehaviour, IUnityAdsInitializationListener, IUnityAdsLoadListener, IUnityAdsShowListener
{
    // Use a singleton pattern to make the class globally accessible

    #region Singleton

    private static AdsController AdsControllerInstance;

    public static AdsController Instance
    {
        get
        {
            if (AdsControllerInstance == null) AdsControllerInstance = FindObjectOfType<AdsController>();
            return AdsControllerInstance;
        }
    }

    #endregion

    private const string AndroidGameId = "4387035";
    private const string iOSGameId = "4387034";
    private bool testMode = false;
    private bool enablePerPlacementMode = false;

    private string gameId;

    private const string AndroidBannerAdUnitId = "Banner_Android";
    private const string iOSBannerAdUnitId = "Banner_iOS";
    private string bannerAdUnit;

    private const string AndroidInterstitialAdUnitId = "Interstitial_Android";
    private const string iOSInterstitialAdUnitId = "Interstitial_iOS";
    private string interstitialAdUnit;

    private BannerPosition bannerPosition = BannerPosition.TOP_CENTER;

    /// <summary>
    /// Unity Event function.
    /// Get component references.
    /// </summary>
    private void Awake()
    {
        gameId = (Application.platform == RuntimePlatform.Android) ? AndroidGameId : iOSGameId;
        bannerAdUnit = (Application.platform == RuntimePlatform.Android) ? AndroidBannerAdUnitId : iOSBannerAdUnitId;
        interstitialAdUnit = (Application.platform == RuntimePlatform.Android) ? AndroidInterstitialAdUnitId : iOSInterstitialAdUnitId;

        Advertisement.Initialize(gameId, testMode, enablePerPlacementMode, this);
        Advertisement.Banner.SetPosition(bannerPosition);
    }

    /// <summary>
    /// On Ads initialization successful.
    /// </summary>
    public void OnInitializationComplete()
    {
        Debug.Log("Unity Ads initialization complete.");
    }

    /// <summary>
    /// On Ads initialization failed.
    /// </summary>
    public void OnInitializationFailed(UnityAdsInitializationError error, string message)
    {
        Debug.Log($"Unity Ads Initialization Failed: {error.ToString()} - {message}");
    }

    /// <summary>
    /// Display a banner ad at a position on screen.
    /// </summary>
    public void ShowBanner()
    {
        // Load banner first
        BannerLoadOptions bannerLoadOptions = new BannerLoadOptions
        {
            loadCallback = OnBannerLoaded,
            errorCallback = OnBannerError,
        };
        Advertisement.Banner.Load(bannerAdUnit, bannerLoadOptions);

        // Then show the banner
        BannerOptions bannerOptions = new BannerOptions
        {
            clickCallback = OnBannerClicked,
            hideCallback = OnBannerHidden,
            showCallback = OnBannerShown,
        };
        Advertisement.Banner.Show(bannerAdUnit, bannerOptions);
    }

    /// <summary>
    /// Display a full screen ad that can be closed by user.
    /// </summary>
    public void ShowInterstitialAd()
    {
        // Load ad first
        Advertisement.Load(interstitialAdUnit, this);
        // Then show the ad
        Advertisement.Show(interstitialAdUnit, this);
    }

    /// <summary>
    /// On Banner Ad load successful.
    /// </summary>
    private void OnBannerLoaded() { }

    /// <summary>
    /// On Banner Ad load failed.
    /// </summary>
    /// <param name="message">Error message</param>
    private void OnBannerError(string message) { }

    /// <summary>
    /// On Banner Ad clicked.
    /// </summary>
    private void OnBannerClicked() { }

    /// <summary>
    /// On Banner Ad load hidden.
    /// </summary>
    private void OnBannerHidden() { }

    /// <summary>
    /// On Banner Ad load displayed.
    /// </summary>
    private void OnBannerShown() { }

    /// <summary>
    /// On Display Ad load successful.
    /// </summary>
    public void OnUnityAdsAdLoaded(string adUnitId) { }

    /// <summary>
    /// On Display Ad load failed.
    /// </summary>
    public void OnUnityAdsFailedToLoad(string adUnitId, UnityAdsLoadError error, string message) { }

    /// <summary>
    /// On Display Ad shown failed.
    /// </summary>  
    public void OnUnityAdsShowFailure(string adUnitId, UnityAdsShowError error, string message) { }

    /// <summary>
    /// On Display Ad shown start.
    /// </summary>
    public void OnUnityAdsShowStart(string adUnitId) { }

    /// <summary>
    /// On Display Ad clicked.
    /// </summary>
    public void OnUnityAdsShowClick(string adUnitId) { }

    /// <summary>
    /// On Display Ad completed.
    /// </summary>
    public void OnUnityAdsShowComplete(string adUnitId, UnityAdsShowCompletionState showCompletionState) { }
}
