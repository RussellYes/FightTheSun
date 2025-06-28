using TMPro;
using UnityEditor;
using UnityEngine;

public class LegendUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI memoyText;
    [SerializeField] private TextMeshProUGUI moneyText;
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private TextMeshProUGUI toolText;
    [SerializeField] private TextMeshProUGUI comicText;
    [SerializeField] private TextMeshProUGUI ironText;
    [SerializeField] private TextMeshProUGUI cobaltText;

    private bool isInitialized = false;

    private void OnEnable()
    {
        DataPersister.InitializationComplete += DataPersister_InitializationComplete;
        if (isInitialized)
        {
            UpdateUI();
        }
    }

    private void OnDisable()
    {
        DataPersister.InitializationComplete -= DataPersister_InitializationComplete;
    }

    private void DataPersister_InitializationComplete()
    {
        isInitialized = true;
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (DataPersister.Instance == null || DataPersister.Instance.CurrentGameData == null)
        {
            Debug.LogWarning("DataPersister or CurrentGameData is not initialized.");
            return;
        }

        GameData gameData = DataPersister.Instance.CurrentGameData;

        // Memory
        memoyText.text = $"Memory {gameData.playerData[0].playerMemoryScore:F0}";

        // Money
        moneyText.text = $"Money {gameData.totalMoney:F0}";

        // Time
        float timeRemaining = gameData.totalTime;

        int minutes = Mathf.FloorToInt(timeRemaining / 60);
        int seconds = Mathf.FloorToInt(timeRemaining % 60);
        timeText.text = $"Time: {minutes:00}:{seconds:00}";

        // Tool
        toolText.text = $"Tools";

        // Comic
        comicText.text = $"Comics";

        // Iron
        ironText.text = $"Iron {gameData.totalMetal:F0}";

        // Cobalt
        cobaltText.text = $"Cobalt {gameData.totalRareMetal:F0}";
    }



}
