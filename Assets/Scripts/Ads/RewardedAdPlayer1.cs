using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class RewardedAdPlayer1 : MonoBehaviour
{/*
    // === Events the loader listens to ===
    public static event Action RequestLoadAd;
    // Let loader know the cycle ended, and whether a reward was actually granted.
    public static event Action<bool> AdClosed;
    // Expose for your Rewards script
    public static event Action RewardGranted;

    private bool rewardReportedThisShow;
    private bool adWasShowing;
    private bool isShowingGuard; // one-at-a-time guard

    private void Awake()
    {
        Debug.Log("AdsDebug RewardedAdPlayer.Awake");
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        Debug.Log("AdsDebug RewardedAdPlayer.OnEnable subscribe");
        AdLoader.RewardedAdLoaded += HandleAdLoaded;
        EndConditionsUI.AdRequestLevelLostReward += OnShowAdButtonPressed;
    }

    private void OnDisable()
    {
        Debug.Log("AdsDebug RewardedAdPlayer.OnDisable unsubscribe");
        AdLoader.RewardedAdLoaded -= HandleAdLoaded;
        EndConditionsUI.AdRequestLevelLostReward -= OnShowAdButtonPressed;
    }

    // === UI Hook ===
    public void OnShowAdButtonPressed()
    {
        Debug.Log("AdsDebug RewardedAdPlayer.OnShowAdButtonPressed");

        // ADD DIAGNOSTIC:
        var loader = AdLoader.Instance;
        if (loader == null)
        {
            Debug.LogError("AdsDebug: AdLoader.Instance is NULL despite DontDestroyOnLoad!");

            // Check if any AdLoader exists in the scene
            var allLoaders = FindObjectsByType<AdLoader>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            Debug.Log($"AdsDebug: Found {allLoaders.Length} AdLoaders in scene");

            foreach (var l in allLoaders)
            {
                Debug.Log(message: $"AdsDebug: AdLoader: {l.name}, Instance==this: {l.GetInstanceID()}");
            }

            // Create a new loader since none exists
            Debug.Log("AdsDebug RewardedAdPlayer: no loader present; spawning one now");
            var go = new GameObject("AdLoader_Auto");
            loader = go.AddComponent<AdLoader>();
        }
        else
        {
            Debug.Log($"AdsDebug: AdLoader exists. Ready: {loader.IsRewardedReady()}");
        }

        if (isShowingGuard)
        {
            Debug.Log("AdsDebug RewardedAdPlayer.OnShowAdButtonPressed ignored (already showing)");
            return;
        }

        if (loader.IsRewardedReady())
        {
            Debug.Log("AdsDebug RewardedAdPlayer: ad is ready → show");
            BeginShow(loader);
            return;
        }

        Debug.Log("AdsDebug RewardedAdPlayer: ad NOT ready → toast + RequestLoadAd");
        StartCoroutine(ShowMagentaToast("Ad not available.", 3f));
        RequestLoadAd?.Invoke();
    }

    private void BeginShow(AdLoader loader)
    {
        rewardReportedThisShow = false;
        adWasShowing = true;
        isShowingGuard = true;

        bool started = loader.TryShowAd();
        Debug.Log($"AdsDebug RewardedAdPlayer.BeginShow TryShowAd started={started}");
        if (!started)
        {
            isShowingGuard = false;
            adWasShowing = false;
            Debug.LogWarning("AdsDebug RewardedAdPlayer.BeginShow readiness lost; requesting load and showing toast");
            StartCoroutine(ShowMagentaToast("Ad not available.", 3f));
            RequestLoadAd?.Invoke();
        }
    }

    private void HandleAdLoaded()
    {
        Debug.Log("AdsDebug RewardedAdPlayer.HandleAdLoaded (ready)");
        // We only show when the user taps.
    }

    // ===== Called by AdLoader’s SDK wiring =====
    public void OnSdkAdRewarded()
    {
        Debug.Log("AdsDebug RewardedAdPlayer.OnSdkAdRewarded");
        rewardReportedThisShow = true;
    }

    public void OnSdkAdClosed()
    {
        Debug.Log($"AdsDebug RewardedAdPlayer.OnSdkAdClosed rewardReported={rewardReportedThisShow}");
        adWasShowing = false;
        isShowingGuard = false;

        if (rewardReportedThisShow)
        {
            Debug.Log("AdsDebug RewardedAdPlayer -> RewardGranted");
            try { RewardGranted?.Invoke(); } catch (Exception e) { Debug.LogError($"AdsDebug RewardedAdPlayer RewardGranted exception {e}"); }
        }
        else
        {
            Debug.Log("AdsDebug RewardedAdPlayer -> no reward granted");
        }

        // Tell loader the cycle ended and whether reward was granted.
        try { AdClosed?.Invoke(rewardReportedThisShow); } catch (Exception e) { Debug.LogError($"AdsDebug RewardedAdPlayer AdClosed exception {e}"); }

        // If reward was granted, spec says: loader is destroyed; we control respawn.
        if (rewardReportedThisShow)
        {
            Debug.Log("AdsDebug RewardedAdPlayer requesting fresh loader after granted cycle");
            RequestLoadAd?.Invoke(); // this will spawn/reuse loader and start a fresh load
        }
    }

    // ===== App focus logging (SDK handles actual pausing) =====
    private void OnApplicationPause(bool pause)
    {
        Debug.Log($"AdsDebug RewardedAdPlayer.OnApplicationPause pause={pause} wasShowing={adWasShowing}");
    }

    private void OnApplicationQuit()
    {
        Debug.Log("AdsDebug RewardedAdPlayer.OnApplicationQuit");
    }

    private void OnDestroy()
    {
        Debug.LogWarning("AdsDebug AdLoader.OnDestroy - WHY AM I BEING DESTROYED?");
    }

    private void OnLevelWasLoaded(int level)
    {
        Debug.Log($"AdsDebug AdLoader.OnLevelWasLoaded: level {level}, Instance: {this.GetInstanceID()}");
    }

    // ===== Minimal magenta toast per spec =====
    private IEnumerator ShowMagentaToast(string message, float seconds)
    {
        Debug.Log($"AdsDebug RewardedAdPlayer.ShowMagentaToast '{message}' {seconds}s");
        Canvas canvas = GetComponent<Canvas>();
        if (canvas == null)
        {
            var toastCanvasGO = new GameObject("ToastCanvas");
            canvas = toastCanvasGO.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            // Configured for 900x1600 mobile resolution with width matching
            var canvasScaler = toastCanvasGO.AddComponent<CanvasScaler>();
            canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasScaler.referenceResolution = new Vector2(900, 1600);
            canvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            canvasScaler.matchWidthOrHeight = 0f; // (0=width, 1=height)

            toastCanvasGO.AddComponent<GraphicRaycaster>();
        }

        GameObject panel = new GameObject("AdToast");
        panel.transform.SetParent(transform, false);

        var rt = panel.AddComponent<RectTransform>();
        rt.sizeDelta = new Vector2(700, 140);
        rt.anchoredPosition = Vector2.zero;

        var img = panel.AddComponent<Image>();
        img.color = new Color(0, 0, 0, 1);

        // Add rounded corners effect (optional)
        var outline = panel.AddComponent<Outline>();
        outline.effectColor = new Color(0.5f, 0f, 0.5f, 0.3f);
        outline.effectDistance = new Vector2(2, 2);

        var textGO = new GameObject("Text");
        textGO.transform.SetParent(panel.transform, false);
        var textRT = textGO.AddComponent<RectTransform>();
        textRT.anchorMin = Vector2.zero; // Stretch to fill panel
        textRT.anchorMax = Vector2.one;
        textRT.sizeDelta = Vector2.zero;
        textRT.offsetMin = new Vector2(20, 10); // Padding inside panel
        textRT.offsetMax = new Vector2(-20, -10);

        var tx = textGO.AddComponent<Text>();
        tx.text = message;
        tx.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        tx.fontStyle = FontStyle.Bold;
        tx.fontSize = 40;
        tx.color = Color.magenta;
        tx.alignment = TextAnchor.MiddleCenter;
        tx.horizontalOverflow = HorizontalWrapMode.Wrap;
        tx.verticalOverflow = VerticalWrapMode.Truncate;

        // Enable text resizing for different screen sizes
        tx.resizeTextForBestFit = true;
        tx.resizeTextMinSize = 24;
        tx.resizeTextMaxSize = 40;

        var canvasRoot = canvas.GetComponent<Canvas>();
        int originalSort = canvasRoot.sortingOrder;
        canvasRoot.sortingOrder = 999; // int.MaxValue; // High but not max to avoid issues

        yield return new WaitForSecondsRealtime(seconds);

        var fadeGroup = panel.AddComponent<CanvasGroup>();        // renamed from 'cg'
        fadeGroup.alpha = 1f;
        float t = 0f;
        const float fade = 0.5f;
        while (t < fade)
        {
            t += Time.unscaledDeltaTime;
            fadeGroup.alpha = 1f - Mathf.Clamp01(t / fade);
            yield return null;
        }

        Destroy(panel);
        canvasRoot.sortingOrder = originalSort;
    }*/
}
