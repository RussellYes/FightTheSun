using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static GameManager;

// This script controls the pause menu UI.

public class PauseMenuUI : MonoBehaviour
{
    public static PauseMenuUI Instance; // Singleton instance

    [SerializeField] private GameObject pauseMenuUIHolder;
    [SerializeField] private Button homeButton;
    [SerializeField] private Button playButton;
    [SerializeField] private AudioClip[] buttonSFX;

    SFXManager SFXManager => SFXManager.Instance;
    private ScoreManager scoreManager;
    private GameManager gameManager;

    [SerializeField] private Image pauseBackground;
    [SerializeField] private TextMeshProUGUI pauseText;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI obstaclesDestroyedText;
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private Color pauseColor;

    [Header("Volume Controls")]
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider sFXVolumeSlider;

    [Header("Icons")]
    [SerializeField] private GameObject xMusicIcon; // Icon for muted music
    [SerializeField] private GameObject xSFXIcon;   // Icon for muted SFX

    [Header("Audio Clips")]
    [SerializeField] private AudioClip sliderSFX; // Sound effect to play when adjusting the SFX slider
    [SerializeField] private AudioClip musicPreview; // Music clip to play when adjusting the music slider
    private AudioClip originalMusicClip; // Track the original music clip

    private bool isPaused;

    private void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Set up slider listeners
        if (musicVolumeSlider != null)
        {
            musicVolumeSlider.onValueChanged.AddListener(SetMusicVolume);
        }

        if (sFXVolumeSlider != null)
        {
            sFXVolumeSlider.onValueChanged.AddListener(SetSFXVolume);
        }
    }

    private void OnEnable()
    {
        GameManager.pauseMenuUIEvent += OnPauseEvent;

        homeButton.onClick.AddListener(() => {
            SFXManager.PlaySFX(buttonSFX[UnityEngine.Random.Range(0, buttonSFX.Length)]);
            Loader.Load(Loader.Scene.MainMenuScene);
            Debug.Log("PauseMenuUI OnEnable - Loading Scene");
        });

        playButton.onClick.AddListener(() =>
        {
            SFXManager.PlaySFX(buttonSFX[UnityEngine.Random.Range(0, buttonSFX.Length)]);
            isPaused = false; // Game is no longer paused
        });
    }

    private void OnDisable()
    {
        GameManager.pauseMenuUIEvent -= OnPauseEvent;
        homeButton.onClick.RemoveAllListeners();
        musicVolumeSlider.onValueChanged.RemoveAllListeners();
        sFXVolumeSlider.onValueChanged.RemoveAllListeners();
    }
    private void Start()
    {
        // Initialize slider values to current volumes
        if (musicVolumeSlider != null)
        {
            musicVolumeSlider.value = MusicManager.Instance.GetMusicVolume();
        }

        if (sFXVolumeSlider != null)
        {
            sFXVolumeSlider.value = SFXManager.Instance.GetSFXVolume();
        }

        // Initialize icon states based on current volumes
        UpdateMusicIcon();
        UpdateSFXIcon();
    }

    private void OnPauseEvent(bool pause)
    {
        Debug.Log($"PauseMenuUI OnPauseEvent");
        if (pause)
        {
            OnPause();
        }
        else
        {
            pauseMenuUIHolder.SetActive(false);
        }
    }
    public void OnPause()
    {
        Debug.Log("PauseMenuUI OnPause");
        pauseMenuUIHolder.SetActive(true);

        isPaused = true; // Game is paused

        // Store the original music clip
        originalMusicClip = MusicManager.Instance.GetCurrentClip();

        // Mute music and SFX immediately when the pause menu is enabled
        MusicManager.Instance.PauseMusic();
        MusicManager.Instance.MuteMusic(true);
        SFXManager.Instance.MuteSFX(true);

        // Find references to ScoreManager and GameManager
        scoreManager = FindAnyObjectByType<ScoreManager>();
        gameManager = FindAnyObjectByType<GameManager>();

        pauseBackground.color = pauseColor;
        pauseText.text = "Paused";

        // Update the score text
        scoreText.text = $"Score: {scoreManager.GetLevelMoney()}";

        // Update the obstacles destroyed text
        obstaclesDestroyedText.text = $"Destroyed: {scoreManager.GetLevelObstaclesDestroyed()}";

        // Update the time text
        int minutes = Mathf.FloorToInt(gameManager.LevelTime / 60);
        int seconds = Mathf.FloorToInt(gameManager.LevelTime % 60);
        timeText.text = $"Time: {minutes:00}:{seconds:00}";

        // Play the pause menu music
        if (musicPreview != null)
        {
            MusicManager.Instance.PlayMusic(musicPreview);
        }
    }

    public AudioClip GetOriginalMusicClip()
    {
        return originalMusicClip;
    }
    public void SetMusicVolume(float volume)
    {
        // Temporarily unmute Music if paused
        if (isPaused)
        {
            MusicManager.Instance.MuteMusic(false);
        }

        // Set the music volume
        MusicManager.Instance.SetMusicVolume(volume);
        UpdateMusicIcon(); // Update the music icon when volume changes

        // Play a preview of the music when adjusting the slider
        if (musicPreview != null)
        {
            MusicManager.Instance.PlayMusic(musicPreview);
        }

        // Re-mute music if still paused
        if (isPaused)
        {
            StartCoroutine(ReMuteMusicAfterDelay());
        }
    }

    private IEnumerator ReMuteMusicAfterDelay()
    {
        yield return new WaitForSecondsRealtime(0.1f); // Wait for a short delay

        if (isPaused)
        {
            MusicManager.Instance.MuteMusic(true);
        }
    }

    public void SetSFXVolume(float volume)
    {
        // Temporarily unmute SFX if paused
        if (isPaused)
        {
            SFXManager.Instance.MuteSFX(false);
        }

        // Set the SFX volume
        SFXManager.Instance.SetSFXVolume(volume);
        UpdateSFXIcon(); // Update the SFX icon when volume changes

        // Play a sound effect when adjusting the SFX slider
        if (sliderSFX != null)
        {
            SFXManager.Instance.PlaySFX(sliderSFX);
        }

        // Re-mute SFX if still paused
        if (isPaused)
        {
            StartCoroutine(ReMuteSFXAfterDelay());
        }
    }

    private IEnumerator ReMuteSFXAfterDelay()
    {
        yield return new WaitForSecondsRealtime(0.1f); // Wait for a short delay

        if (isPaused)
        {
            SFXManager.Instance.MuteSFX(true);
        }
    }

    private void UpdateMusicIcon()
    {
        // Enable the icon if volume is 0, otherwise disable it
        xMusicIcon.SetActive(MusicManager.Instance.GetMusicVolume() == 0);
    }

    private void UpdateSFXIcon()
    {
        // Enable the icon if volume is 0, otherwise disable it
        xSFXIcon.SetActive(SFXManager.Instance.GetSFXVolume() == 0);
    }
}