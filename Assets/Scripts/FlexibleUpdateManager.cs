using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Google.Play.AppUpdate;
using System.Collections;
using System;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

// Runs a flexible in-app update with zero Inspector hookups.
// Behavior:
// - Checks for update on startup (and on resume, debounced).
// - If found, downloads silently in background.
// - When download finishes, shows a one-line banner at the bottom:
//     "Update downloaded. Restart to complete update."
// - When the app exits, it calls CompleteUpdate() to apply it.
//
// Class name kept for compatibility with existing code.
namespace com.Google.Play.AppUpdate
{
    public class FlexibleUpdateManager : MonoBehaviour
    {
        public static event Action OnUpdateProcessComplete;
        public static event Action TestingWithoutUpdateManagerEvent;

        [SerializeField] private bool isTesting = false;

        private AppUpdateManager updateManager;
        private AppUpdateRequest updateRequest;

        private static FlexibleUpdateManager _instance;
        private bool _checking;
        private float _lastCheckAt = -999f;
        private const float MinSecondsBetweenChecks = 90f;
        private bool _updateDownloaded = false;

        private TMP_Text _bottomBannerText;

        private void Awake()
        {
            if (_instance != null) { Destroy(gameObject); return; }
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            if (isTesting)
            {
                Debug.Log("[FlexibleUpdate] Testing mode: not creating AppUpdateManager.");
                TestingWithoutUpdateManagerEvent?.Invoke();
                return;
            }

            updateManager = new AppUpdateManager();
            EnsureBottomBanner();
            DetectInputSystem();

            StartSafeCheck("startup");
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            if (hasFocus) StartSafeCheck("focus");
        }

        private void OnApplicationPause(bool paused)
        {
            if (!paused) StartSafeCheck("resume");
        }

        private void StartSafeCheck(string reason)
        {
            if (_checking) return;
            if (Time.realtimeSinceStartup - _lastCheckAt < MinSecondsBetweenChecks)
            {
                Debug.Log($"[FlexibleUpdate] Debounced recheck ({reason})");
                return;
            }
            _lastCheckAt = Time.realtimeSinceStartup;
            StartCoroutine(CheckForUpdates(reason));
        }

        private IEnumerator CheckForUpdates(string reason)
        {
            _checking = true;
            Debug.Log($"[FlexibleUpdate] Check start reason={reason}");

            yield return new WaitForSecondsRealtime(1.0f);

            var infoTask = updateManager.GetAppUpdateInfo();

            const float INFO_TIMEOUT = 12f;
            float t = 0f;
            while (!infoTask.IsDone && t < INFO_TIMEOUT)
            {
                t += Time.unscaledDeltaTime;
                yield return null;
            }

            if (!infoTask.IsDone || !infoTask.IsSuccessful)
            {
                Debug.LogWarning($"[FlexibleUpdate] GetAppUpdateInfo failed/timeout. done={infoTask.IsDone} ok={infoTask.IsSuccessful}");
                _checking = false;
                NotifyCompleteSafely();
                yield break;
            }

            var info = infoTask.GetResult();
            var flexible = AppUpdateOptions.FlexibleAppUpdateOptions();
            Debug.Log($"[FlexibleUpdate] availability={info.UpdateAvailability} allowed={info.IsUpdateTypeAllowed(flexible)}");

            if (info.UpdateAvailability == UpdateAvailability.DeveloperTriggeredUpdateInProgress)
            {
                // Resume the ongoing flexible update
                try
                {
                    updateRequest = updateManager.StartUpdate(info, flexible);
                    Debug.Log("[FlexibleUpdate] Resuming in-progress update.");
                }
                catch (Exception e)
                {
                    Debug.LogWarning("[FlexibleUpdate] Resume StartUpdate exception: " + e);
                    _checking = false;
                    NotifyCompleteSafely();
                    yield break;
                }

                while (!updateRequest.IsDone)
                    yield return null;

                if (updateRequest.Error != AppUpdateErrorCode.NoError)
                {
                    Debug.LogWarning($"[FlexibleUpdate] Resume failed: {updateRequest.Error}");
                    _checking = false;
                    NotifyCompleteSafely();
                    yield break;
                }

                while (!updateRequest.IsDone)
                    yield return null;

                if (updateRequest.Error != AppUpdateErrorCode.NoError)
                {
                    Debug.LogWarning($"[FlexibleUpdate] Resume failed: {updateRequest.Error}");
                    _checking = false;
                    NotifyCompleteSafely();
                    yield break;
                }

                OnDownloadedReady();
            }
            else
            {
                Debug.Log("[FlexibleUpdate] No update available or not allowed.");
                NotifyCompleteSafely();
            }

            _checking = false;
        }

