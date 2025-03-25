using System;
using UnityEngine;
using UnityEngine.UIElements;

public class ScoreManager : MonoBehaviour
{
    private int totalObstaclesInScene;
    private int killedByPlayerCount;
    private int currentObstaclesInScene;
    public int money = 0;
    private float metal;
    private float rareMetal;

    // Define events for score changes and obstacles destroyed by the player
    public static event Action<int> OnScoreChanged;
    public static event Action<int> OnObstaclesDestroyedByPlayerChanged;

    private void Awake()
    {
        LoadData();
    }

    private void OnEnable()
    {
        // Subscribe to obstacle events
        Obstacle.ObstacleEntersSceneEvent += OnObstacleEntersScene;
        Obstacle.ObstacleExitsSceneEvent += OnObstacleExitsScene;
        Loot.PlayerGainsLootEvent += OnPlayerGainsLoot;
    }

    private void OnDisable()
    {
        // Unsubscribe from obstacle events
        Obstacle.ObstacleEntersSceneEvent -= OnObstacleEntersScene;
        Obstacle.ObstacleExitsSceneEvent -= OnObstacleExitsScene;
        Loot.PlayerGainsLootEvent -= OnPlayerGainsLoot;
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
            ChangeMoney(pointValue);
            Debug.Log($"Money updated: {money}");

            // Trigger the obstacles destroyed by player event
            OnObstaclesDestroyedByPlayerChanged?.Invoke(killedByPlayerCount);
        }
    }

    private void OnPlayerGainsLoot(float metalGained, float rareMetalGained)
    {
        metal += metalGained;
        rareMetal += rareMetalGained;
        SaveData();
    }

    private void ChangeMoney(int points)
    {
        money += points;

        // Trigger the score change event
        OnScoreChanged?.Invoke(money);
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