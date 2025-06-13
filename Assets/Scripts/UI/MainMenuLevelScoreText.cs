using System.Collections;
using System.Numerics;
using TMPro;
using UnityEngine;

// This script displays saved scores for each level in the main menu.

public class MainMenuLevelScoreText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI bestTimeText;
    [SerializeField] private TextMeshProUGUI bestMoneyText;
    [SerializeField] private TextMeshProUGUI bestObstaclesDestroyedText;
    [SerializeField] private GameObject lockedPlanet;

    [SerializeField] private int levelNumber = 0; // Set this in inspector for each level display

    private void Start()
    {
        StartCoroutine(Initialize());
    }

    private IEnumerator Initialize()
    {
        // Initial wait to ensure other systems are initialized
        yield return new WaitForEndOfFrame();

        // Brief additional wait if DataPersister isn't ready
        if (DataPersister.Instance == null || DataPersister.Instance.CurrentGameData == null)
        {
            yield return new WaitForSeconds(0.1f);
        }

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

        CheckLevelUnlockStatus(levelNumber);
    }

    private IEnumerator DelayedLoad(int levelNumber)
    {
        yield return null; // Wait one frame
        LoadLevelData(levelNumber); // Try again
    }
    private void SetDefaultText()
    {
        bestTimeText.text = "--:--";
        bestMoneyText.text = "---";
        bestObstaclesDestroyedText.text = "---";
    }
    private void UpdateTimeText(float timeInSeconds)
    {
        bestTimeText.text = timeInSeconds > 0 ?
            $"{Mathf.FloorToInt(timeInSeconds / 60):00}:{Mathf.FloorToInt(timeInSeconds % 60):00}" :
            "--:--";
    }

    private void UpdateMoneyText(float money)
    {
        bestMoneyText.text = money > 0 ? $"{money}" : "---";
    }

    private void UpdateObstaclesDestroyedText(int obstaclesDestroyed)
    {
        bestObstaclesDestroyedText.text = obstaclesDestroyed > 0 ?
            $"{obstaclesDestroyed}" : "---";
    }
    private void CheckLevelUnlockStatus(int levelNumber)
    {
        bool isLevelUnlocked = false;
        var gameData = DataPersister.Instance.CurrentGameData;

        // Force Level 1 to always be unlocked
        if (levelNumber == 1)
        {
            isLevelUnlocked = true;
            gameData.SetMissionUnlocked(1, true); // Ensure unlock state is set
        }
        else if (levelNumber >= 2 && levelNumber <= 10)
        {
            isLevelUnlocked = gameData.GetMissionUnlocked(levelNumber);
        }

        lockedPlanet.SetActive(!isLevelUnlocked);
        Debug.Log($"Level {levelNumber} unlocked: {isLevelUnlocked}");
    }
}

