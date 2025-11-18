using System.Collections;
using UnityEngine;

// This script control obstacles spawning.
public class ObstacleSpawner : MonoBehaviour
{
    public enum SpawnerType
    {
        SingleObstacleSingleLocation,
        SingleObstacleRandomLocation,
        RandomObstacleSingleLocation,
        RandomObstacleRandomLocation,
        Special1,
        Boss1,
        Boss2,
        Boss3
    }

    public enum EventGroupType
    {
        None,
        Group1,
        Group2,
        Group3,
        Group4,
        Group5
    }

    [Header("Spawner Configuration")]
    [SerializeField] private SpawnerType spawnerType;
    [SerializeField] private EventGroupType eventGroupType;

    [Header("Obstacle creation")]
    private bool isSpawnerOn;
    [SerializeField] private GameObject[] obstacles;
    [SerializeField] private float[] obstacleProbabilities;
    [SerializeField] private Transform spawnLocation; // Changed to Transform for easier position access
    [SerializeField] private float laneDistance = 1f; // Distance between lanes
    [SerializeField] private int numberOfLanes = 5;
    [SerializeField] private float timeBetweenSpawningMin = 1f;
    [SerializeField] private float timeBetweenSpawningMax = 3f;
    private float minimumSpawnTime = 1f;
    private float spawnTimer;

    [Header("Event Group Prefabs")]
    [SerializeField] private GameObject groupPrefab;


    private void Start()
    {
        isSpawnerOn = true;
        spawnTimer = timeBetweenSpawningMin;
        if (obstacleProbabilities == null || obstacleProbabilities.Length != obstacles.Length)
        {
            InitializeDefaultProbabilities();
        }
    }
    private void InitializeDefaultProbabilities()
    {
        obstacleProbabilities = new float[obstacles.Length];
        float equalProbability = 1f / obstacles.Length;
        for (int i = 0; i < obstacleProbabilities.Length; i++)
        {
            obstacleProbabilities[i] = equalProbability;
        }
    }

    private void OnEnable()
    {
        GameManager.StopSpawning += TurnOffSpawner;
        GameManager.StartSpawning += TurnOnSpawner;
        DialogueManager.SpawnSpecialEvent += SpawnSpecial;
        DialogueManager.ShipGraveyardEvent += Boss3;
        Boss.StopSpawnersEvent += DelayedTurnOnSpawner;
        Boss.SpawnEventGroup1 += EventGroup1;
        Boss.SpawnEventGroup2 += EventGroup2;
        Boss.SpawnEventGroup3 += EventGroup3;
        Boss.SpawnEventGroup4 += EventGroup4;
        Boss.SpawnEventGroup5 += EventGroup5;
    }

    private void OnDisable()
    {
        GameManager.StopSpawning -= TurnOffSpawner;
        GameManager.StartSpawning -= TurnOnSpawner;
        DialogueManager.SpawnSpecialEvent -= SpawnSpecial;
        DialogueManager.ShipGraveyardEvent -= Boss3;
        Boss.StopSpawnersEvent -= DelayedTurnOnSpawner;
        Boss.SpawnEventGroup1 -= EventGroup1;
        Boss.SpawnEventGroup2 -= EventGroup2;
        Boss.SpawnEventGroup3 -= EventGroup3;
        Boss.SpawnEventGroup4 -= EventGroup4;
        Boss.SpawnEventGroup5 -= EventGroup5;
    }

    private void Update()
    {
        TimerForSpawning();
    }

    private void DelayedTurnOnSpawner()
    {
        StartCoroutine(DelayedSpawnerActivation()); // This will wait for 1 second before turning on the spawner during a boss fight.
    }

    IEnumerator DelayedSpawnerActivation()
    {
        yield return new WaitForSeconds(1f);
        TurnOnSpawner();
    }
    private void TurnOffSpawner()
    {
        isSpawnerOn = false;
        //Debug.Log("Spawners are off");
    }

    private void TurnOnSpawner()
    {
        isSpawnerOn = true;
        //Debug.Log("Spawners are on");
    }

    private void TimerForSpawning()
    {
        if (isSpawnerOn)
        {
            spawnTimer -= Time.deltaTime;

            if (spawnTimer <= 0)
            {
                SpawnObstacle();
                spawnTimer = Random.Range(timeBetweenSpawningMin, timeBetweenSpawningMax);
            }
        }
    }

    private void SpawnObstacle()
    {
        switch (spawnerType)
        {
            case SpawnerType.SingleObstacleSingleLocation:
                SpawnSingleObstacleSingleLocation();
                break;
            case SpawnerType.SingleObstacleRandomLocation:
                SpawnSingleObstacleRandomLocation();
                break;
            case SpawnerType.RandomObstacleSingleLocation:
                SpawnRandomObstacleSingleLocation();
                break;
            case SpawnerType.RandomObstacleRandomLocation:
                SpawnRandomObstacleRandomLocation();
                break;
            case SpawnerType.Special1:
            case SpawnerType.Boss1:
            case SpawnerType.Boss2:
                SpawnBossOrSpecial();
                break;
        }
    }

    private void SpawnBossOrSpecial()
    {
        if (obstacles.Length > 0 && obstacles[0] != null)
        {
            Instantiate(obstacles[0], spawnLocation.position, Quaternion.identity);
        }
    }

    private void SpawnSpecial()
    {
        if (spawnerType == SpawnerType.Special1)
        {
            SpawnBossOrSpecial();
        }
    }

