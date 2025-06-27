using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;  
using TMPro;
using Google.Play.AppUpdate;
using System.Collections;

namespace com.google.play.appupdate
{
    public class FlexibleUpdateManager : MonoBehaviour
    {
        [Header("Progress UI")]
        [SerializeField] private Slider progressBar;
        [SerializeField] private TMP_Text statusText;

        [Header("Restart Prompt")]
        [SerializeField] private GameObject restartPopup;
        [SerializeField] private TMP_Text readyText;
        [SerializeField] private Button yesButton;
        [SerializeField] private Button noButton;

        public static bool UpdateCheckComplete = false;

        private AppUpdateManager updateManager;
        private AppUpdateRequest updateRequest;

        private void Start()
        {
            Debug.Log("[FlexibleUpdate] Start()");
            progressBar.value = 0f;
            restartPopup.SetActive(false);
            updateManager = new AppUpdateManager();

            Debug.Log("[FlexibleUpdate] Initialized AppUpdateManager, starting update check...");
            StartCoroutine(CheckForUpdates());
        }

        private IEnumerator CheckForUpdates()
        {
            Debug.Log("[FlexibleUpdate] Checking for updates...");
            statusText.text = "Checking for updates...";
            progressBar.value = 0.05f;

            var updateInfoTask = updateManager.GetAppUpdateInfo();
            Debug.Log("[FlexibleUpdate] Awaiting update info task...");

            float timeout = 10f;
            float elapsed = 0f;

            while (!updateInfoTask.IsDone && elapsed < timeout)
            {
                elapsed += Time.deltaTime;
                yield return null;
            }

            if (!updateInfoTask.IsDone)
            {
                Debug.LogError("[FlexibleUpdate] Timeout waiting for update info.");
                FlexibleUpdateManager.UpdateCheckComplete = true;
                yield break;
            }

            if (!updateInfoTask.IsSuccessful)
            {
                Debug.LogError($"[FlexibleUpdate] Failed to get update info: {updateInfoTask.Error}");
                FlexibleUpdateManager.UpdateCheckComplete = true;
                yield break;
            }

            AppUpdateInfo updateinfo;
            try
            {
                updateinfo = updateInfoTask.GetResult();
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[FlexibleUpdate] Exception getting update info: {e}");
                FlexibleUpdateManager.UpdateCheckComplete = true;
                yield break;
            }
            var updateInfo = updateInfoTask.GetResult();
            Debug.Log($"[FlexibleUpdate] Update availability: {updateInfo.UpdateAvailability}");
            Debug.Log($"[FlexibleUpdate] Update allowed: {updateInfo.IsUpdateTypeAllowed(AppUpdateOptions.FlexibleAppUpdateOptions())}");

            var flexibleOptions = AppUpdateOptions.FlexibleAppUpdateOptions();

            if (updateInfo.UpdateAvailability == UpdateAvailability.UpdateAvailable &&
                updateInfo.IsUpdateTypeAllowed(flexibleOptions))
            {
                Debug.Log("[FlexibleUpdate] Update is available and allowed. Starting flexible update...");
                yield return StartFlexibleUpdate(updateInfo);
            }
            else
            {
                Debug.Log("[FlexibleUpdate] No update available or update type not allowed.");

                FlexibleUpdateManager.UpdateCheckComplete = true;   
            }
        }

        private IEnumerator StartFlexibleUpdate(AppUpdateInfo info)
        {
            Debug.Log("[FlexibleUpdate] Entered StartFlexibleUpdate()");
            statusText.text = "Downloading update...";
            progressBar.value = 0.05f;

            var options = AppUpdateOptions.FlexibleAppUpdateOptions();

            bool startUpdateFailed = false;

            try
            {
                Debug.Log("[FlexibleUpdate] Attempting to start update...");
                updateRequest = updateManager.StartUpdate(info, options);
            }
            catch (System.Exception e)
            {
                Debug.LogError("[FlexibleUpdate] StartUpdate threw an exception: " + e);
                startUpdateFailed = true;
            }
            if (startUpdateFailed)
            {
                Debug.Log("[FlexibleUpdate] Update failed to start. Proceeding to game.");

                FlexibleUpdateManager.UpdateCheckComplete = true;
                yield break;
            }

            float timeout = 60f; // 60-second timeout
            float timer = 0f;

            Debug.Log("[FlexibleUpdate] Update download started. Monitoring progress...");
            while (!updateRequest.IsDone && timer < timeout)
            {
                float progress = Mathf.Clamp01(updateRequest.DownloadProgress);
                progressBar.value = 0.05f + progress * 0.95f;
                statusText.text = $"Downloading... {Mathf.RoundToInt(progress * 100)}%";
                timer += Time.deltaTime;
                yield return null;
            }
            if (!updateRequest.IsDone)
            {
                Debug.LogError("[FlexibleUpdate] Download timed out.");
                FlexibleUpdateManager.UpdateCheckComplete = true;
                yield break;
            }
            if (updateRequest.Error != AppUpdateErrorCode.NoError)
            {
                Debug.LogError($"[FlexibleUpdate] Download failed: {updateRequest.Error}");
                FlexibleUpdateManager.UpdateCheckComplete = true;
            }
            else
            {
                Debug.Log("[FlexibleUpdate] Update downloaded successfully. Showing restart prompt.");
                statusText.text = "Update ready.";
                progressBar.value = 1f;
                ShowRestartPrompt();
                FlexibleUpdateManager.UpdateCheckComplete = true;
            }
        }

        private void ShowRestartPrompt()
        {
            Debug.Log("[FlexibleUpdate] Showing restart popup...");
            readyText.text = "Update downloaded. Restart now?";
            restartPopup.SetActive(true);

            yesButton.onClick.RemoveAllListeners();
            noButton.onClick.RemoveAllListeners();

            yesButton.onClick.AddListener(() =>
            {
                Debug.Log("[FlexibleUpdate] User chose to restart. Completing update...");
                updateManager.CompleteUpdate();
            });

            noButton.onClick.AddListener(() =>
            {
                Debug.Log("[FlexibleUpdate] User chose to restart later.");
                restartPopup.SetActive(false);
                FlexibleUpdateManager.UpdateCheckComplete = true;   
            });
        }

    }
}
