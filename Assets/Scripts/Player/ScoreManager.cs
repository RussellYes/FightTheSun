using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class ScoreManager : MonoBehaviour
{
    private int totalObstaclesInScene;
    private int killedByPlayerCount;
    private int currentObstaclesInScene;
    public int money = 0;
    public int levelMoney = 0;
    private float metal;
    private float levelMetal;
    private float rareMetal;
    private float LevelRareMetal;

    // Define events for score changes and obstacles destroyed by the player
    public static event Action<int> OnLevelMoneyChanged;
    public static event Action<int> OnMoneyChanged;
    public static event Action<float> OnMetalChanged;
    public static event Action<float> OnLevelMetalChanged;
    public static event Action<float> OnRareMetalChanged;
    public static event Action<float> OnLevelRareMetalChanged;

    public static event Action<int> OnObstaclesDestroyedByPlayerChanged;

    private void Awake()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;

        LoadData();
    }

    private void Start()
    {
        levelMoney = 0;
        levelMetal = 0;
        LevelRareMetal = 0;
    }

    private void OnEnable()
    {
        // Subscribe to obstacle events
        Obstacle.ObstacleEntersSceneEvent += OnObstacleEntersScene;
        Obstacle.ObstacleExitsSceneEvent += OnObstacleExitsScene;
        Loot.PlayerGainsLootEvent += OnPlayerGainsLoot;
        GameManager.GameManagerEndGameEvent += SaveData;
    }

    private void OnDisable()
    {
        // Unsubscribe from obstacle events
        Obstacle.ObstacleEntersSceneEvent -= OnObstacleEntersScene;
        Obstacle.ObstacleExitsSceneEvent -= OnObstacleExitsScene;
        Loot.PlayerGainsLootEvent -= OnPlayerGainsLoot;
        GameManager.GameManagerEndGameEvent -= SaveDataAtEndOfLevel;
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {;
        // Check if the scene index is within the bounds of the array
        if (scene.buildIndex == 0)
        {
            LoadData();
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
            killedByPlayerCount++;
            ChangeLevelMoney(pointValue);
            Debug.Log($"Money updated: {money}");

            // Trigger the obstacles destroyed by player event
            OnObstaclesDestroyedByPlayerChanged?.Invoke(killedByPlayerCount);
        }
    }

    private void OnPlayerGainsLoot(float metalGained, float rareMetalGained)
    {
        ChangeLevelMetal(metalGained);
        ChangeLevelRareMetal(rareMetalGained);
    }

    private void ChangeMoney(int points)
    {
        money += points;

        // Trigger the money change event
        OnMoneyChanged?.Invoke(money);
    }
    private void ChangeLevelMoney(int points)
    {
        levelMoney += points;

        // Trigger the levelMoney change event
        OnLevelMoneyChanged?.Invoke(levelMoney);
    }
    private void ChangeMetal(float points)
    {
        metal += points;

        OnMetalChanged?.Invoke(metal);

    }
    private void ChangeLevelMetal(float points)
    {
        levelMetal += points;
        OnLevelMetalChanged?.Invoke(levelMetal);
    }
    private void ChangeRareMetal(float points)
    {
        rareMetal += points;
        OnRareMetalChanged?.Invoke(rareMetal);

    }
    private void ChangeLevelRareMetal(float points)
    {
        LevelRareMetal += points;
        OnLevelRareMetalChanged?.Invoke(LevelRareMetal);
    }

    public float GetMetal()
    {
        return metal;
    }

    public float GetRareMetal()
    {
        return rareMetal;
    }

    public int GetMoney()
    {
        return money;
    }

    public int KilledByPlayerCount()
    {
        return killedByPlayerCount;
    }

    private void SaveDataAtEndOfLevel()
    {
        LoadData();
        ChangeMoney(levelMoney);
        ChangeMetal(levelMetal);
        ChangeRareMetal(LevelRareMetal);
        SaveData();
    }

    private void SaveData()
    {
        PlayerPrefs.SetInt("Money", money);
        PlayerPrefs.SetFloat("Metal", metal);
        PlayerPrefs.SetFloat("RareMetal", rareMetal);
        PlayerPrefs.Save();
    }

    private void LoadData()
    {
        money = PlayerPrefs.GetInt("Money", 0); // Default: 0 if not found
        metal = PlayerPrefs.GetFloat("Metal", 0f); // Default: 0
        rareMetal = PlayerPrefs.GetFloat("RareMetal", 0f); // Default: 0
    }
}