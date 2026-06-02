using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static GameManager;

// This script controls the pause menu UI.

public class PauseMenuUI : MonoBehaviour
{
    public static PauseMenuUI Instance; // Singleton instance

    public static event Action PauseEvent;
    public static event Action UnpauseEvent;

    [SerializeField] private GameObject pauseMenuUIHolder;
    [SerializeField] private Button pauseButton;
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
    private bool hasBeenPausedOnce = false;

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
        hasBeenPausedOnce = false;
        isPaused = false;

        pauseButton.onClick.AddListener(() => { OnPause(); });
        homeButton.onClick.AddListener(() => { HandleHomeButton(); });
        playButton.onClick.AddListener(() => {HandlePlayButton(); });
    }

    private void OnDisable()
    {
        pauseButton.onClick.RemoveAllListeners();
        homeButton.onClick.RemoveAllListeners();
        playButton.onClick.RemoveAllListeners();
        musicVolumeSlider.onValueChanged.RemoveAllListeners();
        sFXVolumeSlider.onValueChanged.RemoveAllListeners();
    }
    private void Start()
    {
        Debug.Log($"PauseMenuUI Start");
        // Initialize slider values to current volumes
        if (musicVolumeSlider != null)
        {
            musicVolumeSlider.onValueChanged.RemoveListener(SetMusicVolume);
            musicVolumeSlider.value = MusicManager.Instance.GetMusicVolume();
            musicVolumeSlider.onValueChanged.AddListener(SetMusicVolume);
        }

        if (sFXVolumeSlider != null)
        {
            sFXVolumeSlider.onValueChanged.RemoveListener(SetSFXVolume);
            sFXVolumeSlider.value = SFXManager.Instance.GetSFXVolume();
            sFXVolumeSlider.onValueChanged.AddListener(SetSFXVolume);
        }

        // Initialize icon states based on current volumes
        UpdateMusicIcon();
        UpdateSFXIcon();
    }

    private void HandlePlayButton()
    {
        Debug.Log($"PauseMenuUI HandlePlayButton - hasBeenPausedOnce: {hasBeenPausedOnce}, isPaused: {isPaused}, originalMusicClip: {originalMusicClip?.name ?? "null"}");
        SFXManager.PlaySFX(buttonSFX[UnityEngine.Random.Range(0, buttonSFX.Length)]);
        if (hasBeenPausedOnce)
        {
            OnUnpause();
        }
        else
        {
            pauseMenuUIHolder.SetActive(false);
        }
    }

    public void OnPause()
    {
        if (isPaused == true)
        {
            hasBeenPausedOnce = true;
        }
        else
        {
            if (hasBeenPausedOnce)
            {
                OnUnpause();
            }
            else
            {
                Debug.Log($"PauseMenuUI OnPauseEvent - Skipping unpause logic, never been properly paused");
                pauseMenuUIHolder.SetActive(false);
            }
        }

        Debug.Log($"PauseMenuUI OnPause - Original music: {MusicManager.Instance.GetCurrentClip()?.name ?? "null"}");
        PauseEvent?.Invoke();
        pauseMenuUIHolder.SetActive(true);

        isPaused = true; // Game is paused

        // Store the original music clip
        originalMusicClip = MusicManager.Instance.GetCurrentClip();
        Debug.Log($"PauseMenuUI OnPause - Storing originalMusicClip: {originalMusicClip?.name ?? "null"}");

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
            MusicManager.Instance.PlayMusic(musicPreview, this.name);
        }
    }

    private void OnUnpause()
    {
        Debug.Log($"PauseMenuUI OnUnpause - isPaused: {isPaused}, originalMusicClip: {originalMusicClip?.name ?? "null"}, currentMusic: {MusicManager.Instance.GetCurrentClip()?.name ?? "null"}");
        pauseMenuUIHolder.SetActive(false);
        UnpauseEvent?.Invoke();

        // Restore the original music clip that was playing before pause
        if (isPaused && originalMusicClip != null)
        {
            MusicManager.Instance.PlayMusic(originalMusicClip, "PauseMenuUI_Unpause");
        }

        // Unmute and resume music
        MusicManager.Instance.MuteMusic(false);
        MusicManager.Instance.ResumeMusic();
        SFXManager.Instance.MuteSFX(false);

        isPaused = false;
    }

    private void HandleHomeButton()
    {
        Debug.Log("PauseMenuUI HandleHomeButton - Loading Scene");
        SFXManager.PlaySFX(buttonSFX[UnityEngine.Random.Range(0, buttonSFX.Length)]);
        Loader.Load(Loader.Scene.MainMenuScene);
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
            MusicManager.Instance.PlayMusic(musicPreview, this.name);
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