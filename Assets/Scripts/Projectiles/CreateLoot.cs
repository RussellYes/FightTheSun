using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateLoot : MonoBehaviour
{
    [SerializeField] private GameObject[] lootPrefab;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Destroy(gameObject);
        }
    }



    private void OnDestroy()
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