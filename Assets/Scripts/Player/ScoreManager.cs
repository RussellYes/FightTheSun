using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScoreManager : MonoBehaviour
{
    GameManager gameManager;
    private EndConditionsUI endConditionsUI;

    // Level-specific resources (reset each level)
    public float levelMoney = 0;
    public float levelMetal = 0f;
    public float levelRareMetal = 0f;
    public float levelTime = 0f;
    public int levelObstaclesDestroyed = 0;
    
    // Local copies of totals for quick access
    private float totalMoney;
    private float totalMetal;
    private float totalRareMetal;
    private float totalTime;
    private int totalObstaclesDestroyed;

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
        gameManager = FindFirstObjectByType<GameManager>();
        endConditionsUI = FindFirstObjectByType<EndConditionsUI>();
    }

    private void Start()
    {
        // Initialize with loaded data if available
        if (DataPersister.Instance != null && DataPersister.Instance.CurrentGameData != null)
        {
            LoadTotals();
        }
    }

    private void OnEnable()
    {
        Obstacle.ObstacleEntersSceneEvent += OnObstacleEntersScene;
        Obstacle.ObstacleExitsSceneEvent += OnObstacleExitsScene;
        Loot.PlayerGainsLootEvent += OnPlayerGainsLoot;
        GameManager.EndGameDataSaveEvent += EndGameLevelDataSave; 
        EndConditionsUI.EndConditionUIScoreChoiceEvent += SaveBestDataAtEndOfLevel;
        EndConditionsUI.reviveEvent += ResetDataOnDeath;
        DataPersister.NewGameEvent += ResetDataOnNewGame;
        MainMenuUI.NewGameEvent += ResetDataOnNewGame;
    }

    private void OnDisable()
    {
        Obstacle.ObstacleEntersSceneEvent -= OnObstacleEntersScene;
        Obstacle.ObstacleExitsSceneEvent -= OnObstacleExitsScene;
        Loot.PlayerGainsLootEvent -= OnPlayerGainsLoot;
        GameManager.EndGameDataSaveEvent -= EndGameLevelDataSave;
        EndConditionsUI.EndConditionUIScoreChoiceEvent -= SaveBestDataAtEndOfLevel; 
        SceneManager.sceneLoaded -= OnSceneLoaded;
        EndConditionsUI.reviveEvent -= ResetDataOnDeath;
        DataPersister.NewGameEvent -= ResetDataOnNewGame;
        MainMenuUI.NewGameEvent -= ResetDataOnNewGame;
    }

    private void ResetDataOnNewGame()
    {

        if (DataPersister.Instance != null && DataPersister.Instance.CurrentGameData != null)
        {
            var gameData = DataPersister.Instance.CurrentGameData;

            // Reset all level data
            gameData.levelData.Clear();
            for (int i = 1; i <= 10; i++)
            {
                gameData.levelData[i] = new LevelData(0, 0, 0);
                gameData.SetMissionComplete(i, false);
            }

            // Reset unlock status (only Level 1 unlocked)
            gameData.isMission1Unlocked = true;
            for (int i = 2; i <= 10; i++)
            {
                gameData.SetMissionUnlocked(i, false);
            }

            ResetAllValues();

            // Update totals in GameData
            gameData.totalMoney = totalMoney;
            gameData.totalTime = totalTime;
            gameData.totalMetal = totalMetal;
            gameData.totalRareMetal = totalRareMetal;
            gameData.totalObstaclesDestroyed = totalObstaclesDestroyed;

            DataPersister.Instance.SaveCurrentGame();

            OnLevelMoneyChanged?.Invoke(levelMoney);
            OnLevelMetalChanged?.Invoke(levelMetal);
            OnLevelRareMetalChanged?.Invoke(levelRareMetal);
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.buildIndex == 0) // Main menu
        {
            LoadTotals();
        }
        else // Game level
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

        if (isKilledByPlayer)
        {
            levelObstaclesDestroyed++;
            ChangeLevelMoney(pointValue);
            OnObstaclesDestroyedByPlayerChanged?.Invoke(levelObstaclesDestroyed);
        }
    }

    private void ResetLevelResources()
    {
        levelMoney = 0;
        levelMetal = 0f;
        levelRareMetal = 0f;
        levelTime = 0f;
        levelObstaclesDestroyed = 0;
        totalObstaclesInScene = 0;
        currentObstaclesInScene = 0;
    }

    private void OnPlayerGainsLoot(float metalGained, float rareMetalGained)
    {
        ChangeLevelMetal(metalGained);
        ChangeLevelRareMetal(rareMetalGained);
    }

    private void ChangeLevelMoney(int points)
    {
        levelMoney += points;
        OnLevelMoneyChanged?.Invoke(levelMoney);
    }

    private void ChangeLevelMetal(float points)
    {
        levelMetal += points;
        OnLevelMetalChanged?.Invoke(levelMetal);
    }

    private void ChangeLevelRareMetal(float points)
    {
        levelRareMetal += points;
        OnLevelRareMetalChanged?.Invoke(levelRareMetal);
    }

    private void ChangeTotalMoney(float points)
    {
        totalMoney += points;
        OnMoneyChanged?.Invoke(totalMoney);
    }

    private void ChangeTotalMetal(float points)
    {
        totalMetal += points;
        OnTotalMetalChanged?.Invoke(totalMetal);
    }

    private void ChangeTotalRareMetal(float points)
    {
        totalRareMetal += points;
        OnTotalRareMetalChanged?.Invoke(totalRareMetal);
    }

    private void EndGameLevelDataSave()
    {
        // Add level progress to totals
        ChangeTotalMoney(levelMoney);
        ChangeTotalMetal(levelMetal);
        ChangeTotalRareMetal(levelRareMetal);
        totalTime += levelTime;
        totalObstaclesDestroyed += levelObstaclesDestroyed;

        Debug.Log("ScoreManager - UpdateData - TotalObstaclesDestroyed = " + totalObstaclesDestroyed);

        // Update GameData
        if (DataPersister.Instance != null && DataPersister.Instance.CurrentGameData != null)
        {
            var gameData = DataPersister.Instance.CurrentGameData;
            gameData.totalMoney = totalMoney;
            gameData.totalMetal = totalMetal;
            gameData.totalRareMetal = totalRareMetal;
            gameData.totalTime = totalTime;
            gameData.totalObstaclesDestroyed = totalObstaclesDestroyed;
        }
    }

    private void SaveBestDataAtEndOfLevel()
    {
        int levelNumber = SceneManager.GetActiveScene().buildIndex -1;

        if (levelNumber >= 0 && DataPersister.Instance != null && DataPersister.Instance.CurrentGameData != null)
        {
            if (gameManager == null)
            {
                gameManager = FindFirstObjectByType<GameManager>();
            }

            levelTime = gameManager.GameTime;
            var gameData = DataPersister.Instance.CurrentGameData;

            if (gameData.levelData.ContainsKey(levelNumber))
            {
                gameData.levelData[levelNumber] = new LevelData(levelTime, levelMoney, levelObstaclesDestroyed);
            }
            else
            {
                gameData.levelData.Add(levelNumber, new LevelData(levelTime, levelMoney, levelObstaclesDestroyed));
            }

            // Mark level as complete
            gameData.SetMissionComplete(levelNumber, true);

            // Unlock next level (if not last level)
            if (levelNumber < 10)
            {
                gameData.SetMissionUnlocked(levelNumber + 1, true);
            }

            // Update totals in GameData
            gameData.totalMoney = totalMoney;
            gameData.totalMetal = totalMetal;
            gameData.totalRareMetal = totalRareMetal;
            gameData.totalTime = totalTime;
            gameData.totalObstaclesDestroyed = totalObstaclesDestroyed;

            DataPersister.Instance.SaveCurrentGame();
        }

        SavedTotalEvent?.Invoke();
    }

    private void LoadTotals()
    {
        if (DataPersister.Instance != null && DataPersister.Instance.CurrentGameData != null)
        {
            var gameData = DataPersister.Instance.CurrentGameData;

            // Update local totals
            totalMoney = gameData.totalMoney;
            totalMetal = gameData.totalMetal;
            totalRareMetal = gameData.totalRareMetal;
            totalTime = gameData.totalTime;
            totalObstaclesDestroyed = gameData.totalObstaclesDestroyed;

            // Update UI
            OnMoneyChanged?.Invoke(totalMoney);
            OnTotalMetalChanged?.Invoke(totalMetal);
            OnTotalRareMetalChanged?.Invoke(totalRareMetal);
        }
    }

    private void ResetDataOnDeath()
    {
        ResetAllValues();

        if (DataPersister.Instance != null && DataPersister.Instance.CurrentGameData != null)
        {
            var gameData = DataPersister.Instance.CurrentGameData;
            int currentLevel = SceneManager.GetActiveScene().buildIndex - 1;

            // Reset all levels saved data
            for (int i = 1; i <= 10; i++)
            {
                gameData.levelData[i] = new LevelData(0, 0, 0);
            }

            // Reset all resource totals
            gameData.totalMoney = 0f;
            gameData.totalTime = 0f;
            gameData.totalMetal = 0f;
            gameData.totalRareMetal = 0f;
            gameData.totalObstaclesDestroyed = 0;

            // Lock all levels except Level 1
            for (int i = 2; i <= 10; i++)
            {
                gameData.SetMissionUnlocked(i, false);
            }

            UpdateAllUI();

            DataPersister.Instance.SaveCurrentGame();
            Debug.Log("ScoreManager - ResetDataOnDeath - all level data zeroed after death");
        }
    }

    private void UpdateAllUI()
    {
        OnMoneyChanged?.Invoke(totalMoney);
        OnTotalMetalChanged?.Invoke(totalMetal);
        OnTotalRareMetalChanged?.Invoke(totalRareMetal);
        OnLevelMoneyChanged?.Invoke(levelMoney);
        OnLevelMetalChanged?.Invoke(levelMetal);
        OnLevelRareMetalChanged?.Invoke(levelRareMetal);
        OnObstaclesDestroyedByPlayerChanged?.Invoke(levelObstaclesDestroyed);
    }
    private void ResetAllValues()
    {
        // Level-specific
        levelMoney = 0;
        levelMetal = 0f;
        levelRareMetal = 0f;
        levelTime = 0f;
        levelObstaclesDestroyed = 0;

        // Totals
        totalMoney = 0f;
        totalMetal = 0f;
        totalRareMetal = 0f;
        totalTime = 0f;
        totalObstaclesDestroyed = 0;

        // Obstacle tracking
        totalObstaclesInScene = 0;
        currentObstaclesInScene = 0;
    }

}