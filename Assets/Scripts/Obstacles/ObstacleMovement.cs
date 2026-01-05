using System;
using Unity.VisualScripting;
using UnityEngine;

// This script controls the movement and movement triggered actions such as SFX, particles, and destruction.

public class ObstacleMovement : MonoBehaviour
{
    public static event Action <int, string> MissilePickupEvent;
    public static event Action <int> ShieldPickupEvent;
    public static event Action <int> NukePickupEvent;

    private PlayerStatsManager playerStatsManager;
    private SFXManager sFXManager;

    public static event Action gravityWaveEvent;
    public static event Action<Vector3> gravityWellEvent;
    public static event Action<float> turbulanceEvent;

    [Header("Obstacle Type")]

    [SerializeField] private bool isGravityWave;
    [SerializeField] private bool isGravityWell;
    [SerializeField] private bool isTurbulance;
    [SerializeField] private bool isLoot;
    [SerializeField] private bool isMissilePickUp;
    [SerializeField] private bool isShieldPickUp;
    [SerializeField] private bool isNukePickUp;
    [SerializeField] private bool isLootShip;


    [Header("Obstacle Settings")]
    [SerializeField] private Obstacle obstacle;
    [SerializeField] private float obstacleSpeedMultiplier; // Speed multiplier for obstacles
    [SerializeField] private float rotationSpeedMin;
    [SerializeField] private float rotationSpeedMax;
    private float rotationSpeed;

    [SerializeField] private Transform verticalWarning;
    [SerializeField] private AudioClip[] entranceSounds;
    [SerializeField] private AudioClip[] collisionSounds;
    [SerializeField] private AudioClip thunderSound;
    [SerializeField] private ParticleSystem collisionParticles;
    [SerializeField] private ParticleSystem lightningParticles;
    private float lightningTimer = 1.2f;

    [Header("Loot Ship Settings")]
    private float strafeTimer;
    private float strafeTimerCountdown;
    private int targetLane = 2;
    private int laneDistance = 1;
    private Vector3 strafeDirection;
    private float strafeRotation = 5f;
    private Quaternion targetRotation;

    private void OnEnable()
    {
        Nuke.NukeDamageEvent += HandleNukeDamageEvent;
    }
    private void OnDisable()
    {
        Nuke.NukeDamageEvent -= HandleNukeDamageEvent;
    }

    private void Start()
    {
        // Find the PlayerStatsManager in the scene
        playerStatsManager = FindAnyObjectByType<PlayerStatsManager>();
        if (playerStatsManager == null)
        {
            Debug.LogError("PlayerStatsManager not found in the scene!");
        }

        rotationSpeed = UnityEngine.Random.Range(rotationSpeedMin, rotationSpeedMax);
        strafeTimerCountdown = strafeTimer;
    }

    private void Update()
    {
        if (playerStatsManager == null)
        {
            Debug.Log("ObstacleMovement Update playerStatsManager == null");
            return;
        }
        // Move the obstacle along the -y axis using levelSpeed and the speed multiplier
        if (playerStatsManager.PlayerMass > 0)
        {
            // Rotate the obstacle around the Z-axis
            transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);

            float minRandomSpeedMultiplier = 0.8f;
            float maxRandomSpeedMultiplier = 1f;            
            float movementSpeed = (playerStatsManager.PlayerThrust / playerStatsManager.PlayerMass) * (obstacleSpeedMultiplier * UnityEngine.Random.Range(minRandomSpeedMultiplier, maxRandomSpeedMultiplier));
            transform.Translate(Vector3.down * movementSpeed * Time.deltaTime, Space.World);

            // Ensure the vertical warning always faces downward (in the direction of movement)
            if (verticalWarning != null)
            {
                verticalWarning.rotation = Quaternion.identity; // Reset rotation to face downward
            }
        }

        if (isTurbulance)
        {
            Lightning();
        }

