using UnityEngine;

public class ObstacleMovement : MonoBehaviour
{
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
        if (playerStatsManager != null)
        {
            // Calculate the speed with mass
            float movementSpeed = (playerStatsManager.PlayerThrust / playerStatsManager.PlayerMass) * obstacleSpeedMultiplier;
            transform.Translate(Vector3.down * movementSpeed * Time.deltaTime);
        }
    }
}