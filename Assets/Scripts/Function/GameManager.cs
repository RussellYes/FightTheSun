using System;
using UnityEditor;
using UnityEditor.EditorTools;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private ScoreManager scoreManager;
    public static GameManager Instance;

    public GameState CurrentState { get; private set; }

    public delegate void EndGameAction(bool isWin);
    public static event EndGameAction EndGameEvent;
    public static event Action <float> ChangeThrottleEvent;
    [SerializeField] private GameObject endConditionsUI;

    public delegate void SpawningAction();
    public static event SpawningAction StopSpawning;
    public static event SpawningAction StartSpawning;

    [SerializeField] private GameObject ShipHUDUI;
    [SerializeField] private GameObject pauseMenuUI;
    [SerializeField] private Button pauseButton;

    private bool isPaused = false;

    private float gameTime;

    [SerializeField] private float goal; // Distance to the goal
    private bool isGoalActive = false;

    // Public read-only property
    public bool IsGoalActive => isGoalActive;

    // Public property to access the goal
    public float Goal => goal;

    // Public property to access the game time
    public float GameTime => gameTime;



    // Track the current mission number
    private int currentMission;



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
            SetState(GameState.Paused);
        });
    }

    private void Start()
    {
        scoreManager = FindAnyObjectByType<ScoreManager>();


        SetState(GameState.Playing);
        if (CurrentState != GameState.StartDialogue)
        {
            SetState(GameState.StartDialogue);
        }
           


            endConditionsUI.SetActive(false);
        pauseMenuUI.SetActive(false);
        gameTime = 0f; // Initialize game time

        // Set the current mission (e.g., from a mission selection menu or level loader)
        currentMission = 1; // Replace with dynamic mission number if needed
    }

    private void Update()
    {
        if (isGoalActive)
        {
            gameTime += Time.deltaTime; // Increment game time
        }
        
    }

    public enum GameState
    {
        StartDialogue,  // State 1: Show dialogue boxes at the beginning
        Playing,        // State 2: Normal gameplay
        Paused,         // State 3: Paused (e.g., when settings menu is open)
        EndDialogue,    // State 4: Show dialogue boxes at the end
        EndUI           // State 5: End UI active, game paused
    }

    public void SetState(GameState newState)
    {
        // Prevent setting the same state repeatedly
        if (CurrentState == newState)
        {
            Debug.LogWarning($"Already in state: {newState}");
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
            case GameState.Paused:
                HandlePaused();
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
        isPaused = false;

        // Set player speed to 25%
        ChangeThrottleEvent?.Invoke(-0.75f);

        // Stop spawners
        StopSpawning?.Invoke();

        // Stop goal progress
        StopGoalProgress();

        //Hide Ship HUD
        ShipHUDUI.SetActive(false);

        // Show dialogue boxes
        DialogueManager.Instance.ShowDialogue("StartDialogue");
    }

    private void HandlePlaying()
    {
        ShipHUDUI.SetActive(true);

        // Unause the game time
        Time.timeScale = 1;

        isPaused = false;

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
        isPaused = true;

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
        if (!isPaused)
        {
            





        }
        /*
        if (isPaused)
        {
            isPaused = false;
            MusicManager.Instance.ResumeMusic();
            MusicManager.Instance.MuteMusic(false);
            SFXManager.Instance.MuteSFX(false);
            // Resume the game time
            Time.timeScale = 1;
            // Hide the pause menu
            pauseMenuUI.gameObject.SetActive(false);
        }
        */
    }

    private void HandleEndDialogue()
    {
        ShipHUDUI.SetActive(false);

        isPaused = false;

        // Set player speed to 25%
        ChangeThrottleEvent?.Invoke(-1f);

        // Stop spawners
        StopSpawning?.Invoke();

        // Stop goal progress
        StopGoalProgress();

        // Show dialogue boxes
        DialogueManager.Instance.ShowDialogue("EndDialogue");
    }

    private void HandleEndUI()
    {
        isPaused = true;

        // Stop spawners
        StopSpawning?.Invoke();

        // Stop goal progress
        StopGoalProgress();

        // Show end UI
        endConditionsUI.SetActive(true); // Activate the end conditions UI GameObject


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
        // Activate the end conditions UI
        endConditionsUI.SetActive(true);

        // Trigger the EndGameEvent
        EndGameEvent?.Invoke(isWin);

        // Stop all spawning
        StopSpawning?.Invoke();

        // Pause the game time
        Time.timeScale = 0;

        // Save the high score, best time, and obstacles destroyed for the current mission
        SaveMissionData(currentMission, scoreManager.GetScore(), gameTime, scoreManager.KilledByPlayerCount());
    }

    private void SaveMissionData(int missionNumber, int score, float time, int obstaclesDestroyed)
    {
        // Create unique keys for each mission
        string scoreKey = $"Mission{missionNumber}BestScore";
        string timeKey = $"Mission{missionNumber}BestTime";
        string obstaclesKey = $"Mission{missionNumber}BestObstaclesDestroyed";

        // Load existing best values
        int bestScore = PlayerPrefs.GetInt(scoreKey, 0);
        float bestTime = PlayerPrefs.GetFloat(timeKey, float.MaxValue);
        int bestObstacles = PlayerPrefs.GetInt(obstaclesKey, 0);

        // Update best values if the current values are better
        if (score > bestScore)
        {
            PlayerPrefs.SetInt(scoreKey, score);
        }
        if (time < bestTime)
        {
            PlayerPrefs.SetFloat(timeKey, time);
        }
        if (obstaclesDestroyed > bestObstacles)
        {
            PlayerPrefs.SetInt(obstaclesKey, obstaclesDestroyed);
        }

        PlayerPrefs.Save(); // Save the data
    }
}