        if (isLootShip)
        {
            LootShipMovement();
        }
    }

    private void Lightning()
    {
        lightningTimer += Time.deltaTime;

        if (lightningTimer >= 1.5f)
        {
            lightningTimer = 0;
            if (lightningParticles != null)
            {
                ParticleSystem particles = Instantiate(lightningParticles, transform.position, Quaternion.identity);
                particles.transform.SetParent(transform); // Set the parent to the obstacle
                Destroy(particles.gameObject, 1f); // Destroy the particles after 1 second
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("WorldTopScreenBarrier"))
        {
            if (entranceSounds.Length > 0)
            {
                sFXManager = FindAnyObjectByType<SFXManager>();
                if (sFXManager != null && entranceSounds.Length > 0)
                {
                    sFXManager.PlaySFX(entranceSounds[UnityEngine.Random.Range(0, entranceSounds.Length)]);
                }
            }
            return;
        }

        // Check if the collided object is the WorldLowerBarrier
        else if (collision.CompareTag("WorldLowerBarrier"))
        {
            SelfDestruct();
            return;
        }

        else if (collision.CompareTag("Player"))
        {
            Debug.Log("Obstacle collided with Player.");
            sFXManager = FindAnyObjectByType<SFXManager>();
            if (sFXManager != null && collisionSounds.Length > 0)
            {
                sFXManager.PlaySFX(collisionSounds[UnityEngine.Random.Range(0, collisionSounds.Length)]);
            }
            if (sFXManager != null && thunderSound != null)
            {
                sFXManager.PlaySFX(thunderSound);
            }


            if (collisionParticles != null)
            {
                ParticleSystem particles = Instantiate(collisionParticles, transform.position, Quaternion.identity);

                //coroutine to make the particles follow the obstacle's movement
                StartCoroutine(FollowParentMovement(particles.transform));
            }

            if (isGravityWave)
            {
                gravityWaveEvent?.Invoke();
            }
            if (isGravityWell)
            {
                gravityWellEvent?.Invoke(transform.position);
            }
            if (isTurbulance)
            {
                float turbulanceAmt = UnityEngine.Random.Range(-0.1f, -0.3f);
                turbulanceEvent?.Invoke(turbulanceAmt);
            }
            if (isLoot)
            {
                CreateLoot createLoot = GetComponent<CreateLoot>();
                if (createLoot != null)
                {
                    createLoot.SpawnLoot();
                }
                Destroy(gameObject);
            }

            if (isMissilePickUp)
            {
                MissilePickupEvent?.Invoke(6, "Missile");
                Destroy(gameObject);
            }
            if (isShieldPickUp)
            {
                MissilePickupEvent?.Invoke(1, "Shield");
                Destroy(gameObject);
            }
            if (isNukePickUp)
            {
                MissilePickupEvent?.Invoke(1, "Nuke");
                Destroy(gameObject);
            }
        }
    }

    private System.Collections.IEnumerator FollowParentMovement(Transform particlesTransform)
    {
        // Store the initial offset between the obstacle and the particles
        Vector3 offset = particlesTransform.position - transform.position;

        // Update the particles' position every frame while both objects exist
        while (this != null && particlesTransform != null)
        {
            // Update the particles' position to match the parent's position plus the offset
            particlesTransform.position = transform.position + offset;

            // Wait for the next frame
            yield return null;
        }
    }

    private void SelfDestruct()
    {
        Debug.Log("Obstacle collided with WorldLowerBarrier. Self-destructing.");

        // Stop all coroutines to prevent any further updates
        StopAllCoroutines();

        // Disable the collider to prevent further interactions
        GetComponent<Collider2D>().enabled = false;

        if (obstacle != null)
        {
            Debug.Log("ObstacleMovement.SelfDestruct obstacle is != null");
            obstacle.Die(false);
        }
    }

    private void HandleNukeDamageEvent(float damageAmt)
    {
        Health health = GetComponent<Health>();
        if (health != null)
        {
            health.ChangeHealth(damageAmt);
        }
    }

    #region Loot Ship Movement

    private void LootShipMovement()
    {
        if (strafeTimerCountdown <= 0)
        {
            SetRandomStrafeTimer();
            SetTargetLane();
            Strafe();
        }
        else if (strafeTimerCountdown > 0)
        {
            strafeTimerCountdown -= Time.deltaTime;
        }
    }

    private void SetRandomStrafeTimer()
    {
        float minWaitTime = 3f;
        float maxWaitTime = 10f;
        strafeTimer = UnityEngine.Random.Range(minWaitTime, maxWaitTime);
        strafeTimerCountdown = strafeTimer;
    }

    private void SetTargetLane()
    {
        // Determine if we should move left or right
        if (UnityEngine.Random.value > 0.5f && targetLane < 4)
        {
            // Move right if not already at rightmost lane
            targetLane = Mathf.Min(targetLane + 1, 4);
            strafeDirection = Vector3.right;
        }
        else if (targetLane > 0)
        {
            // Move left if not already at leftmost lane
            targetLane = Mathf.Max(targetLane - 1, 0);
            strafeDirection = Vector3.left;
        }
        // If we're at a boundary and can't move in the random direction,
        // move in the opposite direction instead
        else if (targetLane == 4)
        {
            // At right boundary, must move left
            targetLane = Mathf.Max(targetLane - 1, 0);
            strafeDirection = Vector3.left;
        }
        else if (targetLane == 0)
        {
            // At left boundary, must move right
            targetLane = Mathf.Min(targetLane + 1, 4);
            strafeDirection = Vector3.right;
        }
    }

    private void Strafe()
    {
        // Calculate the target position based on the target lane
        Vector3 targetPosition = transform.position;
        targetPosition.x = (targetLane - 2) * laneDistance; // Adjust x position based on lane
                                                            // Set rotation based on strafe direction
        if (strafeDirection == Vector3.right)
        {
            targetRotation = Quaternion.Euler(0, 45, 0); // Rotate right
        }
        else if (strafeDirection == Vector3.left)
        {
            targetRotation = Quaternion.Euler(0, -45, 0); // Rotate left
        }

        if (playerStatsManager.PlayerMass > 0)
        {
            // Calculate the speed with mass
            float minRandomSpeedMultiplier = 0.8f;
            float maxRandomSpeedMultiplier = 1f;
            float movementSpeed = (playerStatsManager.PlayerThrust / playerStatsManager.PlayerMass) * (obstacleSpeedMultiplier * UnityEngine.Random.Range(minRandomSpeedMultiplier, maxRandomSpeedMultiplier));

            // Smoothly move the player to the target position
            transform.position = Vector3.Lerp(transform.position, targetPosition, movementSpeed * Time.deltaTime);

            // Check if the player has reached the target position
            if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
            {
                targetRotation = Quaternion.Euler(0, 0, 0); // Reset rotation to 0 degrees on Y-axis
            }
        }
        // Smoothly interpolate the rotation towards the target rotation
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, strafeRotation * Time.deltaTime);
    }

    #endregion
}