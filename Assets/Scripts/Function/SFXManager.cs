using UnityEngine;

public class SFXManager : MonoBehaviour
{
    public static SFXManager Instance;

    private AudioSource sfxSource;
    private float sfxVolume = 1f; // Default volume

    public float GetSFXVolume()
    {
        return sfxVolume;
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

        // Initialize the AudioSource component
        sfxSource = GetComponent<AudioSource>();
        if (sfxSource == null)
        {
            Debug.LogError("SFXManager: No AudioSource found on the GameObject.");
            sfxSource = gameObject.AddComponent<AudioSource>(); // Add AudioSource if missing
        }

        // Ensure the AudioSource is enabled
        sfxSource.enabled = true;

        // Load saved SFX volume
        sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 1f); // Default to 1 if no saved value exists
        sfxSource.volume = sfxVolume;

        Debug.Log("SFXManager: Initialized in Awake.");
    }
    private void Start()
    {
        MuteSFX(false);
    }

    public void PlaySFX(AudioClip sfxClip)
    {
        sfxSource.PlayOneShot(sfxClip, sfxVolume);
    }

    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
        sfxSource.volume = sfxVolume;
        // Save the SFX volume to PlayerPrefs
        PlayerPrefs.SetFloat("SFXVolume", sfxVolume);
        PlayerPrefs.Save(); // Ensure the data is saved immediately
    }

    public void MuteSFX(bool mute)
    {
        sfxSource.mute = mute;
    }
}