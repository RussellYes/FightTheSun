using UnityEngine;

public class MiningMissileLauncher : MonoBehaviour
{
    [Header("Prefab")]
    [SerializeField] private MiningMissile miningMissilePrefab;

    [Header("Firing Settings")]
    [SerializeField] private float reloadTime = 1f;
    [SerializeField] private float miningMissileSpeed = 5f;
    [SerializeField] private float initialDelay = 0.5f;

    private float reloadCountdown;
    private int currentFireMode = 0;
    private bool keyPressProcessed = false;

    private void Start()
    {
        reloadCountdown = initialDelay;
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
                keyPressProcessed = true;
            }
        }
        else
        {
            keyPressProcessed = false;
        }
    }

    private void FireMissiles()
    {
        switch (currentFireMode)
        {
            case 0: // Single shot (0°)
                FireMissileAtAngle(0f);
                break;

            case 1: // Dual shots (±10°)
                FireMissileAtAngle(-10f);
                FireMissileAtAngle(10f);
                break;

            case 2: // Triple shots (0°, ±15°)
                FireMissileAtAngle(-15f);
                FireMissileAtAngle(0f);
                FireMissileAtAngle(15f);
                break;

            case 3: // Quad shots (±10°, ±25°)
                FireMissileAtAngle(-25f);
                FireMissileAtAngle(-10f);
                FireMissileAtAngle(10f);
                FireMissileAtAngle(25f);
                break;

            case 4: // Penta shots (0°, ±15°, ±30°)
                FireMissileAtAngle(-30f);
                FireMissileAtAngle(-15f);
                FireMissileAtAngle(0f);
                FireMissileAtAngle(15f);
                FireMissileAtAngle(30f);
                break;
        }
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