using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelUnlockerUI : MonoBehaviour
{
    public static event Action LevelUnlockedEvent;


    SFXManager sfxManager;

    public enum Level
    {
        Level1,
        Level2,
        Level3,
        Level4,
        Level5,
        Level6,
        Level7,
        Level8,
        Level9
    }

    [Header("Level Select")]
    [SerializeField] private Level selectedLevel = Level.Level1;

    [Header("Level Unlocker UI")]
    [SerializeField] private GameObject levelUnlockerHolder;
    [SerializeField] private Button openUnlockerUIButton;
    [SerializeField] private Button closeUnlockerUIButon;
    [SerializeField] private Button unlockLevelButton;
    [SerializeField] private TextMeshProUGUI unlockLevelText;
    private float unlockCost = 118f;
    [SerializeField] private AudioClip openCloseButtonSFX;
    [SerializeField] private AudioClip[] buttonSuccessSFX;
    [SerializeField] private AudioClip buttonFailSFX;

    private void Start()
    {
        sfxManager = SFXManager.Instance;
    }
    private void OnEnable()
    {
        openUnlockerUIButton.onClick.AddListener(OpenLevelUnlocker);
        closeUnlockerUIButon.onClick.AddListener(CloseLevelUnlocker);
        unlockLevelButton.onClick.AddListener(UnlockLevel);
    }

    private void OnDisable()
    {
        openUnlockerUIButton.onClick.RemoveListener(OpenLevelUnlocker);
        closeUnlockerUIButon.onClick.RemoveListener(CloseLevelUnlocker);
        unlockLevelButton.onClick.RemoveListener(UnlockLevel);
    }

    private void OpenLevelUnlocker()
    {
        PlayOpenCloseButtonSFX();
        levelUnlockerHolder.SetActive(true);
        UpdateUnlockLevelText();
    }

    private void CloseLevelUnlocker()
    {
        PlayOpenCloseButtonSFX();
        levelUnlockerHolder.SetActive(false);
    }

    private void UpdateUnlockLevelText()
    {
        if (DataPersister.Instance == null || DataPersister.Instance.CurrentGameData == null)
        {
            Debug.LogError("LevelUnlockerUI UpdateUnlockLevelText - DataPersister not initialized!");
            return;
        }

        int levelNumber = (int)selectedLevel;
        bool isUnlocked = DataPersister.Instance.CurrentGameData.GetMissionUnlocked(levelNumber);

        float cost = unlockCost * levelNumber;
        float currentMemory = DataPersister.Instance.CurrentGameData.playerData[0].playerMemoryScore;
        unlockLevelText.text = $"Unlock {selectedLevel} for {cost} Memory? (You have: {currentMemory})";
    }

    private void UnlockLevel()
    {
        if (DataPersister.Instance == null || DataPersister.Instance.CurrentGameData == null)
        {
            Debug.LogError("DataPersister not initialized!");
            return;
        }

        int levelNumber = (int)selectedLevel;
        float cost = unlockCost * levelNumber;
        float currentMemory = DataPersister.Instance.CurrentGameData.playerData[0].playerMemoryScore;

        if (cost <= currentMemory)
        {
            DataPersister.Instance.CurrentGameData.playerData[0].playerMemoryScore -= cost;
            DataPersister.Instance.CurrentGameData.SetMissionUnlocked(levelNumber, true);
            DataPersister.Instance.SaveCurrentGame();
            PlayButtonSuccessSFX();
            LevelUnlockedEvent?.Invoke();
            CloseLevelUnlocker();
            Debug.Log($"Level {selectedLevel} unlocked! Remaining memory: {currentMemory}");
        }
        else
        {
            PlayButtonFailSFX();
            Debug.LogWarning($"Not enough memory to unlock level {selectedLevel}");
        }

    }
    private void PlayOpenCloseButtonSFX()
    {
        if (sfxManager != null && openCloseButtonSFX != null)
        {
            sfxManager.PlaySFX(openCloseButtonSFX);
        }
    }

    private void PlayButtonSuccessSFX()
    {
        if (sfxManager != null && buttonSuccessSFX != null)
        {
            sfxManager.PlaySFX(buttonSuccessSFX[UnityEngine.Random.Range(0, buttonSuccessSFX.Length)]);           
        }
    }

    private void PlayButtonFailSFX()
    {
        if (sfxManager != null && buttonFailSFX != null)
        {
            sfxManager.PlaySFX(buttonFailSFX);
        }
    }

}
