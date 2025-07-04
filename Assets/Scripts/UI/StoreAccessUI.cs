using UnityEngine;

public class StoreAccessUI : MonoBehaviour
{
    [SerializeField] private bool isPlanet2Store;
    [SerializeField] private GameObject lockedPlanet2Store;
    [SerializeField] private bool isPlanet3Store;
    [SerializeField] private GameObject lockedPlanet3Store;
    [SerializeField] private bool isPlanet4Store;
    [SerializeField] private GameObject lockedPlanet4Store;

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
        if (lockedPlanet2Store != null)
            lockedPlanet2Store.SetActive(true);
        if (lockedPlanet3Store != null)
            lockedPlanet3Store.SetActive(true);
        if (lockedPlanet4Store != null)
            lockedPlanet4Store.SetActive(true);


        if (isPlanet2Store)
        {
            if (DataPersister.Instance.CurrentGameData.isMission2Unlocked)
            {
                if (lockedPlanet2Store != null)
                {
                    lockedPlanet2Store.SetActive(false);
                }
            }
        }
        if (isPlanet3Store)
        {
            if (DataPersister.Instance.CurrentGameData.isMission3Unlocked)
            {
                if (lockedPlanet3Store != null)
                {
                    lockedPlanet3Store.SetActive(false);
                }
            }
        }
        if (isPlanet4Store)
        {
            if (DataPersister.Instance.CurrentGameData.isMission4Unlocked)
            {
                if (lockedPlanet4Store != null)
                {
                    lockedPlanet4Store.SetActive(false);
                }
            }
        }
    }

}
