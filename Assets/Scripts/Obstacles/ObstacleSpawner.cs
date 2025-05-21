using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This script control obstacles spawning.
public class ObstacleSpawner : MonoBehaviour
{
    private GameManager gameManager;


    [Header("Spawner type")]
    [SerializeField] private bool isSpawnSpecial1;
    [SerializeField] private bool isSpawnBoss1;
    [SerializeField] private bool isSpawnBoss2;
    [SerializeField] private bool isSpawnBoss3;
    [SerializeField] private bool isSpawnSingleObstacleSingleLocation;
    [SerializeField] private bool isSpawnSingleObstacleRandomLocation;
    [SerializeField] private bool isSpawnRandomObstacleSingleLocation;
    [SerializeField] private bool isSpawnRandomObstacleRandomLocation;
    
    [Header("Obstacle creation")]
    private bool isSpawnerOn;
    [SerializeField] private GameObject[] obstacles;
    [SerializeField] private Transform spawnLocation; // Changed to Transform for easier position access
    [SerializeField] private float laneDistance; // Distance between lanes
    [SerializeField] private float timeBetweenSpawningMin;
    [SerializeField] private float timeBetweenSpawningMax;
    private float minimumSpawnTime = 1f;
    private float spawnTimer;

    [Header("SpawnerEventGroups")]
    [SerializeField] private bool isSpawnEventGroup1;
    [SerializeField] private bool isSpawnEventGroup2;
    [SerializeField] private bool isSpawnEventGroup3;
    [SerializeField] private bool isSpawnEventGroup4;
    [SerializeField] private bool isSpawnEventGroup5;
    [SerializeField] private GameObject group1Prefab;
    [SerializeField] private GameObject group2Prefab;
    [SerializeField] private GameObject group3Prefab;
    [SerializeField] private GameObject group4Prefab;
    [SerializeField] private GameObject group5Prefab;

    private void Start()
    {
        gameManager = FindAnyObjectByType<GameManager>();

        isSpawnerOn = true;
        spawnTimer = timeBetweenSpawningMin;
    }

    private void OnEnable()
    {
        GameManager.StopSpawning += TurnOffSpawner;
        GameManager.StartSpawning += TurnOnSpawner;
        DialogueManager.SpawnSpecialEvent.AddListener(SpawnSpecial);
        DialogueManager.ShipGraveyardEvent += Boss3;
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
        DialogueManager.SpawnSpecialEvent.RemoveListener(SpawnSpecial);
        DialogueManager.ShipGraveyardEvent -= Boss3;
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
        if (isSpawnSingleObstacleSingleLocation)
        {
            SpawnSingleObstacleSingleLocation();
        }
        else if (isSpawnSingleObstacleRandomLocation)
        {
            SpawnSingleObstacleRandomLocation();
        }
        else if (isSpawnRandomObstacleSingleLocation)
        {
            SpawnRandomObstacleSingleLocation();
        }
        else if (isSpawnRandomObstacleRandomLocation)
        {
            SpawnRandomObstacleRandomLocation();
        }
    }

    private void SpawnSpecial()
    {
        if (isSpawnSpecial1)
        {
            // Select the first obstacle from the array
            GameObject obstacleToSpawn = obstacles[0];

            // Instantiate the obstacle at the calculated position
            Instantiate(obstacleToSpawn, spawnLocation.position, Quaternion.identity);
        }
        if (isSpawnBoss1)
        {
            // Select the first obstacle from the array
            GameObject obstacleToSpawn = obstacles[0];

            // Instantiate the obstacle at the calculated position
            Instantiate(obstacleToSpawn, spawnLocation.position, Quaternion.identity);
        }
        if (isSpawnBoss2)
        {
            // Select the first obstacle from the array
            GameObject obstacleToSpawn = obstacles[0];

            // Instantiate the obstacle at the calculated position
            Instantiate(obstacleToSpawn, spawnLocation.position, Quaternion.identity);
        }
    }

    private void Boss3(GameObject boss3)
    {
        StartCoroutine(SpawnObstaclesWithDelay(boss3));
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
        Debug.Log("SpawnSingleObstacleSingleLocation");
        if (isSpawnSpecial1)
        {
            SpawnSingleObstacleSingleLocation();
        }
        // Select the first obstacle from the array
        GameObject obstacleToSpawn = obstacles[0];

        // Instantiate the obstacle at the calculated position
        Instantiate(obstacleToSpawn, spawnLocation.position, Quaternion.identity);

    }

    private void SpawnSingleObstacleRandomLocation()
    {
        // Randomly select an obstacle from the array
        GameObject obstacleToSpawn = obstacles[0];

        // Randomly select a lane (0 to 4)
        int lane = Random.Range(0, 5);

        // Calculate the spawn position based on the lane
        Vector3 spawnPosition = spawnLocation.position;
        spawnPosition.x = (lane - 2) * laneDistance; // Adjust x position based on lane

        // Instantiate the obstacle at the calculated position
        Instantiate(obstacleToSpawn, spawnPosition, Quaternion.identity);
    }
    
    private void SpawnRandomObstacleSingleLocation()
    {
        // Randomly select an obstacle from the array
        GameObject obstacleToSpawn = obstacles[Random.Range(0, obstacles.Length)];

        // Randomly select a lane (0 to 4)
        int lane = Random.Range(0, 5);

        // The spawn position
        Vector3 spawnPosition = spawnLocation.position;

        // Instantiate the obstacle at the calculated position
        Instantiate(obstacleToSpawn, spawnPosition, Quaternion.identity);
    }
    
    private void SpawnRandomObstacleRandomLocation()
    {
        // Randomly select an obstacle from the array
        GameObject obstacleToSpawn = obstacles[Random.Range(0, obstacles.Length)];

        // Randomly select a lane (0 to 4)
        int lane = Random.Range(0, 5);

        // Calculate the spawn position based on the lane
        Vector3 spawnPosition = spawnLocation.position;
        spawnPosition.x = (lane - 2) * laneDistance; // Adjust x position based on lane

        // Instantiate the obstacle at the calculated position
        Instantiate(obstacleToSpawn, spawnPosition, Quaternion.identity);
    }

    public void ChangeTimeBetweenSpawning(float amt)
    {
        timeBetweenSpawningMax += amt;

        if (timeBetweenSpawningMax <= minimumSpawnTime)
        {
            timeBetweenSpawningMax = minimumSpawnTime;
        }
    }

    private void EventGroup1()
    {
        if (isSpawnEventGroup1 && group1Prefab != null)
        {
            Instantiate(group1Prefab, spawnLocation.position, Quaternion.identity);
        }
    }
    private void EventGroup2()
    {
        if (isSpawnEventGroup2 && group2Prefab != null)
        {
            Instantiate(group2Prefab, spawnLocation.position, Quaternion.identity);
        }
    }
    private void EventGroup3()
    {
        if (isSpawnEventGroup3 && group3Prefab != null)
        {
            Instantiate(group3Prefab, spawnLocation.position, Quaternion.identity);
        }
    }
    private void EventGroup4()
    {
        if (isSpawnEventGroup4 && group4Prefab != null)
        {
            Instantiate(group4Prefab, spawnLocation.position, Quaternion.identity);
        }
    }
    private void EventGroup5()
    {
        if (isSpawnEventGroup5 && group5Prefab != null)
        {
            Instantiate(group5Prefab, spawnLocation.position, Quaternion.identity);
        }
    }

}