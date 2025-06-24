using UnityEngine;

public class StoreAccessUI : MonoBehaviour
{
    [SerializeField] private bool isPlanet2Store;
    [SerializeField] private GameObject lockedPlanet2Store;
    [SerializeField] private bool isPlanet3Store;
    [SerializeField] private GameObject lockedPlanet3Store;

    private void OnEnable()
    {
        DataPersister.InitializationComplete += OnDataInitialized;
    }

    private void OnDisable()
    {
        DataPersister.InitializationComplete -= OnDataInitialized;
    }

    private void OnDataInitialized()
    {
        if (lockedPlanet2Store != null)
            lockedPlanet2Store.SetActive(true);
        if (lockedPlanet3Store != null)
            lockedPlanet3Store.SetActive(true);

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



    }


}
