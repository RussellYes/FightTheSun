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

    // Serialized level data list (for JSON serialization)
    public List<LevelDataEntry> serializedLevelData;

    // Level-specific data dictionary (level number as key)
    public Dictionary<int, LevelData> levelData;

    // Empty constructor for loading
    public GameData()
    {
        playerData = new List<PlayerSaveData>();
        levelData = new Dictionary<int, LevelData>();
        serializedLevelData = new List<LevelDataEntry>();
        totalMoney = 0;
        totalMetal = 0f;
        totalRareMetal = 0f;
        totalTime = 0f;
        totalObstaclesDestroyed = 0;
    }

    // Call this after deserialization to rebuild the dictionary
    public void RebuildDictionary()
    {
        levelData = new Dictionary<int, LevelData>();
        foreach (var entry in serializedLevelData)
        {
            levelData[entry.levelNumber] = entry.levelData;
        }
    }

    // Call this before serialization to update the list
    public void UpdateSerializedData()
    {
        serializedLevelData.Clear();
        foreach (var kvp in levelData)
        {
            serializedLevelData.Add(new LevelDataEntry(kvp.Key, kvp.Value));
        }
    }
}

// Helper class for dictionary serialization
[System.Serializable]
public class LevelDataEntry
{
    public int levelNumber;
    public LevelData levelData;

    public LevelDataEntry(int levelNumber, LevelData levelData)
    {
        this.levelNumber = levelNumber;
        this.levelData = levelData;
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