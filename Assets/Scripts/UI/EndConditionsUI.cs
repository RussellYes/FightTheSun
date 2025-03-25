using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EndConditionsUI : MonoBehaviour
{
    private ScoreManager scoreManager;
    private GameManager gameManager;

    [SerializeField] private bool isWin;
    [SerializeField] private Image winBackground;
    [SerializeField] private Button homeButton;
    [SerializeField] private TextMeshProUGUI winText;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI obstaclesDestroyedText;
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private Color winColor;
    [SerializeField] private Color loseColor;



    private void Awake()
    {
        scoreManager = FindAnyObjectByType<ScoreManager>();
        gameManager = FindAnyObjectByType<GameManager>();

        homeButton.onClick.AddListener(() => {
            Loader.Load(Loader.Scene.MainMenuScene);
            Debug.Log("Loading Scene");
        });
    }

    private void Start()
    {

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
                winBackground.color = winColor;
                winText.text = "You win";
            }
            else
            {
                winBackground.color = loseColor;
                winText.text = "You lose";
            }

            // Update the score text
            scoreText.text = $"Score: {scoreManager.GetMoney()}";

            // Update the obstacles destroyed text
            obstaclesDestroyedText.text = $"Destroyed: {scoreManager.KilledByPlayerCount()}";

            // Update the time text
            int minutes = Mathf.FloorToInt(gameManager.GameTime / 60);
            int seconds = Mathf.FloorToInt(gameManager.GameTime % 60);
            timeText.text = $"Time: {minutes:00}:{seconds:00}";
        }
        else
        {
            Debug.Log("EndConditionsUI can't find ScoreManager or GameManager");
        }
    }
}