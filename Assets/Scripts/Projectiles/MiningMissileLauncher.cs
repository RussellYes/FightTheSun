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
    [SerializeField] private Sprite Launcher1;
    [SerializeField] private Sprite Launcher2;
    [SerializeField] private Sprite Launcher3;
    [SerializeField] private Sprite Launcher4;
    [SerializeField] private Sprite Launcher5;

    private void Start()
    {
        missileCount = 0;
        launcherLevel = 0;
        isLauncherActive = false;
        UpdateLauncherVisual();
    }

    private void OnEnable()
    {
        ShipUIManager.FireMissilesEvent += FireMissiles;
        ObstacleMovement.MissilePickupEvent += MissilePickUp;
    }
    private void OnDisable()
    {
        ShipUIManager.FireMissilesEvent -= FireMissiles;
        ObstacleMovement.MissilePickupEvent -= MissilePickUp;
    }
    private void MissilePickUp()
    {
        isLauncherActive = true;
        missileCount += 6;
        UpdateLauncherLevel();
        UpdateLauncherVisual();
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
        }
    }
    private void UpdateLauncherLevel()
    {
        if (launcherLevel == 0)
        {
            spriteRenderer.sprite = Launcher1;
            launcherLevel = 1;
        }
        else if (launcherLevel == 1)
        {
            spriteRenderer.sprite = Launcher2;
            launcherLevel = 2;
        }
        else if (launcherLevel == 2)
        {
            spriteRenderer.sprite = Launcher3;
            launcherLevel = 3;
        }
        else if (launcherLevel == 3)
        {
            spriteRenderer.sprite = Launcher4;
            launcherLevel = 4;
        }
        else if (launcherLevel >= 4)
        {
            spriteRenderer.sprite = Launcher5;
            launcherLevel = 5;
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