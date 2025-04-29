using TMPro;
using UnityEngine;
using UnityEngine.UI;

// This script controls the player's movement within 5 lanes.

public class PlayerMovement : MonoBehaviour
{
    public static PlayerMovement Instance { get; private set; }

    private PlayerStatsManager playerStatsManager;
    private SFXManager sFXManager;

    [SerializeField] private float laneDistance; // Distance between lanes
    private int targetLane = 2; // Start in the middle lane (lane 0 is 1st, lane 1 is 2nd, lane 2 is the 3rd of 5 lanes.)

    private Button leftButton;
    private Button rightButton;

    [SerializeField] private AudioClip[] thrusterSounds;

    private Quaternion targetRotation;
    private float yAxisRotationSpeed = 1f;
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

    private void OnEnable()
    {
        ObstacleMovement.gravityWaveEvent += RandomMove;
        ObstacleMovement.gravityWellEvent += MoveCloser;
        SwipeControls.Instance.OnMoveLeft += MoveLeft;
        SwipeControls.Instance.OnMoveRight += MoveRight;
    }

    private void OnDisable()
    {
        ObstacleMovement.gravityWaveEvent -= RandomMove;
        ObstacleMovement.gravityWellEvent -= MoveCloser;
        ObstacleMovement.gravityWaveEvent -= RandomMove;
        ObstacleMovement.gravityWellEvent -= MoveCloser;
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
            float calculatedSpeed = (playerStatsManager.PlayerThrust / playerStatsManager.PlayerMass) * playerStatsManager.PilotingSkill;

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

    private void MoveCloser(Vector3 obstaclePosition)
    {
        if (obstaclePosition.x > transform.position.x)
        {
            MoveRight();
        }
        else
        {
            MoveLeft();
        }
    }

    private void RandomMove()
    {
        // Randomly move left or right
        if (Random.value > 0.5f)
        {
            MoveLeft();
        }
        else
        {
            MoveRight();
        }
    }

    private void MoveRight()
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

    private void MoveLeft()
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