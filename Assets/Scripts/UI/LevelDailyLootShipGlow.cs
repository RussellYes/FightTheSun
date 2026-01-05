using UnityEngine;

public class LevelDailyLootShipGlow : MonoBehaviour
{
    [SerializeField] private int levelNumber;
    [SerializeField] private GameObject glowObject;
    private bool isInitialized = false;

    private void OnEnable()
    {
        DataPersister.InitializationComplete += HandleInitializationComplete;
    }
    private void OnDisable()
    {
        DataPersister.InitializationComplete -= HandleInitializationComplete;
    }

    private void Update()
    {
        if (isInitialized && levelNumber == DataPersister.Instance.CurrentGameData.randomLootShipLevel)
        {
            glowObject.SetActive(true);
        }
        else
        {
            glowObject.SetActive(false);
        }
    }

    private void HandleInitializationComplete()
    {
        isInitialized = true;
    }
}
