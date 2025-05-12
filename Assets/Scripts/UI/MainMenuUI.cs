using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    AchievmentsUI achievmentsUI;

    [Header("Planets")]
    [SerializeField] private Button playMission1Button;
    [SerializeField] private Button playMission2Button;
    [SerializeField] private Button playMission3Button;
    [SerializeField] private Button playMission4Button;
    [SerializeField] private Button playMission5Button;
    [SerializeField] private Button playMission6Button;
    [SerializeField] private Button playMission7Button;
    [SerializeField] private Button playMission8Button;
    [SerializeField] private Button playMission9Button;
    [SerializeField] private Button playMission10Button;

    [Header("Settings")]
    [SerializeField] private Button upButton;
    [SerializeField] private Button downButton;
    [SerializeField] private Button openSettingButton;
    [SerializeField] private Button continueButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private GameObject settingsMenu;
    [SerializeField] private Color pauseColor;
    [SerializeField] private Image mainMenuBackground;


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

    private bool isMenuOpen;

    // Screen movement variables
    private float screenHeight = 10f;
    private float minYPosition = 0f;
    private float maxYPosition = 40;

    private void Awake()
    {
        playMission1Button.onClick.AddListener(() =>{
            Loader.Load(Loader.Scene.Mission1Scene);
        });  
        
        playMission2Button.onClick.AddListener(() =>{
            Loader.Load(Loader.Scene.Mission2Scene);
        });
        
        playMission3Button.onClick.AddListener(() =>{
            Loader.Load(Loader.Scene.Mission3Scene);
        });
        
        playMission4Button.onClick.AddListener(() =>{
            Loader.Load(Loader.Scene.Mission4Scene);;
        });
        
        playMission5Button.onClick.AddListener(() =>{
            Loader.Load(Loader.Scene.Mission5Scene);
        });
        
        playMission6Button.onClick.AddListener(() =>{
            Loader.Load(Loader.Scene.Mission6Scene);
        });  
        
        playMission7Button.onClick.AddListener(() =>{
            Loader.Load(Loader.Scene.Mission7Scene);
        });
        
        playMission8Button.onClick.AddListener(() =>{
            Loader.Load(Loader.Scene.Mission8Scene);
        });
        
        playMission9Button.onClick.AddListener(() =>{
            Loader.Load(Loader.Scene.Mission9Scene);;
        });
        
        playMission10Button.onClick.AddListener(() =>{
            Loader.Load(Loader.Scene.Mission10Scene);
        });

        upButton.onClick.AddListener(() =>{
            MoveScreenUp();
        });
        
        downButton.onClick.AddListener(() =>{
            MoveScreenDown();
        });
        
        openSettingButton.onClick.AddListener(() =>{
            Debug.Log("openSettingButton.onClick");
            OpenMenu();
        });
        continueButton.onClick.AddListener(() =>{
            Debug.Log("continueButton.onClick");
            CloseMenu();
        });
        quitButton.onClick.AddListener(() =>
        {
            Debug.Log("Quit");
            Application.Quit();
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


        Time.timeScale = 1f;
    }

    private void OnEnable()
    {
        SwipeControls.OnSwipeUp += MoveScreenDown;
        SwipeControls.OnSwipeDown += MoveScreenUp;
    }

    private void OnDisable()
    {
        SwipeControls.OnSwipeUp -= MoveScreenDown;
        SwipeControls.OnSwipeDown -= MoveScreenUp;
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
        settingsMenu.SetActive(false);

        achievmentsUI = FindFirstObjectByType<AchievmentsUI>();
    }

    private void OpenMenu()
    {
        isMenuOpen = true; // Menu is now open
        settingsMenu.SetActive(true);
        mainMenuBackground.color = pauseColor;

        // Store the original music clip
        originalMusicClip = MusicManager.Instance.GetCurrentClip();

        // Mute music and SFX when the menu is opened
        MusicManager.Instance.PauseMusic();
        MusicManager.Instance.MuteMusic(true);
        SFXManager.Instance.MuteSFX(true);

        // Play the setting menu music
        if (musicPreview != null)
        {
            MusicManager.Instance.PlayMusic(musicPreview);
        }
    }

    private void CloseMenu()
    {
        // Stop the sample music and resume the original music
        MusicManager.Instance.StopMusic();

        // Get the original music clip
        if (originalMusicClip != null)
        {
            MusicManager.Instance.PlayMusic(originalMusicClip);
        }

        isMenuOpen = false; // Menu is now closed
        settingsMenu.SetActive(false);

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

    private void MoveScreenUp()
    {
        //move screen up the height of the screen. Do not go above a maximum value
        // Move the camera up by the height of the screen
        Vector3 newPosition = Camera.main.transform.position + Vector3.up * screenHeight;
        newPosition.y = Mathf.Clamp(newPosition.y, minYPosition, maxYPosition);
        Camera.main.transform.position = newPosition;
    }

    private void MoveScreenDown()
    {
        //move screen down the height of the screen. Do not go below a minimum value
        Vector3 newPosition = Camera.main.transform.position + Vector3.down * screenHeight;
        newPosition.y = Mathf.Clamp(newPosition.y, minYPosition, maxYPosition);
        Camera.main.transform.position = newPosition;
    }
}