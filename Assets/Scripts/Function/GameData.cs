using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    [Header("Game Data")]
    // Type variables in this section to save as GameData such as:
    //" public float level; "

    public List<PlayerSaveData> playerData;

    // Empty constructor for loading
    public GameData()
    {
        playerData = new List<PlayerSaveData>();
    }
}

[System.Serializable]
public class PlayerSaveData
{
    [Header("Player Data")]
    // Type variables in this section to save as GameData such as:
    //" public float playerName; "

    public float playerMemoryScore;
}