using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;

    private AudioSource musicSource;
    private float musicVolume = 1f; // Default volume

    // Array to hold music clips for each scene
    public AudioClip[] sceneMusicClips;

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
        }

        musicSource = GetComponent<AudioSource>();
        if (musicSource == null)
        {
            Debug.Log("MusicManager: No AudioSource found on the GameObject.");

            // Ensure the AudioSource is enabled
            musicSource.enabled = true;

            musicSource = GetComponent<AudioSource>();
        }

        // Ensure the AudioSource is enabled
        musicSource.enabled = true;

        // Subscribe to the sceneLoaded event
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Check if the scene index is within the bounds of the array
        if (scene.buildIndex < sceneMusicClips.Length)
        {
            PlayMusic(sceneMusicClips[scene.buildIndex]);
        }
        else
        {
            Debug.LogWarning($"No music clip found for scene index {scene.buildIndex}.");
        }
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

    public void SetMusicVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);
        musicSource.volume = musicVolume;
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