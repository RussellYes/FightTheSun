using TMPro;
using UnityEngine;

// This scrpt displays saved scores for each level in the main menu.

public class MainMenuLevelScoreText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI bestTimeText;
    [SerializeField] private TextMeshProUGUI bestMoneyText;
    [SerializeField] private TextMeshProUGUI bestObstaclesDestroyedText;

    [SerializeField] private int levelNumber = 1; // Set this in inspector for each level display

    private void Start()
    {
        LoadLevelData(levelNumber);
    }

    private void LoadLevelData(int levelNumber)
    {
        Debug.Log("MainMenuLevelScoreText LoadLevelData");

        // First verify the key exists
        string obstacleKey = $"Level_{levelNumber}_ObstaclesDestroyed";

        if (!PlayerPrefs.HasKey(obstacleKey))
        {
            Debug.Log($"Key not found: {obstacleKey}");
            bestObstaclesDestroyedText.text = "Obstacles: ---";
            return;
        }

        // Load level data using ScoreManager's save format
        float levelMoney = PlayerPrefs.GetFloat($"Level_{levelNumber}_Money", 0);
        float levelTime = PlayerPrefs.GetFloat($"Level_{levelNumber}_Time", 0);
        int levelObstacles = PlayerPrefs.GetInt($"Level_{levelNumber}_ObstaclesDestroyed", 0);

        // Update UI text
        UpdateTimeText(levelTime);
        UpdateMoneyText(levelMoney);
        UpdateObstaclesDestroyedText(levelObstacles);
    }

    private void UpdateTimeText(float timeInSeconds)
    {
        bestTimeText.text = timeInSeconds > 0 ?
            $"Best Time: {Mathf.FloorToInt(timeInSeconds / 60):00}:{Mathf.FloorToInt(timeInSeconds % 60):00}" :
            "Time: --:--";
    }

    private void UpdateMoneyText(float money)
    {
        bestMoneyText.text = money > 0 ? $"Money: {money}" : "Money: ---";
    }

    private void UpdateObstaclesDestroyedText(int obstaclesDestroyed)
    {
        bestObstaclesDestroyedText.text = obstaclesDestroyed > 0 ?
            $"Obstacles: {obstaclesDestroyed}" : "Obstacles: ---";
    }
}