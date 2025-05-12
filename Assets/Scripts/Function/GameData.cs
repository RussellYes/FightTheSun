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

    [Header("Achievements")]
    public bool isMission1Complete;
    public bool isMission2Complete;
    public bool isMission3Complete;
    public bool isMission4Complete;
    public bool isMission5Complete;
    public bool isMission6Complete;
    public bool isMission7Complete;
    public bool isMission8Complete;
    public bool isMission9Complete;
    public bool isMission10Complete;

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

        isMission1Complete = false;
        isMission2Complete = false;
        isMission3Complete = false;
        isMission4Complete = false;
        isMission5Complete = false;
        isMission6Complete = false;
        isMission7Complete = false;
        isMission8Complete = false;
        isMission9Complete = false;
        isMission10Complete = false;

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