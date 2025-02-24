using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenuUI : MonoBehaviour
{
    [SerializeField] private Button unpauseButton;
    [SerializeField] private Button homeButton;

    private ScoreManager scoreManager;
    private GameManager gameManager;

    [SerializeField] private Image pauseBackground;
    [SerializeField] private TextMeshProUGUI pauseText;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI obstaclesDestroyedText;
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private Color pauseColor;



    private void Awake()
    {
        // Set up button listeners
        homeButton.onClick.AddListener(() => {
            Loader.Load(Loader.Scene.MainMenuScene);
            Debug.Log("Loading Scene");
        });

        gameManager = FindAnyObjectByType<GameManager>();

        unpauseButton.onClick.AddListener(() => {
            gameManager.UnpauseGame();
        });
    }

    private void Start()
    {


        if (scoreManager != null )
        {
            Debug.Log("Yay");
        }
        if (scoreManager == null )
        {
            Debug.Log("aww");
        }
    }

    private void OnEnable()
    {
        OnPause();
    }


    public void OnPause()
    {
        // Find references to ScoreManager and GameManager
        scoreManager = FindAnyObjectByType<ScoreManager>();
        gameManager = FindAnyObjectByType<GameManager>();

        pauseBackground.color = pauseColor;
        pauseText.text = "Paused";
        
        // Update the score text
        scoreText.text = $"Score: {scoreManager.GetScore()}";

        // Update the obstacles destroyed text
        obstaclesDestroyedText.text = $"Destroyed: {scoreManager.KilledByPlayerCount()}";

        // Update the time text
        int minutes = Mathf.FloorToInt(gameManager.GameTime / 60);
        int seconds = Mathf.FloorToInt(gameManager.GameTime % 60);
        timeText.text = $"Time: {minutes:00}:{seconds:00}";
        
    }
}