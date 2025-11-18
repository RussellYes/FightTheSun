using System;
using UnityEngine;
using Unity.Services.LevelPlay;

public class AdLoader1 : MonoBehaviour
{/*
    public static AdLoader Instance { get; private set; }
    public static event Action RewardedAdLoaded;

    [Header("LevelPlay Keys (edit to your values)")]
    [SerializeField] private string appKey = "23928db7d";
    [SerializeField] private string rewardedAdUnitId = "ellw22gymypqk08a";

    private LevelPlayRewardedAd rewarded;
    private bool consentApplied;
    private bool initRequested;
    private bool sdkInitialized;
    private bool loadRequested;     // queue “please load” until init succeeds
    private bool isShuttingDown;
    private bool userConsent = false;
    private int loadRetryCount = 0;
    private const int MAX_RETRIES = 3;

    private void Awake()
    {
        Debug.Log("AdsDebug AdLoader.Awake");
        if (Instance != null && Instance != this)
        {
            Debug.Log("AdsDebug AdLoader.Awake duplicate detected, destroying self");
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        Debug.Log("AdsDebug AdLoader.OnEnable subscribe");
        DataPersister.InitializationComplete += OnDataReady;
        RewardedAdPlayer.RequestLoadAd += HandleRequestLoadAd;
        RewardedAdPlayer.AdClosed += HandleAdClosedFromPlayer;

        // LevelPlay init callbacks (note the signatures)
        LevelPlay.OnInitSuccess += OnLevelPlayInitSuccess;           // Action<LevelPlayConfiguration>
        LevelPlay.OnInitFailed  += OnLevelPlayInitFailed;            // Action<LevelPlayInitError>
    }

    private void OnDisable()
    {
        Debug.Log("AdsDebug AdLoader.OnDisable unsubscribe");
        DataPersister.InitializationComplete -= OnDataReady;
        RewardedAdPlayer.RequestLoadAd -= HandleRequestLoadAd;
        RewardedAdPlayer.AdClosed -= HandleAdClosedFromPlayer;

        LevelPlay.OnInitSuccess -= OnLevelPlayInitSuccess;
        LevelPlay.OnInitFailed  -= OnLevelPlayInitFailed;

        if (Instance == this) Instance = null;
    }

    private void Update()
    {
        // Log state every 30 seconds for debugging
        if (Time.frameCount % 1800 == 0) // ~30 seconds at 60 FPS
        {
            LogState("Periodic Update");
        }
    }

    private void OnApplicationQuit()
    {
        Debug.Log("AdsDebug AdLoader.OnApplicationQuit");
        isShuttingDown = true;
    }

    private void OnDataReady()
    {
        Debug.Log("AdsDebug AdLoader.OnDataReady");
        InitializeConsentAndRequestInit();
    }

    // ---------- Init path ----------
    private void InitializeConsentAndRequestInit()
    {
        Debug.Log("AdsDebug AdLoader.InitializeConsentAndRequestInit");

        if (sdkInitialized) // IronSource's SDK
        {
            Debug.Log("AdsDebug AdLoader.InitializeConsentAndRequestInit already initialized");
            EnsureRewardedInstance();
            return;
        }

        if (initRequested)
        {
            Debug.Log("AdsDebug AdLoader.InitializeConsentAndRequestInit already requested init");
            return;
        }

        // Apply GDPR consent immediately (no waiting for user decision)
        CheckAndApplyGdprConsent();

        // PROCEED DIRECTLY TO SDK INITIALIZATION
        InitializeSDK();
    }


    private void CheckAndApplyGdprConsent()
    {
        Debug.Log("AdsDebug AdLoader.CheckAndApplyGdprConsent");

        // FORCE NON-PERSONALIZED ADS (no consent)
        userConsent = false; // Always set to non-personalized
        ApplyGdprConsentToSDK(false); // Apply non-personalized settings to SDK
        Debug.Log("AdsDebug AdLoader: FORCING non-personalized ads (no consent)");
    }

    private void ApplyGdprConsentToSDK(bool consent)
    {
        Debug.Log($"AdsDebug AdLoader.ApplyGdprConsentToSDK: {consent} (ALWAYS FALSE = non-personalized)");

        // Set basic GDPR consent - ALWAYS FALSE FOR NON-PERSONALIZED ADS
        LevelPlay.SetConsent(false); // Force non-personalized

        // Set metadata for NON-PERSONALIZED ads only
        LevelPlay.SetMetaData("gdpr_consent", "false");
        LevelPlay.SetMetaData("do_not_sell", "true");       // Block data selling
        LevelPlay.SetMetaData("restrict_data_processing", "true"); // Restrict data processing

        Debug.Log("AdsDebug AdLoader: Non-personalized ads configured (GDPR consent = false)");
    }

    private void InitializeSDK()
    {
        if (Application.isEditor)
        {
            Debug.Log("AdsDebug AdLoader: Editor mode – simulating LevelPlay init success");
            initRequested = true;
            sdkInitialized = true;
            EnsureRewardedInstance();
            if (loadRequested) TryStartQueuedLoad();
            return;
        }

        try
        {
            Debug.Log("AdsDebug AdLoader calling LevelPlay.Init");
            initRequested = true;
            LevelPlay.Init(appKey);  // wait for OnInitSuccess before loading
        }
        catch (Exception ex)
        {
            Debug.LogError($"AdsDebug AdLoader LevelPlay.Init exception: {ex}");
        }
    }

    // MUST match Action<LevelPlayConfiguration>
    private void OnLevelPlayInitSuccess(LevelPlayConfiguration config)
    {
        Debug.Log("AdsDebug AdLoader.LevelPlay OnInitSuccess");
        sdkInitialized = true;
        EnsureRewardedInstance();

        // request an ad to load
        loadRequested = true;
        TryStartQueuedLoad();
    }

    // Matches Action<LevelPlayInitError>
    private void OnLevelPlayInitFailed(LevelPlayInitError error)
    {
        Debug.LogError($"AdsDebug AdLoader.LevelPlay OnInitFailed: {error}");
        // allow another attempt if needed
        initRequested = false;
    }

    private void ApplyGdprMetaFromSave()
    {
        bool haveChoice = false;
        bool consent = false;

        if (DataPersister.Instance != null && DataPersister.Instance.CurrentGameData != null)
        {
            var gd = DataPersister.Instance.CurrentGameData;
          //  haveChoice = gd.gdprConsentSet;
          //  consent    = gd.gdprConsentValue; // true = personalized
        }

        var apply = haveChoice ? consent : false; // default to non-personalized if unset
        LevelPlay.SetMetaData("gdpr_consent", apply ? "true" : "false");
        Debug.Log($"AdsDebug AdLoader.ApplyGdprMetaFromSave choiceSet={haveChoice} value={apply}");
    }

    // ---------- Rewarded instance & events ----------
    private void EnsureRewardedInstance()
    {
        if (!sdkInitialized)
        {
            Debug.Log("AdsDebug AdLoader.EnsureRewardedInstance skipped – SDK not initialized yet");
            return;
        }
        if (rewarded != null)
        {
            Debug.Log("AdsDebug AdLoader.EnsureRewardedInstance already exists");
            return;
        }

        Debug.Log("AdsDebug AdLoader creating LevelPlayRewardedAd instance");
        rewarded = new LevelPlayRewardedAd(rewardedAdUnitId);

        rewarded.OnAdLoaded += info =>
        {
            Debug.Log($"AdsDebug AdLoader.OnAdLoaded adUnit={info.AdUnitId}");
            loadRequested = false;
            RewardedAdLoaded?.Invoke();
        };

        rewarded.OnAdLoadFailed += error =>
        {
            Debug.LogError($"AdsDebug AdLoader.OnAdLoadFailed {error}");

            loadRequested = false;

            if (loadRetryCount < MAX_RETRIES && !isShuttingDown)
            {
                loadRetryCount++;
                Debug.Log($"AdsDebug Retrying ad load ({loadRetryCount}/{MAX_RETRIES}) in 3 seconds...");
                Invoke(nameof(RetryLoad), 3f);
            }
            else
            {
                loadRetryCount = 0;
            }
        };

        rewarded.OnAdDisplayed += info =>
        {
            Debug.Log($"AdsDebug AdLoader.OnAdDisplayed adUnit={info.AdUnitId}");
        };

        rewarded.OnAdDisplayFailed += (info, error) =>
        {
            Debug.LogError($"AdsDebug AdLoader.OnAdDisplayFailed: {error}");
        };



        // Fan-out so the player decides reward/close sequencing
        rewarded.OnAdRewarded += (info, reward) =>
        {
            Debug.Log($"AdsDebug AdLoader.OnAdRewarded amount={reward.Amount}");
            var player = FindFirstObjectByType<RewardedAdPlayer>(FindObjectsInactive.Include);
            if (player != null) player.OnSdkAdRewarded();
        };

        rewarded.OnAdClosed += info =>
        {
            Debug.Log($"AdsDebug AdLoader.OnAdClosed adUnit={info.AdUnitId}");
            var player = FindFirstObjectByType<RewardedAdPlayer>(FindObjectsInactive.Include);
            if (player != null) player.OnSdkAdClosed();
        };

        rewarded.OnAdInfoChanged += info =>
        {
            Debug.Log($"AdsDebug AdLoader.OnAdInfoChanged adUnit={info.AdUnitId}");
        };
    }

    private void RetryLoad()
    {
        Debug.Log("AdsDebug AdLoader.RetryLoad");
        loadRequested = true;
        TryStartQueuedLoad();
    }

    private void DisposeRewarded()
    {
        if (rewarded != null)
        {
            try
            {
                Debug.Log("AdsDebug AdLoader.DisposeRewarded destroying rewarded instance");
                rewarded.DestroyAd();
            }
            catch (Exception e)
            {
                Debug.LogWarning($"AdsDebug AdLoader.DisposeRewarded exception: {e}");
            }
            rewarded = null;
        }
    }

    // ---------- API used by RewardedAdPlayer ----------
    public bool IsRewardedReady()
    {
        if (rewarded == null)
        {
            Debug.Log("AdsDebug AdLoader.IsRewardedReady => false (rewarded is null)");
            return false;
        }

        bool ready = rewarded.IsAdReady();
        Debug.Log($"AdsDebug AdLoader.IsRewardedReady => {ready}");

        return ready;
    }

    public void RequestLoad()
    {
        Debug.Log("AdsDebug AdLoader.RequestLoad called");

        InitializeConsentAndRequestInit();   // make sure init is underway
        loadRequested = true;                // queue the intent to load

        if (!sdkInitialized)
        {
            Debug.Log("AdsDebug AdLoader.RequestLoad queued (waiting for LevelPlay init success)");
            return;
        }

        EnsureRewardedInstance();
        TryStartQueuedLoad();
    }

    private void TryStartQueuedLoad()
    {
        if (!sdkInitialized)
        {
            Debug.Log("AdsDebug AdLoader.TryStartQueuedLoad blocked – SDK not initialized");
            return;
        }
        if (rewarded == null)
        {
            Debug.Log("AdsDebug AdLoader.TryStartQueuedLoad blocked – rewarded is null");
            return;
        }

        Debug.Log("AdsDebug AdLoader.TryStartQueuedLoad calling rewarded.LoadAd()");
        rewarded.LoadAd();
    }

    public bool TryShowAd(string placement = null)
    {
        if (rewarded == null)
        {
            Debug.LogWarning("AdsDebug AdLoader.TryShowAd rewarded is null");
            return false;
        }
        if (!rewarded.IsAdReady())
        {
            Debug.Log("AdsDebug AdLoader.TryShowAd not ready");
            return false;
        }

        Debug.Log("AdsDebug AdLoader.TryShowAd showing now");
        rewarded.ShowAd(placement);
        return true;
    }

    // Player ended the cycle and told us if reward was granted.
    private void HandleAdClosedFromPlayer(bool rewardGranted)
    {
        Debug.Log($"AdsDebug AdLoader.HandleAdClosedFromPlayer rewardGranted={rewardGranted}");
        if (isShuttingDown) return;

        DisposeRewarded();

        if (rewardGranted)
        {
            Debug.Log("AdsDebug AdLoader destroying loader (granted cycle complete)");
            Destroy(gameObject);
            return;
        }

        Debug.Log("AdsDebug AdLoader keeping loader alive; next RequestLoadAd will reuse it");
        loadRequested = false;
    }

    private void HandleRequestLoadAd()
    {
        Debug.Log("AdsDebug AdLoader.HandleRequestLoadAd");
        RequestLoad();
    }

    private void LogState(string context)
    {
        Debug.Log($"AdsDebug State [{context}]: " +
                  $"initRequested={initRequested}, " +
                  $"sdkInitialized={sdkInitialized}, " +
                  $"loadRequested={loadRequested}, " +
                  $"rewardedExists={rewarded != null}, " +
                  $"adReady={rewarded?.IsAdReady() ?? false}");
    }*/
}
