using UnityEngine;

public class StoreAccessUI : MonoBehaviour
{
    public enum StorePlanet
    {
        Planet1Store = 1,
        Planet2Store = 2,
        Planet3Store = 3,
        Planet4Store = 4,
        Planet5Store = 5
    }

    [SerializeField] private StorePlanet storePlanet;
    [SerializeField] private GameObject storeButton;
    [SerializeField] private GameObject lockImage;

    private void OnEnable()
    {
        DataPersister.InitializationComplete += OnDataInitialized;
        LevelUnlockerUI.LevelUnlockedEvent += OnLevelUnlocked;
    }

    private void OnDisable()
    {
        DataPersister.InitializationComplete -= OnDataInitialized;
        LevelUnlockerUI.LevelUnlockedEvent -= OnLevelUnlocked;
    }

    private void OnDataInitialized()
    {
        UpdateLevelLock();
    }

    private void OnLevelUnlocked()
    {
        UpdateLevelLock();
    }

    private void UpdateLevelLock()
    {
        if (storeButton == null || lockImage == null) return;

        bool isUnlocked = IsStoreUnlocked();

        storeButton.SetActive(isUnlocked);
        lockImage.SetActive(!isUnlocked);
    }

    private bool IsStoreUnlocked()
    {
        if (DataPersister.Instance == null) return false;

        int planetNumber = (int)storePlanet;

        // Planet 1 is always unlocked
        if (planetNumber == 1) return true;

        bool isStoreUnlocked = DataPersister.Instance.CurrentGameData.GetMissionUnlocked(planetNumber);

        Debug.Log("StoreAccessUI IsStoreUnlocked: " + $"Store {planetNumber} unlocked: {isStoreUnlocked}");
        return isStoreUnlocked;
    }
}
