using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    [Header("Game Data")]
    public List<PlayerSaveData> playerData;

    [Header("Score Manager Data")]
    // Persistent totals across all levels
    public float totalMoney;
    public float totalMetal;
    public float totalRareMetal;
    public float totalTime;
    public int totalObstaclesDestroyed;

    // Level-specific data dictionary (level number as key)
    public Dictionary<int, LevelData> levelData;

    // Empty constructor for loading
    public GameData()
    {
        playerData = new List<PlayerSaveData>();
        levelData = new Dictionary<int, LevelData>();
        totalMoney = 0;
        totalMetal = 0f;
        totalRareMetal = 0f;
        totalTime = 0f;
        totalObstaclesDestroyed = 0;
    }
}

[System.Serializable]
public class LevelData
{
    public float levelTime;
    public float levelMoney;
    public int levelObstaclesDestroyed;

    public LevelData(float time, float money, int obstacles)
    {
        levelTime = time;
        levelMoney = money;
        levelObstaclesDestroyed = obstacles;
    }
}

[System.Serializable]
public class PlayerSaveData
{
    [Header("Player Data")]
    public float playerMemoryScore;
}