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
            SetState(GameState.Paused);
        });


    }

    private void Start()
    {
        scoreManager = FindAnyObjectByType<ScoreManager>();

        // Log the initial state
        Debug.Log("Initial GameState: " + GameManager.Instance.CurrentState.ToString());

        endConditionsUI.SetActive(false);
        pauseMenuUI.SetActive(false);
        gameTime = 0f; // Initialize game time

        // Set the current mission (e.g., from a mission selection menu or level loader)
        CurrentMission = 1; // Replace with dynamic mission number if needed

        // Explicitly set the state to StartDialogue to trigger HandleStateChange
        SetState(GameState.StartDialogue);
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
        None,           // State 0: No state set    
        StartDialogue,  // State 1: Show dialogue boxes at the beginning
        Playing,        // State 2: Normal gameplay
        DialogueDuringPlay, // State 3: Show dialogue boxes during gameplay
        Paused,         // State 4: Paused (e.g., when settings menu is open)
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
            case GameState.Paused:
                HandlePaused();
                break;
            case GameState.DialogueDuringPlay:
                HandleDialogueDuringPlay();
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

        // Show dialogue boxes
        DialogueManager.Instance.MissionDialogue();
    }

    private void HandlePlaying()
    {
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

    private void HandleDialogueDuringPlay()
    {
        // Unause the game time
        Time.timeScale = 1;
        isPaused = false;

        // Start spawners
        StartSpawning?.Invoke();

        MusicManager.Instance.ResumeMusic();
        MusicManager.Instance.MuteMusic(false);
        SFXManager.Instance.MuteSFX(false);
        pauseMenuUI.gameObject.SetActive(false);

        // Show dialogue boxes
        DialogueManager.Instance.MissionDialogue();
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

        isPaused = false;

        // Set player speed to 25%
        ChangeThrottleEvent?.Invoke(-1f);

        // Stop spawners
        StopSpawning?.Invoke();

        // Stop goal progress
        StopGoalProgress();

        // Show dialogue boxes
        DialogueManager.Instance.MissionDialogue();
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
        SaveMissionData(CurrentMission, scoreManager.GetScore(), gameTime, scoreManager.KilledByPlayerCount());
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