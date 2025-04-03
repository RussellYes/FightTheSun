using System;
using UnityEngine;
using UnityEngine.SceneManagement;

// This script manages the player's collectable resources for a score output, and saving and loading.

public class ScoreManager : MonoBehaviour
{
    GameManager gameManager;
    EndConditionsUI endConditionsUI;

    // Level-specific resources (reset each level)
    public float levelMoney = 0;
    public float levelMetal = 0f;
    public float levelRareMetal = 0f;
    public float levelTime = 0f;
    public int levelObstaclesDestroyed = 0;

    // Persistent totals across all levels
    private float totalMoney = 0;
    private float totalMetal = 0f;
    private float totalRareMetal = 0f;
    public float totalTime = 0f;
    public int totalObstaclesDestroyed = 0;

    // Obstacle tracking
    private int totalObstaclesInScene;
    private int currentObstaclesInScene;

    // Events for UI updates
    public static event Action<float> OnMoneyChanged;
    public static event Action<float> OnLevelMoneyChanged;
    public static event Action<float> OnTotalMetalChanged;
    public static event Action<float> OnLevelMetalChanged;
    public static event Action<float> OnTotalRareMetalChanged;
    public static event Action<float> OnLevelRareMetalChanged;
    public static event Action<int> OnObstaclesDestroyedByPlayerChanged;
    public static event Action<float> OnLevelTimeChanged;
    public static event Action SavedTotalEvent;

    // Public getters
    public float GetTotalMoney() => totalMoney;
    public float GetTotalMetal() => totalMetal;
    public float GetTotalRareMetal() => totalRareMetal;
    public float GetTotalTime() => totalTime;

    public int GetTotalObstaclesDestroyed() => totalObstaclesDestroyed;
    public float GetLevelMoney() => levelMoney;
    public float GetLevelMetal() => levelMetal;
    public float GetLevelRareMetal() => levelRareMetal;
    public float GetLevelTime() => levelTime;
    public int GetLevelObstaclesDestroyed() => levelObstaclesDestroyed;


