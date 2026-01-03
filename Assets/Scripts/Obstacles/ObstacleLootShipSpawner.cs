using UnityEngine;

public class ObstacleLootShipSpawner : MonoBehaviour
{
    [SerializeField] private GameObject lootShipPrefab;
    [SerializeField] private int levelNumber;
    private float lootShipSpawnTimer;
    private float lootShipSpawnTimerCoutdown;
    private bool isInitialized = false;

    private void OnEnable()
    {
        DataPersister.InitializationComplete += HandleInitializationComplete;
    }
    private void OnDisable()
    {
        DataPersister.InitializationComplete -= HandleInitializationComplete;
    }

    private void HandleInitializationComplete()
    {
        int lootShipLevel = DataPersister.Instance.CurrentGameData.randomLootShipLevel;
        if (lootShipLevel == 0) return;
        if (lootShipLevel == levelNumber)
        {
            lootShipSpawnTimer = Random.Range(8f, 60f);
            isInitialized = true;
        }
        Debug.Log($"ObstacleLootShipSpawner HandleInitializationComplete - Loot ship spawner is active: {isInitialized} with countdown {lootShipSpawnTimer}");
    }

    private void Update()
    {
        if (isInitialized)
        {
            lootShipSpawnTimerCoutdown -= Time.deltaTime;
            if (lootShipSpawnTimerCoutdown <= 0f)
            {
                SpawnLootShip();
            }
        }
    }

    private void SpawnLootShip()
    {
        Instantiate(lootShipPrefab, transform.position, Quaternion.identity);
    }
}
