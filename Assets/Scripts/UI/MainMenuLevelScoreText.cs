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

        CheckAchievements(levelNumber);
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


    private void CheckAchievements(int levelNumber)
    {
        GameData gameData = DataPersister.Instance.CurrentGameData;
        bool isLevelUnlocked = false;

        // Level 1 is always unlocked
        if (levelNumber == 1)
        {
            isLevelUnlocked = true;
        }

        // Check which mission corresponds to this level
        switch (levelNumber)
        {
            case 2:
                isLevelUnlocked = gameData.isMission1Complete;
                break;
            case 3:
                isLevelUnlocked = gameData.isMission2Complete;
                break;
            case 4:
                isLevelUnlocked = gameData.isMission3Complete;
                break;
            case 5:
                isLevelUnlocked = gameData.isMission4Complete;
                break;
            case 6:
                isLevelUnlocked = gameData.isMission5Complete;
                break;
            case 7:
                isLevelUnlocked = gameData.isMission6Complete;
                break;
            case 8:
                isLevelUnlocked = gameData.isMission7Complete;
                break;
            case 9:
                isLevelUnlocked = gameData.isMission8Complete;
                break;
            case 10:
                isLevelUnlocked = gameData.isMission9Complete;
                break;
            default:
                Debug.LogWarning($"No unlock condition for level {levelNumber}");
                break;
        }
    

        // Set planet active state based on unlock status
        lockedPlanet.SetActive(!isLevelUnlocked);
        Debug.Log($"Level {levelNumber} unlocked status: {!isLevelUnlocked}");
    }
}

