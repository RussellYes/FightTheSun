using UnityEngine;

// This script saves and retrieves consent for personalized ads using player prefs. This is where AdsBootstrap looks.

public static class ConsentStorage
{
    private const string PP_KEY = "gdpr_consent_value"; // "true" or "false"
    private const string PP_HAS = "gdpr_consent_saved"; // 1 or 0

    // Read saved consent (null if never set)
    public static bool? Get()
    {
        // Prefer DataPersister if your save is loaded
        if (DataPersister.Instance != null && DataPersister.Instance.CurrentGameData != null)
        {
            // If you later add a field to GameData (e.g., public bool gdprConsentSet; public bool gdprConsentValue;)
            // read it here instead of PlayerPrefs.
        }

        if (PlayerPrefs.GetInt(PP_HAS, 0) == 1)
            return PlayerPrefs.GetString(PP_KEY, "false") == "true";
        return null;
    }

    // Save consent persistently (and immediately flush)
    public static void Set(bool consent)
    {
        // If you add fields to GameData later, set them here too, then call DataPersister.Instance.SaveCurrentGame();
        PlayerPrefs.SetString(PP_KEY, consent ? "true" : "false");
        PlayerPrefs.SetInt(PP_HAS, 1);
        PlayerPrefs.Save();
    }
}