        private void OnDownloadedReady()
        {
            _updateDownloaded = true;
            Debug.Log("[FlexibleUpdate] Update downloaded. Will apply on app exit.");
            ShowBottomMessage("Update downloaded. Restart to complete update.");
        }

        public void InstallNow()
        {
            if (_updateDownloaded)
            {
                Debug.Log("[FlexibleUpdate] InstallNow ? CompleteUpdate()");
                updateManager.CompleteUpdate(); // App restarts and applies the update.
            }
        }

        private void OnApplicationQuit()
        {
            if (_updateDownloaded)
            {
                Debug.Log("[FlexibleUpdate] OnApplicationQuit ? CompleteUpdate()");
                updateManager.CompleteUpdate();
            }
        }

        private void NotifyCompleteSafely()
        {

            StartCoroutine(InvokeUpdateCompleteEventAfterDelay(0.5f));
        }

        private IEnumerator InvokeUpdateCompleteEventAfterDelay(float delay)
        {
            yield return new WaitForSecondsRealtime(delay);
            OnUpdateProcessComplete?.Invoke();
        }

        private void DetectInputSystem()
        {
#if ENABLE_INPUT_SYSTEM
            Debug.Log("[FlexibleUpdate] New Input System detected.");
#else
            Debug.Log("[FlexibleUpdate] Using Legacy Input System.");
#endif
        }

        private void EnsureBottomBanner()
        {
            if (_bottomBannerText != null) return;

            var canvasGO = new GameObject("InAppUpdateCanvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            var canvas = canvasGO.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = short.MaxValue;
            var scaler = canvasGO.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            DontDestroyOnLoad(canvasGO);

            var panelGO = new GameObject("InAppUpdatePanel", typeof(RectTransform), typeof(Image));
            panelGO.transform.SetParent(canvasGO.transform, false);

            var rtPanel = panelGO.GetComponent<RectTransform>();
            rtPanel.anchorMin = new Vector2(0f, 0f);
            rtPanel.anchorMax = new Vector2(1f, 0f);
            rtPanel.pivot = new Vector2(0.5f, 0f);
            rtPanel.anchoredPosition = new Vector2(0f, 10f);
            rtPanel.sizeDelta = new Vector2(0f, 20f);
            panelGO.GetComponent<Image>().color = Color.white;

            var textGO = new GameObject("InAppUpdateText", typeof(RectTransform), typeof(TextMeshProUGUI));
            textGO.transform.SetParent(panelGO.transform, false);

            var rtText = textGO.GetComponent<RectTransform>();
            rtText.anchorMin = new Vector2(0f, 0f);
            rtText.anchorMax = new Vector2(1f, 1f);
            rtText.pivot = new Vector2(0.5f, 0.5f);
            rtText.anchoredPosition = Vector2.zero;
            rtText.sizeDelta = Vector2.zero;

            _bottomBannerText = textGO.GetComponent<TextMeshProUGUI>();
            _bottomBannerText.alignment = TextAlignmentOptions.Center;
            _bottomBannerText.textWrappingMode = TextWrappingModes.NoWrap;
            _bottomBannerText.fontSize = 18;
            var cookies = Resources.Load<TMP_FontAsset>("Cookies SDF");
            if (cookies != null) _bottomBannerText.font = cookies;
            _bottomBannerText.color = Color.black;
            StartCoroutine(FadeInBanner());
        }
        private void ShowBottomMessage(string msg)
        {
            if (_bottomBannerText == null) EnsureBottomBanner();
            _bottomBannerText.text = msg;
        }
        private IEnumerator FadeInBanner()
        {
            CanvasGroup cg = _bottomBannerText.GetComponentInParent<CanvasGroup>();
            if (cg == null)
            {
                cg = _bottomBannerText.transform.parent.gameObject.AddComponent<CanvasGroup>();
                cg.alpha = 0f;
            }
            for (float t = 0; t < 1f; t += Time.unscaledDeltaTime)
            {
                cg.alpha = Mathf.Clamp01(t / 0.5f); // fade over 0.5 seconds
                yield return null;
            }
            cg.alpha = 1f;
        }

    }
}
