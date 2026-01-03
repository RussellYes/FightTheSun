using System;
using UnityEngine;

public class LootShipLoot : MonoBehaviour
{
    public static event Action<string, string, float> lootShipMessageEvent;

    private float stockChange;

    void Start()
    {
        SetStockChange();
    }

    private void SetStockChange()
    {
        float minStockChange = 0.5f;
        float maxStockChange = 5f;
        stockChange = UnityEngine.Random.Range(minStockChange, maxStockChange);
    }

    public void ChangeSharePrice()
    {
        string companyName = DataPersister.Instance.CurrentGameData.randomLootShipCompanyName;
        int companyNumber = DataPersister.Instance.CurrentGameData.investingCompanyNumber + 1; // +1 for 1 based company names from a 0 based index in InvestingUI
        TimeManager timeManager = FindFirstObjectByType<TimeManager>();
        string currentDay = timeManager.GetDayOfWeek();

        // Construct the field name
        string fieldName = $"company{companyNumber}{currentDay}SharePrice";

        // Use reflection to get and set the field value
        var gameData = DataPersister.Instance.CurrentGameData;
        var field = typeof(GameData).GetField(fieldName);

        if (field != null)
        {
            float currentPrice = (float)field.GetValue(gameData);
            field.SetValue(gameData, currentPrice + stockChange);
            DataPersister.Instance.SaveCurrentGame();
        }
        else
        {
            Debug.LogError($"LootShipLoot ChangeSharePrice - Company name {fieldName} not found in GameData");
        }

        lootShipMessageEvent?.Invoke("mavis", $"Loot Ship Captured! {companyName}'s share price changed by {stockChange:F2}!", 5f);

        ResetDailyLootShip();
    }

    private void ResetDailyLootShip()
    {
        DataPersister.Instance.CurrentGameData.randomLootShipLevel = 0;
        DataPersister.Instance.CurrentGameData.randomLootShipCompanyName = "";
        DataPersister.Instance.CurrentGameData.randomLootShipCompanyColour = Color.white;
        DataPersister.Instance.CurrentGameData.investingCompanyNumber = 0;
        DataPersister.Instance.SaveCurrentGame();
    }

}


