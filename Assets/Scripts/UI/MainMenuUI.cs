using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{

    [SerializeField] private Button playMission1Button;
    [SerializeField] private Button playMission2Button;
    [SerializeField] private Button quitButton;

    private void Awake()
    {
        playMission1Button.onClick.AddListener(() =>{
            Loader.Load(Loader.Scene.MissionAlphaScene);
            Debug.Log("Loading Scene");
        });  
        
        playMission2Button.onClick.AddListener(() =>{
            Loader.Load(Loader.Scene.MissionBravoScene);
            Debug.Log("Loading Scene");
        });        
        
        quitButton.onClick.AddListener(() =>{
            Debug.Log("Quit");
            Application.Quit();
        });

        Time.timeScale = 1f;
    }

    private void Start()
    {
        // Initialize slider values to current volumes without triggering mute logic
        if (musicVolumeSlider != null)
        {
            musicVolumeSlider.SetValueWithoutNotify(MusicManager.Instance.GetMusicVolume());
        }

        if (sFXVolumeSlider != null)
        {
            sFXVolumeSlider.SetValueWithoutNotify(SFXManager.Instance.GetSFXVolume());
        }

        // Initialize icon states based on current volumes
        UpdateMusicIcon();
        UpdateSFXIcon();

        // Ensure the menu is closed on start
        isMenuOpen = false;
        mainMenu.SetActive(false);
    }

    private void OpenMenu()
    {
        isMenuOpen = true; // Menu is now open
        mainMenu.SetActive(true);
        mainMenuBackground.color = pauseColor;

        // Mute music and SFX when the menu is opened
        MusicManager.Instance.MuteMusic(true);
        SFXManager.Instance.MuteSFX(true);
    }

    private void CloseMenu()
    {
        isMenuOpen = false; // Menu is now closed
        mainMenu.SetActive(false);

        // Unmute music and SFX when the menu is closed
        MusicManager.Instance.MuteMusic(false);
        SFXManager.Instance.MuteSFX(false);
    }

    public void SetMusicVolume(float volume)
    {
        // Temporarily unmute music if the menu is open
        if (isMenuOpen)
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

        // Re-mute music if the menu is still open
        if (isMenuOpen)
        {
            StartCoroutine(ReMuteMusicAfterDelay());
        }
    }

    private IEnumerator ReMuteMusicAfterDelay()
    {
        yield return new WaitForSecondsRealtime(0.1f); // Wait for a short delay

        if (isMenuOpen) // Only re-mute if the menu is still open
        {
            MusicManager.Instance.MuteMusic(true);
        }
    }

    public void SetSFXVolume(float volume)
    {
        // Temporarily unmute SFX if the menu is open
        if (isMenuOpen)
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

        // Re-mute SFX if the menu is still open
        if (isMenuOpen)
        {
            StartCoroutine(ReMuteSFXAfterDelay());
        }
    }

    private IEnumerator ReMuteSFXAfterDelay()
    {
        yield return new WaitForSecondsRealtime(0.1f); // Wait for a short delay

        if (isMenuOpen) // Only re-mute if the menu is still open
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