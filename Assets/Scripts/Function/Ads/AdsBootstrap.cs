using UnityEngine;
using Unity.Services.LevelPlay;

// This script shows GDPR consent dialog if needed, saves choice, inits LevelPlay SDK with consent, and preloads a rewarded ad. 

public class AdsBootstrap : MonoBehaviour

{public static AdsBootstrap I { get; private set; }
    void Awake()
    {
        if (I != null && I != this) { Destroy(gameObject); return; }
        I = this;
        DontDestroyOnLoad(gameObject);
    } 

    [Header("LevelPlay Keys (swap these only)")]
    [SerializeField] private string appKey = "23928db7d";            // DEV or PROD
    [SerializeField] private string rewardedAdUnitId = "ellw22gymypqk08a";  // DEV or PROD

    private LevelPlayRewardedAd rewarded;

private void Start()
{
    var saved = ConsentStorage.Get();
    if (saved == null)
    {
        OpeningStoryUI.OnStoryFinished += HandleStoryFinished;
        return;
    }

    ApplyConsent(saved.Value);
    InitAndLoad();
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
        ApplyConsent(false);
        InitAndLoad();
    }
}

    private void OnConsentChosen(bool consent)
    {
        ConsentUI.OnConsentChosen -= OnConsentChosen;
        ConsentStorage.Set(consent);
        ApplyConsent(consent);
        InitAndLoad();
    }

    private void ApplyConsent(bool consent)
    {
        // Must be set BEFORE init
        LevelPlay.SetMetaData("gdpr_consent", consent ? "true" : "false");
        Debug.Log($"GDPR consent set: {consent}");
    }

private void InitAndLoad()
{
    try
    {
LevelPlay.Init(appKey);

var rewarded = new LevelPlayRewardedAd(rewardedAdUnitId);

rewarded.OnAdLoaded        += (info)   => Debug.Log($"RV loaded: {info.AdUnitId}");
rewarded.OnAdLoadFailed    += (error)  => Debug.LogError($"RV load fail {error}");
rewarded.OnAdDisplayed     += (info)   => Debug.Log($"RV displayed: {info.AdUnitId}");
rewarded.OnAdDisplayFailed += (error)  => Debug.LogError($"RV show fail {error}");
rewarded.OnAdClosed        += (info)   => { Debug.Log($"RV closed: {info.AdUnitId}"); rewarded.LoadAd(); };
rewarded.OnAdRewarded      += (info, reward) => { Debug.Log($"RV reward: {reward.Amount}"); OnRewardGranted(); };

rewarded.LoadAd();

// Show helper:
if (rewarded.IsAdReady()) rewarded.ShowAd(); else rewarded.LoadAd();

    }
    catch (System.Exception ex)
    {
        Debug.LogError($"LevelPlay.Init exception: {ex}");
    }
}

// Hook this to your button: AdsBootstrap.ShowRewardedIfReady()
public void ShowRewardedIfReady()
{
    if (rewarded != null && rewarded.IsAdReady())
        rewarded.ShowAd();
    else
    {
        Debug.Log("Rewarded not ready; reloading");
        rewarded?.LoadAd();
    }
}

    // TODO: call into your existing double-reward path here
    private void OnRewardGranted()
    {
        // e.g., LevelCompleteBroadcaster.RaiseBonusDouble();
        // or notify your EndConditionsUI/ScoreManager.
    }
}