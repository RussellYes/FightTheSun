using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This script spawns projectiles.

public class ProjectileSpawner : MonoBehaviour
{
    private bool isProjectilesOn;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform spawnLocation; // Changed to Transform for easier position access

    [SerializeField] private float timeBetweenSpawning;
    [SerializeField] private float minimumSpawnTime;
    private float spawnTimer;


    private void Start()
    {
        isProjectilesOn = true;
        spawnTimer = timeBetweenSpawning;
    }

    private void OnEnable()
    {
        GameManager.StopSpawning += TurnOffProjectiles;
        GameManager.StartSpawning += TurnOnProjectiles;
    }

    private void OnDisable()
    {
        GameManager.StopSpawning -= TurnOffProjectiles;
        GameManager.StartSpawning -= TurnOnProjectiles;
    }

    private void Update()
    {
        TimerForSpawning();
    }

    private void TurnOffProjectiles()
    {
        isProjectilesOn = false;
        Debug.Log("Spawners are off");
    }

    private void TurnOnProjectiles()
    {
        isProjectilesOn = true;
        Debug.Log("Spawners are on");
    }

    private void TimerForSpawning()
    {
        if (isProjectilesOn)
        {
            spawnTimer -= Time.deltaTime;

            if (spawnTimer <= 0)
            {
                SpawnProjectiles();
                spawnTimer = timeBetweenSpawning;
            }
        }
    }

    private void SpawnProjectiles()
    {
        // Instantiate the Projectile at the calculated position
        Instantiate(projectilePrefab, spawnLocation.position, Quaternion.identity);
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
