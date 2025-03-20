using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    public static PlayerMovement Instance { get; private set; }

    private PlayerStatsManager playerStatsManager;
    private SFXManager sFXManager;

    [SerializeField] private float laneDistance; // Distance between lanes
    private int targetLane = 2; // Start in the middle lane (lane 3)
    private float playerHorizontalSpeedMultiplier = 1.5f;

    private Button leftButton;
    private Button rightButton;

    [SerializeField] private AudioClip[] thrusterSounds;

    private Quaternion targetRotation; // Target rotation for smooth interpolation
    private float yAxisRotationSpeed = 1f; // Rotation speed on the Y-axis
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        
        // Find the buttons by their tags or names
        leftButton = GameObject.Find("LeftButton").GetComponent<Button>();
        rightButton = GameObject.Find("RightButton").GetComponent<Button>();


        // Ensure buttons are found
        if (leftButton == null || rightButton == null)
        {
            Debug.LogError("LeftButton or RightButton not found in the scene.");
            return;
        }

        // Add listeners to the buttons
        leftButton.onClick.AddListener(MoveLeft);
        rightButton.onClick.AddListener(MoveRight);

    }

    private void Start()
    {
        playerStatsManager = FindAnyObjectByType<PlayerStatsManager>();
        sFXManager = FindAnyObjectByType<SFXManager>();

        // Debug to ensure PlayerStatsManager is found
        if (playerStatsManager == null)
        {
            Debug.LogError("PlayerStatsManager not found in the scene.");
        }

        // Initialize target rotation to the current rotation
        targetRotation = transform.rotation;
    }



    void Update()
    {
        // Calculate the target position based on the target lane
        Vector3 targetPosition = transform.position;
        targetPosition.x = (targetLane - 2) * laneDistance; // Adjust x position based on lane

        if (playerStatsManager.PlayerMass > 0)
        {
            // Calculate the speed with mass
            float calculatedSpeed = (playerStatsManager.PlayerThrust / playerStatsManager.PlayerMass) * playerHorizontalSpeedMultiplier;

            // Smoothly move the player to the target position
            transform.position = Vector3.Lerp(transform.position, targetPosition, calculatedSpeed * Time.deltaTime);

            // Check if the player has reached the target position
            if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
            {
                targetRotation = Quaternion.Euler(0, 0, 0); // Reset rotation to 0 degrees on Y-axis
            }
        }
        // Smoothly interpolate the rotation towards the target rotation
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, yAxisRotationSpeed * Time.deltaTime);
    }

    void MoveRight()
    {
        // Move to the right lane, but don't go beyond the rightmost lane (lane 4)
        targetLane = Mathf.Min(targetLane + 1, 4);
        Debug.Log($"Moved Right. Current Lane: {targetLane}");

        // Rotate the player 45 degrees on the Y-axis
        transform.rotation = Quaternion.Euler(0, 45, 0);

        if (sFXManager == null)
        {
            sFXManager = FindAnyObjectByType<SFXManager>();
        }
        sFXManager.PlaySFX(thrusterSounds[Random.Range(0, thrusterSounds.Length)]);
    }

    void MoveLeft()
    {
        // Move to the left lane, but don't go beyond the leftmost lane (lane 0)
        targetLane = Mathf.Max(targetLane - 1, 0);
        Debug.Log($"Moved Left. Current Lane: {targetLane}");

        // Rotate the player -45 degrees on the Y-axis
        transform.rotation = Quaternion.Euler(0, -45, 0);

        if (sFXManager == null)
        {
            sFXManager = FindAnyObjectByType<SFXManager>();
        }
        sFXManager.PlaySFX(thrusterSounds[Random.Range(0, thrusterSounds.Length)]);
    }
}