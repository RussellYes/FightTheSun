using System;
using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// This script displays the end screen and gives the player a choice to save their score.

public class EndConditionsUI : MonoBehaviour
{
    private ScoreManager scoreManager;
    private GameManager gameManager;
    private SFXManager sFXManager;
    private PlayerStatsManager playerStatsManager;

    public static event Action EndConditionUIScoreChoiceEvent;
    public static event Action reviveEvent;

    [Header("Winning UI")]
    [SerializeField] private bool isWin;
    [SerializeField] private Image winBackground;
    [SerializeField] private Sprite winSprite;
    [SerializeField] private TextMeshProUGUI endText;

    [Header("New Score UI")]
    [SerializeField] private Button newScoreSaveButtonFront;
    [SerializeField] private Button newScoreSaveButtonBack;
    [SerializeField] private TextMeshProUGUI newMoneyText;
    [SerializeField] private GameObject moneyTextShineBar;
    [SerializeField] private TextMeshProUGUI newObstaclesDestroyedText;
    [SerializeField] private GameObject obstaclesDestroyedShineBar;
    [SerializeField] private TextMeshProUGUI newTimeText;
    [SerializeField] private GameObject timeShineBar;
    [SerializeField] private AudioClip[] textMovementSfx;
    [SerializeField] private float textAppearDelay;
    [SerializeField] private float sFXDelay;

    [Header("Old Score UI")]
    [SerializeField] private Button oldScoreSaveButtonFront;
    [SerializeField] private Button oldScoreSaveButtonBack;
    [SerializeField] private TextMeshProUGUI oldMoneyText;
    [SerializeField] private TextMeshProUGUI oldObstaclesDestroyedText;
    [SerializeField] private TextMeshProUGUI oldTimeText;

    [Header("Lose UI")]
    [SerializeField] private Sprite loseSprite;
    [SerializeField] private TextMeshProUGUI loseText;
    [SerializeField] private TextMeshProUGUI totalGameTimeText;
    [SerializeField] private TextMeshProUGUI totalObstaclesDestroyedText;
    [SerializeField] private TextMeshProUGUI totalMoneyText;
    [SerializeField] private TextMeshProUGUI lineText;
    [SerializeField] private GameObject loseComicHolder;
    [SerializeField] private Image loseBackground;
    [SerializeField] private Sprite[] loseComics;
    [SerializeField] private float loseComicPanelDisplayTime = 3f;
    [SerializeField] private Button skipComicButton;

    [Header("Time Capsule")]
    [SerializeField] private TextMeshProUGUI timeCapsuleCompletionText;
    private float capsuledLoseMoney;
    private float memoryScore;
    [SerializeField] private TextMeshProUGUI moneyText;
    [SerializeField] private TextMeshProUGUI ironText;
    [SerializeField] private TextMeshProUGUI cobaltText;

    [Header("Upgrade Buttons")]
    [SerializeField] private TextMeshProUGUI memoryScoreText;


    public Button saveButtonFront;
    [SerializeField] private GameObject saveButtonHolder;


    private void Awake()
    {
        scoreManager = FindAnyObjectByType<ScoreManager>();
        gameManager = FindAnyObjectByType<GameManager>();
        sFXManager = FindAnyObjectByType<SFXManager>();
        playerStatsManager = FindAnyObjectByType<PlayerStatsManager>();

        HideUI();
    }

    private void Start()
    {
        saveButtonFront.onClick.AddListener(() => { Revive(); });
    }

    private void OnEnable()
    {
        Debug.Log("EndConditionsUI subscribed to EndGameEvent");
        GameManager.EndGameEvent += EndGame;
        skipComicButton.onClick.AddListener(() => {SkipComic();
            Debug.Log("skip comic Button clicked!"); });
    }

    private void OnDisable()
    {
        Debug.Log("EndConditionsUI unsubscribed from EndGameEvent");
        GameManager.EndGameEvent -= EndGame;
        skipComicButton.onClick.RemoveListener(() => { SkipComic(); });
    }

