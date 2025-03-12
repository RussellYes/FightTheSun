using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static GameManager;

//I think many menus should pause the game. 
//Should return to this script and write a more generic pause menu script, and seperate scripts for the different levels.

public class PauseMenuUI : MonoBehaviour
{
    public static PauseMenuUI Instance; // Singleton instance

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

    private bool isPaused; // Track whether the game is paused

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

        // Set up button listeners
        homeButton.onClick.AddListener(() => {
            Loader.Load(Loader.Scene.MainMenuScene);
            Debug.Log("Loading Scene");
        });

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

    private void OnEnable()
    {
        OnPause();
    }

    public void OnPause()
    {
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
        scoreText.text = $"Score: {scoreManager.GetScore()}";

        // Update the obstacles destroyed text
        obstaclesDestroyedText.text = $"Destroyed: {scoreManager.KilledByPlayerCount()}";

        // Update the time text
        int minutes = Mathf.FloorToInt(gameManager.GameTime / 60);
        int seconds = Mathf.FloorToInt(gameManager.GameTime % 60);
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