using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance;
    public System.Action<DateTime, string, string> OnTimeFetched; // DateTime, day, source
    public System.Action<string> OnTimeFetchFailed;

    private DateTime currentTime;
    private string dayOfWeek;
    private string timeSource;
    private bool useNetworkTime = true;
    private bool isTimeAvailable = false;

    public enum TimeSource { Device, Network, Fallback }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // Start with device time immediately
        UseDeviceTime();

        // Then try to get network time for accuracy
        if (useNetworkTime && Application.internetReachability != NetworkReachability.NotReachable)
        {
            StartCoroutine(FetchNetworkTime());
        }
    }

    private void UseDeviceTime(TimeSource source = TimeSource.Device)
    {
        currentTime = DateTime.UtcNow;
        dayOfWeek = currentTime.DayOfWeek.ToString();
        timeSource = source.ToString();
        isTimeAvailable = true; // Add this line

        Debug.Log($"Using {source} Time: {currentTime.ToString("yyyy-MM-dd HH:mm:ss")}, Day: {dayOfWeek}");
        OnTimeFetched?.Invoke(currentTime, dayOfWeek, timeSource);
    }

    private IEnumerator FetchNetworkTime()
    {
        string[] apiUrls = {
            "https://worldtimeapi.org/api/timezone/Etc/UTC",
            "https://worldtimeapi.org/api/ip",
            "https://timeapi.io/api/Time/current/zone?timeZone=UTC"
        };

        foreach (string apiUrl in apiUrls)
        {
            using (UnityWebRequest webRequest = UnityWebRequest.Get(apiUrl))
            {
                webRequest.timeout = 5;
                yield return webRequest.SendWebRequest();

                if (webRequest.result == UnityWebRequest.Result.Success)
                {
                    if (ParseNetworkTime(webRequest.downloadHandler.text))
                    {
                        timeSource = TimeSource.Network.ToString();
                        Debug.Log($"Network time fetched successfully from: {apiUrl}");
                        yield break; // Success, exit coroutine
                    }
                }
            }
            yield return new WaitForSeconds(0.5f);
        }

        // All network attempts failed, stick with device time
        Debug.LogWarning("Network time unavailable, using device time");
        OnTimeFetchFailed?.Invoke("Using device time - network time unavailable");
    }

    private bool ParseNetworkTime(string jsonData)
    {
        try
        {
            // Simple parsing - you might need to adjust based on the API response structure
            if (jsonData.Contains("datetime") || jsonData.Contains("currentDateTime"))
            {
                // Extract datetime from JSON (simplified)
                int startIndex = jsonData.IndexOf("datetime") + 10;
                int endIndex = jsonData.IndexOf("\"", startIndex);

                if (startIndex < 10 || endIndex < 0) return false;

                string dateTimeString = jsonData.Substring(startIndex, endIndex - startIndex);

                if (DateTime.TryParse(dateTimeString, out DateTime networkTime))
                {
                    currentTime = networkTime;
                    dayOfWeek = currentTime.DayOfWeek.ToString();
                    isTimeAvailable = true;

                    OnTimeFetched?.Invoke(currentTime, dayOfWeek, TimeSource.Network.ToString());
                    return true;
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error parsing network time: {e.Message}");
        }

        return false;
    }

    // Public methods
    public DateTime GetCurrentTime()
    {
        return currentTime;
    }

    public string GetMonth()
    {
        if (!isTimeAvailable)
        {
            Debug.LogWarning("TimeManager GetMonth - Time not available, using device time");
            return DateTime.UtcNow.ToString("MMM");
        }

        return currentTime.ToString("MMM"); // Returns "Jan", "Feb", etc.
    }

    public string GetDayOfWeek()
    {
        return dayOfWeek;
    }

    public string GetTimeSource()
    {
        return timeSource;
    }

    public bool IsNetworkTimeAvailable()
    {
        return timeSource == TimeSource.Network.ToString();
    }

    // Get time with timezone offset
    public DateTime GetLocalTime()
    {
        return currentTime.ToLocalTime();
    }

    // Refresh time
    public void RefreshTime()
    {
        UseDeviceTime();
        if (useNetworkTime)
        {
            StartCoroutine(FetchNetworkTime());
        }
    }

    public DateTime GetGMTTime()
    {
        return currentTime; // currentTime is already UTC/GMT
    }

    public bool IsTimeAvailable()
    {
        return isTimeAvailable;
    }

    public string GetFormattedTime()
    {
        return currentTime.ToString("yyyy-MM-dd HH:mm:ss");
    }

    public DateTime GetTimeWithOffset(int hoursOffset = 0)
    {
        return currentTime.AddHours(hoursOffset);
    }
}