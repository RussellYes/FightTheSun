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
    public static event Action <string> AdRequestLevelLostReward;

    private ScoreManager scoreManager;
    private GameManager gameManager;
    private SFXManager sFXManager;
    private MusicManager musicManager;

    public static event Action EndConditionUIScoreChoiceEvent;
    public static event Action reviveEvent;

    [SerializeField] private GameObject endConditionsUIHolder;

    [Header("Winning UI")]
    [SerializeField] private bool isWin;
    [SerializeField] private Image winBackground;
    [SerializeField] private Sprite winSprite;
    [SerializeField] private TextMeshProUGUI endText;
    [SerializeField] private AudioClip EndWinConditionMusic;
    [SerializeField] private GameObject oldScoreHolder;
    [SerializeField] private GameObject newScoreHolder;

    [Header("New Score UI")]
    [SerializeField] private Button newScoreSaveButtonFront;
    [SerializeField] private Button newScoreSaveButtonBack;
    [SerializeField] private TextMeshProUGUI newMoneyText;
    [SerializeField] private GameObject textShineBar;
    [SerializeField] private TextMeshProUGUI newObstaclesDestroyedText;
    [SerializeField] private TextMeshProUGUI newTimeText;
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
    [SerializeField] private GameObject loseScoreHolder;
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
    [SerializeField] private AudioClip EndLoseConditionMusic;
    [SerializeField] private AudioClip endTextLerpSFX;

    [Header("Time Capsule")]
    [SerializeField] private TextMeshProUGUI timeCapsuleCompletionText;
    private float capsuledLoseMoney;
    private float memoryScore;
    private float memoryGainScore;
    private float originalMemoryBeforeGain; // Store original memory before gain for ad rewards
    private float newTotalMemoryScore;
    [SerializeField] private TextMeshProUGUI moneyText;
    [SerializeField] private TextMeshProUGUI ironText;
    [SerializeField] private TextMeshProUGUI cobaltText;

    [Header("Upgrade Buttons")]
    [SerializeField] private TextMeshProUGUI memoryScoreGainText;
    [SerializeField] private TextMeshProUGUI memoryTotalText;

    [Header("Ad Rewards")]
    public Button rewardButtonFront;
    [SerializeField] private GameObject rewardButtonHolder;
    public Button saveButtonFront;
    [SerializeField] private GameObject saveButtonHolder;
    [SerializeField] private GameObject adRewardRecievedHolder;
    [SerializeField] private TextMeshProUGUI adRewardRecievedText;

    private void Awake()
    {
        scoreManager = FindAnyObjectByType<ScoreManager>();
        gameManager = FindAnyObjectByType<GameManager>();
        sFXManager = FindAnyObjectByType<SFXManager>();
        musicManager = FindAnyObjectByType<MusicManager>();

        HideUI();
    }

    private void Start()
    {
        adRewardRecievedHolder.SetActive(false);
    }


    private void OnEnable()
    {
        Debug.Log("EndConditionsUI subscribed to EndGameEvent");
        GameManager.EndGameEvent += EndGame;
        skipComicButton.onClick.AddListener(() => {SkipComic();
        Debug.Log("skip comic Button clicked!"); });
        saveButtonFront.onClick.AddListener(() => { Revive(); });
        rewardButtonFront.onClick.AddListener(() => HandleRewardRequestButton());
        RewardedAdPlayer.RewardGranted += AdReward2xThenRevive;
        RewardedAdPlayer.ActivateGameLoseButtonsEvent += HandleActivateGameLoseButtonsEvent;
    }

    private void OnDisable()
    {
        Debug.Log("EndConditionsUI unsubscribed from EndGameEvent");
        GameManager.EndGameEvent -= EndGame;
        skipComicButton.onClick.RemoveListener(() => { SkipComic(); });
        saveButtonFront.onClick.RemoveListener(() => { Revive(); });
        rewardButtonFront.onClick.RemoveListener(() => HandleRewardRequestButton());
        RewardedAdPlayer.RewardGranted -= AdReward2xThenRevive;
        RewardedAdPlayer.ActivateGameLoseButtonsEvent -= HandleActivateGameLoseButtonsEvent;
    }

    private void HideUI()
    {
        // Initially hide all text elements and their shine bars
        endConditionsUIHolder.SetActive(false);
        winBackground.gameObject.SetActive(false);
        oldScoreHolder.SetActive(false);
        newScoreHolder.SetActive(false);
        endText.gameObject.SetActive(false);
        newMoneyText.gameObject.SetActive(false);
        newObstaclesDestroyedText.gameObject.SetActive(false);
        newTimeText.gameObject.SetActive(false);
        oldMoneyText.gameObject.SetActive(false);
        oldObstaclesDestroyedText.gameObject.SetActive(false);
        oldTimeText.gameObject.SetActive(false);

        loseText.gameObject.SetActive(false);
        loseScoreHolder.SetActive(false);
        totalGameTimeText.gameObject.SetActive(false);
        totalObstaclesDestroyedText.gameObject.SetActive(false);
        totalMoneyText.gameObject.SetActive(false);
        lineText.gameObject.SetActive(false);
        memoryScoreGainText.gameObject.SetActive(false);
        memoryTotalText.gameObject.SetActive(false);

        saveButtonHolder.gameObject.SetActive(false);
        rewardButtonHolder.gameObject.SetActive(false);

        newScoreSaveButtonFront.gameObject.SetActive(false);
        newScoreSaveButtonBack.gameObject.SetActive(false);
        oldScoreSaveButtonFront.gameObject.SetActive(false);
        oldScoreSaveButtonBack.gameObject.SetActive(false);
    }

    private void EndGame(bool didWinLevel)
    {
        endConditionsUIHolder.SetActive(true);
        adRewardRecievedHolder.SetActive(false);

        isWin = didWinLevel;
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
                    memoryScore = DataPersister.Instance.CurrentGameData.memory;
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

                PlayLoseMusic();
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
        PlayWinMusic();

        oldScoreHolder.SetActive(true);
        newScoreHolder.SetActive(true);

        // Show win text
        endText.gameObject.SetActive(true);
        yield return new WaitForSecondsRealtime(textAppearDelay);

        // Show time text and shine bar
        newTimeText.gameObject.SetActive(true);
        Instantiate(textShineBar, newTimeText.transform.position, Quaternion.identity, newTimeText.transform.parent);
        PlayRandomTextSfx();
        yield return new WaitForSecondsRealtime(textAppearDelay);

        // Show obstacles destroyed text and shine bar
        newObstaclesDestroyedText.gameObject.SetActive(true);
        Instantiate(textShineBar, newObstaclesDestroyedText.transform.position, Quaternion.identity, newObstaclesDestroyedText.transform.parent);
        PlayRandomTextSfx();
        yield return new WaitForSecondsRealtime(textAppearDelay);

        // Show score text and shine bar
        newMoneyText.gameObject.SetActive(true);
        Instantiate(textShineBar, newMoneyText.transform.position, Quaternion.identity, newMoneyText.transform.parent);
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
                DataPersister.Instance.CurrentGameData.memory = memoryScore;
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
                DataPersister.Instance.CurrentGameData.memory = memoryScore;
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
        loseScoreHolder.SetActive(true);
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
        totalGameTimeText.text = $"{minutes2:00}:{seconds2:00}";

        // Update the obstacles destroyed text
        totalObstaclesDestroyedText.text = $"{loseObstacles}";

        // Update the money text
        totalMoneyText.text = $"{loseMoney.ToString("0")}";

        endText.gameObject.SetActive(false);
        loseText.gameObject.SetActive(true);
        totalGameTimeText.gameObject.SetActive(true);
        totalObstaclesDestroyedText.gameObject.SetActive(true);
        totalMoneyText.gameObject.SetActive(true);

        PlayRandomTextSfx();
        Instantiate(textShineBar, totalGameTimeText.transform.position, Quaternion.identity, totalGameTimeText.transform.parent);
        Instantiate(textShineBar, totalObstaclesDestroyedText.transform.position, Quaternion.identity, totalObstaclesDestroyedText.transform.parent);
        Instantiate(textShineBar, totalMoneyText.transform.position, Quaternion.identity, totalMoneyText.transform.parent);
        yield return new WaitForSecondsRealtime(textAppearDelay);

        // Load memory score
        if (DataPersister.Instance != null &&
            DataPersister.Instance.CurrentGameData != null &&
            DataPersister.Instance.CurrentGameData.playerData.Count > 0)
        {
            memoryScore = DataPersister.Instance.CurrentGameData.memory;
        }
        Debug.Log($"EndConditionsUI ShowLoseTextsWithDelay - memoryScore before calculations: {memoryScore}");
        lineText.gameObject.SetActive(true);
        memoryScoreGainText.gameObject.SetActive(true);
        memoryTotalText.gameObject.SetActive(true);
        memoryScoreGainText.text = $"+0";
        memoryTotalText.text = $"{Mathf.RoundToInt(memoryScore)}";
        PlayRandomTextSfx();
        Instantiate(textShineBar, memoryScoreGainText.transform.position, Quaternion.identity, memoryScoreGainText.transform.parent);
        yield return new WaitForSecondsRealtime(textAppearDelay);
        PlayRandomTextSfx();
        Instantiate(textShineBar, memoryTotalText.transform.position, Quaternion.identity, memoryTotalText.transform.parent);
        yield return new WaitForSecondsRealtime(textAppearDelay);

        // Calculate memory score gain
        memoryGainScore = loseObstacles * 2 + loseMoney + (loseTime / 2);
        Debug.Log($"EndConditionsUI ShowLoseTextsWithDelay - memoryScoreGain: {memoryGainScore}");

        // Get comic unlocked data
        int unlockedComics = DataPersister.Instance.CurrentGameData.comicData.Count(kvp => kvp.Value.isUnlocked);
        int totalComics = DataPersister.Instance.CurrentGameData.comicNumbersLength;
        float comicUnlockPercent = (float)unlockedComics / totalComics * 100f;
        timeCapsuleCompletionText.text = $"{comicUnlockPercent:F0}%";

        // calculate new currentMoney with comic data
        capsuledLoseMoney = loseMoney * (comicUnlockPercent / 100f);
        Debug.Log($"EndConditionsUI ShowLoseTextsWithDelay - capsuledLoseMoney {capsuledLoseMoney}");

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

        float pauseToShowLootValues = 2f;
        yield return new WaitForSecondsRealtime(pauseToShowLootValues);

        // Lerp all values simultaneously over seconds
        float lerpDuration = 4f;
        float elapsedTime = 0f;
        PlayTextLerpSFX();
        while (elapsedTime < lerpDuration)
        {
            float progress = elapsedTime / lerpDuration;

            // Lerp new memory score up
            float currentMemoryValue = Mathf.Lerp(0, memoryGainScore, progress);
            memoryScoreGainText.text = "+" + Mathf.RoundToInt(currentMemoryValue).ToString("0");

            // Lerp other values down
            float currentTime = Mathf.Lerp(loseTime, 0, progress);
            int minutes3 = Mathf.FloorToInt(currentTime / 60);
            int seconds3 = Mathf.FloorToInt(currentTime % 60);
            totalGameTimeText.text = $"{minutes3:00}:{seconds3:00}";

            float currentObstacles = Mathf.Lerp(loseObstacles, 0, progress);
            totalObstaclesDestroyedText.text = $"{Mathf.RoundToInt(currentObstacles)}";

            float currentMoney = Mathf.Lerp(loseMoney, capsuledLoseMoney, progress);
            totalMoneyText.text = $"{Mathf.RoundToInt(currentMoney)}";

            elapsedTime += Time.unscaledDeltaTime;
            yield return null;
        }

        PlayRandomTextSfx();
        Instantiate(textShineBar, memoryScoreGainText.transform.position, Quaternion.identity, memoryScoreGainText.transform.parent);
        yield return new WaitForSecondsRealtime(textAppearDelay);

        {

            // Calculate new total memory score
            originalMemoryBeforeGain = memoryScore;
            newTotalMemoryScore = memoryScore + memoryGainScore;
            Debug.Log($"EndConditionsUI ShowLoseTextsWithDelay - newTotalMemoryScore: {newTotalMemoryScore}");

            // Lerp memory total text from current memory score to total memory score
            float startMemoryValue = memoryScore;
            float lerpDuration2 = 2f;
            float elapsedTime2 = 0f;
            PlayTextLerpSFX();

            while (elapsedTime2 < lerpDuration2)
            {
                float progress = elapsedTime2 / lerpDuration2;
                float currentMemoryValue = Mathf.Lerp(startMemoryValue, newTotalMemoryScore, progress);
                memoryTotalText.text = $"{Mathf.RoundToInt(currentMemoryValue)}";
                elapsedTime2 += Time.unscaledDeltaTime;
                yield return null;
            }
            StopSFX();

            // Ensure final value is set exactly
            memoryTotalText.text = $"{Mathf.RoundToInt(newTotalMemoryScore)}";
            PlayRandomTextSfx();
            Instantiate(textShineBar, memoryTotalText.transform.position, Quaternion.identity, memoryTotalText.transform.parent);
        }
        memoryScore = newTotalMemoryScore;

        // Show save button halves
        saveButtonHolder.gameObject.SetActive(true);
        rewardButtonHolder.gameObject.SetActive(true);
    }



    private void PlayWinMusic()
    {
        Debug.Log($"EndCnditionsUI PlayWinMusic");
        if (musicManager != null && EndWinConditionMusic != null)
        {
            musicManager.StopMusic();
            musicManager.PlayMusic(EndWinConditionMusic, this.name);
        }
    }
    private void PlayLoseMusic()
    {
        Debug.Log($"EndCnditionsUI PlayLoseMusic");
        if (musicManager != null && EndLoseConditionMusic != null)
        {
            musicManager.StopMusic();
            musicManager.PlayMusic(EndLoseConditionMusic, this.name);
        }
    }
    private void PlayTextLerpSFX()
    {
        Debug.Log($"EndCnditionsUI PlayTextLerpSFX");
        if (sFXManager != null && endTextLerpSFX != null)
        {
            sFXManager.PlaySFX(endTextLerpSFX);
        }
    }
    private void StopSFX()
    {
        Debug.Log($"EndCnditionsUI StopSFX");
        if (sFXManager != null)
        {
            sFXManager.StopSFX();
        }
    }
    private void PlayRandomTextSfx()
    {
        Debug.Log($"EndCnditionsUI PlayRandomTextSfx");
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
            Debug.Log($"EndConditionsUI SavingScreenBeforeChangingScene - DataPersister memoryScore before saving: {DataPersister.Instance.CurrentGameData.memory}");
            Debug.Log($"EndConditionsUI SavingScreenBeforeChangingScene - memoryScore before saving: {memoryScore}");
            // Update memory score
            DataPersister.Instance.CurrentGameData.memory = memoryScore;        
    
            // Save the game
            DataPersister.Instance.SaveCurrentGame();
            Debug.Log($"EndConditionsUI SavingScreenBeforeChangingScene - memoryScore before saving: {DataPersister.Instance.CurrentGameData.memory}");
            Debug.Log($"EndConditionsUI SavingScreenBeforeChangingScene - memoryScore before saving: {memoryScore}");
        }

        yield return new WaitForSecondsRealtime(1);

        Loader.Load(Loader.Scene.MainMenuScene);
        Debug.Log("Loading Scene");
    }


    private void HandleRewardRequestButton()
    {
        saveButtonHolder.SetActive(false);
        rewardButtonHolder.SetActive(false);
        AdRequestLevelLostReward?.Invoke("endLoss");
    }
    private void AdReward2xThenRevive(string requesterID)
    {
        if (requesterID == "endLoss")
        {
            StartCoroutine(AdReward2xThenRevived());
        }
    }

    IEnumerator AdReward2xThenRevived()
    {
        float adRewardMultiplier = 2f;
        float waitForPlayerToReadDuration = 1f;

        // Display current values (they're already shown from ShowLoseTextsWithDelay)
        // memoryScoreGainText shows the gain, memoryTotalText shows the total

        // Calculate memory gain
        float currentMemoryGain = memoryGainScore;
        float doubledMemoryGain = currentMemoryGain * adRewardMultiplier;

        yield return new WaitForSecondsRealtime(waitForPlayerToReadDuration);

        // Lerp memoryGain to the 2xMemoryGain
        float lerpTime = 1.5f;
        float elapsedTime = 0f;
        PlayTextLerpSFX();
        while (elapsedTime < lerpTime)
        {
            float progress = elapsedTime / lerpTime;
            float currentLerpedGain = Mathf.Lerp(currentMemoryGain, doubledMemoryGain, progress);
            memoryScoreGainText.text = "+" + Mathf.RoundToInt(currentLerpedGain).ToString("0");

            elapsedTime += Time.unscaledDeltaTime;
            yield return null;
        }

        memoryScoreGainText.text = "+" + Mathf.RoundToInt(doubledMemoryGain).ToString("0");
        StopSFX();
        PlayRandomTextSfx();
        Instantiate(textShineBar, memoryScoreGainText.transform.position, Quaternion.identity, memoryScoreGainText.transform.parent);

        yield return new WaitForSecondsRealtime(waitForPlayerToReadDuration);

        float newMemoryTotal = originalMemoryBeforeGain + doubledMemoryGain;
        elapsedTime = 0f;
        PlayTextLerpSFX();

        // Lerp memory total text from current to new total
        while (elapsedTime < lerpTime)
        {
            float progress = elapsedTime / lerpTime;
            float currentLerpedTotal = Mathf.Lerp(memoryScore, newMemoryTotal, progress);
            memoryTotalText.text = Mathf.RoundToInt(currentLerpedTotal).ToString("0");

            elapsedTime += Time.unscaledDeltaTime;
            yield return null;
        }

        // Ensure final total value
        memoryTotalText.text = Mathf.RoundToInt(newMemoryTotal).ToString("0");
        StopSFX();
        PlayRandomTextSfx();
        Instantiate(textShineBar, memoryTotalText.transform.position, Quaternion.identity, memoryTotalText.transform.parent);

        // Update the actual memoryScore variable with the new total
        memoryScore = newMemoryTotal;

        float pauseToLetPlayerSeeFinalValues = 1f;
        yield return new WaitForSecondsRealtime(pauseToLetPlayerSeeFinalValues);

        // Ad break
        StartCoroutine(AdRewardRecievedMessage(doubledMemoryGain));
    }

    IEnumerator AdRewardRecievedMessage(float doubledMemoryGain)
    {
        Debug.Log($"EndConditionsUI AdRewardRecievedMessage - doubledMemoryGain: {doubledMemoryGain}");
        adRewardRecievedHolder.SetActive(true);
        adRewardRecievedText.text = ($"${doubledMemoryGain: 0}");
        yield return new WaitForSecondsRealtime(3f);
        adRewardRecievedHolder.SetActive(false);

        Revive();
    }

    private void Revive()
    {
        Debug.Log("EndConditionsUI Revive");
        reviveEvent?.Invoke();

        var gameData = DataPersister.Instance.CurrentGameData;
        int currentLevel = SceneManager.GetActiveScene().buildIndex - 1;

        /* Commented out this code for casual players to enjoy the game more.
        // Reset all levels saved data
        for (int i = 1; i <= 10; i++)
        {
            gameData.levelData[i] = new LevelData(0, 0, 0);
        }*/

        // Get comic unlock data to use as a multiplier for currencies.
        int unlockedComics = DataPersister.Instance.CurrentGameData.comicData.Count(kvp => kvp.Value.isUnlocked);
        int totalComics = DataPersister.Instance.CurrentGameData.comicNumbersLength;
        float comicUnlockRawPercent = (float)unlockedComics / totalComics;

        // Reset all resource totals
        gameData.totalMoney = capsuledLoseMoney;
        // gameData.totalTime = 0f; // Commented out this line to see if time is displayed correctly after reviving.
        gameData.totalMetal = gameData.totalMetal * comicUnlockRawPercent;
        gameData.totalRareMetal = gameData.totalRareMetal * comicUnlockRawPercent;
        gameData.totalObstaclesDestroyed = 0;

        // Reset missile and launcher data
        gameData.missileCount = 0;
        gameData.launcherLevel = 1;
        gameData.shieldCount = 0;
        gameData.nukeCount = 0;

        /* // This code is for a proper hardcore game mode where the player loses level progress on death.
        // Keep it commented out because the audiance are casual players.
        // Maybe include hardcore mode in the future.

        // Lock all levels except Level 1
        for (int i = 2; i <= 10; i++)
        {
            gameData.SetMissionUnlocked(i, false);
        }
        Debug.Log($"EndConditionsUI Revive - gameData.totalTime: {gameData.totalTime} FindTime");
        Debug.Log($"EndConditionsUI Revive - Total: {DataPersister.Instance.CurrentGameData.totalTime}, " +
          $"Level: {gameManager.LevelTime}, Remaining: {gameManager.TimeRemaining} FindTime");
        */

        LoadMainMenuScene();
    }

    private void HandleActivateGameLoseButtonsEvent()
    {
        saveButtonHolder.SetActive(true);
        rewardButtonHolder.SetActive(true);
    }
}