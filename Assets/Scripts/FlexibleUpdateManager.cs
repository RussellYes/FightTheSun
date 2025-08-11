using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Google.Play.AppUpdate;
using System.Collections;
using System;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

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

        private bool _updateDownloaded = false;
        private bool _hasTriedToChangeScene = false; 

        private TMP_Text _bottomBannerText;
        private CanvasGroup _bannerCanvasGroup;
        private Coroutine _fadeRoutine;

        private void Awake()
        {
            Debug.Log("[FlexibleUpdate] Awake fired");
            if (_instance != null) { Destroy(gameObject); return; }
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            Debug.Log("[FlexibleUpdate] Start");
            if (isTesting)
            {
                Debug.Log("[FlexibleUpdate] Testing mode: not creating AppUpdateManager.");
                TestingWithoutUpdateManagerEvent?.Invoke();
                return;
            }

            updateManager = new AppUpdateManager();
            EnsureBottomBanner();
            DetectInputSystem();
            TryToChangeScene("startup");
        }

        private void TryToChangeScene(string reason)
        {
            Debug.Log($"[FlexibleUpdate] ENTERING CheckForUpdates, reason={reason}");

            if (_hasTriedToChangeScene)
            {
                Debug.LogWarning("[FlexibleUpdate] CheckForUpdates called again, but already _hasTriedToChangeScene once.");  
                return;
            }

            _hasTriedToChangeScene = true;
            StartCoroutine(LoadSceneWithDelay());

            Debug.Log("[FlexibleUpdate] WaitThenSendEventToChangeScene invoked.");

            if (!isTesting)
            {
                Debug.Log("[FlexibleUpdate] WaitThenSendEventToChangeScene - starting CheckForUpdates2().");
                StartCoroutine(CheckForUpdates());
            }
        }

        private IEnumerator LoadSceneWithDelay()
        {
            yield return new WaitForSecondsRealtime(1.0f);
            OnUpdateProcessComplete?.Invoke();

        }

        IEnumerator CheckForUpdates()
        {
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
                yield break;
            }

            var info = infoTask.GetResult();
            var flexible = AppUpdateOptions.FlexibleAppUpdateOptions();
            Debug.Log($"[FlexibleUpdate] availability={info.UpdateAvailability}");
            Debug.Log($"[FlexibleUpdate] allowed(FLEX)={info.IsUpdateTypeAllowed(flexible)}");

            if (info.UpdateAvailability == UpdateAvailability.DeveloperTriggeredUpdateInProgress)
            {
                // Start download in background
                StartCoroutine(StartOrResumeDownload(info, flexible, isResume: true));
                yield break;
            }

            if (info.UpdateAvailability == UpdateAvailability.UpdateAvailable &&
                info.IsUpdateTypeAllowed(flexible))
            {
                // Start download in background
                StartCoroutine(StartOrResumeDownload(info, flexible, isResume: false));
                yield break;
            }

            Debug.Log("[FlexibleUpdate] No update available or not allowed.");
        }


        private IEnumerator StartOrResumeDownload(AppUpdateInfo info, AppUpdateOptions flexible, bool isResume)
        {
            try
            {
                updateRequest = updateManager.StartUpdate(info, flexible);
                Debug.Log(isResume
                    ? "[FlexibleUpdate] Resuming in-progress update."
                    : "[FlexibleUpdate] Flexible update started.");
            }
            catch (Exception e)
            {
                Debug.LogWarning("[FlexibleUpdate] StartUpdate exception: " + e);
                yield break;
            }

 
            while (!updateRequest.IsDone)
                yield return null;

            if (updateRequest.Error != AppUpdateErrorCode.NoError)
            {
                Debug.LogWarning($"[FlexibleUpdate] Download failed: {updateRequest.Error}");
                yield break;
            }
            OnDownloadedReady(); 
        }

        private void OnDownloadedReady()
        {
            _updateDownloaded = true;
            Debug.Log("[FlexibleUpdate] Update downloaded. Will apply on app exit.");
            ShowBottomMessage("Update downloaded. Restart to complete update.");
        }

 
        private void OnApplicationQuit()
        {
            if (_updateDownloaded)
            {
                Debug.Log("[FlexibleUpdate] OnApplicationQuit → CompleteUpdate()");
                updateManager.CompleteUpdate();
            }
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

            // Overlay canvas on top of everything
            var canvasGO = new GameObject("InAppUpdateCanvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            var canvas = canvasGO.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = short.MaxValue; // top-most
            var scaler = canvasGO.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            DontDestroyOnLoad(canvasGO);


            var panelGO = new GameObject("InAppUpdatePanel", typeof(RectTransform), typeof(Image), typeof(CanvasGroup));
            panelGO.transform.SetParent(canvasGO.transform, false);

            var rtPanel = panelGO.GetComponent<RectTransform>();
            rtPanel.anchorMin = new Vector2(0f, 0f);
            rtPanel.anchorMax = new Vector2(1f, 0f);
            rtPanel.pivot = new Vector2(0.5f, 0f);
            rtPanel.anchoredPosition = new Vector2(0f, 10f); 
            rtPanel.sizeDelta = new Vector2(0f, 20f);       
            panelGO.GetComponent<Image>().color = Color.white;

            _bannerCanvasGroup = panelGO.GetComponent<CanvasGroup>();
            _bannerCanvasGroup.alpha = 0f; 

        
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
            _bottomBannerText.text = "";
        }

        private void ShowBottomMessage(string msg)
        {
            if (_bottomBannerText == null) EnsureBottomBanner();
            _bottomBannerText.text = msg;
            if (_fadeRoutine != null)
            {
                StopCoroutine(_fadeRoutine);   // stop only the previous fade
                _fadeRoutine = null;
            }
            _fadeRoutine = StartCoroutine(FadeInBanner());
        }

        private IEnumerator FadeInBanner()
        {
            if (_bannerCanvasGroup == null)
            {
                // Fallback: try to find on parent
                _bannerCanvasGroup = _bottomBannerText.GetComponentInParent<CanvasGroup>();
                if (_bannerCanvasGroup == null) yield break;
            }

            float dur = 0.5f, t = 0f;
            while (t < dur)
            {
                t += Time.unscaledDeltaTime;
                _bannerCanvasGroup.alpha = Mathf.Clamp01(t / dur);
                yield return null;
            }
            _bannerCanvasGroup.alpha = 1f;
        }
    }
}