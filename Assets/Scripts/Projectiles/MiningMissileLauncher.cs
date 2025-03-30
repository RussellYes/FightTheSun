using UnityEngine;

public class MiningMissileLauncher : MonoBehaviour
{
    [Header("Prefab")]
    [SerializeField] private MiningMissile miningMissilePrefab;

    [Header("Firing Settings")]
    [SerializeField] private float reloadTime;
    [SerializeField] private float miningMissileSpeed;
    [SerializeField] private float initialDelay;

    [Header("Visuals")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Sprite Launcher1;
    [SerializeField] private Sprite Launcher2;
    [SerializeField] private Sprite Launcher3;
    [SerializeField] private Sprite Launcher4;
    [SerializeField] private Sprite Launcher5;

    private float reloadCountdown;
    private int currentFireMode = 0;
    private bool keyPressProcessed = false;

    private void Start()
    {
        reloadCountdown = initialDelay;
        UpdateLauncherVisual(); // Initialize visual
        currentFireMode = 4;
    }

    private void Update()
    {
        ReloadCountdown();
        HandleFireModeToggle();
    }

    private void ReloadCountdown()
    {
        if (reloadCountdown > 0)
        {
            reloadCountdown -= Time.deltaTime;
        }
        else
        {
            FireMissiles();
            reloadCountdown = reloadTime;
        }
    }

    private void HandleFireModeToggle()
    {
        if (Input.GetKey(KeyCode.T))
        {
            if (!keyPressProcessed)
            {
                currentFireMode = (currentFireMode + 1) % 5;
                Debug.Log($"Switched to Mode {currentFireMode + 1}");
                UpdateLauncherVisual(); // Update visual immediately
                keyPressProcessed = true;
            }
        }
        else
        {
            keyPressProcessed = false;
        }
    }

    private void UpdateLauncherVisual()
    {
        if (spriteRenderer == null)
        {
            Debug.LogWarning("SpriteRenderer is not assigned!");
            return;
        }

        switch (currentFireMode)
        {
            case 0: spriteRenderer.sprite = Launcher1; break;
            case 1: spriteRenderer.sprite = Launcher2; break;
            case 2: spriteRenderer.sprite = Launcher3; break;
            case 3: spriteRenderer.sprite = Launcher4; break;
            case 4: spriteRenderer.sprite = Launcher5; break;
        }
    }

    private void FireMissiles()
    {
        switch (currentFireMode)
        {
            case 0: FireMissileAtAngle(0f); break;
            case 1: FireDualMissiles(); break;
            case 2: FireTripleMissiles(); break;
            case 3: FireQuadMissiles(); break;
            case 4: FirePentaMissiles(); break;
        }
    }

    private void FireDualMissiles()
    {
        FireMissileAtAngle(-10f);
        FireMissileAtAngle(10f);
    }

    private void FireTripleMissiles()
    {
        FireMissileAtAngle(-15f);
        FireMissileAtAngle(0f);
        FireMissileAtAngle(15f);
    }

    private void FireQuadMissiles()
    {
        FireMissileAtAngle(-25f);
        FireMissileAtAngle(-10f);
        FireMissileAtAngle(10f);
        FireMissileAtAngle(25f);
    }

    private void FirePentaMissiles()
    {
        FireMissileAtAngle(-30f);
        FireMissileAtAngle(-15f);
        FireMissileAtAngle(0f);
        FireMissileAtAngle(15f);
        FireMissileAtAngle(30f);
    }

    private void FireMissileAtAngle(float angle)
    {
        Quaternion spreadRotation = Quaternion.Euler(0, 0, angle);
        Vector2 fireDirection = spreadRotation * transform.up;

        MiningMissile missile = Instantiate(
            miningMissilePrefab,
            transform.position,
            Quaternion.identity
        );

        if (missile.TryGetComponent<Rigidbody2D>(out var rb))
        {
            rb.velocity = fireDirection * miningMissileSpeed;
        }
    }
}