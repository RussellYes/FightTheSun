using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This script spawns loot when called.

public class CreateLoot : MonoBehaviour
{
    SFXManager SFXManager => SFXManager.Instance;
    PlayerStatsManager PlayerStatsManager => PlayerStatsManager.Instance;

    [SerializeField] private GameObject[] lootPrefab;

    [SerializeField] private float miningTime;
    [SerializeField] private AudioClip[] minedSFX;

    public float MiningTime { get { return miningTime; }}


    private void OnEnable()
    {
        DataPersister.InitializationComplete += Initialize;
    }
    private void OnDisable()
    {
        DataPersister.InitializationComplete -= Initialize;
    }
    private void Initialize()
    {
        miningTime -= PlayerStatsManager.MiningSkill;
        if (miningTime <= 0.5f)
        {
            miningTime = 0.5f;
        }
    }

    public void SpawnLoot()
    {
        if (lootPrefab == null || lootPrefab.Length == 0) return;

        foreach (GameObject prefab in lootPrefab)
        {
            if (prefab != null)
            {
                // Play the mined sound effect
                if (minedSFX != null && SFXManager != null)
                {
                    SFXManager.PlaySFX(minedSFX[UnityEngine.Random.Range(0, minedSFX.Length)]);
                }
                Instantiate(prefab, transform.position, Quaternion.identity);
            }
        }
    }
}