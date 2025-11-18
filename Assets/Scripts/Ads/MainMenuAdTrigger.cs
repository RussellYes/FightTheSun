using UnityEngine;

public class MainMenuAdTrigger : MonoBehaviour
{
    private void Start()
    {
        InitializeGame();
    }

    void InitializeGame()
    {
        // Small delay to ensure everything is initialized
        Invoke(nameof(PrewarmAds), 1f);
    }

    void PrewarmAds()
    {
        var adLoader = AdLoader.Instance;
        if (adLoader != null)
        {
            Debug.Log("MainMenuAdTrigger PrewarmAds - Prewarming ad system");
            SimpleAdLogger.Instance?.Log("PREWARM", "MainMenuAdTrigger Prewarming - ad system");
            adLoader.PrewarmAdSystem();
        }
        else
        {
            Debug.LogWarning("MainMenuAdTrigger PrewarmAds - AdLoader instance not found");
            SimpleAdLogger.Instance?.Log("PREWARM", "MainMenuAdTrigger PrewarmAds - AdLoader instance not found");
        }
    }
}


