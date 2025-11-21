using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public class GameData
{
    [Header("Game State")]
    public bool hasLost; // Track if player has lost a game

    [Header("Mining Missile Data")]
    public float missileCost;
    public float missileDamageCost;
    public int missileCount;
    public float missileDamage;
    public int launcherLevel;
    public bool launcherActive;

    [Header("Shield Data")]
    public float shieldCost;
    public float shieldHpCost;
    public float shieldHp;
    public float shieldDamage;
    public float sheildRechargeRate;
    public int shieldCount;

    [Header("Nuke Data")]
    public int nukeCount;
    public float nukeCost;

    [Header("Game Data")]
    public List<PlayerSaveData> playerData;
    [SerializeField] private List<ComicDataEntry> _serializedComicData = new List<ComicDataEntry>();

    [Header("Score Manager Data")]
    // Persistent totals across all levels
    public float totalMoney;
    public float totalMetal;
    public float totalRareMetal;
    public float totalTime;
    public int totalObstaclesDestroyed;
    public int sunCount;
    public int comicNumbersLength;

    [Header("Investing")]
    // Company1 daily prices
    public float company1SundaySharePrice;
    public float company1MondaySharePrice;
    public float company1TuesdaySharePrice;
    public float company1WednesdaySharePrice;
    public float company1ThursdaySharePrice;
    public float company1FridaySharePrice;
    public float company1SaturdaySharePrice;
    // Company2 daily prices
    public float company2SundaySharePrice;
    public float company2MondaySharePrice;
    public float company2TuesdaySharePrice;
    public float company2WednesdaySharePrice;
    public float company2ThursdaySharePrice;
    public float company2FridaySharePrice;
    public float company2SaturdaySharePrice;
    // Company3 daily prices
    public float company3SundaySharePrice;
    public float company3MondaySharePrice;
    public float company3TuesdaySharePrice;
    public float company3WednesdaySharePrice;
    public float company3ThursdaySharePrice;
    public float company3FridaySharePrice;
    public float company3SaturdaySharePrice;
    // Company4 daily prices
    public float company4SundaySharePrice;
    public float company4MondaySharePrice;
    public float company4TuesdaySharePrice;
    public float company4WednesdaySharePrice;
    public float company4ThursdaySharePrice;
    public float company4FridaySharePrice;
    public float company4SaturdaySharePrice;
    // Company5 daily prices
    public float company5SundaySharePrice;
    public float company5MondaySharePrice;
    public float company5TuesdaySharePrice;
    public float company5WednesdaySharePrice;
    public float company5ThursdaySharePrice;
    public float company5FridaySharePrice;
    public float company5SaturdaySharePrice;
    // Shares owned in each company
    public float company1Shares;
    public float company2Shares;
    public float company3Shares;
    public float company4Shares;
    public float company5Shares;
    // Dividend
    public float dailyDividendCount;

    [Header("Time Data")]
    public float appMessageSavedMonth;
    public float investingDayOfTheWeek;
    public float investingSavedTime;

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

    public bool hasOpenedComics;
    public bool hasOpenedShipUpgrades;
    public bool hasOpenedInvesting;

    [Header("Level Unlocks")]
    public bool isMission1Unlocked;
    public bool isMission2Unlocked;
    public bool isMission3Unlocked;
    public bool isMission4Unlocked;
    public bool isMission5Unlocked;
    public bool isMission6Unlocked;
    public bool isMission7Unlocked;
    public bool isMission8Unlocked;
    public bool isMission9Unlocked;
    public bool isMission10Unlocked;

    // Serialized level data list (for JSON serialization)
    public List<LevelDataEntry> serializedLevelData;

    // Level-specific data dictionary (level number as key)
    public Dictionary<int, LevelData> levelData;

    public Dictionary<float, ComicData> comicData;

    // Empty constructor for loading
    public GameData()
    {
        playerData = new List<PlayerSaveData>();
        // Initialize first player data with default values
        playerData.Add(new PlayerSaveData()
        {
            playerMemoryScore = 0,
            engineeringSkill = 1,
            pilotingSkill = 1,
            mechanicsSkill = 1,
            miningSkill = 1,
            roboticsSkill = 1,
            combatSkill = 1
        });

        comicData = new Dictionary<float, ComicData>();
        levelData = new Dictionary<int, LevelData>();
        serializedLevelData = new List<LevelDataEntry>();
        totalMoney = 0;
        totalMetal = 0f;
        totalRareMetal = 0f;
        totalTime = 0f;
        totalObstaclesDestroyed = 0;
        sunCount = 0;

        // Investing
        // Company1 daily prices
        company1SundaySharePrice = 0;
        company1MondaySharePrice = 0;
        company1TuesdaySharePrice = 0;
        company1WednesdaySharePrice = 0;
        company1ThursdaySharePrice = 0;
        company1FridaySharePrice = 0;
        company1SaturdaySharePrice = 0;
        // Company2 daily prices
        company2SundaySharePrice = 0;
        company2MondaySharePrice = 0;
        company2TuesdaySharePrice = 0;
        company2WednesdaySharePrice = 0;
        company2ThursdaySharePrice = 0;
        company2FridaySharePrice = 0;
        company2SaturdaySharePrice = 0;
        // Company3 daily prices
        company3SundaySharePrice = 0;
        company3MondaySharePrice = 0;
        company3TuesdaySharePrice = 0;
        company3WednesdaySharePrice = 0;
        company3ThursdaySharePrice = 0;
        company3FridaySharePrice = 0;
        company3SaturdaySharePrice = 0;
        // Company4 daily prices
        company4SundaySharePrice = 0;
        company4MondaySharePrice = 0;
        company4TuesdaySharePrice = 0;
        company4WednesdaySharePrice = 0;
        company4ThursdaySharePrice = 0;
        company4FridaySharePrice = 0;
        company4SaturdaySharePrice = 0;
        // Company5 daily prices
        company5SundaySharePrice = 0;
        company5MondaySharePrice = 0;
        company5TuesdaySharePrice = 0;
        company5WednesdaySharePrice = 0;
        company5ThursdaySharePrice = 0;
        company5FridaySharePrice = 0;
        company5SaturdaySharePrice = 0;
        // Shares owned in each company
        company1Shares = 0;
        company2Shares = 0;
        company3Shares = 0;
        company4Shares = 0;
        company5Shares = 0;
        // Dividend
        dailyDividendCount = 3;

        // Time Data
        appMessageSavedMonth = 0;
        investingDayOfTheWeek = 0;
        investingSavedTime = 0;

        // Missile Launcher
        missileDamageCost = 1000f;
        missileCost = 200f;
        missileCount = 0;
        missileDamage = 10f;
        launcherLevel = 1;
        // Shield
        shieldCost = 500f;
        shieldHpCost = 2500f;
        shieldHp = 100;
        shieldDamage = 1;
        sheildRechargeRate = 60;
        shieldCount = 0;
        // Nuke
        nukeCount = 0;
        nukeCost = 12000f;

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

        hasOpenedComics = false;
        hasOpenedShipUpgrades = false;
        hasOpenedInvesting = false;

        isMission1Unlocked = true; //Level 1 is always unlocked.
        isMission2Unlocked = false;
        isMission3Unlocked = false;
        isMission4Unlocked = false;
        isMission5Unlocked = false;
        isMission6Unlocked = false;
        isMission7Unlocked = false;
        isMission8Unlocked = false;
        isMission9Unlocked = false;
        isMission10Unlocked = false;
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

    // Achievement Complete Status
    public bool GetMissionComplete(int missionNumber)
    {
        return missionNumber switch
        {
            1 => isMission1Complete,
            2 => isMission2Complete,
            3 => isMission3Complete,
            4 => isMission4Complete,
            5 => isMission5Complete,
            6 => isMission6Complete,
            7 => isMission7Complete,
            8 => isMission8Complete,
            9 => isMission9Complete,
            10 => isMission10Complete,
            _ => false
        };
    }



    public void SetMissionComplete(int missionNumber, bool value)
    {
        switch (missionNumber)
        {
            case 1: isMission1Complete = value; break;
            case 2: isMission2Complete = value; break;
            case 3: isMission3Complete = value; break;
            case 4: isMission4Complete = value; break;
            case 5: isMission5Complete = value; break;
            case 6: isMission6Complete = value; break;
            case 7: isMission7Complete = value; break;
            case 8: isMission8Complete = value; break;
            case 9: isMission9Complete = value; break;
            case 10: isMission10Complete = value; break;
        }
    }

    // Level Unlock Status
    public bool GetMissionUnlocked(int missionNumber)
    {
        return missionNumber switch
        {
            1 => isMission1Unlocked,
            2 => isMission2Unlocked,
            3 => isMission3Unlocked,
            4 => isMission4Unlocked,
            5 => isMission5Unlocked,
            6 => isMission6Unlocked,
            7 => isMission7Unlocked,
            8 => isMission8Unlocked,
            9 => isMission9Unlocked,
            10 => isMission10Unlocked,
            _ => false
        };
    }

    public void SetMissionUnlocked(int missionNumber, bool value)
    {
        switch (missionNumber)
        {
            case 1: isMission1Unlocked = value; break;
            case 2: isMission2Unlocked = value; break;
            case 3: isMission3Unlocked = value; break;
            case 4: isMission4Unlocked = value; break;
            case 5: isMission5Unlocked = value; break;
            case 6: isMission6Unlocked = value; break;
            case 7: isMission7Unlocked = value; break;
            case 8: isMission8Unlocked = value; break;
            case 9: isMission9Unlocked = value; break;
            case 10: isMission10Unlocked = value; break;
        }
    }

    public void ResetDataOnNewGame()
    {
        // Reset game state
        hasLost = false;

        // Reset player data
        playerData.Clear();
        playerData.Add(new PlayerSaveData()
        {
            playerMemoryScore = 0,
            engineeringSkill = 1,
            pilotingSkill = 1,
            mechanicsSkill = 1,
            miningSkill = 1,
            roboticsSkill = 1,
            combatSkill = 1
        });

        // Reset level data
        levelData.Clear();
        serializedLevelData.Clear();

        // Reset comic data
        comicData.Clear();

        // Reset resource totals
        totalMoney = 0;
        totalMetal = 0f;
        totalRareMetal = 0f;
        totalTime = 0f;
        totalObstaclesDestroyed = 0;
        sunCount = 0;

        // Investing
        // Company1 daily prices
        company1SundaySharePrice = 0;
        company1MondaySharePrice = 0;
        company1TuesdaySharePrice = 0;
        company1WednesdaySharePrice = 0;
        company1ThursdaySharePrice = 0;
        company1FridaySharePrice = 0;
        company1SaturdaySharePrice = 0;
        // Company2 daily prices
        company2SundaySharePrice = 0;
        company2MondaySharePrice = 0;
        company2TuesdaySharePrice = 0;
        company2WednesdaySharePrice = 0;
        company2ThursdaySharePrice = 0;
        company2FridaySharePrice = 0;
        company2SaturdaySharePrice = 0;
        // Company3 daily prices
        company3SundaySharePrice = 0;
        company3MondaySharePrice = 0;
        company3TuesdaySharePrice = 0;
        company3WednesdaySharePrice = 0;
        company3ThursdaySharePrice = 0;
        company3FridaySharePrice = 0;
        company3SaturdaySharePrice = 0;
        // Company4 daily prices
        company4SundaySharePrice = 0;
        company4MondaySharePrice = 0;
        company4TuesdaySharePrice = 0;
        company4WednesdaySharePrice = 0;
        company4ThursdaySharePrice = 0;
        company4FridaySharePrice = 0;
        company4SaturdaySharePrice = 0;
        // Company5 daily prices
        company5SundaySharePrice = 0;
        company5MondaySharePrice = 0;
        company5TuesdaySharePrice = 0;
        company5WednesdaySharePrice = 0;
        company5ThursdaySharePrice = 0;
        company5FridaySharePrice = 0;
        company5SaturdaySharePrice = 0;
        // Shares owned in each company
        company1Shares = 0;
        company2Shares = 0;
        company3Shares = 0;
        company4Shares = 0;
        company5Shares = 0;
        // Dividend
        dailyDividendCount = 3;

        // Time Data
        appMessageSavedMonth = 0;
        investingDayOfTheWeek = 0;
        investingSavedTime = 0;

        // Missile Launcher
        missileDamageCost = 1000f;
        missileCost = 200f;
        missileCount = 0;
        missileDamage = 10f;
        launcherLevel = 1;
        // Shield
        shieldCost = 500f;
        shieldHpCost = 2500f;
        shieldHp = 100;
        shieldDamage = 1;
        sheildRechargeRate = 60;
        shieldCount = 0;
        // Nuke
        nukeCount = 0;
        nukeCost = 12000f;

        // Reset achievements
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

        hasOpenedComics = false;
        hasOpenedShipUpgrades = false;
        hasOpenedInvesting = false;

        // Reset level unlocks
        isMission1Unlocked = true; // Level 1 is always unlocked
        isMission2Unlocked = false;
        isMission3Unlocked = false;
        isMission4Unlocked = false;
        isMission5Unlocked = false;
        isMission6Unlocked = false;
        isMission7Unlocked = false;
        isMission8Unlocked = false;
        isMission9Unlocked = false;
        isMission10Unlocked = false;
    }

    public void PrepareComicsForSaving()
    {
        _serializedComicData.Clear();
        foreach (var kvp in comicData)
        {
            _serializedComicData.Add(new ComicDataEntry(kvp.Key, kvp.Value));
        }
    }

    public void RebuildComicDictionary()
    {
        comicData.Clear();
        foreach (var entry in _serializedComicData)
        {
            comicData[entry.comicNumber] = entry.comicData;
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
    public float engineeringSkill;
    public float pilotingSkill;
    public float mechanicsSkill;
    public float miningSkill;
    public float roboticsSkill;
    public float combatSkill;
}

[System.Serializable]

public class ComicData
{
    public float comicNumber;
    public bool isUnlocked;
    public ComicData(float number, bool unlocked = false)
    {
        comicNumber = number;
        isUnlocked = unlocked;
    }
}

[System.Serializable]
public class ComicDataEntry
{
    public float comicNumber;
    public ComicData comicData;

    public ComicDataEntry(float number, ComicData data)
    {
        comicNumber = number;
        comicData = data;
    }
}