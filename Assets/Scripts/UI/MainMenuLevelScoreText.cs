using System.Collections;
using TMPro;
using UnityEngine;

// This scrpt displays saved scores for each level in the main menu.

public class MainMenuLevelScoreText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI bestTimeText;
    [SerializeField] private TextMeshProUGUI bestMoneyText;
    [SerializeField] private TextMeshProUGUI bestObstaclesDestroyedText;

    [SerializeField] private int levelNumber = 0; // Set this in inspector for each level display

    private void Start()
    {
        LoadLevelData(levelNumber);
    }

    private void LoadLevelData(int levelNumber)
    {
        Debug.Log("MainMenuLevelScoreText LoadLevelData");

        SetDefaultText();

        // Check if save system is available
        if (DataPersister.Instance == null)
        {
            Debug.LogWarning("DataPersister not initialized yet - trying again next frame");
            StartCoroutine(DelayedLoad(levelNumber));
            return;
        }

        if (DataPersister.Instance.CurrentGameData == null)
        {
            Debug.LogWarning("GameData not loaded yet - trying again next frame");
            StartCoroutine(DelayedLoad(levelNumber));
            return;
        }

        // Try to get level data
        if (DataPersister.Instance.CurrentGameData.levelData.TryGetValue(levelNumber, out LevelData levelData))
        {
            Debug.Log($"Loaded data for level {levelNumber}: " +
                     $"Time={levelData.levelTime}, " +
                     $"Money={levelData.levelMoney}, " +
                     $"Obstacles={levelData.levelObstaclesDestroyed}");

            UpdateTimeText(levelData.levelTime);
            UpdateMoneyText(levelData.levelMoney);
            UpdateObstaclesDestroyedText(levelData.levelObstaclesDestroyed);
        }
        else
        {
            Debug.Log($"No saved data found for level {levelNumber}");
        }
    }

    private IEnumerator DelayedLoad(int levelNumber)
    {
        yield return null; // Wait one frame
        LoadLevelData(levelNumber); // Try again
    }

    private void SetDefaultText()
    {
        bestTimeText.text = "Time: --:--";
        bestMoneyText.text = "Money: ---";
        bestObstaclesDestroyedText.text = "Obstacles: ---";
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