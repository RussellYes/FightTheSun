using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.VFX;

// This script controls the ship UI.

public class ShipUIManager : MonoBehaviour
{
    public static event Action FireMissilesEvent;
    public static event Action PauseButtonEvent;

    SFXManager sFXManager;
    SwipeControls swipeControls;

    [SerializeField] private GameObject shipDashboardUIHolder;
    [SerializeField] private GameObject shipTotalTimeUIHolder;
    [SerializeField] private GameObject shipSunCountHolder;
    [SerializeField] private TextMeshProUGUI SunCountText;

    [Header("Hull UI")]
    [SerializeField] private GameObject hullMeter;

    [Header("Thruster UI")]
    [SerializeField] private GameObject speedMeter;

    [Header("Progress UI")]
    [SerializeField] private GameObject checkpointUI;
    [SerializeField] private GameObject scoreMeter;
    [SerializeField] private GameObject pauseButton;
    [SerializeField] private TextMeshProUGUI totalTimeText;
    private bool initialized = false;
    [SerializeField] private AudioClip clockTimeIsUpSFX;
    private Color defaultTimeTextColor;

    [Header("Missile UI")]
    [SerializeField] private Button fireMissileButton;
    [SerializeField] private TextMeshProUGUI missileCountText;

    [Header("MiningClaw")]
    [SerializeField] private Button miningClawButton;
    [SerializeField] private LineRenderer miningClawAimLineRenderer;
    [SerializeField] private GameObject miningClawAimUIHolder;
    [SerializeField] private GameObject clawJoystickBase;
    [SerializeField] private GameObject clawJoystickHandle;
    [SerializeField] private AudioClip miningClawSFX;
    [SerializeField] private LineRenderer miningClawCableLineRenderer;
    [SerializeField] private GameObject miningClawPrefab;
    private float mingingClawSpeed = 10f;
    private bool miningClawActive = false;
    private float miningTime = 5;
    private float miningCountdown;
    private Vector2 miningClawStartPosition;
    private bool isAimingMiningClaw = false;
    [SerializeField] private float maxClawDistance = 20f;

    private void Start()
    {
        sFXManager = FindFirstObjectByType<SFXManager>();
        swipeControls = FindFirstObjectByType<SwipeControls>();
        defaultTimeTextColor = totalTimeText.color;
        miningCountdown = miningTime;
    }

    private void OnEnable()
    {
        fireMissileButton.onClick.AddListener(() => FireMissilesEvent?.Invoke());
        pauseButton.GetComponent<Button>().onClick.AddListener(() => PauseButtonEvent?.Invoke());
        MiningMissileLauncher.LauncherActiveEvent += UpdateMissileButton;
        DataPersister.InitializationComplete += OnInitializationComplete;
        miningClawButton.onClick.AddListener(() => {AimMiningClaw(); });
    }
    private void OnDisable()
    {
        fireMissileButton.onClick.RemoveListener(() => FireMissilesEvent?.Invoke());
        pauseButton.GetComponent<Button>().onClick.RemoveListener(() => PauseButtonEvent?.Invoke());
        MiningMissileLauncher.LauncherActiveEvent -= UpdateMissileButton;
        DataPersister.InitializationComplete -= OnInitializationComplete;
        miningClawButton.onClick.RemoveListener(() => { AimMiningClaw(); });
    }

    private void OnInitializationComplete()
    {
        initialized = true;
    }
    private void Update()
    {
        if (initialized)
        {
            KeepingTime();
            SunCount();
        }

        if (miningClawActive)
        {
            miningCountdown -= Time.deltaTime;
            if (miningCountdown <= 0)
            {
                miningCountdown = miningTime;
                StopMiningClaw();
            }
        }

        if (isAimingMiningClaw)
        {
            HandleMiningClawAim();
        }
    }
    public void TurnOnShipUI()
    {
        shipDashboardUIHolder.SetActive(true);
        shipTotalTimeUIHolder.SetActive(true);

        hullMeter.SetActive(true);
        speedMeter.SetActive(true);
        checkpointUI.SetActive(true);
        scoreMeter.SetActive(true);
        pauseButton.SetActive(true);
    }
    public void TurnOffShipUI()
    {
        shipDashboardUIHolder.SetActive(false);
        shipTotalTimeUIHolder.SetActive(false);

        hullMeter.SetActive(false);
        speedMeter.SetActive(false);
        checkpointUI.SetActive(false);
        scoreMeter.SetActive(false);
        pauseButton.SetActive(false);
    }

