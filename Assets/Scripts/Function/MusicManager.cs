using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;

    private AudioSource musicSource;
    private float musicVolume = 1f; // Default volume

    // Array to hold music clips for each scene
    public AudioClip[] sceneMusicClips;

    private AudioClip currentSceneMusic; // Track the current scene's music

    public float GetMusicVolume()
    {
        return musicVolume;
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        musicSource = GetComponent<AudioSource>();
        if (musicSource == null)
        {
            Debug.Log("MusicManager: No AudioSource found on the GameObject.");
            musicSource = gameObject.AddComponent<AudioSource>(); // Add AudioSource if missing
        }

        // Ensure the AudioSource is enabled
        musicSource.enabled = true;

        // Load saved music volume
        musicVolume = PlayerPrefs.GetFloat("MusicVolume", 1f); // Default to 1 if no saved value exists
        musicSource.volume = musicVolume;

        // Subscribe to the sceneLoaded event
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Check if the scene index is within the bounds of the array
        if (scene.buildIndex < sceneMusicClips.Length && sceneMusicClips[scene.buildIndex] != null)
        {
            currentSceneMusic = sceneMusicClips[scene.buildIndex];
            PlayMusic(currentSceneMusic);
        }
        else
        {
            Debug.LogWarning($"No music clip found for scene index {scene.buildIndex}.");
        }

        // Reset music state (unmute and resume)
        MuteMusic(false);
        ResumeMusic();
    }

    private void Start()
    {
        MuteMusic(false);
    }

    public void PlayMusic(AudioClip musicClip)
    {
        if (musicClip == null)
        {
            Debug.LogWarning("MusicManager: Attempted to play a null audio clip.");
            return;
        }

        if (musicSource.isPlaying && musicSource.clip == musicClip) return;

        musicSource.clip = musicClip;
        musicSource.Play();
    }

    public void PlayTestMusic(AudioClip testClip)
    {
        if (testClip == null)
        {
            Debug.LogWarning("MusicManager: Attempted to play a null test clip.");
            return;
        }

        // Play the test clip without affecting the current scene music
        musicSource.PlayOneShot(testClip);
    }

    public AudioClip GetCurrentClip()
    {
        return musicSource.clip;
    }

    public void ResumeOriginalMusic(AudioClip originalClip)
    {
        if (originalClip != null)
        {
            musicSource.clip = originalClip;
            musicSource.Play();
        }
    }
    public void StopMusic()
    {
        if (musicSource.isPlaying)
        {
            musicSource.Stop();
        }
    }

    public void SetMusicVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);
        musicSource.volume = musicVolume;

        // Save the music volume to PlayerPrefs
        PlayerPrefs.SetFloat("MusicVolume", musicVolume);
        PlayerPrefs.Save(); // Ensure the data is saved immediately
    }

    public void MuteMusic(bool mute)
    {
        musicSource.mute = mute;
    }

    public void PauseMusic()
    {
        musicSource.Pause();
    }

    public void ResumeMusic()
    {
        musicSource.UnPause();
    }

    private void OnDestroy()
    {
        // Unsubscribe from the sceneLoaded event when the object is destroyed
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}