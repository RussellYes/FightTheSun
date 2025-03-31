using System;
using UnityEngine;

public class ObstacleMovement : MonoBehaviour
{
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


    [Header("Obstacle Settings")]

    [SerializeField] private Obstacle obstacle;
    [SerializeField] private float obstacleSpeedMultiplier; // Speed multiplier for obstacles
    [SerializeField] private float rotationSpeedMin;
    [SerializeField] private float rotationSpeedMax;
    private float rotationSpeed;

    [SerializeField] private Transform verticalWarning;

    [SerializeField] private AudioClip[] entranceSounds;
    [SerializeField] private AudioClip[] collisionSounds;
    [SerializeField] private ParticleSystem collisionParticles;

    private void Start()
    {
        // Find the PlayerStatsManager in the scene
        playerStatsManager = FindAnyObjectByType<PlayerStatsManager>();
        if (playerStatsManager == null)
        {
            Debug.LogError("PlayerStatsManager not found in the scene!");
        }
        

        rotationSpeed = UnityEngine.Random.Range(rotationSpeedMin, rotationSpeedMax);
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

            float movementSpeed = (playerStatsManager.PlayerThrust / playerStatsManager.PlayerMass) * (obstacleSpeedMultiplier * UnityEngine.Random.Range(0.9f, 1f));
            transform.Translate(Vector3.down * movementSpeed * Time.deltaTime, Space.World);

            // Ensure the vertical warning always faces downward (in the direction of movement)
            if (verticalWarning != null)
            {
                verticalWarning.rotation = Quaternion.identity; // Reset rotation to face downward
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
        }

        // Check if the collided object is the WorldLowerBarrier
        else if (collision.CompareTag("WorldLowerBarrier"))
        {
            SelfDestruct();
        }

        else if (collision.CompareTag("Player"))
        {
            Debug.Log("Obstacle collided with Player.");
            sFXManager = FindAnyObjectByType<SFXManager>();
            if (sFXManager != null && collisionSounds.Length > 0)
            {
                sFXManager.PlaySFX(collisionSounds[UnityEngine.Random.Range(0, collisionSounds.Length)]);
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
}