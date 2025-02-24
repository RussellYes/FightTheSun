using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MissionMenuScoreText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI bestTimeText;
    [SerializeField] private TextMeshProUGUI bestScoreText;
    [SerializeField] private TextMeshProUGUI bestObstaclesDestroyedText;

    private int missionNumber = 1; // Set this dynamically based on the selected mission

    private void Start()
    {
        LoadMissionData(missionNumber);
    }

    private void LoadMissionData(int missionNumber)
    {
        // Create unique keys for each mission
        string scoreKey = $"Mission{missionNumber}BestScore";
        string timeKey = $"Mission{missionNumber}BestTime";
        string obstaclesKey = $"Mission{missionNumber}BestObstaclesDestroyed";

        // Load saved values
        int bestScore = PlayerPrefs.GetInt(scoreKey, 0);
        float bestTime = PlayerPrefs.GetFloat(timeKey, 0);
        int bestObstacles = PlayerPrefs.GetInt(obstaclesKey, 0);

        // Update UI text
        UpdateTimeText(bestTime);
        UpdateScoreText(bestScore);
        UpdateObstaclesDestroyedText(bestObstacles);
    }

    private void UpdateTimeText(float timeInSeconds)
    {
        // Format the time as minutes:seconds
        int minutes = Mathf.FloorToInt(timeInSeconds / 60);
        int seconds = Mathf.FloorToInt(timeInSeconds % 60);
        bestTimeText.text = $"Time: {minutes:00}:{seconds:00}";
    }

    private void UpdateScoreText(int score)
    {
        bestScoreText.text = $"Score: {score}";
    }

    private void UpdateObstaclesDestroyedText(int obstaclesDestroyed)
    {
        bestObstaclesDestroyedText.text = $"Obstacles: {obstaclesDestroyed}";
    }
}