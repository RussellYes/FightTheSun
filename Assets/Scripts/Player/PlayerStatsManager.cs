using System;
using UnityEngine;
using UnityEngine.UI;
using static GameManager;

public class PlayerStatsManager : MonoBehaviour
{
    // Singleton instance
    public static PlayerStatsManager Instance { get; private set; }

    // Define custom EventArgs classes for events
    public class OnCurrentHullChangedEventArgs : EventArgs
    {
        public float progressNormalized;
    }

    public class OnCurrentThrustChangedEventArgs : EventArgs
    {
        public float progressNormalized;
    }

    public class OnCheckpointProgressChangedEventArgs : EventArgs
    {
        public float progressNormalized;
    }

    // Declare events
    public static event EventHandler<OnCurrentHullChangedEventArgs> OnCurrentHullChanged;
    public static event EventHandler<OnCurrentThrustChangedEventArgs> OnCurrentThrustChanged;
    public static event EventHandler<OnCheckpointProgressChangedEventArgs> OnCheckpointProgressChanged;

    [SerializeField] private GameManager gameManager;

    [SerializeField] private float playerMass;
    [SerializeField] private float playerThrust;

    private float throttle = 1;
    [SerializeField] private Button throttleUpButton;
    [SerializeField] private Button throttleDownButton;

    private bool isMoving = true;
    [SerializeField] private float playerHullMax;
    [SerializeField] private float playerCurrentHull;

    private float distanceTraveled = 0; // Track distance traveled
    private bool isProgressHalfway = false;

    // Public properties
    public float PlayerThrust => playerThrust * throttle; // Effective thrust is scaled by throttle
    public float PlayerMass => playerMass;

    private void Awake()
    {
        // Initialize singleton
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        // Subscribe to events
        Cockpit.OnCockpitMassChanged += HandleMassChange;
        Cockpit.OnCockpitThrustChanged += HandleThrustChange;

        // Subscribe to Hull events
        Hull.OnHullMaxChanged += HandlePlayerHullMaxChange;
        Hull.OnCurrentHullChanged += HandlePlayerCurrentHullChange;

        // Subscribe to throttle events
        throttleUpButton.onClick.AddListener(() => ThrottleChange(0.25f)); // Increase throttle by 0.25
        throttleDownButton.onClick.AddListener(() => ThrottleChange(-0.25f)); // Decrease throttle by 0.25
        GameManager.ChangeThrottleEvent += GameManager_ChangeThrottleEvent;
        ObstacleMovement.turbulanceEvent += ThrottleChange;

        //Set goal progress. Should be zero.
        float distanceThisFrame = PlayerThrust * Time.deltaTime; // Distance = speed * time
        UpdateDistanceTraveled(distanceThisFrame);

    }

    private void GameManager_ChangeThrottleEvent(float throttleChange)
    {
        ThrottleChange(throttleChange);
    }

    private void OnDisable()
    {
        // Unsubscribe from events
        Cockpit.OnCockpitMassChanged -= HandleMassChange;
        Cockpit.OnCockpitThrustChanged -= HandleThrustChange;

        // Unsubscribe from Hull events
        Hull.OnHullMaxChanged -= HandlePlayerHullMaxChange;
        Hull.OnCurrentHullChanged -= HandlePlayerCurrentHullChange;

        // Unsubscribe from throttle events
        throttleUpButton.onClick.RemoveAllListeners();
        throttleDownButton.onClick.RemoveAllListeners();
        GameManager.ChangeThrottleEvent -= GameManager_ChangeThrottleEvent;
        ObstacleMovement.turbulanceEvent -= ThrottleChange;
    }

    private void Update()
    {
        // Update distance traveled based on player's speed
        if (isMoving && GameManager.Instance.IsGoalActive)
        {
            Debug.Log("PlayerStatsManager_Update_if (isMoving)");
            float distanceThisFrame = PlayerThrust * Time.deltaTime; // Distance = speed * time
            UpdateDistanceTraveled(distanceThisFrame);
        }
    }

    private void HandleMassChange(float mass)
    {
        playerMass += mass;
        Debug.Log("Total Mass Updated: " + playerMass);
    }

