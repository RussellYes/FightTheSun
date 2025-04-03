using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// This script displays the end screen and gives the player a choice to save their score.

public class EndConditionsUI : MonoBehaviour
{
    private ScoreManager scoreManager;
    private GameManager gameManager;
    private SFXManager sFXManager;

    [SerializeField] private bool isWin;
    [SerializeField] private Image winBackground;
    [SerializeField] private Button homeButton;
    [SerializeField] private TextMeshProUGUI winText;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private GameObject scoreTextShineBar;
    [SerializeField] private TextMeshProUGUI obstaclesDestroyedText;
    [SerializeField] private GameObject obstaclesDestroyedShineBar;
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private GameObject timeShineBar;
    [SerializeField] private Sprite winSprite;
    [SerializeField] private Sprite loseSprite;
    [SerializeField] private Color loseColor;
    [SerializeField] private AudioClip[] textMovementSfx;
    [SerializeField] private float textAppearDelay;
    [SerializeField] private float sFXDelay;

    private void Awake()
    {
        scoreManager = FindAnyObjectByType<ScoreManager>();
        gameManager = FindAnyObjectByType<GameManager>();
        sFXManager = FindAnyObjectByType<SFXManager>();

        // Initially hide all text elements and their shine bars
        winText.gameObject.SetActive(false);
        scoreText.gameObject.SetActive(false);
        if (scoreTextShineBar != null) scoreTextShineBar.SetActive(false);
        obstaclesDestroyedText.gameObject.SetActive(false);
        if (obstaclesDestroyedShineBar != null) obstaclesDestroyedShineBar.SetActive(false);
        timeText.gameObject.SetActive(false);
        if (timeShineBar != null) timeShineBar.SetActive(false);

        homeButton.onClick.AddListener(() =>
        {
            Loader.Load(Loader.Scene.MainMenuScene);
            Debug.Log("Loading Scene");
        });
    }

    private void OnEnable()
    {
        Debug.Log("EndConditionsUI subscribed to EndGameEvent");
        GameManager.EndGameEvent += EndGame;
    }

    private void OnDisable()
    {
        Debug.Log("EndConditionsUI unsubscribed from EndGameEvent");
        GameManager.EndGameEvent -= EndGame;
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
            }
            else
            {
                winBackground.color = loseColor;
                winText.text = "You lose";
            }

            // Update the money text
            scoreText.text = $"Money: {scoreManager.GetLevelMoney()}";

            // Update the obstacles destroyed text
            obstaclesDestroyedText.text = $"Destroyed: {scoreManager.GetLevelObstaclesDestroyed()}";

            // Update the time text
            int minutes = Mathf.FloorToInt(gameManager.GameTime / 60);
            int seconds = Mathf.FloorToInt(gameManager.GameTime % 60);
            timeText.text = $"Time: {minutes:00}:{seconds:00}";

            StartCoroutine(ShowTextsWithDelay());
        }
        else
        {
            Debug.Log("EndConditionsUI can't find ScoreManager or GameManager");
        }
    }

    private IEnumerator ShowTextsWithDelay()
    {
        // Show win/lose text and shine bar
        winText.gameObject.SetActive(true);
        yield return new WaitForSecondsRealtime(textAppearDelay);

        // Show score text and shine bar
        scoreText.gameObject.SetActive(true);
        if (scoreTextShineBar != null) scoreTextShineBar.SetActive(true);
        PlayRandomTextSfx();
        yield return new WaitForSecondsRealtime(textAppearDelay);

        // Show obstacles destroyed text and shine bar
        obstaclesDestroyedText.gameObject.SetActive(true);
        if (obstaclesDestroyedShineBar != null) obstaclesDestroyedShineBar.SetActive(true);
        PlayRandomTextSfx();
        yield return new WaitForSecondsRealtime(textAppearDelay);

        // Show time text and shine bar
        timeText.gameObject.SetActive(true);
        if (timeShineBar != null) timeShineBar.SetActive(true);
        PlayRandomTextSfx();
    }

    private void PlayRandomTextSfx()
    {
        if (textMovementSfx != null && textMovementSfx.Length > 0 && sFXManager != null)
        {
            int randomIndex = Random.Range(0, textMovementSfx.Length);
            sFXManager.PlaySFX(textMovementSfx[randomIndex]);
        }
    }
}