    private void HideUI()
    {
        // Initially hide all text elements and their shine bars
        winBackground.gameObject.SetActive(false);
        endText.gameObject.SetActive(false);
        newMoneyText.gameObject.SetActive(false);
        if (moneyTextShineBar != null) moneyTextShineBar.SetActive(false);
        newObstaclesDestroyedText.gameObject.SetActive(false);
        if (obstaclesDestroyedShineBar != null) obstaclesDestroyedShineBar.SetActive(false);
        newTimeText.gameObject.SetActive(false);
        if (timeShineBar != null) timeShineBar.SetActive(false);
        oldMoneyText.gameObject.SetActive(false);
        oldObstaclesDestroyedText.gameObject.SetActive(false);
        oldTimeText.gameObject.SetActive(false);

        loseText.gameObject.SetActive(false);
        totalGameTimeText.gameObject.SetActive(false);
        totalObstaclesDestroyedText.gameObject.SetActive(false);
        totalMoneyText.gameObject.SetActive(false);
        lineText.gameObject.SetActive(false);
        memoryScoreText.gameObject.SetActive(false);

        saveButtonFront.gameObject.SetActive(false);
        saveButtonHolder.gameObject.SetActive(false);

        newScoreSaveButtonFront.gameObject.SetActive(false);
        newScoreSaveButtonBack.gameObject.SetActive(false);
        oldScoreSaveButtonFront.gameObject.SetActive(false);
        oldScoreSaveButtonBack.gameObject.SetActive(false);
    }

    private void EndGame(bool abool)
    {
        isWin = abool;
        if (scoreManager != null && gameManager != null)
        {
            Debug.Log($"EndGame called with isWin = {isWin}");

            // Update the win/lose UI
            if (isWin)
            {
                winBackground.gameObject.SetActive(true);
                winBackground.sprite = winSprite;
                endText.text = "You win";

                if (DataPersister.Instance != null && DataPersister.Instance.CurrentGameData != null && DataPersister.Instance.CurrentGameData.playerData.Count > 0)
                {
                    memoryScore = DataPersister.Instance.CurrentGameData.playerData[0].playerMemoryScore;
                    Debug.Log($"Initial memory score loaded: {memoryScore}");
                }

                // Update the money text
                newMoneyText.text = $"Money: {scoreManager.GetLevelMoney()}";

                // Update the obstacles destroyed text
                newObstaclesDestroyedText.text = $"Destroyed: {scoreManager.GetLevelObstaclesDestroyed()}";

                // Update the time text
                int minutes = Mathf.FloorToInt(gameManager.LevelTime / 60);
                int seconds = Mathf.FloorToInt(gameManager.LevelTime % 60);
                newTimeText.text = $"Time: {minutes:00}:{seconds:00}";

                StartCoroutine(ShowWinTextsWithDelay());
            }
            else
            {
                // Set hasLost flag when player sees lose comics
                if (DataPersister.Instance != null && DataPersister.Instance.CurrentGameData != null)
                {
                    DataPersister.Instance.CurrentGameData.hasLost = true;
                    Debug.Log("^ EndConditionsUI DisplayLoseComics: " + $"{DataPersister.Instance.CurrentGameData.hasLost}");
                    DataPersister.Instance.SaveCurrentGame();
                }
                if (DataPersister.Instance == null || DataPersister.Instance.CurrentGameData == null)
                {
                    Debug.LogError("EndConditionsUI DisplayLoseComics - DataPersister not initialized!");

                }


                StartCoroutine(DisplayLoseComics());
            }
        }
        else
        {
            Debug.Log("EndConditionsUI can't find ScoreManager or GameManager");
        }
    }

