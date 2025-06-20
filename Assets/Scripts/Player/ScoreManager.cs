using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScoreManager : MonoBehaviour
{
    GameManager gameManager;

    // Level-specific resources (reset each level)
    public float levelMoney = 0;
    public float levelMetal = 0f;
    public float levelRareMetal = 0f;
    public float levelTime = 0f;
    public int levelObstaclesDestroyed = 0;

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
    public float GetTotalMoney() => DataPersister.Instance?.CurrentGameData?.totalMoney ?? 0;
    public float GetTotalMetal() => DataPersister.Instance?.CurrentGameData?.totalMetal ?? 0;
    public float GetTotalRareMetal() => DataPersister.Instance?.CurrentGameData?.totalRareMetal ?? 0;
    public float GetTotalTime() => DataPersister.Instance?.CurrentGameData?.totalTime ?? 0;
    public int GetTotalObstaclesDestroyed() => DataPersister.Instance?.CurrentGameData?.totalObstaclesDestroyed ?? 0;
    public float GetLevelMoney() => levelMoney;
    public float GetLevelMetal() => levelMetal;
    public float GetLevelRareMetal() => levelRareMetal;
    public float GetLevelTime() => levelTime;
    public int GetLevelObstaclesDestroyed() => levelObstaclesDestroyed;

    private void Awake()
    {
        gameManager = FindFirstObjectByType<GameManager>();
    }

    private void OnEnable()
    {
        Obstacle.ObstacleEntersSceneEvent += OnObstacleEntersScene;
        Obstacle.ObstacleExitsSceneEvent += OnObstacleExitsScene;
        Loot.PlayerGainsLootEvent += OnPlayerGainsLoot;
        GameManager.EndGameDataSaveEvent += EndGameLevelDataSave; 
        DataPersister.InitializationComplete += Initialized; // Ensure totals are loaded after initialization
    }

    private void OnDisable()
    {
        Obstacle.ObstacleEntersSceneEvent -= OnObstacleEntersScene;
        Obstacle.ObstacleExitsSceneEvent -= OnObstacleExitsScene;
        Loot.PlayerGainsLootEvent -= OnPlayerGainsLoot;
        GameManager.EndGameDataSaveEvent -= EndGameLevelDataSave;
        EndConditionsUI.EndConditionUIScoreChoiceEvent -= SaveBestDataAtEndOfLevel; 
        DataPersister.InitializationComplete -= Initialized;
    }

    private void Initialized()
    {
        ResetLevelResources();
        UpdateAllUI();
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
        // Level-specific
        levelMoney = 0;
        levelMetal = 0f;
        levelRareMetal = 0f;
        levelTime = 0f;
        levelObstaclesDestroyed = 0;

        // Obstacle tracking
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
        DataPersister.Instance.CurrentGameData.totalMoney += points;
        OnMoneyChanged?.Invoke(DataPersister.Instance.CurrentGameData.totalMoney);
    }

    private void ChangeTotalMetal(float points)
    {
        DataPersister.Instance.CurrentGameData.totalMetal += points;
        OnTotalMetalChanged?.Invoke(DataPersister.Instance.CurrentGameData.totalMetal);
    }

    private void ChangeTotalRareMetal(float points)
    {
        DataPersister.Instance.CurrentGameData.totalRareMetal += points;
        OnTotalRareMetalChanged?.Invoke(DataPersister.Instance.CurrentGameData.totalRareMetal);
    }

    private void EndGameLevelDataSave()
    {
        // Add level progress to totals
        ChangeTotalMoney(levelMoney);
        ChangeTotalMetal(levelMetal);
        ChangeTotalRareMetal(levelRareMetal);
        DataPersister.Instance.CurrentGameData.totalTime += levelTime;
        DataPersister.Instance.CurrentGameData.totalObstaclesDestroyed += levelObstaclesDestroyed;
    }
    private void SaveBestDataAtEndOfLevel()
    {
        int levelNumber = SceneManager.GetActiveScene().buildIndex -1;
        levelTime = gameManager.LevelTime;
        var gameData = DataPersister.Instance.CurrentGameData;

        if (levelNumber >= 0 && DataPersister.Instance != null && DataPersister.Instance.CurrentGameData != null)
        {
            if (gameManager == null)
            {
                gameManager = FindFirstObjectByType<GameManager>();
            }
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
            gameData.totalMoney += levelMoney;
            gameData.totalMetal += levelMetal;
            gameData.totalRareMetal += levelRareMetal;
            gameData.totalTime += levelTime;
            gameData.totalObstaclesDestroyed += levelObstaclesDestroyed;
        }

        DataPersister.Instance.SaveCurrentGame();
        SavedTotalEvent?.Invoke();
    }

    private void UpdateAllUI()
    {
        OnMoneyChanged?.Invoke(DataPersister.Instance.CurrentGameData.totalMoney);
        OnTotalMetalChanged?.Invoke(DataPersister.Instance.CurrentGameData.totalMetal);
        OnTotalRareMetalChanged?.Invoke(DataPersister.Instance.CurrentGameData.totalRareMetal);
        OnLevelMoneyChanged?.Invoke(levelMoney);
        OnLevelMetalChanged?.Invoke(levelMetal);
        OnLevelRareMetalChanged?.Invoke(levelRareMetal);
        OnObstaclesDestroyedByPlayerChanged?.Invoke(levelObstaclesDestroyed);
    }


}