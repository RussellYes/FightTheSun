using System;
using UnityEngine;
using Unity.Services.LevelPlay;

public class AdLoader : MonoBehaviour
{
    public static AdLoader Instance { get; private set; }
    public static event Action RewardedAdLoaded;

    [Header("LevelPlay Keys")]
    [SerializeField] private string appKey;
    [SerializeField] private string rewardedAdUnitId;

    private LevelPlayRewardedAd rewardedAd;
    private bool sdkInitialized;
    private bool initRequested;
    private bool loadRequested;
    private bool isShuttingDown;
    private int loadRetryCount = 0;
    private const int MAX_RETRIES = 3;
    // Ad loading state
    private bool isAdLoading;
    public static event Action AdLoadStarted;
    // public static event Action AdLoadFinished; // (not used currently)

    private void Awake()
    {
        Debug.Log("AdLoader Awake");
        if (Instance != null && Instance != this)
        {
            Debug.Log("AdLoader Awake duplicate detected, destroying self");
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        Debug.Log("AdLoader Start - Initializing LevelPlay");

        initRequested = true;

        // Register initialization callbacks
        LevelPlay.OnInitSuccess += SdkInitializationCompletedEvent;
        LevelPlay.OnInitFailed += SdkInitializationFailedEvent;

        // Apply GDPR consent before initialization
        ApplyGdprConsent();

        // SDK init
        LevelPlay.Init(appKey);
    }

    private void OnEnable()
    {
        Debug.Log("AdLoader OnEnable");
        RewardedAdPlayer.RequestLoadAd += HandleRequestLoadAd;
        RewardedAdPlayer.AdClosed += HandleAdClosedFromPlayer;
    }

    private void OnDisable()
    {
        Debug.Log("AdLoader OnDisable");
        RewardedAdPlayer.RequestLoadAd -= HandleRequestLoadAd;
        RewardedAdPlayer.AdClosed -= HandleAdClosedFromPlayer;

        LevelPlay.OnInitSuccess -= SdkInitializationCompletedEvent;
        LevelPlay.OnInitFailed  -= SdkInitializationFailedEvent;

        if (Instance == this) Instance = null;
    }


    private void OnApplicationQuit()
    {
        Debug.Log("AdLoader OnApplicationQuit");
        isShuttingDown = true;
    }

    private void ApplyGdprConsent()
    {
        Debug.Log("AdLoader ApplyGdprConsent");

        // FORCE NON-PERSONALIZED ADS (no consent)
        LevelPlay.SetConsent(false);
        LevelPlay.SetMetaData("gdpr_consent", "false");
        LevelPlay.SetMetaData("do_not_sell", "true");
        LevelPlay.SetMetaData("restrict_data_processing", "true");

        Debug.Log("AdLoader ApplyGdprConsent - Non personalized ads configured");
    }

    // ===== SDK Initialization Callbacks =====
    private void SdkInitializationCompletedEvent(LevelPlayConfiguration config)
    {
        Debug.Log("AdLoader SdkInitializationCompletedEvent");
        sdkInitialized = true;
        initRequested = false;
        CreateRewardedAdInstance();

        // Auto-load an ad if requested
        if (loadRequested)
        {
            TryStartQueuedLoad();
        }
    }

    private void SdkInitializationFailedEvent(LevelPlayInitError error)
    {
        Debug.LogError($"AdLoader SdkInitializationFailedEvent: {error}");
        initRequested = false;
        sdkInitialized = false;
    }

    // ===== Rewarded Ad Instance Creation =====
    private void CreateRewardedAdInstance()
    {
        if (rewardedAd != null)
        {
            Debug.Log("AdLoader CreateRewardedAdInstance - already exists");
            return;
        }

        Debug.Log("AdLoader SdkInitializationFailedEvent - creating LevelPlayRewardedAd instance");
        rewardedAd = new LevelPlayRewardedAd(rewardedAdUnitId);

        // Register to Rewarded Video events (IronSource pattern)
        rewardedAd.OnAdLoaded += RewardedVideoOnLoadedEvent;
        rewardedAd.OnAdLoadFailed += RewardedVideoOnAdLoadFailedEvent;
        rewardedAd.OnAdDisplayed += RewardedVideoOnAdDisplayedEvent;
        rewardedAd.OnAdDisplayFailed += RewardedVideoOnAdDisplayedFailedEvent;
        rewardedAd.OnAdRewarded += RewardedVideoOnAdRewardedEvent;
        rewardedAd.OnAdClosed += RewardedVideoOnAdClosedEvent;
    }

    // ===== Rewarded Ad Event Handlers =====
    private void RewardedVideoOnLoadedEvent(LevelPlayAdInfo adInfo)
    {
        Debug.Log($"AdLoader RewardedVideoOnLoadedEvent - adUnit={adInfo.AdUnitId}");
        FindFirstObjectByType<AppUpdateChecker>()?.TrackAdEvent("ad_load_success", "retry_count", loadRetryCount);

        loadRequested = false;
        isAdLoading = false;
        loadRequested = false;
        loadRetryCount = 0;
        RewardedAdLoaded?.Invoke();
    }

    private void RewardedVideoOnAdLoadFailedEvent(LevelPlayAdError error)
    {
        Debug.LogError($"AdLoader RewardedVideoOnAdLoadFailedEvent - LevelPlayAdError {error} ErrorCode: {error.ErrorCode}, ErrorMessage: {error.ErrorMessage}");
        FindFirstObjectByType<AppUpdateChecker>()?.TrackAdEvent("ad_load_failed", "retry_count", loadRetryCount);

        loadRequested = false;

        if (loadRetryCount < MAX_RETRIES && !isShuttingDown)
        {
            loadRetryCount++;
            Debug.Log($"AdLoader RewardedVideoOnAdLoadFailedEvent - Retrying ad load ({loadRetryCount}/{MAX_RETRIES}) in 3 seconds...");
            Invoke(nameof(RetryLoad), 3f);
        }
        else
        {
            loadRetryCount = 0;
        }
    }

    private void RewardedVideoOnAdDisplayedEvent(LevelPlayAdInfo adInfo)
    {
        Debug.Log($"AdLoader RewardedVideoOnAdDisplayedEvent - adUnit={adInfo.AdUnitId}");
    }

    private void RewardedVideoOnAdDisplayedFailedEvent(LevelPlayAdInfo adInfo, LevelPlayAdError error)
    {
        Debug.LogError($"AdLoader RewardedVideoOnAdDisplayedFailedEvent - {error}");
    }

    private void RewardedVideoOnAdRewardedEvent(LevelPlayAdInfo adInfo, LevelPlayReward reward)
    {
        Debug.Log($"AdLoader RewardedVideoOnAdRewardedEvent - amount={reward.Amount}");
        var player = FindFirstObjectByType<RewardedAdPlayer>(FindObjectsInactive.Include);
        if (player != null) player.OnSdkAdRewarded();
    }

    private void RewardedVideoOnAdClosedEvent(LevelPlayAdInfo adInfo)
    {
        Debug.Log($"AdLoader RewardedVideoOnAdClosedEvent - adUnit={adInfo.AdUnitId}");
        var player = FindFirstObjectByType<RewardedAdPlayer>(FindObjectsInactive.Include);
        if (player != null) player.OnSdkAdClosed();
    }

    // ===== API used by RewardedAdPlayer =====
    public bool IsRewardedReady()
    {
        if (rewardedAd == null)
        {
            Debug.Log("AdLoader IsRewardedReady - false (rewardedAd is null)");
            return false;
        }

        bool ready = rewardedAd.IsAdReady();
        Debug.Log($"AdLoader IsRewardedReady - {ready}");
        return ready;
    }

    public void RequestLoad()
    {
        Debug.Log("AdLoader RequestLoad");

        if (isAdLoading || (rewardedAd != null && rewardedAd.IsAdReady()))
        {
            Debug.Log("AdLoader RequestLoad - already loading or ready");
            return;
        }

        if (initRequested && !sdkInitialized)
        {
            Debug.Log("AdLoader RequestLoad - SDK still initializing, queuing load request");
            loadRequested = true;
            return;
        }

        if (!sdkInitialized)
        {
            Debug.LogError("AdLoader RequestLoad - SDK not initialized, cannot load ads");
            return;
        }

        loadRequested = true;
        isAdLoading = true;
        AdLoadStarted?.Invoke();

        if (rewardedAd == null)
        {
            CreateRewardedAdInstance();
        }

        TryStartQueuedLoad();
    }

    private void TryStartQueuedLoad()
    {
        if (!sdkInitialized || rewardedAd == null)
        {
            Debug.Log("AdLoader TryStartQueuedLoad - SDK not ready");
            return;
        }
        if (!loadRequested || isAdLoading)
        {
            Debug.Log("AdLoader TryStartQueuedLoad – no load requested or already loading");
            return;
        }

        Debug.Log("AdLoader TryStartQueuedLoad - calling rewardedAd.LoadAd()");
        rewardedAd.LoadAd();
    }

    private void RetryLoad()
    {
        Debug.Log($"AdLoader RetryLoad - is sdkInitialized: {sdkInitialized}");
        if (sdkInitialized)
        {
            loadRequested = true;
            TryStartQueuedLoad();
        }
    }

    public bool TryShowAd(string placement = null)
    {
        Debug.Log($"AdLoader TryShowAd - ENTER: rewardedAd={rewardedAd != null}");
        if (rewardedAd == null)
        {
            Debug.LogWarning("AdLoader TryShowAd rewardedAd is null");
            return false;
        }
        if (!rewardedAd.IsAdReady())
        {
            Debug.Log("AdLoader TryShowAd - not ready");
            return false;
        }

        Debug.Log("AdLoader TryShowAd - showing now");
        rewardedAd.ShowAd(placement);
        return true;
    }

    // ===== Event Handlers from RewardedAdPlayer =====
    private void HandleAdClosedFromPlayer(bool rewardGranted)
    {
        Debug.Log($"AdLoader HandleAdClosedFromPlayer - rewardGranted={rewardGranted}");
        if (isShuttingDown) return;

        if (rewardGranted)
        {
            Debug.Log("AdLoader HandleAdClosedFromPlayer - destroying instance (granted cycle complete)");
            rewardedAd?.DestroyAd();
            rewardedAd = null;

            // Auto-reload for next time
            if (sdkInitialized)
            {
                loadRequested = true;
                CreateRewardedAdInstance();
                TryStartQueuedLoad();
            }
        }
    }

    private void HandleRequestLoadAd()
    {
        Debug.Log("AdLoader HandleRequestLoadAd");
        RequestLoad();
    }

    // Ads ALWAYS fail on the first request, so this code section is to fake an ad request as a temp fix for LevelPlay's issue.
    public void PrewarmAdSystem()
    {
        Debug.Log("AdsDebug AdLoader.PrewarmAdSystem - Making initial ad request");

        if (rewardedAd == null)
        {
            CreateRewardedAdInstance();
        }

        // This first load will likely fail, but it primes the system
        RequestLoad();
    }

}
