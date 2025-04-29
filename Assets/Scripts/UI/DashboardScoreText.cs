using TMPro;
using Unity.VisualScripting;
using UnityEngine;

// This script displays time, money, and obstacles destroyed in the dashboard UI.

public class DashboardScoreText : MonoBehaviour
{
    private GameManager gameManager;
    private ScoreManager scoreManager;

    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private TextMeshProUGUI dashboardMoneyText;
    [SerializeField] private TextMeshProUGUI obstaclesDestroyedText;

    private void OnEnable()
    {
        // Subscribe to score and obstacles destroyed events
        ScoreManager.OnLevelMoneyChanged += UpdateMoneyText;
        ScoreManager.OnObstaclesDestroyedByPlayerChanged += UpdateObstaclesDestroyedText;
    }

    private void OnDisable()
    {
        // Unsubscribe from events
        ScoreManager.OnLevelMoneyChanged -= UpdateMoneyText;
        ScoreManager.OnObstaclesDestroyedByPlayerChanged -= UpdateObstaclesDestroyedText;
    }

    private void Start()
    {
        // Initialize the UI elements
        scoreManager = FindFirstObjectByType<ScoreManager>();
        gameManager = FindFirstObjectByType<GameManager>();
        if (scoreManager != null)
        {
            UpdateMoneyText(scoreManager.GetLevelMoney());
            UpdateObstaclesDestroyedText(scoreManager.GetLevelObstaclesDestroyed());
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

    private void UpdateMoneyText(float newScore)
    {
        if (dashboardMoneyText != null)
        {
            dashboardMoneyText.text = $"Money: {newScore}";
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