    private void UpdateMissileButton(int missileCount)
    {
        if (missileCount > 0)
        {
            fireMissileButton.interactable = true;
            missileCountText.text = missileCount.ToString();
        }
        else if (missileCount <= 0)
        {
            fireMissileButton.interactable = false;
            missileCountText.text = "0";
        }
    }

    private void KeepingTime()
    {
        if (totalTimeText != null)
        {
            float totalCountdownTime = GameManager.Instance.TotalCountdownTime;

            int minutes = Mathf.FloorToInt(totalCountdownTime / 60);
            int seconds = Mathf.FloorToInt(totalCountdownTime % 60);
            totalTimeText.text = $"{minutes:00}:{seconds:00}";

            if (Math.Floor(totalCountdownTime) == totalCountdownTime && totalCountdownTime >= 1 && totalCountdownTime <= 5)
            {
                totalTimeText.color = Color.red;
                sFXManager.PlaySFX(clockTimeIsUpSFX);
            }
            else
            {
                totalTimeText.color = defaultTimeTextColor;
            }
        }
        else
        {
            Debug.LogError("Total Time Text is not found by ShipUIManager");
        }
    }

    private void SunCount()
    {
        if (SunCountText != null)
        {
            SunCountText.text = DataPersister.Instance.CurrentGameData.sunCount.ToString();
        }
        else
        {
            Debug.LogError("Sun Count Text is not found by ShipUIManager");
        }
    }

    private void AimMiningClaw()
{
    // Deactivate swipe controls
    swipeControls.EnableTouchControls(false);
    
    // Activate mining claw UI
    miningClawAimUIHolder.SetActive(true);
    clawJoystickBase.SetActive(true);
    clawJoystickHandle.SetActive(true);
    

    miningClawStartPosition = swipeControls.transform.position;

    // Set initial position for joystick UI at ship's position
    Vector2 screenPos = Camera.main.WorldToScreenPoint(miningClawStartPosition);
    clawJoystickBase.transform.position = screenPos;
    clawJoystickHandle.transform.position = screenPos;
    
    // Enable line renderer
    miningClawAimLineRenderer.enabled = true;
    miningClawAimLineRenderer.SetPosition(0, miningClawStartPosition);
    miningClawAimLineRenderer.SetPosition(1, miningClawStartPosition);
    
    isAimingMiningClaw = true;
}

    private void HandleMiningClawAim()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            Vector2 touchWorldPosition = Camera.main.ScreenToWorldPoint(touch.position);

            // Update joystick handle position (screen space)
            clawJoystickHandle.transform.position = touch.position;

            // Calculate direction from ship to touch
            Vector2 direction = touchWorldPosition - miningClawStartPosition;
            float distance = Mathf.Clamp(direction.magnitude, 0, 5f); // Limit the distance

            // Update aim line renderer
            miningClawAimLineRenderer.SetPosition(0, miningClawStartPosition);
            miningClawAimLineRenderer.SetPosition(1, miningClawStartPosition + direction.normalized * distance);

            // On touch release, launch the mining claw
            if (touch.phase == TouchPhase.Ended)
            {
                if (direction.magnitude > 0.5f) // Minimum distance threshold
                {
                    LaunchMiningClaw(direction.normalized);
                }
                else
                {
                    // If swipe was too short, cancel mining claw
                    StopMiningClaw();
                }
            }
        }
    }

    private void LaunchMiningClaw(Vector2 direction)
    {
        // Play sound effect
        sFXManager.PlaySFX(miningClawSFX);

        // Instantiate mining claw
        GameObject claw = Instantiate(miningClawPrefab, miningClawStartPosition, Quaternion.identity);

        // Set claw direction and speed
        Rigidbody2D clawRb = claw.GetComponent<Rigidbody2D>();
        if (clawRb != null)
        {
            clawRb.linearVelocity = direction * mingingClawSpeed;
        }

        // Set up cable line renderer
        MiningClaw clawScript = claw.GetComponent<MiningClaw>();
        if (clawScript != null)
        {
            clawScript.Initialize(miningClawStartPosition, miningClawCableLineRenderer);
        }

        // Activate mining claw tracking
        miningClawActive = true;
        miningCountdown = miningTime;
        StopMiningClaw(); // This will clean up the aiming UI
    }
    private void StopMiningClaw()
{
    // Reactivate swipe controls
    swipeControls.EnableTouchControls(true);
    
    // Deactivate mining claw UI
    miningClawAimUIHolder.SetActive(false);
    clawJoystickBase.SetActive(false);
    clawJoystickHandle.SetActive(false);
    miningClawAimLineRenderer.enabled = false;
    
    miningClawActive = false;
    isAimingMiningClaw = false;
}




}
