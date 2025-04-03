using System;
using System.Collections;
using System.Collections.Generic;
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

    public static Action EndConditionUIScoreChoiceEvent;


    [Header("Winning UI")]
    [SerializeField] private bool isWin;
    [SerializeField] private Image winBackground;
    [SerializeField] private Sprite winSprite;
    [SerializeField] private TextMeshProUGUI winText;

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
    [SerializeField] private Color loseColor;

    private void Awake()
    {
        scoreManager = FindAnyObjectByType<ScoreManager>();
        gameManager = FindAnyObjectByType<GameManager>();
        sFXManager = FindAnyObjectByType<SFXManager>();

        // Initially hide all text elements and their shine bars
        winText.gameObject.SetActive(false);
        newMoneyText.gameObject.SetActive(false);
        if (moneyTextShineBar != null) moneyTextShineBar.SetActive(false);
        newObstaclesDestroyedText.gameObject.SetActive(false);
        if (obstaclesDestroyedShineBar != null) obstaclesDestroyedShineBar.SetActive(false);
        newTimeText.gameObject.SetActive(false);
        if (timeShineBar != null) timeShineBar.SetActive(false);
        oldMoneyText.gameObject.SetActive(false);
        oldObstaclesDestroyedText.gameObject.SetActive(false);
        oldTimeText.gameObject.SetActive(false);

        newScoreSaveButtonFront.gameObject.SetActive(false);
        newScoreSaveButtonBack.gameObject.SetActive(false);

        oldScoreSaveButtonFront.gameObject.SetActive(false);
        oldScoreSaveButtonBack.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        Debug.Log("EndConditionsUI subscribed to EndGameEvent");
        GameManager.EndGameEvent += EndGame;
        ScoreManager.SavedTotalEvent += LoadMainMenuScene;
    }

    private void OnDisable()
    {
        Debug.Log("EndConditionsUI unsubscribed from EndGameEvent");
        GameManager.EndGameEvent -= EndGame;
        ScoreManager.SavedTotalEvent -= LoadMainMenuScene;
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
                winText.text = "You win";

                // Update the money text
                newMoneyText.text = $"Money: {scoreManager.GetLevelMoney()}";

                // Update the obstacles destroyed text
                newObstaclesDestroyedText.text = $"Destroyed: {scoreManager.GetLevelObstaclesDestroyed()}";

                // Update the time text
                int minutes = Mathf.FloorToInt(gameManager.GameTime / 60);
                int seconds = Mathf.FloorToInt(gameManager.GameTime % 60);
                newTimeText.text = $"Time: {minutes:00}:{seconds:00}";

                StartCoroutine(ShowTextsWithDelay());
            }
            else
            {
                winBackground.color = loseColor;
                winText.text = "You lose";
            }
        }
        else
        {
            Debug.Log("EndConditionsUI can't find ScoreManager or GameManager");
        }
    }

    private IEnumerator ShowTextsWithDelay()
    {
        // Show win text
        winText.gameObject.SetActive(true);
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

        // Set initial alpha to 0
        var newBack = newScoreSaveButtonBack.GetComponent<CanvasGroup>();
        var newFront = newScoreSaveButtonFront.GetComponent<CanvasGroup>();
        var oldBack = oldScoreSaveButtonBack.GetComponent<CanvasGroup>();
        var oldFront = oldScoreSaveButtonFront.GetComponent<CanvasGroup>();
        newBack.alpha = 0f;
        newFront.alpha = 0f;
        oldBack.alpha = 0f;
        newBack.alpha = 0f;

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

            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }

        // Ensure full visibility at the end
        newBack.alpha = 1f;
        newFront.alpha = 1f;
        oldBack.alpha = 1f;
        oldFront.alpha = 1f;


        yield return new WaitForSecondsRealtime(textAppearDelay);

        oldMoneyText.gameObject.SetActive(true);
        oldObstaclesDestroyedText.gameObject.SetActive(true);
        oldTimeText.gameObject.SetActive(true);

        winText.text = "Save One Score";

        // Get current level index
        int levelNumber = SceneManager.GetActiveScene().buildIndex - 1;
        // Load level data using ScoreManager's save format
        float money = PlayerPrefs.GetFloat($"Level_{levelNumber}_Money", 0);
        oldMoneyText.text = money > 0 ? $"Money: {money}" : "Money: 0";

        int obstaclesDestroyed = PlayerPrefs.GetInt($"Level_{levelNumber}_ObstaclesDestroyed", 0);
        oldObstaclesDestroyedText.text = obstaclesDestroyed > 0 ? $"Obstacles: {obstaclesDestroyed}" : "Obstacles: 0";

        float timeInSeconds = PlayerPrefs.GetFloat($"Level_{levelNumber}_Time", 0);
        oldTimeText.text = timeInSeconds > 0 ? $"Best Time: {Mathf.FloorToInt(timeInSeconds / 60):00}:{Mathf.FloorToInt(timeInSeconds % 60):00}" : "Time: --:--";


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
        Loader.Load(Loader.Scene.MainMenuScene);
        Debug.Log("Loading Scene");
    }



}