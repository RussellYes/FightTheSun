using System.Collections;
using System.Numerics;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// This script displays saved scores for each level in the main menu.

public class MainMenuPlanetLevelScoreText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI bestTimeText;
    [SerializeField] private TextMeshProUGUI bestMoneyText;
    [SerializeField] private TextMeshProUGUI bestObstaclesDestroyedText;
    [SerializeField] private GameObject lockedLevelCheckerPlanet;
    [SerializeField] private Button openUnlockerUIButton;

    [SerializeField] private int levelNumber;

    private void OnEnable()
    {
        DataPersister.InitializationComplete += LoadLevelData;
        LevelUnlockerUI.LevelUnlockedEvent += OnLevelUnlocked;

        if (openUnlockerUIButton)
        {
            openUnlockerUIButton.onClick.AddListener(() =>
            {
                LevelUnlockerUI.Instance.OpenLevelUnlocker(levelNumber);
            });
        }

    }

    private void OnDisable()
    {
        DataPersister.InitializationComplete -= LoadLevelData;
        LevelUnlockerUI.LevelUnlockedEvent -= OnLevelUnlocked;
    }



    private void LoadLevelData()
    {
        Debug.Log("MainMenuLevelScoreText LoadLevelData");

        SetDefaultText();

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
            UpdateLevelLock(levelNumber);
        }
        else
        {
            Debug.Log($"No saved data found for level {levelNumber}");
        }

        CheckLevelUnlockStatus(levelNumber);
    }

    private void OnLevelUnlocked()
    {
        UpdateLevelLock(levelNumber);
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
    }

    private void UpdateLevelLock(float levelNumber)
    {
        bool isLevelUnlocked = DataPersister.Instance.CurrentGameData.GetMissionUnlocked((int)levelNumber);

        lockedLevelCheckerPlanet.SetActive(!isLevelUnlocked);
    }

}