    private void Awake()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        gameManager = FindObjectOfType<GameManager>();
        endConditionsUI = FindObjectOfType<EndConditionsUI>();
        LoadTotals();
    }
    private void OnEnable()
    {
        // Subscribe to obstacle events
        Obstacle.ObstacleEntersSceneEvent += OnObstacleEntersScene;
        Obstacle.ObstacleExitsSceneEvent += OnObstacleExitsScene;
        Loot.PlayerGainsLootEvent += OnPlayerGainsLoot;
        GameManager.EndGameDataSaveEvent += UpdateData;
        EndConditionsUI.EndConditionUIScoreChoiceEvent += SaveDataAtEndOfLevel;
    }

    private void OnDisable()
    {
        // Unsubscribe from obstacle events
        Obstacle.ObstacleEntersSceneEvent -= OnObstacleEntersScene;
        Obstacle.ObstacleExitsSceneEvent -= OnObstacleExitsScene;
        Loot.PlayerGainsLootEvent -= OnPlayerGainsLoot;
        GameManager.EndGameDataSaveEvent -= UpdateData;
        EndConditionsUI.EndConditionUIScoreChoiceEvent -= SaveDataAtEndOfLevel;
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {;
        // Check if the scene index is within the bounds of the array
        if (scene.buildIndex == 0)
        {
            LoadTotals();
        }
        else // New level started
        {
            ResetLevelResources();
        }
    }

    private void OnObstacleEntersScene()
    {
        totalObstaclesInScene++;
        currentObstaclesInScene++;
        Debug.Log($"Obstacle entered scene. Total: {totalObstaclesInScene}, Current: {currentObstaclesInScene}");
    }

    private void OnObstacleExitsScene(bool isKilledByPlayer, int pointValue)
    {
        currentObstaclesInScene--;
        Debug.Log($"Obstacle exited scene. Total: {totalObstaclesInScene}, Current: {currentObstaclesInScene}");

        if (isKilledByPlayer)
        {
            levelObstaclesDestroyed++;
            ChangeLevelMoney(pointValue);
            Debug.Log($"Money updated: {totalMoney}");

            // Trigger the obstacles destroyed by player event
            OnObstaclesDestroyedByPlayerChanged?.Invoke(levelObstaclesDestroyed);
        }
    }

    private void ResetLevelResources()
    {
        levelMoney = 0;
        levelMetal = 0f;
        levelRareMetal = 0f;
        levelTime = 0;
        levelObstaclesDestroyed = 0;
        totalObstaclesInScene = 0;
        levelObstaclesDestroyed = 0;
        currentObstaclesInScene = 0;

        // Notify UI of reset
        OnLevelMoneyChanged?.Invoke(levelMoney);
        OnLevelMetalChanged?.Invoke(levelMetal);
        OnLevelRareMetalChanged?.Invoke(levelRareMetal);
    }

    public void OnKeyboardInput()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            Debug.Log("P key pressed. Resetting data.");
            ResetData();
        }
    }

    private void ResetData()
    {
        ResetLevelResources();

        totalMoney = 0f;
        totalMetal = 0f;
        totalRareMetal = 0f;
        totalObstaclesDestroyed = 0;

        // Clear all level-specific data
        int levelCount = SceneManager.sceneCountInBuildSettings;
        for (int i = 1; i <= levelCount - 2; i++) // Now using 1-based
        {
            PlayerPrefs.DeleteKey($"Level_{i}_Money");
            PlayerPrefs.DeleteKey($"Level_{i}_Time");
            PlayerPrefs.DeleteKey($"Level_{i}_ObstaclesDestroyed");
        }

        SaveTotals();

        //Update UI
        OnMoneyChanged?.Invoke(totalMoney);
        OnTotalMetalChanged?.Invoke(totalMetal);
        OnTotalRareMetalChanged?.Invoke(totalRareMetal);
    }
    private void OnPlayerGainsLoot(float metalGained, float rareMetalGained)
    {
        ChangeLevelMetal(metalGained);
        ChangeLevelRareMetal(rareMetalGained);
    }

    private void ChangeTotalMoney(float points)
    {
        totalMoney += points;

        // Trigger the money change event
        OnMoneyChanged?.Invoke(totalMoney);
    }
    private void ChangeLevelMoney(int points)
    {

        levelMoney += points;
        Debug.Log($"Added {points} to levelMoney. Total: {levelMoney}");
        // Trigger the levelMoney change event
        OnLevelMoneyChanged?.Invoke(levelMoney);
    }
    private void ChangeTotalMetal(float points)
    {
        totalMetal += points;
        OnTotalMetalChanged?.Invoke(totalMetal);

    }
    private void ChangeLevelMetal(float points)
    {
        levelMetal += points;
        OnLevelMetalChanged?.Invoke(levelMetal);
    }
    private void ChangeTotalRareMetal(float points)
    {
        totalRareMetal += points;
        OnTotalRareMetalChanged?.Invoke(totalRareMetal);

    }
    private void ChangeLevelRareMetal(float points)
    {
        levelRareMetal += points;
        OnLevelRareMetalChanged?.Invoke(levelRareMetal);
    }

    private string FormatTime(float timeInSeconds)
    {
        int minutes = Mathf.FloorToInt(timeInSeconds / 60);
        int seconds = Mathf.FloorToInt(timeInSeconds % 60);
        return $"{minutes:00}:{seconds:00}";
    }

    private void UpdateData()
    {
        // Add level progress to totals
        ChangeTotalMoney(levelMoney);
        ChangeTotalMetal(levelMetal);
        ChangeTotalRareMetal(levelRareMetal);
        totalTime += levelTime;
        totalObstaclesDestroyed += levelObstaclesDestroyed;
    }

    private void SaveDataAtEndOfLevel()
    {
        Debug.Log("ScoreManager SaveDataAtEndOfLevel");

        // Get current level index
        int levelNumber = SceneManager.GetActiveScene().buildIndex - 1;
        Debug.Log($"Current level number: {levelNumber} (Scene name: {SceneManager.GetActiveScene().name})");

        // Only process for actual game levels (skip menu/loading screens)
        if (levelNumber >= 1)
        {
            // Debug log all level values before saving
            Debug.Log($"Level {levelNumber} stats - " +
                     $"Time: {levelTime}, " +
                     $"Money: {levelMoney}, " +
                     $"Obstacles: {levelObstaclesDestroyed}");

            // Get final time from GameManager
            if (gameManager == null)
            {
                gameManager = FindObjectOfType<GameManager>();
            }

            levelTime = gameManager.GameTime;
            Debug.Log($"Final level time from GameManager: {levelTime} seconds");

            // FIRST save the current levelvariables to PlayerPrefs
            PlayerPrefs.SetFloat($"Level_{levelNumber}_Time", levelTime);
            Debug.Log($"Saved level {levelNumber} time: {levelTime}"); // Verification

            PlayerPrefs.SetFloat($"Level_{levelNumber}_Money", levelMoney);
            Debug.Log($"Saved level {levelNumber} money: {levelMoney}"); // Verification

            PlayerPrefs.SetInt($"Level_{levelNumber}_ObstaclesDestroyed", levelObstaclesDestroyed);
            Debug.Log($"Saved level {levelNumber} obstacles destroyed: {levelObstaclesDestroyed}"); // Verification

            float savedTime = PlayerPrefs.GetFloat($"Level_{levelNumber}_Time", -1f);
            Debug.Log($"Verify saved time: {savedTime}");

            // Verify saved data
            Debug.Log($"Updated totals - " +
                     $"TotalTime: {FormatTime(totalTime)}, " +
                     $"TotalMoney: {totalMoney}, " +
                     $"TotalObstaclesDestroyed: {totalObstaclesDestroyed}");
        }
        SaveTotals();
    }

    private void SaveTotals()
    {
        Debug.Log("ScoreManager SaveTotals");
        PlayerPrefs.SetFloat("TotalMoney", totalMoney);
        PlayerPrefs.SetFloat("TotalMetal", totalMetal);
        PlayerPrefs.SetFloat("TotalRareMetal", totalRareMetal);
        PlayerPrefs.SetFloat("TotalTime", totalTime);
        PlayerPrefs.SetInt("TotalObstaclesDestroyed", totalObstaclesDestroyed);
        PlayerPrefs.Save();

        SavedTotalEvent?.Invoke();
    }

    private void LoadTotals()
    {
        Debug.Log("ScoreManager LoadTotals");
        totalMoney = PlayerPrefs.GetFloat("TotalMoney", 0); // Default: 0 if not found
        totalMetal = PlayerPrefs.GetFloat("TotalMetal", 0f); // Default: 0
        totalRareMetal = PlayerPrefs.GetFloat("TotalRareMetal", 0f); // Default: 0
        totalTime = PlayerPrefs.GetFloat("TotalTime", 0f); // Default: 0
        totalObstaclesDestroyed = PlayerPrefs.GetInt("TotalObstaclesDestroyed", 0); // Default: 0
    }
}