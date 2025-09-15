using UnityEngine;
using Unity.Services.LevelPlay;

public class AdsBootstrap : MonoBehaviour
{
    public static AdsBootstrap I { get; private set; }

    [Header("LevelPlay Keys (swap these only)")]
    [SerializeField] private string appKey = "23928db7d";
    [SerializeField] private string rewardedAdUnitId = "ellw22gymypqk08a";

    private LevelPlayRewardedAd rewarded;
    private bool initialized; // <— prevent double init

    private void Awake()
    {
        if (I != null && I != this) { Destroy(gameObject); return; }
        I = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        // If save isn’t ready yet, wait for it
        if (DataPersister.Instance == null || DataPersister.Instance.CurrentGameData == null)
        {
            DataPersister.InitializationComplete += OnDataReady;
            return;
        }

        BeginConsentFlow();
    }

    private void OnDestroy()
    {
        DataPersister.InitializationComplete -= OnDataReady;
        OpeningStoryUI.OnStoryFinished -= HandleStoryFinished;
        ConsentUI.OnConsentChosen -= OnConsentChosen;
    }

    private void OnDataReady()
    {
        DataPersister.InitializationComplete -= OnDataReady;
        BeginConsentFlow();
    }

    private void BeginConsentFlow()
    {
        if (initialized) return;

        var saved = ConsentStorage.Get(); // from GameData only
        if (saved == null)
        {
            // Wait until your opening story finishes, then prompt
            OpeningStoryUI.OnStoryFinished += HandleStoryFinished;
            return;
        }

        ApplyConsent(saved.Value);
        InitAndLoad();
        initialized = true;
    }

    private void HandleStoryFinished()
    {
        OpeningStoryUI.OnStoryFinished -= HandleStoryFinished;
        StartCoroutine(ShowConsentAfterDelay(4f));
    }

    private System.Collections.IEnumerator ShowConsentAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        var ui = FindFirstObjectByType<ConsentUI>(FindObjectsInactive.Include);
        if (ui != null)
        {
            ConsentUI.OnConsentChosen += OnConsentChosen;
            ui.Show("Do you consent to personalized ads?");
        }
        else
        {
            ConsentStorage.Set(false);
            ApplyConsent(false);
            InitAndLoad();
            initialized = true;
        }
    }

    private void OnConsentChosen(bool consent)
    {
        ConsentUI.OnConsentChosen -= OnConsentChosen;

        ConsentStorage.Set(consent);
        ApplyConsent(consent);
        InitAndLoad();
        initialized = true;
    }

    private void ApplyConsent(bool consent)
    {
        LevelPlay.SetMetaData("gdpr_consent", consent ? "true" : "false");
        Debug.Log($"GDPR consent set: {consent}");
    }

    private void InitAndLoad()
    {
        try
        {
            LevelPlay.Init(appKey);

            rewarded = new LevelPlayRewardedAd(rewardedAdUnitId);

            rewarded.OnAdLoaded        += (info)             => Debug.Log($"RV loaded: {info.AdUnitId}");
            rewarded.OnAdLoadFailed    += (error)            => Debug.LogError($"RV load fail {error}");
            rewarded.OnAdDisplayed     += (info)             => Debug.Log($"RV displayed: {info.AdUnitId}");
            rewarded.OnAdDisplayFailed += (error)            => Debug.LogError($"RV show fail {error}");
            rewarded.OnAdClosed        += (info)             => { Debug.Log($"RV closed: {info.AdUnitId}"); rewarded.LoadAd(); };
            rewarded.OnAdRewarded      += (info, reward)     => { Debug.Log($"RV reward: {reward.Amount}"); OnRewardGranted(); };

            rewarded.LoadAd();

            if (rewarded.IsAdReady()) rewarded.ShowAd(); else rewarded.LoadAd();
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"LevelPlay.Init exception: {ex}");
        }
    }

    public void ShowRewardedIfReady()
    {
        if (rewarded != null && rewarded.IsAdReady()) rewarded.ShowAd();
        else { Debug.Log("Rewarded not ready; reloading"); rewarded?.LoadAd(); }
    }

    private void OnRewardGranted() { /* hook bonus path */ }
}
