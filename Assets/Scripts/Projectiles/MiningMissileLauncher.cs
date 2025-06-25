using System;
using TMPro;
using Unity.Hierarchy;
using UnityEngine;
using UnityEngine.UI;

// This script spawns mining missiles.

public class MiningMissileLauncher : MonoBehaviour
{
    public static event Action <int> LauncherActiveEvent;

    [Header("Prefab")]
    [SerializeField] private MiningMissile miningMissilePrefab;

    [Header("Firing Settings")]
    [SerializeField] private float miningMissileSpeed;
    private float fireAngle;
    [SerializeField] private int missileCount;

    [Header("Visuals")]
    private bool isLauncherActive;
    private int launcherLevel;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Sprite launcher1;
    [SerializeField] private Sprite launcher2;
    [SerializeField] private Sprite launcher3;
    [SerializeField] private Sprite launcher4;
    [SerializeField] private Sprite launcher5;

    public float MissileCount => missileCount;


    private void OnDataInitialized()
    {
        if (DataPersister.Instance != null && DataPersister.Instance.CurrentGameData != null)
        {
            missileCount = DataPersister.Instance.CurrentGameData.savedMissileCount;
            launcherLevel = DataPersister.Instance.CurrentGameData.savedLauncherLevel;
            isLauncherActive = DataPersister.Instance.CurrentGameData.savedLauncherActive;
        }
        else 
        {
            missileCount = 0;
            launcherLevel = 1;
            isLauncherActive = false;
        }

        UpdateLauncherVisual();
    }

    private void OnEnable()
    {
        ShipUIManager.FireMissilesEvent += FireMissiles;
        ObstacleMovement.MissilePickupEvent += MissilePickUp;
        DataPersister.InitializationComplete += OnDataInitialized;
    }
    private void OnDisable()
    {
        ShipUIManager.FireMissilesEvent -= FireMissiles;
        ObstacleMovement.MissilePickupEvent -= MissilePickUp;
        DataPersister.InitializationComplete -= OnDataInitialized;
        SaveData();
    }

    private void SaveData()
    {
        if (DataPersister.Instance != null && DataPersister.Instance.CurrentGameData != null)
        {
            DataPersister.Instance.CurrentGameData.savedMissileCount = missileCount;
            DataPersister.Instance.CurrentGameData.savedLauncherLevel = launcherLevel;
            DataPersister.Instance.CurrentGameData.savedLauncherActive = isLauncherActive;
        }
    }
    private void MissilePickUp(int amt)
    {
        isLauncherActive = true;
        missileCount += amt;
        UpdateLauncherVisual();
        SaveData();
    }
    private void UpdateLauncherVisual()
    {
        if (!isLauncherActive)
        {
            LauncherActiveEvent?.Invoke(missileCount);
            spriteRenderer.enabled = false;
            return;
        }
        if (isLauncherActive)
        {
            LauncherActiveEvent?.Invoke(missileCount);
            if (spriteRenderer == null)
            {
                Debug.LogWarning("MiningMissileLauncher: No SpriteRenderer found.");
                return;
            }

            spriteRenderer.enabled = true;
            if (launcherLevel == 1)
            {
                spriteRenderer.sprite = launcher1;
            }
            else if (launcherLevel == 2)
            {
                spriteRenderer.sprite = launcher2;
            }
            else if (launcherLevel == 3)
            {
                spriteRenderer.sprite = launcher3;
            }
            else if (launcherLevel == 4)
            {
                spriteRenderer.sprite = launcher4;
            }
            else if (launcherLevel >= 5)
            {
                spriteRenderer.sprite = launcher5;
            }
        }
    }


    private void FireMissiles()
    {
        if (missileCount <= 0)
        {
            return;
        }
        if (missileCount >= 1)
        {
            missileCount--;
            LauncherActiveEvent?.Invoke(missileCount);

            if (launcherLevel == 1)
            {
                FireMissileAtAngle(UnityEngine.Random.Range(0f, 5f));
            }
            else if (launcherLevel == 2)
            {
                FireDualMissiles();
            }
            else if (launcherLevel == 3)
            {
                FireTripleMissiles();
            }
            else if (launcherLevel == 4)
            {
                FireQuadMissiles();
            }
            else if (launcherLevel >= 5)
            {
                FirePentaMissiles();
            }
        }
    }

    private void FireDualMissiles()
    {
        fireAngle = UnityEngine.Random.Range(5f, 10f);
        FireMissileAtAngle(-fireAngle);
        FireMissileAtAngle(fireAngle);
    }

    private void FireTripleMissiles()
    {
        fireAngle = UnityEngine.Random.Range(5f, 10f);
        FireMissileAtAngle(-(fireAngle + 5));
        FireMissileAtAngle(0f);
        FireMissileAtAngle((fireAngle + 5));
    }

    private void FireQuadMissiles()
    {
        fireAngle = UnityEngine.Random.Range(5f, 10f);
        FireMissileAtAngle(-(fireAngle + 10));
        FireMissileAtAngle(-fireAngle);
        FireMissileAtAngle(fireAngle);
        FireMissileAtAngle((fireAngle + 10));
    }

    private void FirePentaMissiles()
    {
        fireAngle = UnityEngine.Random.Range(5f, 10f);
        FireMissileAtAngle(-(fireAngle + 15));
        FireMissileAtAngle(-(fireAngle + 5));
        FireMissileAtAngle(0f);
        FireMissileAtAngle((fireAngle + 5));
        FireMissileAtAngle((fireAngle + 15));
    }

    private void FireMissileAtAngle(float angle)
    {
        Quaternion spreadRotation = Quaternion.Euler(0, 0, angle);
        Vector2 fireDirection = spreadRotation * transform.up;

        Transform playerTransform = transform.root; // Gets the parent object

        MiningMissile missile = Instantiate(
            miningMissilePrefab,
            playerTransform.position,
            Quaternion.identity
        );

        if (missile.TryGetComponent<Rigidbody2D>(out var rb))
        {
            rb.linearVelocity = fireDirection * miningMissileSpeed;
        }
    }
}