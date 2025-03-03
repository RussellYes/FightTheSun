using UnityEngine;

public class ObstacleMovement : MonoBehaviour
{
    private PlayerStatsManager playerStatsManager;

    [SerializeField] private Obstacle obstacle;
    [SerializeField] private float obstacleSpeedMultiplier; // Speed multiplier for obstacles
    [SerializeField] private float rotationSpeedMin;
    [SerializeField] private float rotationSpeedMax;
    private float rotationSpeed;

    [SerializeField] private Transform verticalWarning;


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
        // Check if the collided object is the WorldLowerBarrier
        if (collision.CompareTag("WorldLowerBarrier"))
        {
            SelfDestruct();
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