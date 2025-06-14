using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// This script controls the game states that trigger goals, obstacle spawning, pausing, and end UI.

public class GameManager : MonoBehaviour
{
    private ScoreManager scoreManager;
    public static GameManager Instance;

    public GameState CurrentState { get; private set; }

    public delegate void EndGameAction(bool isWin);
    public static event EndGameAction EndGameEvent;
    public static event Action EndGameDataSaveEvent;
    public static event Action <float> ChangeThrottleEvent;
    [SerializeField] private GameObject endConditionsUI;

    public delegate void SpawningAction();
    public static event SpawningAction StopSpawning;
    public static event SpawningAction StartSpawning;

    [SerializeField] private GameObject pauseMenuUI;
    [SerializeField] private Button pauseButton;
    [SerializeField] private Button unpauseButton;

    private float gameTime;

    [SerializeField] private float goal; // Distance to the goal
    private bool isGoalActive = false;
    private bool isBossBattle = false;



    // Public read-only property
    public bool IsGoalActive => isGoalActive;

    // Public property to access the goal
    public float Goal => goal;

    // Public property to access the game time
    public float GameTime => gameTime;



    // Track the current mission number
    public int CurrentMission { get; private set; } // Make CurrentMission public



    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        pauseButton.onClick.AddListener(() => {
            HandlePaused();
        });
        unpauseButton.onClick.AddListener(() => {
            HandleUnpaused();
        });


    }

    private void Start()
    {
        scoreManager = FindAnyObjectByType<ScoreManager>();

        // Log the initial state
        Debug.Log("Initial GameState: " + GameManager.Instance.CurrentState.ToString());

        endConditionsUI.SetActive(false);

        // Set the current mission based on the scene
        SetMissionBasedOnScene();

        // Explicitly set the state to StartDialogue to trigger HandleStateChange
        SetState(GameState.StartDialogue);
    }

    private void OnEnable()
    {
        Boss.StartSpawnersEvent += StartSpawners;
        Boss.StopSpawnersEvent += StopSpawners;
    }

    private void OnDisable()
    {
        Boss.StartSpawnersEvent -= StartSpawners;
        Boss.StopSpawnersEvent -= StopSpawners;
    }

    private void Update()
    {
        if (isGoalActive || isBossBattle)
        {
            gameTime += Time.deltaTime; // Increment game time
        }
    }

    private void SetMissionBasedOnScene()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

        // Map scene indices to missions
        switch (currentSceneIndex)
        {
            case 2: // Mission1Scene corresponds to Mission 1
                CurrentMission = 1;
                break;
            case 3: // Mission2Scene corresponds to Mission 2
                CurrentMission = 2;
                break;
            case 4: // Mission3Scene corresponds to Mission 3
                CurrentMission = 3;
                break;
            case 5: // Mission4Scene corresponds to Mission 4
                CurrentMission = 4;
                break;
            case 6: // Mission5Scene corresponds to Mission 5
                CurrentMission = 5;
                break;
            case 7: // Mission6Scene corresponds to Mission 6
                CurrentMission = 6;
                break;
            case 8: // Mission7Scene corresponds to Mission 7
                CurrentMission = 7;
                break;
            case 9: // Mission8Scene corresponds to Mission 8
                CurrentMission = 8;
                break;
            case 10: // Mission9Scene corresponds to Mission 9
                CurrentMission = 9;
                break;
            case 11: // Mission10Scene corresponds to Mission 10
                CurrentMission = 10;
                break;
            default:
                // Suppress warning for non-mission scenes (e.g., MainMenuScene, LoadingScene)
                if (currentSceneIndex != 0 && currentSceneIndex != 1)
                {
                    Debug.LogWarning($"No mission mapped for scene index {currentSceneIndex}. Defaulting to Mission 1.");
                }
                CurrentMission = 1;
                break;
        }

        Debug.Log($"Scene {currentSceneIndex} loaded. Current Mission set to {CurrentMission}.");
    }

    public enum GameState
    {
        None,           // State 0: No state set    
        StartDialogue,  // State 1: Show dialogue boxes at the beginning
        Playing,        // State 2: Normal gameplay
        DialogueDuringPlay, // State 3: Show dialogue boxes during gameplay
        BossBattle,     // State 4: Boss battle
        EndDialogue,    // State 5: Show dialogue boxes at the end
        EndUI           // State 6: End UI active, game paused
    }

    public void SetState(GameState newState)
    {
        // Prevent setting the same state repeatedly
        if (CurrentState == newState)
        {
            Debug.Log($"Already in state: {newState}");
            return;
        }

        Debug.Log($"Transitioning from {CurrentState} to {newState}");
        CurrentState = newState;
        HandleStateChange();
    }

    private void HandleStateChange()
    {
        switch (CurrentState)
        {
            case GameState.StartDialogue:
                HandleStartDialogue();
                break;
            case GameState.Playing:
                HandlePlaying();
                break;
            case GameState.BossBattle:
                HandleBossBattle();
                break;
            case GameState.EndDialogue:
                HandleEndDialogue();
                break;
            case GameState.EndUI:
                HandleEndUI();
                break;
        }
    }

    private void HandleStartDialogue()
    {
        // Unause the game time
        Time.timeScale = 1;

        // Set player speed to 25%
        ChangeThrottleEvent?.Invoke(-0.75f);

        // Stop spawners
        StopSpawning?.Invoke();

        // Stop goal progress
        StopGoalProgress();

        // Show dialogue boxes
        DialogueManager.Instance.MissionDialogue();
    }

    private void HandlePlaying()
    {
        Debug.Log("GameManager HandlePlaying");
        // Unause the game time
        Time.timeScale = 1;

        // Set player speed to 100%
        ChangeThrottleEvent?.Invoke(1f);

        // Start spawners
        StartSpawning?.Invoke();

        // Start goal progress
        StartGoalProgress();

        MusicManager.Instance.ResumeMusic();
        MusicManager.Instance.MuteMusic(false);
        SFXManager.Instance.MuteSFX(false);

        pauseMenuUI.gameObject.SetActive(false);
    }
    private void HandlePaused()
    {
        MusicManager.Instance.PauseMusic();
        MusicManager.Instance.MuteMusic(true);
        SFXManager.Instance.MuteSFX(true);

        // Show the pause menu
        pauseMenuUI.gameObject.SetActive(true);

        // Pause the game time
        Time.timeScale = 0;

        // Stop spawners
        StopSpawning?.Invoke();

        // Stop goal progress
        StopGoalProgress();
    }

    private void HandleUnpaused()
    {
        // Stop the sample music and resume the original music
        MusicManager.Instance.StopMusic();

        // Get the original music clip from PauseMenuUI
        AudioClip originalMusicClip = PauseMenuUI.Instance.GetOriginalMusicClip();
        if (originalMusicClip != null)
        {
            MusicManager.Instance.PlayMusic(originalMusicClip);
        }

        MusicManager.Instance.MuteMusic(false);
        SFXManager.Instance.MuteSFX(false);

        // Show the pause menu
        pauseMenuUI.gameObject.SetActive(false);

        // Pause the game time
        Time.timeScale = 1;

        if (CurrentState != GameState.BossBattle)
        {
            // Stop spawners
            StartSpawning?.Invoke();

            // Stop goal progress
            StartGoalProgress();
        }

    }

    private void HandleBossBattle()
    {
        Debug.Log("GameManager HandleBossBattle");

        // Stop spawners
        StopSpawning?.Invoke();

        isBossBattle = true;

        // Stop goal progress
        StopGoalProgress();
    }

    private void HandleEndDialogue()
    {
        // Stop spawners
        StopSpawning?.Invoke();

        isBossBattle = false;

        // Stop goal progress
        StopGoalProgress();

        StartCoroutine(WaitToClearPlayArea());
    }

    IEnumerator WaitToClearPlayArea()
    {
        //Wait for a few seconds to let the game area clear obstacles
        yield return new WaitForSeconds(2);

        DialogueManager.Instance.MissionDialogue();

        // Set player speed to 25%
        ChangeThrottleEvent?.Invoke(-1f);
    }
    private void StopGoalProgress()
    {
        isGoalActive = false;
    }
    private void StartGoalProgress()
    {
        isGoalActive = true;
    }

    public void EndGame(bool isWin)
    {
        Debug.Log($"EndGame called with isWin = {isWin}");

        SetState(GameState.EndUI);

        // Trigger the EndGameEvent
        EndGameEvent?.Invoke(isWin);
        EndGameDataSaveEvent?.Invoke();

        // Stop all spawning
        StopSpawning?.Invoke();

        // Pause the game time
        Time.timeScale = 0;
    }

    private void HandleEndUI()
    {
        Debug.Log("GameManager HandleEndUI");

        // Stop goal progress
        StopGoalProgress();

        // Show end UI
        endConditionsUI.SetActive(true); // Activate the end conditions UI GameObject
    }
    private void StartSpawners()
    {
        StartSpawning?.Invoke();  
    }

    private void StopSpawners()
    {
        StopSpawning?.Invoke();
    }


}