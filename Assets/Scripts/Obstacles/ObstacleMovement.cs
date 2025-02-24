using UnityEngine;

public class ObstacleMovement : MonoBehaviour
{
    [SerializeField] private Obstacle obstacle;
    [SerializeField] private float obstacleSpeedMultiplier; // Speed multiplier for obstacles
    private PlayerStatsManager playerStatsManager;

    private void Start()
    {
        // Find the PlayerStatsManager in the scene
        playerStatsManager = FindAnyObjectByType<PlayerStatsManager>();
        if (playerStatsManager == null)
        {
            Debug.LogError("PlayerStatsManager not found in the scene!");
        }
    }

    private void Update()
    {
        // Move the obstacle along the -y axis using levelSpeed and the speed multiplier
        
        // Calculate the speed with mass
        if (playerStatsManager.PlayerMass > 0)
        {
            float movementSpeed = (playerStatsManager.PlayerThrust / playerStatsManager.PlayerMass) * obstacleSpeedMultiplier;
            transform.Translate(Vector3.down * movementSpeed * Time.deltaTime);
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