    private void SkipComic()
    {
        Debug.Log("Skipping comics display");
        StopAllCoroutines();
        loseComicHolder.SetActive(false);
        StartCoroutine(ShowLoseTextsWithDelay());
    }
    IEnumerator DisplayLoseComics()
    {
        loseComicHolder.SetActive(true);
        
            for (int i = 0; i < loseComics.Length; i++)
            {
                Debug.Log($"Displaying comic {i}: {loseComics[i].name}");

                // Set the new sprite
                loseBackground.sprite = loseComics[i];

                // Wait using unscaled time
                float endTime = Time.unscaledTime + loseComicPanelDisplayTime;
                while (Time.unscaledTime < endTime)
                {
                    yield return null; // Wait each frame until time passes
                }
            }

        loseComicHolder.SetActive(false);
        StartCoroutine(ShowLoseTextsWithDelay());
    }

    private IEnumerator ShowWinTextsWithDelay()
    {
        // Show win text
        endText.gameObject.SetActive(true);
        yield return new WaitForSecondsRealtime(textAppearDelay);

        // Show time text and shine bar
        newTimeText.gameObject.SetActive(true);
        if (timeShineBar != null) timeShineBar.SetActive(true);
        PlayRandomTextSfx();
        yield return new WaitForSecondsRealtime(textAppearDelay);

        // Show obstacles destroyed text and shine bar
        newObstaclesDestroyedText.gameObject.SetActive(true);
        if (obstaclesDestroyedShineBar != null) obstaclesDestroyedShineBar.SetActive(true);
        PlayRandomTextSfx();
        yield return new WaitForSecondsRealtime(textAppearDelay);

        // Show score text and shine bar
        newMoneyText.gameObject.SetActive(true);
        if (moneyTextShineBar != null) moneyTextShineBar.SetActive(true);
        PlayRandomTextSfx();
        yield return new WaitForSecondsRealtime(2);

        newScoreSaveButtonFront.gameObject.SetActive(true);
        newScoreSaveButtonBack.gameObject.SetActive(true);
        oldScoreSaveButtonFront.gameObject.SetActive(true);
        oldScoreSaveButtonBack.gameObject.SetActive(true);

        oldMoneyText.gameObject.SetActive(true);
        oldObstaclesDestroyedText.gameObject.SetActive(true);
        oldTimeText.gameObject.SetActive(true);

        // Set initial alpha to 0
        var newBack = newScoreSaveButtonBack.GetComponent<CanvasGroup>();
        var newFront = newScoreSaveButtonFront.GetComponent<CanvasGroup>();
        var oldBack = oldScoreSaveButtonBack.GetComponent<CanvasGroup>();
        var oldFront = oldScoreSaveButtonFront.GetComponent<CanvasGroup>();
        var oldMoney = oldMoneyText.GetComponent<CanvasGroup>();
        var oldObstacles = oldObstaclesDestroyedText.GetComponent<CanvasGroup>();
        var oldTime = oldTimeText.GetComponent<CanvasGroup>();
        newBack.alpha = 0f;
        newFront.alpha = 0f;
        oldBack.alpha = 0f;
        newBack.alpha = 0f;
        oldMoney.alpha = 0f;
        oldObstacles.alpha = 0f;
        oldTime.alpha = 0f;

        endText.text = "Choose One Score";

        // Get current level index
        int levelNumber = SceneManager.GetActiveScene().buildIndex - 1;

        Debug.Log($"EndConditionsUI ShowWinTextsWithDelay DataPersister.Instance.CurrentGameData.totalTime = {DataPersister.Instance.CurrentGameData.totalTime} FindTime");
        Debug.Log($"EndConditionsUI ShowWinTextsWithDelay gameManager.LevelTime = {gameManager.LevelTime} FindTime");

        // Load level data from JSON save
        float money = 0;
        int obstaclesDestroyed = 0;
        float timeInSeconds = 0;

        if (DataPersister.Instance != null && DataPersister.Instance.CurrentGameData != null)
        {
            if (DataPersister.Instance.CurrentGameData.levelData.TryGetValue(levelNumber, out LevelData levelData))
            {
                money = levelData.levelMoney;
                obstaclesDestroyed = levelData.levelObstaclesDestroyed;
                timeInSeconds = levelData.levelTime;
            }
        }

        oldMoneyText.text = money > 0 ? $"Money: {money}" : "Money: 0";
        oldObstaclesDestroyedText.text = obstaclesDestroyed > 0 ? $"Obstacles: {obstaclesDestroyed}" : "Obstacles: 0";
        oldTimeText.text = timeInSeconds > 0 ? $"Best Time: {Mathf.FloorToInt(timeInSeconds / 60):00}:{Mathf.FloorToInt(timeInSeconds % 60):00}" : "Time: --:--";

        // Fade in over time duration
        float fadeDuration = 1f;
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            float alpha = Mathf.Lerp(0f, 1f, elapsed / fadeDuration);

            newBack.alpha = alpha;
            newFront.alpha = alpha;
            oldBack.alpha = alpha;
            oldFront.alpha = alpha;
            oldMoney.alpha = alpha;
            oldObstacles.alpha = alpha;
            oldTime.alpha = alpha;

            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }

