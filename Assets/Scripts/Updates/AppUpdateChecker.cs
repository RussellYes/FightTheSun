using Firebase.Extensions;
using Firebase.RemoteConfig;
using Firebase.Analytics;
using UnityEngine;

public class AppUpdateChecker : MonoBehaviour
{
    private const string MinimumVersionCodeKey = "minimum_required_version_code";

    void Start()
    {
        // First, get the current app's version code
        long currentAppVersionCode = GetAndroidVersionCode();
        Debug.Log($"FightTheSun current app version code: {currentAppVersionCode}");

        // Now, fetch the Remote Config value
        FirebaseRemoteConfig.DefaultInstance.FetchAndActivateAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                Debug.Log("Remote Config fetched and activated!");
                long requiredVersionCode = FirebaseRemoteConfig.DefaultInstance.GetValue(MinimumVersionCodeKey).LongValue;
                Debug.Log($"Remote Config required version code: {requiredVersionCode}");

                if (currentAppVersionCode < requiredVersionCode)
                {
                    ShowUpdatePrompt();
                }
            }
            else if (task.IsFaulted)
            {
                Debug.LogError($"Remote Config fetch failed: {task.Exception}");
                // Handle error: perhaps log it or show a message to the user
            }
        });
    }

    private long GetAndroidVersionCode()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            try
            {
                // Get the UnityPlayer class
                AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                // Get the current Activity
                AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
                // Get the PackageManager
                AndroidJavaObject packageManager = currentActivity.Call<AndroidJavaObject>("getPackageManager");
                // Get the PackageInfo for our own application
                string packageName = currentActivity.Call<string>("getPackageName");
                AndroidJavaObject packageInfo = packageManager.Call<AndroidJavaObject>("getPackageInfo", packageName, 0);

                // Extract the versionCode (which is an int in Java, but we'll cast to long for safety)
                return packageInfo.Get<long>("versionCode");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Failed to get Android versionCode: {e.Message}");
            }
        }
        // Return a default or error value if not on Android or an error occurred
        return -1;
    }

    private void ShowUpdatePrompt()
    {
        Debug.Log("Update required! Showing prompt.");
        // --- YOUR UI CODE TO SHOW THE UPDATE PROMPT GOES HERE ---
        // For example:
        // YourUpdateUIManager.Instance.ShowUpdatePopup("A new version of FightTheSun is available!");
        // And include a button that calls GoToPlayStore()
    }

    public void GoToPlayStore()
    {
        Application.OpenURL($"market://details?id={Application.identifier}");
    }

    public void TrackAdEvent(string eventName, string parameterName = null, object parameterValue = null)
    {
        try
        {
            if (parameterName != null && parameterValue != null)
            {
                Parameter[] parameters = { new Parameter(parameterName, parameterValue.ToString()) };
                FirebaseAnalytics.LogEvent(eventName, parameters);
            }
            else
            {
                FirebaseAnalytics.LogEvent(eventName);
            }
            Debug.Log($"Firebase Event: {eventName} ({parameterName}: {parameterValue})");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Firebase analytics error: {e.Message}");
        }
    }
}