    private void Boss3(GameObject boss3)
    {
        if (spawnerType == SpawnerType.Boss3)
        {
            StartCoroutine(SpawnObstaclesWithDelay(boss3));
        }
    }

    IEnumerator SpawnObstaclesWithDelay(GameObject boss3)
    {
        // Random number of obstacles to spawn (2-4)
        int numberOfObstacles = Random.Range(2, 5);

        for (int i = 0; i < numberOfObstacles; i++)
        {
            // Select the first obstacle from the array
            GameObject obstacleToSpawn = obstacles[0];

            // Instantiate the obstacle at the calculated position
            Instantiate(obstacleToSpawn, spawnLocation.position, Quaternion.identity);

            // Only wait if this isn't the last obstacle
            if (i < numberOfObstacles - 1)
            {
                // Wait for a random time between 1-3 seconds
                float waitTime = Random.Range(1f, 3f);
                yield return new WaitForSeconds(waitTime);
            }
        }
    }

    private void SpawnSingleObstacleSingleLocation()
    {
        Debug.Log("ObstacleSpawner SpawnSingleObstacleSingleLocation");
        if (obstacles.Length == 0) return;

        // Select the first obstacle from the array
        GameObject obstacleToSpawn = obstacles[0];

        // Instantiate the obstacle at the calculated position
        Instantiate(obstacleToSpawn, spawnLocation.position, Quaternion.identity);

    }

    private void SpawnSingleObstacleRandomLocation()
    {
        Debug.Log("ObstacleSpawner SpawnSingleObstacleRandomLocation");
        if (obstacles.Length == 0) return;

        // Randomly select an obstacle from the array
        GameObject obstacleToSpawn = obstacles[0];

        // Instantiate the obstacle at the calculated position
        Vector3 spawnPosition = CalculateRandomLanePosition();
        Instantiate(obstacleToSpawn, spawnPosition, Quaternion.identity);
    }
    
    private void SpawnRandomObstacleSingleLocation()
    {
        Debug.Log("ObstacleSpawner SpawnRandomObstacleSingleLocation");
        if (obstacles.Length == 0) return;

        GameObject obstacleToSpawn = GetRandomObstacleWithProbability();

        // Instantiate the obstacle at the spawner's position
        Instantiate(obstacleToSpawn, spawnLocation.position, Quaternion.identity);
    }
    
    private void SpawnRandomObstacleRandomLocation()
    {
        Debug.Log("ObstacleSpawner SpawnRandomObstacleRandomLocation");
        if (obstacles.Length == 0) return;

        GameObject obstacleToSpawn = GetRandomObstacleWithProbability();

        // Instantiate the obstacle at the calculated position
        Vector3 spawnPosition = CalculateRandomLanePosition();
        Instantiate(obstacleToSpawn, spawnPosition, Quaternion.identity);
    }

    private Vector3 CalculateRandomLanePosition()
    {
        int lane = Random.Range(0, numberOfLanes);
        Vector3 spawnPosition = spawnLocation.position;
        spawnPosition.x = (lane - (numberOfLanes - 1) / 2f) * laneDistance;
        return spawnPosition;
    }

    private GameObject GetRandomObstacleWithProbability()
    {
        // If probabilities aren't set up properly, fall back to random selection
        if (obstacleProbabilities == null || obstacleProbabilities.Length != obstacles.Length)
        {
            return obstacles[Random.Range(0, obstacles.Length)];
        }

        // Calculate total probability for normalization
        float totalProbability = 0f;
        foreach (float prob in obstacleProbabilities)
        {
            totalProbability += prob;
        }

        // Generate random value
        float randomValue = Random.Range(0f, totalProbability);
        float cumulativeProbability = 0f;

        // Select obstacle based on probability
        for (int i = 0; i < obstacles.Length; i++)
        {
            cumulativeProbability += obstacleProbabilities[i];
            if (randomValue <= cumulativeProbability)
            {
                return obstacles[i];
            }
        }

        // Fallback
        return obstacles[obstacles.Length - 1];
    }

    private GameObject GetEventGroupPrefab()
    {
        return eventGroupType switch
        {
            EventGroupType.Group1 => groupPrefab,
            EventGroupType.Group2 => groupPrefab,
            EventGroupType.Group3 => groupPrefab,
            EventGroupType.Group4 => groupPrefab,
            EventGroupType.Group5 => groupPrefab,
            _ => null
        };
    }

    private void SpawnEventGroup()
    {
        GameObject prefab = GetEventGroupPrefab();
        if (prefab != null)
        {
            Instantiate(prefab, spawnLocation.position, Quaternion.identity);
        }
    }

    private void EventGroup1() { if (eventGroupType == EventGroupType.Group1) SpawnEventGroup(); }
    private void EventGroup2() { if (eventGroupType == EventGroupType.Group2) SpawnEventGroup(); }
    private void EventGroup3() { if (eventGroupType == EventGroupType.Group3) SpawnEventGroup(); }
    private void EventGroup4() { if (eventGroupType == EventGroupType.Group4) SpawnEventGroup(); }
    private void EventGroup5() { if (eventGroupType == EventGroupType.Group5) SpawnEventGroup(); }

    public void ChangeTimeBetweenSpawning(float amount)
    {
        timeBetweenSpawningMax = Mathf.Max(minimumSpawnTime, timeBetweenSpawningMax + amount);
    }
}