        // Ensure full visibility at the end
        newBack.alpha = 1f;
        newFront.alpha = 1f;
        oldBack.alpha = 1f;
        oldFront.alpha = 1f;
        oldMoney.alpha = 1f;
        oldObstacles.alpha = 1f;
        oldTime.alpha = 1f;

        // This button keeps the existing level score.
        oldScoreSaveButtonFront.onClick.AddListener(() =>
        {
            DataPersister.Instance.CurrentGameData.totalTime += gameManager.LevelTime;
            if (DataPersister.Instance.CurrentGameData.playerData.Count > 0)
            {
                DataPersister.Instance.CurrentGameData.playerData[0].playerMemoryScore = memoryScore;
                Debug.Log($"Saving memory score (old): {memoryScore}");
            }
            DataPersister.Instance.SaveCurrentGame();
            LoadMainMenuScene();
        });

        // This button saves a new score over the existing level score.
        newScoreSaveButtonFront.onClick.AddListener(() =>
        {
            Debug.Log("EndConditionsUI newScoreSaveButtonFront");
            EndConditionUIScoreChoiceEvent?.Invoke();
            DataPersister.Instance.CurrentGameData.totalTime += gameManager.LevelTime;
            if (DataPersister.Instance.CurrentGameData.playerData.Count > 0)
            {
                DataPersister.Instance.CurrentGameData.playerData[0].playerMemoryScore = memoryScore;
                Debug.Log($"Saving memory score (old): {memoryScore}");
            }
            DataPersister.Instance.SaveCurrentGame();
            LoadMainMenuScene();
        });
    }

    IEnumerator ShowLoseTextsWithDelay()
    {
        DataPersister.Instance.CurrentGameData.sunCount++;
        winBackground.gameObject.SetActive(true);
        winBackground.sprite = loseSprite;
        loseText.gameObject.SetActive(true);
        loseText.text = " Sent back in time. What will you remember?";

        Debug.Log($"EndConditionsUI ShowLoseTextsWithDelay - Total: {DataPersister.Instance.CurrentGameData.totalTime}, " +
          $"Level: {gameManager.LevelTime}, Remaining: {gameManager.TimeRemaining} FindTime");

        float loseTime = DataPersister.Instance.CurrentGameData.totalTime + gameManager.LevelTime;
        int loseObstacles = scoreManager.GetTotalObstaclesDestroyed() + scoreManager.GetLevelObstaclesDestroyed();
        float loseMoney = scoreManager.GetTotalMoney() + scoreManager.GetLevelMoney();
        Debug.Log($"@ loseMoney {loseMoney}");

        // Update the time text
        int minutes2 = Mathf.FloorToInt(loseTime / 60);
        int seconds2 = Mathf.FloorToInt(loseTime % 60);
        totalGameTimeText.text = $"Time: {minutes2:00}:{seconds2:00}";

        // Update the obstacles destroyed text
        totalObstaclesDestroyedText.text = $"Destroyed: " + loseObstacles;

        // Update the money text
        totalMoneyText.text = $"Money: {loseMoney.ToString("0")}";

        endText.gameObject.SetActive(false);

        loseText.gameObject.SetActive(true);
        totalGameTimeText.gameObject.SetActive(true);
        totalObstaclesDestroyedText.gameObject.SetActive(true);
        totalMoneyText.gameObject.SetActive(true);
        PlayRandomTextSfx();
        yield return new WaitForSecondsRealtime(textAppearDelay);

        // Load memory score
        memoryScore = 0;
        if (DataPersister.Instance != null &&
            DataPersister.Instance.CurrentGameData != null &&
            DataPersister.Instance.CurrentGameData.playerData.Count > 0)
        {
            memoryScore = DataPersister.Instance.CurrentGameData.playerData[0].playerMemoryScore;
        }
        Debug.Log($"EndConditionsUI ShowLoseTextsWithDelay - memoryScore before calculations: {memoryScore}");
        lineText.gameObject.SetActive(true);
        memoryScoreText.gameObject.SetActive(true);
        memoryScoreText.text = memoryScore.ToString("0") + " memories";
        PlayRandomTextSfx();
        yield return new WaitForSecondsRealtime(textAppearDelay);

        // Calculate final memory score
        float finalMemoryScore = memoryScore + (loseObstacles * 2 + loseMoney + (loseTime / 2));
        Debug.Log($"EndConditionsUI ShowLoseTextsWithDelay - memoryScore after calculations: {finalMemoryScore}");

        // Get comic unlocked data
        int unlockedComics = DataPersister.Instance.CurrentGameData.comicData.Count(kvp => kvp.Value.isUnlocked);
        int totalComics = DataPersister.Instance.CurrentGameData.comicNumbersLength;
        float comicUnlockPercent = (float)unlockedComics / totalComics * 100f;
        timeCapsuleCompletionText.text = $"{comicUnlockPercent:F0}%";

        // calculate new currentMoney with comic data
        capsuledLoseMoney = loseMoney * (comicUnlockPercent / 100f);
        Debug.Log($"@ capsuledLoseMoney {capsuledLoseMoney}");

        // Calculate an display time capsule items with comic data
        GameData gameData = DataPersister.Instance.CurrentGameData;
        // Money
        moneyText.text = $"{capsuledLoseMoney:F0}";
        // Iron
        float timeCapsuledIron = gameData.totalMetal * (comicUnlockPercent / 100f);
        ironText.text = $"{timeCapsuledIron:F0}";
        // Cobalt
        float timeCapsuledCobalt = gameData.totalRareMetal * (comicUnlockPercent / 100f);
        cobaltText.text = $"{timeCapsuledCobalt:F0}";

        // Lerp all values simultaneously over 3 seconds
        float lerpDuration = 3f;
        float elapsedTime = 0f;

        while (elapsedTime < lerpDuration)
        {
            float progress = elapsedTime / lerpDuration;

            // Lerp memory score up
            float currentMemoryValue = Mathf.Lerp(0, finalMemoryScore, progress);
            memoryScoreText.text = Mathf.RoundToInt(currentMemoryValue).ToString("0") + " memories";

            // Lerp other values down
            float currentTime = Mathf.Lerp(loseTime, 0, progress);
            int minutes3 = Mathf.FloorToInt(currentTime / 60);
            int seconds3 = Mathf.FloorToInt(currentTime % 60);
            totalGameTimeText.text = $"Time: {minutes3:00}:{seconds3:00}";

            float currentObstacles = Mathf.Lerp(loseObstacles, 0, progress);
            totalObstaclesDestroyedText.text = $"Destroyed: {Mathf.RoundToInt(currentObstacles)}";

            float currentMoney = Mathf.Lerp(loseMoney, capsuledLoseMoney, progress);
            totalMoneyText.text = $"Money: {Mathf.RoundToInt(currentMoney)}";

            elapsedTime += Time.unscaledDeltaTime;
            yield return null;
        }

        memoryScore = finalMemoryScore;

        // Show save button halves
        saveButtonHolder.gameObject.SetActive(true);
        saveButtonFront.gameObject.SetActive(true);
        
    }

    public void UpdateMemoryText()
    {
        if (playerStatsManager == null)
        {
            playerStatsManager = FindAnyObjectByType<PlayerStatsManager>();
        }

        memoryScoreText.text = memoryScore.ToString("0") + " memories";
    }

    private void PlayRandomTextSfx()
    {
        if (textMovementSfx != null && textMovementSfx.Length > 0 && sFXManager != null)
        {
            int randomIndex = UnityEngine.Random.Range(0, textMovementSfx.Length);
            sFXManager.PlaySFX(textMovementSfx[randomIndex]);
        }
    }

    private void LoadMainMenuScene()
    {
        StartCoroutine(SavingScreenBeforeChangingScene());
    }

    IEnumerator SavingScreenBeforeChangingScene()
    {
        HideUI();

        winBackground.gameObject.SetActive(true);
        winBackground.color = Color.black;
        endText.gameObject.SetActive(true);
        endText.text = "Saving...";

        // Save memoryScore with dataPersister and GameData scripts
        if (DataPersister.Instance != null && DataPersister.Instance.CurrentGameData != null)
        {
            // Ensure player data exists
            if (DataPersister.Instance.CurrentGameData.playerData.Count == 0)
            {
                DataPersister.Instance.CurrentGameData.playerData.Add(new PlayerSaveData());
            }
            Debug.Log($"EndConditionsUI SavingScreenBeforeChangingScene - DataPersister memoryScore before saving: {DataPersister.Instance.CurrentGameData.playerData[0].playerMemoryScore}");
            Debug.Log($"EndConditionsUI SavingScreenBeforeChangingScene - memoryScore before saving: {memoryScore}");
            // Update memory score
            DataPersister.Instance.CurrentGameData.playerData[0].playerMemoryScore = memoryScore;        
    
            // Save the game
            DataPersister.Instance.SaveCurrentGame();
            Debug.Log($"EndConditionsUI SavingScreenBeforeChangingScene - memoryScore before saving: {DataPersister.Instance.CurrentGameData.playerData[0].playerMemoryScore}");
            Debug.Log($"EndConditionsUI SavingScreenBeforeChangingScene - memoryScore before saving: {memoryScore}");
        }

        yield return new WaitForSecondsRealtime(1);

        Loader.Load(Loader.Scene.MainMenuScene);
        Debug.Log("Loading Scene");
    }

    private void Revive()
    {
        reviveEvent?.Invoke();

        var gameData = DataPersister.Instance.CurrentGameData;
        int currentLevel = SceneManager.GetActiveScene().buildIndex - 1;

        // Reset all levels saved data
        for (int i = 1; i <= 10; i++)
        {
            gameData.levelData[i] = new LevelData(0, 0, 0);
        }

        // Get comic unlock data to use as a multiplier for currencies.
        int unlockedComics = DataPersister.Instance.CurrentGameData.comicData.Count(kvp => kvp.Value.isUnlocked);
        int totalComics = DataPersister.Instance.CurrentGameData.comicNumbersLength;
        float comicUnlockRawPercent = (float)unlockedComics / totalComics;

        // Reset all resource totals
        gameData.totalMoney = capsuledLoseMoney;
        gameData.totalTime = 0f;
        gameData.totalMetal = gameData.totalMetal * comicUnlockRawPercent;
        gameData.totalRareMetal = gameData.totalRareMetal * comicUnlockRawPercent;
        gameData.totalObstaclesDestroyed = 0;

        // Reset missile and launcher data
        gameData.savedMissileCount = 0;
        gameData.savedLauncherLevel = 1;

        // Lock all levels except Level 1
        for (int i = 2; i <= 10; i++)
        {
            gameData.SetMissionUnlocked(i, false);
        }
        Debug.Log($"EndConditionsUI Revive - gameData.totalTime: {gameData.totalTime} FindTime");
        Debug.Log($"EndConditionsUI Revive - Total: {DataPersister.Instance.CurrentGameData.totalTime}, " +
          $"Level: {gameManager.LevelTime}, Remaining: {gameManager.TimeRemaining} FindTime");
        LoadMainMenuScene();
    }
}