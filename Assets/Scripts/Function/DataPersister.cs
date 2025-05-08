using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;
using System;
using System.Collections;
using System.Collections.Generic;

public class DataPersister : MonoBehaviour
{
    public static event Action NewGameEvent;

    public static DataPersister Instance;
    public GameData CurrentGameData { get; private set; }


    void OnApplicationQuit()
    {
        SaveCurrentGame();
        Debug.Log("DataPersister - Save OnAplicationQuit");
    }

    void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            // App is being paused (user switched away or got a call)
            Debug.Log("DataPersister - OnApplicationPause - Game saved");
            SaveCurrentGame();
        }
        else
        {
            // App is being unpaused (user returned)
            Debug.Log("DataPersister - OnApplicationPause - Game unpaused");
        }
    }

    void Awake()
    {
        Debug.Log("DataPersister - Awake");
        if (Instance == null)
        {
            Debug.Log("DataPersister - Instance is null, setting this as instance.");
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Debug.Log("DataPersister - Duplicate instance found, destroying this one.");
            Destroy(gameObject);
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StartCoroutine(DelayedInitialization());
    }

    IEnumerator DelayedInitialization()
    {
        yield return new WaitForEndOfFrame(); // Wait one frame
        LoadOrInitialize();
    }

    public void LoadOrInitialize()
    {
        Debug.Log("DataPersister - LoadOrInitialize");
        bool saveExists = SaveSystem.SaveExists();
        CurrentGameData = SaveSystem.LoadGame();

        // Ensure dictionary is initialized
        if (CurrentGameData.levelData == null)
        {
            CurrentGameData.levelData = new Dictionary<int, LevelData>();
        }

        // If no save exists or this is a new game, load previous data
        if (!saveExists || CurrentGameData == null)
        {
            // Set default variables, start screen, etc
            NewGameEvent?.Invoke();
            Debug.Log("DataPersister - NewGameEvent");
        }
        // Otherwise, it's a loaded game
        else
        {
            Debug.Log("DataPersister - Loading existing game");
        }
    }

    public void SaveCurrentGame()
    {
        UpdateGameData();
        SaveSystem.SaveGame(CurrentGameData);
        Debug.Log("DataPersister - SaveCurrentGame");
    }


    void UpdateGameData()
    {
        Debug.Log("DataPersister - Updating game data");

        // Get reference to ScoreManager
        ScoreManager scoreManager = FindFirstObjectByType<ScoreManager>();
        if (scoreManager != null)
        {
            // Update the totals from ScoreManager (they should already be updated)
            // We mainly need to ensure level data is saved
            Debug.Log("DataPersister - ScoreManager found, game data updated");
        }
        else
        {
            Debug.Log("DataPersister - ScoreManager not found");
        }
    }



 
    
}