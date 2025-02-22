using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private PlayerStatsManager playerStatsManager;

    [SerializeField] private float laneDistance; // Distance between lanes
    private int targetLane = 2; // Start in the middle lane (lane 3)

    private void Start()
    {
        playerStatsManager = FindAnyObjectByType<PlayerStatsManager>();

        // Debug to ensure PlayerStatsManager is found
        if (playerStatsManager == null)
        {
            Debug.LogError("PlayerStatsManager not found in the scene.");
        }
    }

    void Update()
    {
        if (Input.anyKeyDown)
        {
            Debug.Log("A key was pressed.");
        }

        // Handle input for lane switching
        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
        {
            MoveRight();
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
        {
            MoveLeft();
        }

        // Calculate the target position based on the target lane
        Vector3 targetPosition = transform.position;
        targetPosition.x = (targetLane - 2) * laneDistance; // Adjust x position based on lane

        // Calculate the speed with mass
        float calculatedSpeed = (playerStatsManager.PlayerThrust / playerStatsManager.PlayerMass);

        // Debug the calculated speed and target position
        Debug.Log($"Calculated Speed: {calculatedSpeed}, Target Position: {targetPosition}");

        // Smoothly move the player to the target position
        transform.position = Vector3.Lerp(transform.position, targetPosition, calculatedSpeed * Time.deltaTime);
    }

    void MoveRight()
    {
        // Move to the right lane, but don't go beyond the rightmost lane (lane 4)
        targetLane = Mathf.Min(targetLane + 1, 4);
        Debug.Log($"Moved Right. Current Lane: {targetLane}");
    }

    void MoveLeft()
    {
        // Move to the left lane, but don't go beyond the leftmost lane (lane 0)
        targetLane = Mathf.Max(targetLane - 1, 0);
        Debug.Log($"Moved Left. Current Lane: {targetLane}");
    }
}