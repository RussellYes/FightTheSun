using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private ScoreManager scoreManager;

    public delegate void EndGameAction(bool isWin);
    public static event EndGameAction EndGameEvent;
    [SerializeField] private GameObject endConditionsUI;

    public delegate void SpawningAction();
    public static event SpawningAction StopSpawning;
    public static event SpawningAction StartSpawning;

    [SerializeField] private GameObject pauseMenuUI;
    [SerializeField] private Button pauseButton;
    private bool isPaused = false;

    private float gameTime;

    [SerializeField] private float goal; // Distance to the goal

    // Public property to access the goal
    public float Goal => goal;

    // Public property to access the game time
    public float GameTime => gameTime;

    private int currentMission; // Track the current mission number

    private void Awake()
    {
        pauseButton.onClick.AddListener(() => {
            PauseGame();
        });
    }

    private void Start()
    {
        scoreManager = FindAnyObjectByType<ScoreManager>();

        endConditionsUI.SetActive(false);
        pauseMenuUI.SetActive(false);
        gameTime = 0f; // Initialize game time

        // Set the current mission (e.g., from a mission selection menu or level loader)
        currentMission = 1; // Replace with dynamic mission number if needed
    }

    private void Update()
    {
        gameTime += Time.deltaTime; // Increment game time
    }

    public void StartSpawningTrigger()
    {
        StartSpawning?.Invoke();
    }

    public void StopSpawningTrigger()
    {
        StopSpawning?.Invoke();
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

    private void PauseGame()
    {
        if (!isPaused)
        {
            isPaused = true;

            // Show the pause menu
            pauseMenuUI.gameObject.SetActive(true);

            // Pause the game time
            Time.timeScale = 0;
        }
    }

    public void UnpauseGame()
    {
        if (isPaused)
        {
            isPaused = false;

            // Resume the game time
            Time.timeScale = 1;

            // Hide the pause menu
            pauseMenuUI.gameObject.SetActive(false);
        }
    }
}