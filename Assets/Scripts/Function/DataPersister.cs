using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;
using System;
using System.Collections;
using System.Collections.Generic;

public class DataPersister : MonoBehaviour
{
    public static event Action InitializationComplete;

    public static DataPersister Instance;
    public GameData CurrentGameData { get; private set; }

    private void OnEnable()
    {
        MainMenuUI.NewGameEvent += StartNewGame;
    }
    private void OnDisable()
    {
        MainMenuUI.NewGameEvent -= StartNewGame;
    }
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
        bool saveExists = SaveSystem.SaveExists();
        Debug.Log($"DataPersister - LoadOrInitialize - Save exists: {saveExists}");

        CurrentGameData = SaveSystem.LoadGame();
        Debug.Log($"DataPersister - Loaded GameData: {CurrentGameData != null}");

        if (CurrentGameData == null)
        {
            Debug.Log("DataPersister - Creating new GameData");
            CurrentGameData = new GameData();
        }

        // Ensure all dictionaries and lists are initialized
        CurrentGameData.levelData ??= new Dictionary<int, LevelData>();
        CurrentGameData.playerData ??= new List<PlayerSaveData>();
        CurrentGameData.comicData ??= new Dictionary<float, ComicData>();
        CurrentGameData.serializedLevelData ??= new List<LevelDataEntry>();

        // If no save exists or this is a new game, load previous data
        if (!saveExists || CurrentGameData == null)
        {
            // Set default variables, start screen, etc
            StartNewGame();
            Debug.Log("DataPersister - NewGameEvent");
        }
        // Otherwise, it's a loaded game
        else
        {
            Debug.Log("DataPersister - Loading existing game");
        }
        InitializationComplete?.Invoke();
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

        // Update player stats if PlayerStatsManager exists
        PlayerStatsManager playerStats = FindFirstObjectByType<PlayerStatsManager>();
        if (playerStats != null)
        {
            // Ensure player data exists
            if (CurrentGameData.playerData == null)
            {
                CurrentGameData.playerData = new List<PlayerSaveData>();
            }
            if (CurrentGameData.playerData.Count == 0)
            {
                CurrentGameData.playerData.Add(new PlayerSaveData());
            }

            // Update all skills in playerData
            CurrentGameData.playerData[0].engineeringSkill = playerStats.EngineeringSkill;
            CurrentGameData.playerData[0].pilotingSkill = playerStats.PilotingSkill;
            CurrentGameData.playerData[0].mechanicsSkill = playerStats.MechanicsSkill;
            CurrentGameData.playerData[0].miningSkill = playerStats.MiningSkill;
            CurrentGameData.playerData[0].roboticsSkill = playerStats.RoboticsSkill;
            CurrentGameData.playerData[0].combatSkill = playerStats.CombatSkill;
        }

        ShipUpgradesUI shipUpgrades = FindFirstObjectByType<ShipUpgradesUI>();
        if (shipUpgrades != null)
        {
            // Ensure player data exists
            if (CurrentGameData.playerData.Count == 0)
            {
                CurrentGameData.playerData.Add(new PlayerSaveData());
            }
            CurrentGameData.playerData[0].playerMemoryScore = shipUpgrades.GetMemoryScore();
        }

        // Get reference to ScoreManager
        ScoreManager scoreManager = FindFirstObjectByType<ScoreManager>();
        if (scoreManager != null)
        {
            Debug.Log("DataPersister - ScoreManager found, game data updated");
        }
        else
        {
            Debug.Log("DataPersister - ScoreManager not found");
        }
    }


    public void StartNewGame()
    {
        Debug.Log("DataPersister - Starting new game");

        CurrentGameData = new GameData();

        CurrentGameData.ResetDataOnNewGame();

        SaveSystem.DeleteSave();

        SaveCurrentGame();
    }


}