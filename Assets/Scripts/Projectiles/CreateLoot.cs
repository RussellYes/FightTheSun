using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateLoot : MonoBehaviour
{
    [SerializeField] private GameObject lootPrefab;

    private void OnDestroy()
    {
        Instantiate(lootPrefab, transform.position, Quaternion.identity);
    }
}
