using System;
using System.Collections;
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
    [SerializeField] private AudioClip buttonAppearSFX;
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
    [SerializeField] private float loseComicPanelDisplayTime = 2f;
    [SerializeField] private Button skipComicButton;

    private float memoryScore;

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
        ScoreManager.SavedTotalEvent += LoadMainMenuScene;
        skipComicButton.onClick.AddListener(() => {SkipComic(); });
    }

    private void OnDisable()
    {
        Debug.Log("EndConditionsUI unsubscribed from EndGameEvent");
        GameManager.EndGameEvent -= EndGame;
        ScoreManager.SavedTotalEvent -= LoadMainMenuScene;
        skipComicButton.onClick.RemoveListener(() => { SkipComic(); });
    }

    private void HideUI()
    {
        // Initially hide all text elements and their shine bars
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

    private void EndGame(bool isWin)
    {
        if (scoreManager != null && gameManager != null)
        {
            Debug.Log($"EndGame called with isWin = {isWin}");

            // Update the win/lose UI
            if (isWin)
            {
                winBackground.sprite = winSprite;
                endText.text = "You win";

                // Update the money text
                newMoneyText.text = $"Money: {scoreManager.GetLevelMoney()}";

                // Update the obstacles destroyed text
                newObstaclesDestroyedText.text = $"Destroyed: {scoreManager.GetLevelObstaclesDestroyed()}";

                // Update the time text
                int minutes = Mathf.FloorToInt(gameManager.GameTime / 60);
                int seconds = Mathf.FloorToInt(gameManager.GameTime % 60);
                newTimeText.text = $"Time: {minutes:00}:{seconds:00}";

                StartCoroutine(ShowWinTextsWithDelay());
            }
            else
            {
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
        StopCoroutine(DisplayLoseComics());
        loseComicHolder.SetActive(false);
        //StartCoroutine(ShowLoseTextsWithDelay());
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
            LoadMainMenuScene();
        });

        // This button saves a new score over the existing level score.
        newScoreSaveButtonFront.onClick.AddListener(() =>
        {
            EndConditionUIScoreChoiceEvent?.Invoke();
        });
    }

    IEnumerator ShowLoseTextsWithDelay()
    {
        loseText.gameObject.SetActive(true);
        loseText.text = "What will you remember?";
        winBackground.sprite = loseSprite;

        float loseTime = scoreManager.GetTotalTime() + gameManager.GameTime;
        int loseObstacles = scoreManager.GetTotalObstaclesDestroyed() + scoreManager.GetLevelObstaclesDestroyed();
        float loseMoney = scoreManager.GetTotalMoney() + scoreManager.GetLevelMoney();

        // Update the time text
        int minutes2 = Mathf.FloorToInt(loseTime / 60);
        int seconds2 = Mathf.FloorToInt(loseTime % 60);
        totalGameTimeText.text = $"Time: {minutes2:00}:{seconds2:00}";

        // Update the obstacles destroyed text
        totalObstaclesDestroyedText.text = $"Destroyed: " + loseObstacles;

        // Update the money text
        totalMoneyText.text = $"Money: " + loseMoney;

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

        lineText.gameObject.SetActive(true);
        memoryScoreText.gameObject.SetActive(true);
        memoryScoreText.text = memoryScore.ToString("0") + " memories";
        UpdateMemoryText();
        PlayRandomTextSfx();
        yield return new WaitForSecondsRealtime(textAppearDelay);
        
        // Calculate final memory score
        float finalMemoryScore = memoryScore + (loseObstacles * loseMoney / loseTime);
        memoryScoreText.text = finalMemoryScore.ToString("0") + " memories";
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

            float currentMoney = Mathf.Lerp(loseMoney, 0, progress);
            totalMoneyText.text = $"Money: {Mathf.RoundToInt(currentMoney)}";

            elapsedTime += Time.unscaledDeltaTime;
            yield return null;
        }

        memoryScore = finalMemoryScore;

        // Ensure final values are exact
        memoryScoreText.text = Mathf.RoundToInt(memoryScore).ToString("0") + " memories";
        totalGameTimeText.text = "Time: 00:00";
        totalObstaclesDestroyedText.text = "Destroyed: 0";
        totalMoneyText.text = "Money: 0";
        
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

            // Update memory score
            DataPersister.Instance.CurrentGameData.playerData[0].playerMemoryScore = memoryScore;

            // Save the game
            DataPersister.Instance.SaveCurrentGame();
        }

        yield return new WaitForSecondsRealtime(1);

        Loader.Load(Loader.Scene.MainMenuScene);
        Debug.Log("Loading Scene");
    }

    private void Revive()
    {
        reviveEvent?.Invoke();

        LoadMainMenuScene();
    }
}