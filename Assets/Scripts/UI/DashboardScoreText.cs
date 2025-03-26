using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class DashboardScoreText : MonoBehaviour
{
    private GameManager gameManager;
    private ScoreManager scoreManager;

    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private TextMeshProUGUI dashboardScoreText;
    [SerializeField] private TextMeshProUGUI obstaclesDestroyedText;

    private void OnEnable()
    {
        // Subscribe to score and obstacles destroyed events
        ScoreManager.OnLevelMoneyChanged += UpdateScoreText;
        ScoreManager.OnObstaclesDestroyedByPlayerChanged += UpdateObstaclesDestroyedText;
    }

    private void OnDisable()
    {
        // Unsubscribe from events
        ScoreManager.OnLevelMoneyChanged -= UpdateScoreText;
        ScoreManager.OnObstaclesDestroyedByPlayerChanged -= UpdateObstaclesDestroyedText;
    }

    private void Start()
    {
        // Initialize the UI elements
        scoreManager = FindObjectOfType<ScoreManager>();
        gameManager = FindObjectOfType<GameManager>();
        if (scoreManager != null)
        {
            UpdateScoreText(scoreManager.GetMoney());
            UpdateObstaclesDestroyedText(scoreManager.KilledByPlayerCount());
        }
    }

    private void Update()
    {
            UpdateTimeText(gameManager.GameTime);
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

}