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
    [SerializeField] private Button throttleUpButton;
    [SerializeField] private Button throttleDownButton;

    private bool isMoving = true;
    [SerializeField] private float playerHullMax;
    [SerializeField] private float playerCurrentHull;

    private float distanceTraveled = 0; // Track distance traveled
    private bool isProgressHalfway = false;

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

    private void Start()
    {
        LoadData();
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
        if (playerHullMax == 0)
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

    private void LoadData()
    {
        // Load player stats from PlayerPrefs
        engineeringSkill = PlayerPrefs.GetFloat("EngineeringSkill", 1);
        pilotingSkill = PlayerPrefs.GetFloat("PilotingSkill", 1);
        mechanicsSkill = PlayerPrefs.GetFloat("MechanicsSkill", 1);
        miningSkill = PlayerPrefs.GetFloat("MiningSkill", 1);
        roboticsSkill = PlayerPrefs.GetFloat("RoboticsSkill", 1);
        combatSkill = PlayerPrefs.GetFloat("CombatSkill", 1);

        Debug.Log($"Loaded Robotics: {roboticsSkill}");
    }
    public void MultiplyEngineeringSkill()
    {
        engineeringSkill += engineeringSkill * skillIncreaseAmt;
        PlayerPrefs.SetFloat("EngineeringSkill",engineeringSkill);
        PlayerPrefs.Save();
    }
    public void MultiplyPilotingSkill()
    {
        pilotingSkill += pilotingSkill * skillIncreaseAmt;
        PlayerPrefs.SetFloat("PilotingSkill", pilotingSkill);
        PlayerPrefs.Save();
    }
    public void MultiplyMechanicsSkill()
    {
        mechanicsSkill += mechanicsSkill * skillIncreaseAmt;
        PlayerPrefs.SetFloat("MechanicsSkill", mechanicsSkill);
        PlayerPrefs.Save();
    }
    public void MultiplyMiningSkill()
    {
        miningSkill += miningSkill * skillIncreaseAmt;
        PlayerPrefs.SetFloat("MiningSkill", miningSkill);
        PlayerPrefs.Save();
    }
    public void MultiplyRoboticsSkill()
    {
        roboticsSkill += roboticsSkill * skillIncreaseAmt;
        PlayerPrefs.SetFloat("RoboticsSkill", roboticsSkill);
        PlayerPrefs.Save();
    }
    public void MultiplyCombatSkill()
    {
        combatSkill += combatSkill * skillIncreaseAmt;
        PlayerPrefs.SetFloat("CombatSkill", combatSkill);
        PlayerPrefs.Save();
    }






}