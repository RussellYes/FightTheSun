using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Events;   
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
        [Header("Progress UI")]
        [SerializeField] private Slider progressBar;
        [SerializeField] private TMP_Text statusText;

        [Header("Restart Prompt")]
        [SerializeField] private GameObject restartPopup;
        [SerializeField] private TMP_Text readyText;
        [SerializeField] private Button yesButton;
        [SerializeField] private Button noButton;


        private AppUpdateManager updateManager;
        private AppUpdateRequest updateRequest;

        private void Start()
        {
            Debug.Log("[FlexibleUpdate] Start()");
            progressBar.value = 0f;
            restartPopup.SetActive(false);
            updateManager = new AppUpdateManager();

            DetectInputSystem();
            Debug.Log("[FlexibleUpdate] Initialized AppUpdateManager, starting update check...");
            StartCoroutine(CheckForUpdates());
        }

         private void DetectInputSystem()
        {
#if ENABLE_INPUT_SYSTEM
            Debug.Log("[FlexibleUpdate] New Input System detected.");
#else
            Debug.Log("[FlexibleUpdate] Using Legacy Input System.");
#endif
        }

        private IEnumerator CheckForUpdates()
        {
            Debug.Log("[FlexibleUpdate] Checking for updates...");
            statusText.text = "Checking for updates...";
            progressBar.value = 0.05f;

            var updateInfoTask = updateManager.GetAppUpdateInfo();
            Debug.Log("[FlexibleUpdate] Awaiting update info task...");
            yield return new WaitUntil(() => updateInfoTask.IsDone);

            if (!updateInfoTask.IsSuccessful)
            {
                Debug.LogError($"[FlexibleUpdate] Update check failed: {updateInfoTask.Error}");
                NotifyUpdateComplete("Starting without update.");
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
                NotifyUpdateComplete("No update available.");
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
                NotifyUpdateComplete("Update failed. Starting app.");
                yield break;
            }

            Debug.Log("[FlexibleUpdate] Update download started. Monitoring progress...");
            while (!updateRequest.IsDone)
            {
                float progress = Mathf.Clamp01(updateRequest.DownloadProgress);
                progressBar.value = 0.05f + progress * 0.95f;
                statusText.text = $"Downloading... {Mathf.RoundToInt(progress * 100)}%";
                yield return null;
            }

            if (updateRequest.Error != AppUpdateErrorCode.NoError)
            {
                Debug.LogError($"[FlexibleUpdate] Download failed: {updateRequest.Error}");
                NotifyUpdateComplete("Update failed. Starting app.");
            }
            else
            {
                Debug.Log("[FlexibleUpdate] Update downloaded successfully. Showing restart prompt.");
                statusText.text = "Update ready.";
                progressBar.value = 1f;
                ShowRestartPrompt();
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
                NotifyUpdateComplete("Update downloaded, but not applied.");
            });
        }

         private void NotifyUpdateComplete(string message)
        {
            Debug.Log($"[FlexibleUpdate] NotifyUpdateComplete() called: {message}");
            statusText.text = message;
            progressBar.value = 1f;
            StartCoroutine(InvokeUpdateCompleteEventAfterDelay(1f));
        }

        private IEnumerator InvokeUpdateCompleteEventAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            Debug.Log("[FlexibleUpdate] Invoking OnUpdateProcessComplete event.");
            OnUpdateProcessComplete?.Invoke();
        }
    }
}