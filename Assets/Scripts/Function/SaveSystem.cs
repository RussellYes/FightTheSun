// SaveSystem.cs
using UnityEngine;
using System.IO;

public static class SaveSystem
{
    private static string savePath => Path.Combine(Application.persistentDataPath, "savegame.json");

    public static void SaveGame(GameData data)
    {
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(savePath, json);
        Debug.Log("SaveSystem - Saved to: " + savePath);
    }

    public static GameData LoadGame()
    {
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            return JsonUtility.FromJson<GameData>(json);
        }
        else
        {
            Debug.Log("SaveSystem - No save file found - creating new game");
            return new GameData(); // Returns fresh data if no save exists
        }
    }

    public static void DeleteSave()
    {
        if (File.Exists(savePath))
        {
            File.Delete(savePath);
            Debug.Log("SaveSystem - Save file deleted");
        }
        else
        {
            Debug.Log("SaveSystem - No save file to delete");
        }
    }

    public static void CleanCorruptedSaves()
    {
        if (SaveExists())
        {
            var data = LoadGame();
            SaveGame(data);
        }
    }

    public static bool SaveExists()
    {
        return File.Exists(savePath);
    }
}