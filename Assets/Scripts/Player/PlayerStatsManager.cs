using System;
using System.Collections;
using System.Runtime.InteropServices.WindowsRuntime;
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
        public float currentHull;
        public float maxHull;
    }

    public class OnCurrentThrustChangedEventArgs : EventArgs
    {
        public float progressNormalized;
        public float currentThrust;
        public float maxThrust;
    }

    public class OnCheckpointProgressChangedEventArgs : EventArgs
    {
        public float progressNormalized;
        public float distanceTraveled;
        public float totalDistance;
    }

    // Declare events
    public static event EventHandler<OnCurrentHullChangedEventArgs> OnCurrentHullChanged;
    public static event EventHandler<OnCurrentThrustChangedEventArgs> OnCurrentThrustChanged;
    public static event EventHandler<OnCheckpointProgressChangedEventArgs> OnCheckpointProgressChanged;


    public static event Action <int> GoalProgressEvent;
    public static event Action <int> PlayerHullPercentEvent;

    [SerializeField] private GameManager gameManager;

    [SerializeField] private float playerMass;
    [SerializeField] private float playerThrust;

    private float throttle = 1;

    private bool isMoving = true;
    [SerializeField] private float playerHullMax;
    [SerializeField] private float playerCurrentHull;

    private float distanceTraveled = 0; // Track distance traveled
    private bool isProgress25way = false;
    private bool isProgressHalfway = false;
    private bool isProgress75way = false;

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
        DataPersister.InitializationComplete += HandleInitializationComplete;

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
        DataPersister.InitializationComplete -= HandleInitializationComplete;

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
    private void HandleInitializationComplete()
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
        OnCurrentThrustChanged?.Invoke(this, new OnCurrentThrustChangedEventArgs 
        { 
            progressNormalized = throttle,
            currentThrust = effectiveThrust,
            maxThrust = playerThrust * engineeringSkill
        });
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
            OnCurrentHullChanged?.Invoke(this, new OnCurrentHullChangedEventArgs 
            { 
                progressNormalized = progressNormalized,
                currentHull = playerCurrentHull,
                maxHull = playerHullMax
            });
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
        OnCurrentHullChanged?.Invoke(this, new OnCurrentHullChangedEventArgs 
        { 
            progressNormalized = playerCurrentHull / playerHullMax,
            currentHull = playerCurrentHull,
            maxHull = playerHullMax
        });

        if (playerCurrentHull >= playerHullMax * 0.75f)
        {
            Debug.Log("Hull 100% (75%-100%");
            PlayerHullPercentEvent?.Invoke(100);
        }
        else if (playerCurrentHull < playerHullMax * 0.75f && playerCurrentHull >= playerHullMax * 0.5f)
        {
            Debug.Log("Hull 75% (50%-75%");
            PlayerHullPercentEvent?.Invoke(75);
        }
        else if (playerCurrentHull < playerHullMax * 0.5f && playerCurrentHull >= playerHullMax * 0.25f)
        {
            Debug.Log("Hull 50% (25%-50%)");
            PlayerHullPercentEvent?.Invoke(50);
        }    
        else if (playerCurrentHull < playerHullMax * 0.25f)
        {
            Debug.Log("Hull 25% (0%-25%)");
            PlayerHullPercentEvent?.Invoke(25);
        }
        if (playerCurrentHull <= 0)
        {
            Debug.Log("hull <= 0 triggering Die()");
            playerCurrentHull = 0;
            StartCoroutine(PrepareToDie());
        }
    }

   IEnumerator PrepareToDie()
    {
        yield return new WaitForSeconds(2f); // Wait for 1 second before dying
        Die();
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
        Debug.Log("PlayerStatsManager_UpdateDistanceTraveled");
        if (gameManager != null)
        {
            // Calculate progress toward the goal
            if (gameManager.IsGoalActive)
            {
                Debug.Log("PlayerStatsManager_UpdateDistanceTraveled_GameManager.Instance.IsGoalActive");
                if (gameManager.DistanceToGoal > 0)
                {
                    Debug.Log("PlayerStatsManager_UpdateDistanceTraveled_GameManager.Instance.IsGoalActive_gameManager.Goal > 0");
                    float progressNormalized = Mathf.Clamp01(distanceTraveled / gameManager.DistanceToGoal); // Clamp progress between 0 and 1
                    OnCheckpointProgressChanged?.Invoke(this, new OnCheckpointProgressChangedEventArgs 
                    { 
                        progressNormalized = progressNormalized,
                        distanceTraveled = distanceTraveled,
                        totalDistance = gameManager.DistanceToGoal
                    });

                    if (distanceTraveled >= gameManager.DistanceToGoal * 0.25f && distanceTraveled <= (gameManager.DistanceToGoal * 0.25f) + 0.1f && !isProgress25way)
                    {
                        isProgress25way = true;
                        GoalProgressEvent?.Invoke(1);
                        Debug.Log("PlayerStatsManager - UpdateDistanceTraveled - Goal Progress 25% reached.");
                    }

                    if (distanceTraveled >= gameManager.DistanceToGoal * 0.5f && distanceTraveled <= (gameManager.DistanceToGoal * 0.5f) + 0.1f && !isProgressHalfway)
                    {
                        isProgressHalfway = true;
                        GoalProgressEvent?.Invoke(2);
                        Debug.Log("PlayerStatsManager - UpdateDistanceTraveled - Goal Progress 50% reached.");
                    }

                    if (distanceTraveled >= gameManager.DistanceToGoal * 0.75f && distanceTraveled <= (gameManager.DistanceToGoal * 0.75f) + 0.1f && !isProgress75way)
                    {
                        isProgress75way = true;
                        GoalProgressEvent?.Invoke(3);
                        Debug.Log("PlayerStatsManager - UpdateDistanceTraveled - Goal Progress 75% reached.");
                    }

                    // Check if the goal has been reached
                    if (distanceTraveled >= gameManager.DistanceToGoal)
                    {
                        GoalProgressEvent?.Invoke(4);
                        Debug.Log("PlayerStatsManager - UpdateDistanceTraveled - Goal reached.");
                    }
                }
            }
        }
    }

    public void LoadData()
    {
        // Check if DataPersister exists and has valid data
        if (DataPersister.Instance == null || DataPersister.Instance.CurrentGameData == null)
        {
            Debug.Log("PlayerStatsManager LoadData - DataPersister or CurrentGameData is null.");
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
            Debug.Log($"PlayerStatsManager LoadData - loaded engineeringSkill: {engineeringSkill}, pilotingSkill: {pilotingSkill}, mechanicsSkill: {mechanicsSkill}, miningSkill: {miningSkill}, roboticsSkill: {roboticsSkill}, combatSkill: {combatSkill}");
        }
        else
        {
            Debug.Log("PlayerStatsManager LoadData - Failed to load skills from DataPersister.");
        }

        Debug.Log($"Loaded Engineering: {engineeringSkill}");
        Debug.Log($"Loaded Piloting: {pilotingSkill}");
        Debug.Log($"Loaded Mechanics: {mechanicsSkill}");
        Debug.Log($"Loaded Mining: {miningSkill}");
        Debug.Log($"Loaded Robotics: {roboticsSkill}");
        Debug.Log($"Loaded Combat: {combatSkill}");
    }

    public void MultiplyEngineeringSkill()
    {
        Debug.Log($"PlayerStatsManager MultiplyEngineeringSkill - before change Skill: {DataPersister.Instance.CurrentGameData.playerData[0].engineeringSkill}");
        engineeringSkill = MultiplySkill(engineeringSkill);
        DataPersister.Instance.CurrentGameData.playerData[0].engineeringSkill = engineeringSkill;
        DataPersister.Instance.SaveCurrentGame();
        Debug.Log($"PlayerStatsManager MultiplyEngineeringSkill - after change Skill: {DataPersister.Instance.CurrentGameData.playerData[0].engineeringSkill}");
    }
    public void MultiplyPilotingSkill()
    {
        Debug.Log($"PlayerStatsManager MultiplyPilotingSkill - before change Skill: {DataPersister.Instance.CurrentGameData.playerData[0].pilotingSkill}");
        pilotingSkill = MultiplySkill(pilotingSkill);
        DataPersister.Instance.CurrentGameData.playerData[0].pilotingSkill = pilotingSkill;
        DataPersister.Instance.SaveCurrentGame();
        Debug.Log($"PlayerStatsManager MultiplyPilotingSkill - after change Skill: {DataPersister.Instance.CurrentGameData.playerData[0].pilotingSkill}");
    }
    public void MultiplyMechanicsSkill()
    {
        Debug.Log($"PlayerStatsManager MultiplyMechanicsSkill - before change Skill: {DataPersister.Instance.CurrentGameData.playerData[0].mechanicsSkill}");
        mechanicsSkill = MultiplySkill(mechanicsSkill);
        DataPersister.Instance.CurrentGameData.playerData[0].mechanicsSkill = mechanicsSkill;
        DataPersister.Instance.SaveCurrentGame();
        Debug.Log($"PlayerStatsManager MultiplyMechanicsSkill - after change Skill: {DataPersister.Instance.CurrentGameData.playerData[0].mechanicsSkill}");
    }
    public void MultiplyMiningSkill()
    {
        Debug.Log($"PlayerStatsManager MultiplyMiningSkill - before change Skill: {DataPersister.Instance.CurrentGameData.playerData[0].miningSkill}");
        miningSkill = MultiplySkill(miningSkill);
        DataPersister.Instance.CurrentGameData.playerData[0].miningSkill = miningSkill;
        DataPersister.Instance.SaveCurrentGame();
        Debug.Log($"PlayerStatsManager MultiplyMiningSkill - after change Skill: {DataPersister.Instance.CurrentGameData.playerData[0].miningSkill}");
    }
    public void MultiplyRoboticsSkill()
    {
        Debug.Log($"PlayerStatsManager MultiplyRoboticsSkill - before change Skill: {DataPersister.Instance.CurrentGameData.playerData[0].roboticsSkill}");
        roboticsSkill = MultiplySkill(roboticsSkill);
        DataPersister.Instance.CurrentGameData.playerData[0].roboticsSkill = roboticsSkill;
        DataPersister.Instance.SaveCurrentGame();
        Debug.Log($"PlayerStatsManager MultiplyRoboticsSkill - after change Skill: {DataPersister.Instance.CurrentGameData.playerData[0].roboticsSkill}");
    }
    public void MultiplyCombatSkill()
    {
        Debug.Log($"PlayerStatsManager MultiplyCombatSkill - before change Skill: {DataPersister.Instance.CurrentGameData.playerData[0].combatSkill}");
        combatSkill = MultiplySkill(combatSkill);
        DataPersister.Instance.CurrentGameData.playerData[0].combatSkill = combatSkill;
        DataPersister.Instance.SaveCurrentGame();
        Debug.Log($"PlayerStatsManager MultiplyCombatSkill - after change Skill: {DataPersister.Instance.CurrentGameData.playerData[0].combatSkill}");
    }

    public float MultiplySkill(float skillvalue)
    {
        return skillvalue += skillvalue * skillIncreaseAmt;
    }
}