using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This script spawns loot when called.

public class CreateLoot : MonoBehaviour
{
    [SerializeField] private GameObject[] lootPrefab;

    public void SpawnLoot()
    {
        if (lootPrefab == null || lootPrefab.Length == 0) return;

        foreach (GameObject prefab in lootPrefab)
        {
            if (prefab != null)
            {
                    Instantiate(prefab, transform.position, Quaternion.identity);
            }
        }
    }
}