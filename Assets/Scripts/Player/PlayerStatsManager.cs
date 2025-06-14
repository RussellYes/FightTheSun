using System;
using System.Collections;
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

    // Player ship upgrades
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

    public float PlayerCurrentHull => playerCurrentHull;

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
        DataPersister.InitializationComplete += OnDataPersisterReady;

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
        DataPersister.InitializationComplete -= OnDataPersisterReady;

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
    private void OnDataPersisterReady()
    {
        LoadData();
    }
    private void Update()
    {
        if (GameManager.Instance != null)
        {
            // Update distance traveled based on player's speed
            if (isMoving && GameManager.Instance.IsGoalActive)
            {
                //Debug.Log("PlayerStatsManager_Update_if (isMoving)");
                float distanceThisFrame = PlayerThrust * Time.deltaTime; // Distance = speed * time
                UpdateDistanceTraveled(distanceThisFrame);
            }
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
        if (gameManager != null)
        {
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
                        StartCoroutine(DoubleCheckGameStateEndDialogue());
                    }
                }
            }
        }
    }

    IEnumerator DoubleCheckGameStateEndDialogue()
    {
        yield return new WaitForSeconds(0.1f); // Wait a short time to ensure the state has been set
        if (gameManager.CurrentState != GameState.EndDialogue)
        {
            gameManager.SetState(GameState.EndDialogue);
        }
    }
    public void LoadData()
    {
        // Check if DataPersister exists and has valid data
        if (DataPersister.Instance == null || DataPersister.Instance.CurrentGameData == null)
        {
            Debug.Log("PlayerStatsManager - LoadData - DataPersister or CurrentGameData is null. Loading defaults.");
            LoadDefaultStats();
            return;
        }

        // Load player stats from playerData[0]
        if (DataPersister.Instance.CurrentGameData.playerData.Count > 0)
        {
            var playerData = DataPersister.Instance.CurrentGameData.playerData[0];
            engineeringSkill = playerData.engineeringSkill;
            pilotingSkill = playerData.pilotingSkill;
            mechanicsSkill = playerData.mechanicsSkill;
            miningSkill = playerData.miningSkill;
            roboticsSkill = playerData.roboticsSkill;
            combatSkill = playerData.combatSkill;
        }
        else
        {
            LoadDefaultStats();
        }

        Debug.Log($"Loaded Engineering: {engineeringSkill}");
        Debug.Log($"Loaded Piloting: {pilotingSkill}");
        Debug.Log($"Loaded Mechanics: {mechanicsSkill}");
        Debug.Log($"Loaded Mining: {miningSkill}");
        Debug.Log($"Loaded Robotics: {roboticsSkill}");
        Debug.Log($"Loaded Combat: {combatSkill}");
    }

    private void LoadDefaultStats()
    {
        engineeringSkill = 1f;
        pilotingSkill = 1f;
        mechanicsSkill = 1f;
        miningSkill = 1f;
        roboticsSkill = 1f;
        combatSkill = 1f;

        Debug.Log("PlayerStatsManager - LoadDefaultStats - Using default skill values.");
    }


    public void MultiplyEngineeringSkill()
    {
        engineeringSkill += engineeringSkill * skillIncreaseAmt;
        DataPersister.Instance.CurrentGameData.playerData[0].engineeringSkill = engineeringSkill;
        DataPersister.Instance.SaveCurrentGame();
    }
    public void MultiplyPilotingSkill()
    {
        pilotingSkill += pilotingSkill * skillIncreaseAmt;
        DataPersister.Instance.CurrentGameData.playerData[0].pilotingSkill = pilotingSkill;
        DataPersister.Instance.SaveCurrentGame();
    }
    public void MultiplyMechanicsSkill()
    {
        mechanicsSkill += mechanicsSkill * skillIncreaseAmt;
        DataPersister.Instance.CurrentGameData.playerData[0].mechanicsSkill = mechanicsSkill;
        DataPersister.Instance.SaveCurrentGame();
    }
    public void MultiplyMiningSkill()
    {
        miningSkill += miningSkill * skillIncreaseAmt;
        DataPersister.Instance.CurrentGameData.playerData[0].miningSkill = miningSkill;
        DataPersister.Instance.SaveCurrentGame();
    }
    public void MultiplyRoboticsSkill()
    {
        roboticsSkill += roboticsSkill * skillIncreaseAmt;
        DataPersister.Instance.CurrentGameData.playerData[0].roboticsSkill = roboticsSkill;
        DataPersister.Instance.SaveCurrentGame();
    }
    public void MultiplyCombatSkill()
    {
        combatSkill += combatSkill * skillIncreaseAmt;
        DataPersister.Instance.CurrentGameData.playerData[0].combatSkill = combatSkill;
        DataPersister.Instance.SaveCurrentGame();
    }
}