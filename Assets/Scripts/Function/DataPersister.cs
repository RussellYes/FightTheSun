using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;
using System;
using System.Collections;

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
            ApplyLoadedData();
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
        // This is an example of what to save to the game data:
        /*
        PlayerUI playerUI = FindFirstObjectByType<PlayerUI>();
        if (playerUI != null)
        {
            Debug.Log($"DataPersister - Saving Current playerName in UI: '{playerUI.playerName}'");
            Debug.Log($"DataPersister - Saving Current playerNameText value: '{playerUI.playerNameText?.text}'");

            CurrentGameData.playerName = playerUI.playerName;
            Debug.Log($"DataPersister - Saving name to GameData: '{CurrentGameData.playerName}'");
        }
        else
        {
            Debug.Log("DataPersister - PlayerUI not found, player name not saved.");
        }
        */


    


        CurrentGameData.playerData.Clear();
        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Interactable"))
        {
            //PlayerSaveData data = new PlayerSaveData

            // This is an example of what to save to the player data:
            /*
            CellPhone cellPhone = obj.GetComponent<CellPhone>();
            if (cellPhone != null)
            {
                data.cellPhoneCountdown = cellPhone.cellPhoneCountdown;
                data.cellPhoneRestCountdown = cellPhone.restCountdown;
                data.isTexting = cellPhone.isTexting;
                data.interactableType = "CellPhone";
                data.interactableID = cellPhone.InteractableID;
            }
            */
        }
    }

void ApplyLoadedData()
    {
        // This is an example of what to load from the game data:
        // Apply player data
        /*
        PlayerUI playerUI = FindFirstObjectByType<PlayerUI>();
        if (playerUI != null)
        {
            Debug.Log($"DataPersister - Loading player name: {CurrentGameData.playerName}");
            if (CurrentGameData.playerName != null)
            {
                PlayerLoadedNameEvent?.Invoke(CurrentGameData.playerName);
                Debug.Log("DataPersister - sending loaded name event");
            }
            else
            {
                Debug.LogWarning("DataPersister - Player name is null");
            }
        }
        */

       
    }




 
    
}