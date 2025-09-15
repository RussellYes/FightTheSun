using UnityEngine;

// Stores GDPR consent in GameData
public static class ConsentStorage
{
    // Read saved consent (null if never chosen or save not ready)
    public static bool? Get()
    {
        var dp = DataPersister.Instance;
        var data = (dp != null) ? dp.CurrentGameData : null;

        if (data == null)
        {
            Debug.Log("ConsentStorage.Get: GameData not ready yet (DataPersister null or no CurrentGameData).");
            return null;
        }

        return data.gdprConsentSet ? data.gdprConsentValue : (bool?)null;
    }

    // Save consent persistently through your JSON SaveSystem.
    public static void Set(bool consent)
    {
        var dp = DataPersister.Instance;
        var data = (dp != null) ? dp.CurrentGameData : null;

        if (data == null)
        {
            Debug.LogWarning("ConsentStorage.Set: GameData not ready; cannot save consent yet.");
            return;
        }

        data.gdprConsentSet = true;
        data.gdprConsentValue = consent;

        // Write to disk using your existing save flow
        SaveSystem.SaveGame(data);
        Debug.Log($"ConsentStorage.Set: saved consent = {consent}");
    }
}
