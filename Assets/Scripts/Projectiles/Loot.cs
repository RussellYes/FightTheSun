using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings.Switch;

public class Loot : MonoBehaviour
{
    public static event Action <float, float> PlayerGainsLootEvent;

    PlayerStatsManager playerStatsManager;

    private float metal;
    private float rareMetal;

    [SerializeField] private float lootRotationSpeedMin;
    [SerializeField] private float lootRotationSpeedMax;
    private float lootRotationSpeed;

    // Start is called before the first frame update
    void Start()
    {
        playerStatsManager = FindObjectOfType<PlayerStatsManager>();
        SetRotation();
        SetLootValues();
    }
    private void Update()
    {
        RotateSelf();
    }

    private void SetRotation()
    {
        // Initialize rotation speed with a random direction (-1 or +1)
        int randomDirection = UnityEngine.Random.Range(0, 2) * 2 - 1; // Generates -1 or +1
        lootRotationSpeed = UnityEngine.Random.Range(lootRotationSpeedMin, lootRotationSpeedMax) * randomDirection;
    }

    private void RotateSelf()
    {
        transform.Rotate(Vector3.forward * lootRotationSpeed * Time.deltaTime, Space.World);
    }

    private void SetLootValues()
    {
        float rollTheDice = UnityEngine.Random.Range(0, 101) + playerStatsManager.MiningSkill;
        if (rollTheDice > 95)
        {
            rareMetal = playerStatsManager.MiningSkill / UnityEngine.Random.Range (1f, 11f);
        }
        else
        {
            metal = playerStatsManager.MiningSkill / UnityEngine.Random.Range(1f, 6f);
        }
    }


    private void OnDestroy()
    {
        PlayerGainsLootEvent?.Invoke((float)metal, (float)rareMetal);

    }
}