    private void ThrottleChange(float amt)
    {
        throttle += amt;
        throttle = Mathf.Clamp(throttle, 0.25f, 1); // Clamp throttle between 0 and 1
        Debug.Log("Throttle Updated: " + throttle);

        // Update thrust and motion state
        HandleThrustChange(0);
    }

    private void HandleThrustChange(float thrust)
    {
        // Update base thrust value
        playerThrust = Mathf.Max(0, playerThrust + thrust); // Ensure playerThrust doesn't go below 0

        // Calculate effective thrust
        float effectiveThrust = PlayerThrust;

        // Update motion state
        if (effectiveThrust <= 0 && isMoving)
        {
            isMoving = false;
        }
        else if (effectiveThrust > 0 && !isMoving)
        {
            isMoving = true;
        }

        Debug.Log("Total Thrust Updated: " + effectiveThrust);

        // Trigger thrust change event
        OnCurrentThrustChanged?.Invoke(this, new OnCurrentThrustChangedEventArgs { progressNormalized = throttle });
    }

    private void HandlePlayerHullMaxChange(float hullMax)
    {
        playerHullMax = hullMax;
        Debug.Log("Total hullMax Updated: " + playerHullMax);
    }

    private void HandlePlayerCurrentHullChange(float currentHull)
    {
        playerCurrentHull = currentHull;
        Debug.Log("Total CurrentHull Updated: " + playerCurrentHull);

        // Calculate normalized progress and trigger the event
        if (playerHullMax > 0)
        {
            float progressNormalized = playerCurrentHull / playerHullMax;
            OnCurrentHullChanged?.Invoke(this, new OnCurrentHullChangedEventArgs { progressNormalized = progressNormalized });
        }
        else
        {
            Debug.LogWarning("playerHullMax is 0. Cannot calculate progressNormalized.");
        }
    }

    public void ChangeHealth(float amount)
    {
        playerCurrentHull += amount;
        playerCurrentHull = Mathf.Clamp(playerCurrentHull, 0, playerHullMax); // Ensure hull stays within bounds

        // Trigger event for current hull change
        OnCurrentHullChanged?.Invoke(this, new OnCurrentHullChangedEventArgs { progressNormalized = playerCurrentHull / playerHullMax });

        if (playerCurrentHull <= 0)
        {
            Debug.Log("hull <= 0 triggering Die()");
            playerCurrentHull = 0;
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("Player has died. Calling EndGame(false).");

        gameManager.EndGame(false);

        // Destroy the player GameObject
        PlayerMovement playerMovement = FindObjectOfType<PlayerMovement>();
        if (playerMovement != null)
        {
            Destroy(playerMovement.gameObject);
        }
       

    }

    // Update distance traveled and trigger checkpoint progress event
    private void UpdateDistanceTraveled(float distance)
    {
        distanceTraveled += distance;
        //Debug.Log("PlayerStatsManager_UpdateDistanceTraveled");

        // Calculate progress toward the goal
        if (GameManager.Instance.IsGoalActive)
        {
            //Debug.Log("PlayerStatsManager_UpdateDistanceTraveled_GameManager.Instance.IsGoalActive");
            if (gameManager.Goal > 0)
            {
                //Debug.Log("PlayerStatsManager_UpdateDistanceTraveled_GameManager.Instance.IsGoalActive_gameManager.Goal > 0");
                float progressNormalized = Mathf.Clamp01(distanceTraveled / gameManager.Goal); // Clamp progress between 0 and 1
                OnCheckpointProgressChanged?.Invoke(this, new OnCheckpointProgressChangedEventArgs { progressNormalized = progressNormalized });
                
                if (distanceTraveled >= gameManager.Goal / 2 && distanceTraveled <= (gameManager.Goal / 2) + 0.1f && !isProgressHalfway)
                {
                    isProgressHalfway = true;
                    //Debug.Log("gameManager.Goal / 2");
                    gameManager.SetState(GameState.DialogueDuringPlay);
                }

                // Check if the goal has been reached
                if (distanceTraveled >= gameManager.Goal)
                {
                    gameManager.SetState(GameState.EndDialogue);
                }
            }
        }
        
    }


}