using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

// This script controls the shooting behavior of hostile objects in the game.

public class HostileShoot : MonoBehaviour
{
    private SFXManager sFXManager;

    [SerializeField] private bool isStraightShooter;
    [SerializeField] private bool canTargetPlayer;
    [SerializeField] private bool isShootingBurst;

    [SerializeField] private float shotRange;

    [SerializeField] private float reloadTime = 1f;
    private float reloadCountdownTime;

    [SerializeField] private GameObject bulletPreFab;
    [SerializeField] private float bulletSpeed = 10f;

    [SerializeField] private float burstDelay = 0.1f; // Delay between shots in the burst
    [SerializeField] private int burstCount = 3; // Number of shots in the burst

    [SerializeField] private float rotationSpeed = 1000f; // Speed of rotation in degrees per second

    [SerializeField] private Animator cannonAnim;
    [SerializeField] private AudioClip[] cannonSounds;

    private Transform target;

    private void Start()
    {
        Debug.Log("Shoot Start");
        reloadCountdownTime = 0.3f;

        sFXManager = FindAnyObjectByType<SFXManager>();
    }

    private void Update()
    {
        Debug.Log("Shoot Update");
        GetTarget();

        if (isStraightShooter)
        {
            ShootStraight();
            return;
        }

        if (canTargetPlayer && target != null)
        {
            Debug.Log("Shoot Update - Shoot at target");
            RotateTowardsTarget();
            ShootAtTarget();
        }
    }

    private void GetTarget()
    {
        // Use Physics2D.OverlapCircle to detect the player within a circular range
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, shotRange);
        Debug.Log("Shoot GetTarget");

        foreach (Collider2D hit in hits)
        {
            // Check if the hit object has a Cockpit component
            Cockpit player = hit.GetComponent<Cockpit>();
            if (player != null)
            {
                target = player.transform;
                Debug.Log("Shoot GetTarget - Player targeted");
                return; // Exit after finding the player
            }
        }

        // If no player is found, set target to null
        target = null;
        Debug.Log("Shoot GetTarget - No player found");
    }

    private void RotateTowardsTarget()
    {
        if (target == null) return;

        // Calculate the direction to the target
        Vector2 direction = (target.position - transform.position).normalized;

        // Calculate the target angle in degrees
        float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f; // Subtract 90 degrees to align with Unity's 2D coordinate system

        // Get the current angle of the object
        float currentAngle = transform.eulerAngles.z;

        // Calculate the shortest difference between the current angle and target angle
        float angleDifference = Mathf.DeltaAngle(currentAngle, targetAngle);

        // Determine the direction to rotate (clockwise or counterclockwise)
        float rotationStep = rotationSpeed * Time.deltaTime;

        // Rotate in the shortest direction
        if (Mathf.Abs(angleDifference) > rotationStep)
        {
            float rotationDirection = Mathf.Sign(angleDifference);
            transform.Rotate(0, 0, rotationDirection * rotationStep);
        }
        else
        {
            // Snap to the target angle if the difference is small
            transform.rotation = Quaternion.Euler(0, 0, targetAngle);
        }
    }

    private void ShootAtTarget()
    {
        if (reloadCountdownTime > 0)
        {
            reloadCountdownTime -= Time.deltaTime;
        }
        if (reloadCountdownTime <= 0)
        {
            if (!isShootingBurst)
            {
                reloadCountdownTime = reloadTime;

                // Calculate the direction to the target
                Vector2 direction = (target.position - transform.position).normalized;

                GameObject bullet = Instantiate(bulletPreFab, transform.position, Quaternion.identity);
                bullet.GetComponent<Rigidbody2D>().linearVelocity = direction * bulletSpeed;

                // Play cannon animation
                if (cannonAnim != null)
                {
                    Debug.Log("Cannon animation");
                    cannonAnim.SetTrigger("cannonTrigger");
                }

                //Play cannon SFX
                if (sFXManager == null)
                {
                    Debug.Log("SFX Manager not found");
                    sFXManager = FindAnyObjectByType<SFXManager>();
                }
                if (sFXManager != null && cannonSounds.Length > 0)
                {
                    Debug.Log("Cannon SFX");
                    sFXManager.PlaySFX(cannonSounds[UnityEngine.Random.Range(0, cannonSounds.Length)]);
                }
            }
            if (isShootingBurst) // Prevent overlapping bursts
            {
                reloadCountdownTime = reloadTime;

                // Calculate the direction to the target
                Vector2 direction = (target.position - transform.position).normalized;

                // Start the burst coroutine with the target direction
                StartCoroutine(ShootBurst(direction));
            }
        }
    }

    private void ShootStraight()
    {
        if (reloadCountdownTime > 0)
        {
            reloadCountdownTime -= Time.deltaTime;
        }
        if (reloadCountdownTime <= 0)
        {
            if (!isShootingBurst)
            {
                reloadCountdownTime = reloadTime;

                // Define the direction for straight shooting
                Vector2 direction = transform.up;

                GameObject bullet = Instantiate(bulletPreFab, transform.position, Quaternion.identity);
                bullet.GetComponent<Rigidbody2D>().linearVelocity = direction * bulletSpeed;

                // Play cannon animation
                if (cannonAnim != null)
                {
                    Debug.Log("Cannon animation");
                    cannonAnim.SetTrigger("cannonTrigger");
                }

                //Play cannon SFX
                if (sFXManager == null)
                {
                    Debug.Log("SFX Manager not found");
                    sFXManager = FindAnyObjectByType<SFXManager>();
                }
                if (sFXManager != null && cannonSounds.Length > 0)
                {
                    Debug.Log("Cannon SFX");
                    sFXManager.PlaySFX(cannonSounds[UnityEngine.Random.Range(0, cannonSounds.Length)]);
                }
            }
            if (isShootingBurst) // Prevent overlapping bursts
            {
                reloadCountdownTime = reloadTime;

                // Define the direction for straight shooting
                Vector2 direction = -transform.up;

                // Start the burst coroutine with the straight direction
                StartCoroutine(ShootBurst(direction));
            }
        }
    }

    private IEnumerator ShootBurst(Vector2 direction)
    {
        isShootingBurst = true; // Set flag to prevent overlapping bursts

        for (int i = 0; i < burstCount; i++)
        {
            // Instantiate and shoot a bullet
            GameObject bullet = Instantiate(bulletPreFab, transform.position, Quaternion.identity);
            bullet.GetComponent<Rigidbody2D>().linearVelocity = direction * bulletSpeed;

            // Play cannon animation
            if (cannonAnim != null)
            {
                Debug.Log("Cannon animation");
                cannonAnim.SetTrigger("cannonTrigger");
            }

            // Play cannon SFX
            if (sFXManager == null)
            {
                Debug.Log("SFX Manager not found");
                sFXManager = FindAnyObjectByType<SFXManager>();
            }
            if (sFXManager != null && cannonSounds.Length > 0)
            {
                Debug.Log("Cannon SFX");
                sFXManager.PlaySFX(cannonSounds[UnityEngine.Random.Range(0, cannonSounds.Length)]);
            }

            yield return new WaitForSeconds(burstDelay); // Wait before the next shot
        }

        isShootingBurst = false; // Reset flag after the burst is complete
    }

    private void OnDrawGizmosSelected()
    {
        // Draw a red circle gizmo to show the target area
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, shotRange);
    }
}