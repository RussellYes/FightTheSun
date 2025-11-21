using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AppUpdateMessage : MonoBehaviour
{
    [SerializeField] private GameObject appUpdateMessageHolder;
    [SerializeField] private Button closeMessageButton;
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private TextMeshProUGUI titleText;

    [Header("Time")]
    private float currentMonth;
    private bool isNewMonth = false;

    private void OnEnable()
    {
        DataPersister.InitializationComplete += DataPersisterInitialize;
        closeMessageButton.onClick.AddListener(CloseDisplayMessage);
    }
    private void OnDisable()
    {
        DataPersister.InitializationComplete -= DataPersisterInitialize;
        closeMessageButton.onClick.RemoveListener(CloseDisplayMessage);
    }

    void DataPersisterInitialize()
    {
        appUpdateMessageHolder.SetActive(false);
        GetTime();
    }

    private void GetTime()
    {
        float appMessageSavedMonth = DataPersister.Instance.CurrentGameData.appMessageSavedMonth;

        currentMonth = GetCurrentMonthAsFloat();

        Debug.Log($"InvestingUI GetTime - Current Month: {currentMonth}, Last Saved Month: {appMessageSavedMonth}.");

        isNewMonth = appMessageSavedMonth != currentMonth;

        if (isNewMonth)
        {
            DisplayMessage();
        }

        DataPersister.Instance.CurrentGameData.appMessageSavedMonth = currentMonth;
        DataPersister.Instance.SaveCurrentGame();
    }

    // Convert month to float (0-11)
    private float GetCurrentMonthAsFloat()
    {
        if (!TimeManager.Instance.IsTimeAvailable())
        {
            Debug.LogWarning("AppUpdateMessage GetCurrentMonthAsFloat - WorldTimeAPI time not available, using fallback");
            return (float)System.DateTime.UtcNow.Month;
        }

        string monthString = TimeManager.Instance.GetMonth();
        Debug.Log($"AppUpdateMessage - Raw month string from TimeManager: '{monthString}'");
        return ConvertMonthToFloat(monthString);
    }

    // Convert month string to float (Jan = 0 to Dec = 11)
    private float ConvertMonthToFloat(string month)
    {
        switch (month.ToLower())
        {
            case "jan": return 0f;
            case "feb": return 1f;
            case "mar": return 2f;
            case "apr": return 3f;
            case "may": return 4f;
            case "jun": return 5f;
            case "jul": return 6f;
            case "aug": return 7f;
            case "sep": return 8f;
            case "oct": return 9f;
            case "nov": return 10f;
            case "dec": return 11f;
            default:
                Debug.LogWarning($"AppUpdateMessage ConvertMonthToFloat - Unknown month: {month}, defaulting to January.");
                return 0f;
        }
    }

    private void DisplayMessage()
    {
        appUpdateMessageHolder.gameObject.SetActive(true);
        string month = "Nov";
        string year = "2025";
        string message = "Check out the new stock exchange at planet 5. Daily dividends.";
        string message2 = "December's gift: Game play will affect investments.";
        titleText.text = ($"Game dev news:\n{month} {year}");
        messageText.text = ($"   {message}\n   {message2}\n   Thank you for playing Fight The Sun.");
    }

    private void CloseDisplayMessage()
    {
        appUpdateMessageHolder.gameObject.SetActive(false);
    }

}
