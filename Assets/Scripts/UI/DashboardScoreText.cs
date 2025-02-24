using TMPro;
using UnityEngine;

public class DashboardScoreText : MonoBehaviour
{
    private ScoreManager scoreManager;

    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private TextMeshProUGUI dashboardScoreText;
    [SerializeField] private TextMeshProUGUI obstaclesDestroyedText;

    private float elapsedTime = 0f;
    private bool isGameRunning = true;

    private void OnEnable()
    {
        // Subscribe to score and obstacles destroyed events
        ScoreManager.OnScoreChanged += UpdateScoreText;
        ScoreManager.OnObstaclesDestroyedByPlayerChanged += UpdateObstaclesDestroyedText;
    }

    private void OnDisable()
    {
        // Unsubscribe from events
        ScoreManager.OnScoreChanged -= UpdateScoreText;
        ScoreManager.OnObstaclesDestroyedByPlayerChanged -= UpdateObstaclesDestroyedText;
    }

    private void Start()
    {
        // Initialize the UI elements
        scoreManager = FindObjectOfType<ScoreManager>();
        if (scoreManager != null)
        {
            UpdateScoreText(scoreManager.GetScore());
            UpdateObstaclesDestroyedText(scoreManager.KilledByPlayerCount());
        }

        // Start the timer
        elapsedTime = 0f;
        isGameRunning = true;
    }

    private void Update()
    {
        if (isGameRunning)
        {
            // Update the elapsed time
            elapsedTime += Time.deltaTime;
            UpdateTimeText(elapsedTime);
        }
    }

    private void UpdateTimeText(float timeInSeconds)
    {
        if (timeText != null)
        {
            // Format the time as minutes:seconds
            int minutes = Mathf.FloorToInt(timeInSeconds / 60);
            int seconds = Mathf.FloorToInt(timeInSeconds % 60);
            timeText.text = $"Time: {minutes:00}:{seconds:00}";
        }
        else
        {
            Debug.LogError("timeText is not found by DashboardUI");
        }
    }

    private void UpdateScoreText(int newScore)
    {
        if (dashboardScoreText != null)
        {
            dashboardScoreText.text = $"Score: {newScore}";
        }
        else
        {
            Debug.LogError("dashboardScoreText is not found by DashboardScoreTextUI");
        }
    }

    private void UpdateObstaclesDestroyedText(int newCount)
    {
        if (obstaclesDestroyedText != null)
        {
            obstaclesDestroyedText.text = $"Destroyed: {newCount}";
        }
        else
        {
            Debug.LogError("obstaclesDestroyedText is not found by DashboardScoreTextUI");
        }
    }

    public void StopTimer()
    {
        isGameRunning = false;
    }
}