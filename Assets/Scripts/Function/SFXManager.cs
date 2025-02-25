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

        sfxSource = GetComponent<AudioSource>();
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
    }

    public void MuteSFX(bool mute)
    {
        sfxSource.mute = mute;
    }
}