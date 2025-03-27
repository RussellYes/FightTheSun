using UnityEngine;

public class MiningMissileLauncher : MonoBehaviour
{
    [Header("Prefab")]
    [SerializeField] private MiningMissile miningMissilePrefab;

    [Header("Firing Settings")]
    [SerializeField] private float reloadTime = 1f;
    [SerializeField] private float miningMissileSpeed = 5f;
    [SerializeField] private float fireCone = 30f; // Degrees (±15° from launcher's "up")
    [SerializeField] private float initialDelay = 0.5f; // First-shot delay

    private float reloadCountdown;

    private void Start()
    {
        reloadCountdown = initialDelay;
    }

    private void Update()
    {
        ReloadCountdown();
    }

    private void ReloadCountdown()
    {
        if (reloadCountdown > 0)
        {
            reloadCountdown -= Time.deltaTime;
        }
        else
        {
            FireMiningMissile();
            reloadCountdown = reloadTime; // Reset timer
        }
    }

    private void FireMiningMissile()
    {
        // Calculate random direction within fireCone (±fireCone/2 degrees)
        float randomAngle = Random.Range(-fireCone * 0.5f, fireCone * 0.5f);
        Quaternion spreadRotation = Quaternion.Euler(0, 0, randomAngle);
        Vector2 fireDirection = spreadRotation * transform.up;

        // Instantiate and initialize missile
        MiningMissile missile = Instantiate(
            miningMissilePrefab, 
            transform.position, 
            Quaternion.identity
        );

        // Set velocity via Rigidbody2D (matches your existing missile behavior)
        Rigidbody2D rb = missile.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = fireDirection * miningMissileSpeed;
        }
    }
}