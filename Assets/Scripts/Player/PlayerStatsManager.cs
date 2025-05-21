using System;
using UnityEngine;
using UnityEngine.UI;
using static GameManager;

// This script centralizes and alters the player's stats.

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

    public static event Action PlayerHull100Percent;
    public static event Action PlayerHull75Percent;
    public static event Action PlayerHull50Percent;
    public static event Action PlayerHull25Percent;

    [SerializeField] private GameManager gameManager;

    [SerializeField] private float playerMass;
    [SerializeField] private float playerThrust;

    private float throttle = 1;

    private bool isMoving = true;
    [SerializeField] private float playerHullMax;
    [SerializeField] private float playerCurrentHull;

    private float distanceTraveled = 0; // Track distance traveled
    private bool isProgressHalfway = false;
    private float repairTimer = 200f;
    private float repairCountdown;

    // Player endGame upgrades
    private float engineeringSkill = 1;
    private float pilotingSkill = 1;
    private float mechanicsSkill = 1;
    private float miningSkill = 1;
    private float roboticsSkill = 1;
    private float combatSkill = 1;
    private float skillIncreaseAmt = 0.01f;


    // Public properties
    public float PlayerThrust => playerThrust * throttle * engineeringSkill; // Effective thrust is scaled by throttle
    public float PlayerMass => playerMass;

    public float EngineeringSkill => engineeringSkill;
    public float PilotingSkill => pilotingSkill;
    public float MechanicsSkill => mechanicsSkill;
    public float MiningSkill => miningSkill;
    public float RoboticsSkill => roboticsSkill;
    public float CombatSkill => combatSkill;


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
        GameManager.ChangeThrottleEvent += ThrottleChange;
        ObstacleMovement.turbulanceEvent += ThrottleChange;
        SwipeControls.OnSwipeUp += SwipedUp;
        SwipeControls.OnSwipeDown += SwipedDown;

        //Set goal progress. Should be zero.
        float distanceThisFrame = PlayerThrust * Time.deltaTime; // Distance = speed * time
        UpdateDistanceTraveled(distanceThisFrame);

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
        GameManager.ChangeThrottleEvent -= ThrottleChange;
        ObstacleMovement.turbulanceEvent -= ThrottleChange;
        SwipeControls.OnSwipeUp -= SwipedUp;
        SwipeControls.OnSwipeDown -= SwipedDown;
    }

    private void SwipedUp()
    {
        ThrottleChange(0.25f);
    }
    private void SwipedDown()
    {
        ThrottleChange(-0.25f);
    }
    private void Start()
    {
        LoadData();
    }
    private void Update()
    {
        // Update distance traveled based on player's speed
        if (isMoving && GameManager.Instance.IsGoalActive)
        {
            //Debug.Log("PlayerStatsManager_Update_if (isMoving)");
            float distanceThisFrame = PlayerThrust * Time.deltaTime; // Distance = speed * time
            UpdateDistanceTraveled(distanceThisFrame);
        }

        RepairHull();
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

        Debug.Log("PlayerStatsManager - HandleThrustChange - Total Thrust Updated: " + effectiveThrust);

        // Trigger thrust change event
        OnCurrentThrustChanged?.Invoke(this, new OnCurrentThrustChangedEventArgs { progressNormalized = throttle });
    }

    private void HandlePlayerHullMaxChange(float hullMax)
    {
        playerHullMax = hullMax * mechanicsSkill;
        Debug.Log("PlayerStatsManager - HandlePlayerHullMaxChage - Total hullMax Updated: " + playerHullMax);
    }

    private void HandlePlayerCurrentHullChange(float currentHull)
    {
        playerCurrentHull = currentHull;
        Debug.Log("PlayerStatsManager - HandlePlayerCurrentHullChange - Total CurrentHull Updated: " + playerCurrentHull);

        // Calculate normalized progress and trigger the event
        if (playerHullMax > 0)
        {
            float progressNormalized = playerCurrentHull / playerHullMax;
            OnCurrentHullChanged?.Invoke(this, new OnCurrentHullChangedEventArgs { progressNormalized = progressNormalized });
        }
        if (playerHullMax == 0)
        {
            Debug.LogWarning("PlayerStatsManager - HandlePlayerCurrentHullChange - playerHullMax is 0. Cannot calculate progressNormalized.");
        }
    }

    public void ChangeHealth(float amount)
    {
        playerCurrentHull += amount;
        playerCurrentHull = Mathf.Clamp(playerCurrentHull, 0, playerHullMax); // Ensure hull stays within bounds

        // Trigger event for current hull change
        OnCurrentHullChanged?.Invoke(this, new OnCurrentHullChangedEventArgs { progressNormalized = playerCurrentHull / playerHullMax });

        if (playerCurrentHull >= playerHullMax * 0.75f)
        {
            Debug.Log("Hull 100% (75%-100%");
            PlayerHull100Percent?.Invoke();
        }
        else if (playerCurrentHull < playerHullMax * 0.75f && playerCurrentHull >= playerHullMax * 0.5f)
        {
            Debug.Log("Hull 75% (50%-75%");
            PlayerHull75Percent?.Invoke();
        }
        else if (playerCurrentHull < playerHullMax * 0.5f && playerCurrentHull >= playerHullMax * 0.25f)
        {
            Debug.Log("Hull 50% (25%-50%)");
            PlayerHull50Percent?.Invoke();
        }
        else if (playerCurrentHull < playerHullMax * 0.25f)
        {
            Debug.Log("Hull 25% (0%-25%)");
            PlayerHull25Percent?.Invoke();
        }
        if (playerCurrentHull <= 0)
        {
            Debug.Log("hull <= 0 triggering Die()");
            playerCurrentHull = 0;
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("PlayerStatsManager - Die - Player has died. Calling EndGame(false).");

        gameManager.EndGame(false);

        // Destroy the player GameObject
        PlayerMovement playerMovement = FindFirstObjectByType<PlayerMovement>();
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
        if (gameManager.IsGoalActive)
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
                    DialogueManager.Instance.MissionDialogue();
                }

                // Check if the goal has been reached
                if (distanceTraveled >= gameManager.Goal)
                {
                    gameManager.SetState(GameState.EndDialogue);
                }
            }
        }

    }

    private void LoadData()
    {
        // Load player stats from PlayerPrefs
        engineeringSkill = DataPersister.Instance.CurrentGameData.engineeringSkill;
        pilotingSkill = DataPersister.Instance.CurrentGameData.pilotingSkill;
        mechanicsSkill = DataPersister.Instance.CurrentGameData.mechanicsSkill;
        miningSkill = DataPersister.Instance.CurrentGameData.miningSkill;
        roboticsSkill = DataPersister.Instance.CurrentGameData.roboticsSkill;
        combatSkill = DataPersister.Instance.CurrentGameData.combatSkill;

        Debug.Log($"Loaded Engineering: {engineeringSkill}");
        Debug.Log($"Loaded Piloting: {pilotingSkill}");
        Debug.Log($"Loaded Mechanics: {mechanicsSkill}");
        Debug.Log($"Loaded Mining: {miningSkill}");
        Debug.Log($"Loaded Robotics: {roboticsSkill}");
        Debug.Log($"Loaded Combat: {combatSkill}");
    }
    public void MultiplyEngineeringSkill()
    {
        engineeringSkill += engineeringSkill * skillIncreaseAmt;
        DataPersister.Instance.CurrentGameData.engineeringSkill = engineeringSkill;
        DataPersister.Instance.SaveCurrentGame();
    }
    public void MultiplyPilotingSkill()
    {
        pilotingSkill += pilotingSkill * skillIncreaseAmt;
        DataPersister.Instance.CurrentGameData.pilotingSkill = pilotingSkill;
        DataPersister.Instance.SaveCurrentGame();
    }
    public void MultiplyMechanicsSkill()
    {
        mechanicsSkill += mechanicsSkill * skillIncreaseAmt;
        DataPersister.Instance.CurrentGameData.mechanicsSkill = mechanicsSkill;
        DataPersister.Instance.SaveCurrentGame();
    }
    public void MultiplyMiningSkill()
    {
        miningSkill += miningSkill * skillIncreaseAmt;
        DataPersister.Instance.CurrentGameData.miningSkill = miningSkill;
        DataPersister.Instance.SaveCurrentGame();
    }
    public void MultiplyRoboticsSkill()
    {
        roboticsSkill += roboticsSkill * skillIncreaseAmt;
        DataPersister.Instance.CurrentGameData.roboticsSkill = roboticsSkill;
        DataPersister.Instance.SaveCurrentGame();
    }
    public void MultiplyCombatSkill()
    {
        combatSkill += combatSkill * skillIncreaseAmt;
        DataPersister.Instance.CurrentGameData.combatSkill = combatSkill;
        DataPersister.Instance.SaveCurrentGame();
    }

    private void RepairHull()
    {
        repairCountdown -= Time.deltaTime;

        if (repairCountdown <= 0)
        {
            ChangeHealth(1f);

            repairCountdown = repairTimer - roboticsSkill;

            if (repairCountdown < 1)
            {
                repairCountdown = 1;
            }
        }
    }
}