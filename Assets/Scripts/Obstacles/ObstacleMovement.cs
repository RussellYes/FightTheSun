using UnityEngine;

public class ObstacleMovement : MonoBehaviour
{
    private PlayerStatsManager playerStatsManager;
    private SFXManager sFXManager;

    [SerializeField] private Obstacle obstacle;
    [SerializeField] private float obstacleSpeedMultiplier; // Speed multiplier for obstacles
    [SerializeField] private float rotationSpeedMin;
    [SerializeField] private float rotationSpeedMax;
    private float rotationSpeed;

    [SerializeField] private Transform verticalWarning;

    [SerializeField] private AudioClip[] entranceSounds;
    [SerializeField] private AudioClip[] collisionSound;
    [SerializeField] private ParticleSystem collisionParticles;

    private void Start()
    {
        // Find the PlayerStatsManager in the scene
        playerStatsManager = FindAnyObjectByType<PlayerStatsManager>();
        if (playerStatsManager == null)
        {
            Debug.LogError("PlayerStatsManager not found in the scene!");
        }
        

        rotationSpeed = Random.Range(rotationSpeedMin, rotationSpeedMax);
    }

    private void Update()
    {
        // Move the obstacle along the -y axis using levelSpeed and the speed multiplier
        if (playerStatsManager.PlayerMass > 0)
        {
            // Rotate the obstacle around the Z-axis
            transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);

            float movementSpeed = (playerStatsManager.PlayerThrust / playerStatsManager.PlayerMass) * obstacleSpeedMultiplier;
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
            sFXManager = FindAnyObjectByType<SFXManager>();
            sFXManager.PlaySFX(entranceSounds[Random.Range(0, entranceSounds.Length)]);
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
            sFXManager.PlaySFX(collisionSound[Random.Range(0, collisionSound.Length)]);
            Instantiate(collisionParticles, transform.position, Quaternion.identity);
        }
    }

    private void SelfDestruct()
    {
        Debug.Log("Obstacle collided with WorldLowerBarrier. Self-destructing.");

        // Disable the collider to prevent further interactions
        GetComponent<Collider2D>().enabled = false;

        if (obstacle != null)
        {
            Debug.Log("ObstacleMovement.SelfDestruct obstacle is != null");
            obstacle.Die(false);
        }

    }
}