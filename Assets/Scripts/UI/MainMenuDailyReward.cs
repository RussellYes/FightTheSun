using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuDailyReward : MonoBehaviour
{
    public static event Action <string> DailyRewardButtonEvent;

    [SerializeField] private Button openDailyRewardUIButton;
    [SerializeField] private Button closeDailyRewardUIButton;
    [SerializeField] private GameObject dailyRewardUI;
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private TextMeshProUGUI rewardText;

    [Header("Time")]
    private float currentDayOfTheWeek;
    private float currentTime;
    private bool isNewDay = false;
    private int dailyRewardCount;
    [SerializeField] private float closeUITimer = 6f;
    [SerializeField] private float closeUICountdown;

    private void OnEnable()
    {
        openDailyRewardUIButton.onClick.AddListener(OnOpenDailyRewardButtonClicked);
        closeDailyRewardUIButton.onClick.AddListener(OnCloseDailyRewardButtonClicked);
        RewardedAdPlayer.RewardGranted += DailyRewardGranted;
    }
    private void OnDisable()
    {
        openDailyRewardUIButton.onClick.RemoveListener(OnOpenDailyRewardButtonClicked);
        closeDailyRewardUIButton.onClick.RemoveListener(OnCloseDailyRewardButtonClicked);
        RewardedAdPlayer.RewardGranted -= DailyRewardGranted;
    }

    private void Start()
    {
        dailyRewardUI.SetActive(false);
    }

    private void Update()
    {
        if (closeUICountdown > 0f)
        {
            closeUICountdown -= Time.deltaTime;
            if (closeUICountdown <= 0f)
            {
                dailyRewardUI.SetActive(false);
            }
        }
    }

    private void OnOpenDailyRewardButtonClicked()
    {
        Debug.Log($"MainMenuDailyReward OnOpenDailyRewardButtonClicked");
        DailyRewardButtonEvent?.Invoke("dailyReward");
    }

    private void DailyRewardGranted(string requesterID)
    {
        if (requesterID == "dailyReward")
        {
            dailyRewardUI.SetActive(true);
            closeUICountdown = closeUITimer;
            GetTime();
            float baseRewardAmount = UnityEngine.Random.Range(1000, 1500);
            if (dailyRewardCount <= 0)
            {
                dailyRewardCount = 1;
            }
            float rewardAmount = baseRewardAmount / dailyRewardCount;
            messageText.text = $"Daily Rewards {dailyRewardCount}";
            rewardText.text = $"{rewardAmount}";
            dailyRewardCount++;
            DataPersister.Instance.CurrentGameData.totalMoney += rewardAmount;
            DataPersister.Instance.CurrentGameData.dailyRewardCount = dailyRewardCount;
            DataPersister.Instance.SaveCurrentGame();
        }

    }

    private void OnCloseDailyRewardButtonClicked()
    {
        dailyRewardUI.SetActive(false);
    }

    #region Time Management
    private void GetTime()
    {
        float lastSavedDayOfTheWeek = DataPersister.Instance.CurrentGameData.dailyRewardDayOfTheWeek;
        float lastSavedCurrentTime = DataPersister.Instance.CurrentGameData.dailyRewardSavedTime;

        currentDayOfTheWeek = GetCurrentDayOfWeekAsFloat();
        currentTime = GetCurrentTimeAsFloat();
        Debug.Log($"MainMenuDailyReward GetTime - Current Day: {currentDayOfTheWeek}, Current Time: {currentTime}, last Saved Day: {lastSavedDayOfTheWeek} ");

        isNewDay = lastSavedDayOfTheWeek != currentDayOfTheWeek;

        if (isNewDay)
        {
            Debug.Log($"MainMenuDailyReward GetTime - New day detected! Resetting daily reward count from {dailyRewardCount} to 1");
            dailyRewardCount = 1;
        }

        DataPersister.Instance.CurrentGameData.dailyRewardDayOfTheWeek = currentDayOfTheWeek;
        DataPersister.Instance.CurrentGameData.dailyRewardSavedTime = currentTime;
        DataPersister.Instance.CurrentGameData.dailyRewardCount = dailyRewardCount;
        DataPersister.Instance.SaveCurrentGame();
    }

    // Convert day of week to float (0-6)
    private float GetCurrentDayOfWeekAsFloat()
    {
        if (!TimeManager.Instance.IsTimeAvailable())
        {
            Debug.LogWarning("MainMenuDailyReward GetCurrentDayOfWeekAsFloat - WorldTimeAPI time not available, using fallback");
            return (float)System.DateTime.UtcNow.DayOfWeek;
        }

        string dayString = TimeManager.Instance.GetDayOfWeek();
        return ConvertDayOfWeekToFloat(dayString);
    }

    // Convert time to float (hours as decimal)
    private float GetCurrentTimeAsFloat()
    {
        if (!TimeManager.Instance.IsTimeAvailable())
        {
            Debug.LogWarning("MainMenuDailyReward GetCurrentTimeAsFloat - WorldTimeAPI time not available, using fallback");
            var now = System.DateTime.UtcNow;
            return now.Hour + now.Minute / 60f;
        }

        DateTime gmtTime = TimeManager.Instance.GetGMTTime();
        return gmtTime.Hour + gmtTime.Minute / 60f;
    }

    // Convert day string to float (Sunday = 0 to Saturday = 6)
    private float ConvertDayOfWeekToFloat(string dayOfWeek)
    {
        switch (dayOfWeek.ToLower())
        {
            case "sunday": return 0f;
            case "monday": return 1f;
            case "tuesday": return 2f;
            case "wednesday": return 3f;
            case "thursday": return 4f;
            case "friday": return 5f;
            case "saturday": return 6f;
            default:
                Debug.LogWarning($"MainMenuDailyReward ConvertDayOfWeekToFloat - Unknown day of week: {dayOfWeek}, defaulting to Sunday");
                return 0f;
        }
    }
    #endregion
}
