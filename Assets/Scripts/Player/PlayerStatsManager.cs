using System;
using UnityEngine;

public class PlayerStatsManager : MonoBehaviour
{
    // Define a custom EventArgs class for the event
    public class OnCurrentHullChangedEventArgs : EventArgs
    {
        public float progressNormalized;
    }

    // Declare the event
    public static event EventHandler<OnCurrentHullChangedEventArgs> OnCurrentHullChanged;

    [SerializeField] private GameManager gameManager;

    [SerializeField] private float playerMass;
    [SerializeField] private float playerThrust;
    private bool isMoving;
    [SerializeField] private float playerHullMax;
    [SerializeField] private float playerCurrentHull;

    // Public properties
    public float PlayerThrust => playerThrust;
    public float PlayerMass => playerMass;

    private void OnEnable()
    {
        // Subscribe to events
        Cockpit.OnCockpitMassChanged += HandleMassChange;
        Cockpit.OnCockpitThrustChanged += HandleThrustChange;

        // Subscribe to Hull events
        Hull.OnHullMaxChanged += HandlePlayerHullMaxChange;
        Hull.OnCurrentHullChanged += HandlePlayerCurrentHullChange;
    }

    private void OnDisable()
    {
        // Unsubscribe from events
        Cockpit.OnCockpitMassChanged -= HandleMassChange;
        Cockpit.OnCockpitThrustChanged -= HandleThrustChange;

        // Unsubscribe from Hull events
        Hull.OnHullMaxChanged -= HandlePlayerHullMaxChange;
        Hull.OnCurrentHullChanged -= HandlePlayerCurrentHullChange;
    }

    private void Update()
    {
        HandleInput();
    }

    private void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.E)) ChangeLevelSpeed(1);
        if (Input.GetKeyDown(KeyCode.R)) ChangeLevelSpeed(-1);
    }

    private void ChangeLevelSpeed(float amount)
    {
        playerThrust = Mathf.Max(0, playerThrust + amount); // Ensure playerSpeed doesn't go below 0

        if (playerThrust <= 0 && isMoving)
        {
            isMoving = false;
            gameManager.StopSpawningTrigger();
        }
        if (playerThrust > 0 && isMoving == false)
        {
            isMoving = true;
            gameManager.StartSpawningTrigger();
        }
    }

    private void HandleMassChange(float mass)
    {
        playerMass += mass;
        Debug.Log("Total Mass Updated: " + playerMass);
    }

    private void HandleThrustChange(float thrust)
    {
        playerThrust += thrust;
        Debug.Log("Total Thrust Updated: " + playerThrust);
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
        float progressNormalized = playerCurrentHull / playerHullMax;
        OnCurrentHullChanged?.Invoke(this, new OnCurrentHullChangedEventArgs { progressNormalized = progressNormalized });
    }
}