using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    private bool isSpawnerOn;
    [SerializeField] private GameObject[] obstacles;
    [SerializeField] private Transform spawnLocation; // Changed to Transform for easier position access
    [SerializeField] private float laneDistance; // Distance between lanes
    [SerializeField] private float timeBetweenSpawning;
    [SerializeField] private float minimumSpawnTime;
    private float spawnTimer;

    private void Start()
    {
        isSpawnerOn = true;
        spawnTimer = timeBetweenSpawning;
    }

    private void OnEnable()
    {
        GameManager.StopSpawning += TurnOffSpawner;
        GameManager.StartSpawning += TurnOnSpawner;
    }

    private void OnDisable()
    {
        GameManager.StopSpawning -= TurnOffSpawner;
        GameManager.StartSpawning -= TurnOnSpawner;
    }

    private void Update()
    {
        TimerForSpawning();
    }

    private void TurnOffSpawner()
    {
        isSpawnerOn = false;
        Debug.Log("Spawners are off");
    }

    private void TurnOnSpawner()
    {
        isSpawnerOn = true;
        Debug.Log("Spawners are on");
    }

    private void TimerForSpawning()
    {
        if (isSpawnerOn)
        {
            spawnTimer -= Time.deltaTime;

            if (spawnTimer <= 0)
            {
                SpawnObstacle();
                spawnTimer = timeBetweenSpawning;
            }
        }
    }

    private void SpawnObstacle()
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
        timeBetweenSpawning += amt;

        if (timeBetweenSpawning <= minimumSpawnTime)
        {
            timeBetweenSpawning = minimumSpawnTime;
        }
    }
}