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
    public static event Action<Vector2> LaunchMiningClawEvent;
    public static event Action StopMiningClawEvent;

    SFXManager SFXManager => SFXManager.Instance;
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
    public bool Initialized => initialized;
    [SerializeField] private AudioClip clockTimeIsUpSFX;
    private Color defaultTimeTextColor;

    [Header("Missile UI")]
    [SerializeField] private GameObject fireMissileButton;
    [SerializeField] private TextMeshProUGUI missileCountText;
    [SerializeField] private TextMeshProUGUI missileLauncherLevelText;
    [SerializeField] private AudioClip[] buttonPositiveSFX;
    [SerializeField] private AudioClip buttonNegitiveSFX;

    [Header("MiningClaw")]
    [SerializeField] private GameObject miningClawButton;
    [SerializeField] private GameObject miningClawJoyStickUIHolder;
    [SerializeField] private RectTransform miningClawJoystickBackground;
    [SerializeField] private RectTransform miningClawJoystickHandle;
    [SerializeField] private Image joystickBackgroundImage;
    [SerializeField] private Image joystickHandleImage;
    [SerializeField] private float joystickRadius = 100f;
    [SerializeField] private LineRenderer aimMiningClawLR;
    [SerializeField] private Color aimLineRendererColor;

    private Vector2 joystickCenter;
    private Vector2 miningClawStartPosition;
    private bool isJoystickActive = false;

    private void Start()
    {
        swipeControls = FindFirstObjectByType<SwipeControls>();
        defaultTimeTextColor = totalTimeText.color;

        InitializeMiningClawJoystick();
    }

    private void OnEnable()
    {
        fireMissileButton.GetComponent<Button>().onClick.AddListener(() => { FireMissileButton(); });
        pauseButton.GetComponent<Button>().onClick.AddListener(() => { PauseButtonClicked(); });
        MiningMissileLauncher.LauncherActiveEvent += UpdateMissileButton;
        DataPersister.InitializationComplete += OnInitializationComplete;
        miningClawButton.GetComponent<Button>().onClick.AddListener(() => { ActivateClawJoyStick(); });
    }
    private void OnDisable()
    {
        fireMissileButton.GetComponent<Button>().onClick.RemoveListener(() => { FireMissileButton(); });
        pauseButton.GetComponent<Button>().onClick.RemoveListener(() => { PauseButtonClicked(); });
        MiningMissileLauncher.LauncherActiveEvent -= UpdateMissileButton;
        DataPersister.InitializationComplete -= OnInitializationComplete;
        miningClawButton.GetComponent<Button>().onClick.RemoveListener(() => { ActivateClawJoyStick(); });
    }

    private void PauseButtonClicked()
    {
        PlayButtonPositive();
        PauseButtonEvent?.Invoke();
    }

    private void FireMissileButton()
    {
        if (fireMissileButton.GetComponent<Button>().interactable)
        {
            PlayButtonPositive();
            FireMissilesEvent?.Invoke();
        }
        if (!fireMissileButton.GetComponent<Button>().interactable)
        {
            PlayButtonNegitive();
            Debug.LogWarning("FireMissileButton: Button is not interactable, cannot fire missiles.");
        }


    }
    private void InitializeMiningClawJoystick()
    {
        miningClawJoyStickUIHolder.SetActive(false);
        joystickBackgroundImage.enabled = false;
        joystickHandleImage.enabled = false;
        aimMiningClawLR.enabled = false;

        // Configure line renderer
        aimMiningClawLR.enabled = false;
        aimMiningClawLR.startWidth = 0.1f;  // Thinner line
        aimMiningClawLR.endWidth = 0.05f;   // Tapered end
        aimMiningClawLR.positionCount = 2;
        aimMiningClawLR.startColor = aimLineRendererColor;
        aimMiningClawLR.endColor = aimLineRendererColor;

        isJoystickActive = false;
    }
    private void OnInitializationComplete()
    {
        initialized = true;
        UpdateMissileButton(DataPersister.Instance.CurrentGameData.savedMissileCount);
    }
    private void Update()
    {
        if (initialized)
        {
            KeepingTime();
            SunCount();
        }

        if (isJoystickActive)
        {
            HandleMiningClawJoystick();
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
            fireMissileButton.GetComponent<Button>().interactable = true;
            missileCountText.text = missileCount.ToString();
        }
        else if (missileCount <= 0)
        {
            fireMissileButton.GetComponent<Button>().interactable = false;
            missileCountText.text = "0";
        }
        int launcherLevel = DataPersister.Instance.CurrentGameData.savedLauncherLevel;
        string[] launcherLevels = { "I", "II", "III", "IV", "V" };

        // Make sure the level is within bounds (0-4 for array indices)
        int levelIndex = Mathf.Clamp(launcherLevel - 1, 0, launcherLevels.Length - 1);
        missileLauncherLevelText.text = launcherLevels[levelIndex];
    }

    private void KeepingTime()
    {
        if (totalTimeText != null)
        {
            float timeRemaining = GameManager.Instance.TimeRemaining;

            int minutes = Mathf.FloorToInt(timeRemaining / 60);
            int seconds = Mathf.FloorToInt(timeRemaining % 60);
            totalTimeText.text = $"{minutes:00}:{seconds:00}";

            if (Math.Floor(timeRemaining) == timeRemaining && timeRemaining >= 1 && timeRemaining <= 5)
            {
                totalTimeText.color = Color.red;
                SFXManager.PlaySFX(clockTimeIsUpSFX);
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

    private void ActivateClawJoyStick()
    {
        PlayButtonPositive();

        // Deactivate swipe controls
        swipeControls.EnableTouchControls(false);

        // Activate mining claw UI
        miningClawJoyStickUIHolder.SetActive(true);
        joystickBackgroundImage.enabled = true;
        joystickHandleImage.enabled = true;

        // Position joystick at center of screen
        joystickCenter = new Vector2(Screen.width / 2f, Screen.height / 2f - 100);
        miningClawJoystickBackground.position = joystickCenter;
        miningClawJoystickHandle.position = joystickCenter;

        // Get ship position for reference
        miningClawStartPosition = swipeControls.transform.position;

        aimMiningClawLR.enabled = true;
        aimMiningClawLR.SetPosition(0, miningClawStartPosition);
        aimMiningClawLR.SetPosition(1, miningClawStartPosition);

        StartCoroutine(EnableJoystickAfterDelay(0.5f));
    }

    IEnumerator EnableJoystickAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        // Ready for joystick input
        isJoystickActive = true;
    }
    private void HandleMiningClawJoystick()
    {
        if (Input.touchCount == 0)
        {
            // Turn off line renderer when not touching
            if (aimMiningClawLR.enabled)
            {
                aimMiningClawLR.enabled = false;
            }
            return;
        }

        Touch touch = Input.GetTouch(0);

        switch (touch.phase)
        {
            case TouchPhase.Began:
                if (IsTouchOnJoystick(touch.position))
                {
                    joystickCenter = touch.position;
                    miningClawJoystickBackground.position = joystickCenter;
                    miningClawJoystickHandle.position = joystickCenter;
                    aimMiningClawLR.enabled = true;
                }
                break;

            case TouchPhase.Moved:
                if (!aimMiningClawLR.enabled) break;

                Vector2 touchDelta = touch.position - joystickCenter;
                float distance = Mathf.Clamp(touchDelta.magnitude, 0, joystickRadius);
                Vector2 direction = touchDelta.normalized;

                // Update joystick handle position
                miningClawJoystickHandle.position = joystickCenter + (direction * distance);

                // Calculate INVERSE direction for line renderer
                Vector2 inverseDirection = -direction;
                Vector2 worldInverseDirection = Camera.main.ScreenToWorldPoint(inverseDirection) -
                                             Camera.main.ScreenToWorldPoint(Vector2.zero);

                // Shorter line (3 units instead of 5)
                aimMiningClawLR.SetPosition(0, miningClawStartPosition);
                aimMiningClawLR.SetPosition(1, miningClawStartPosition + (worldInverseDirection.normalized * 3f));
                break;

            case TouchPhase.Ended:
            case TouchPhase.Canceled:
                if (aimMiningClawLR.enabled)
                {
                    if ((miningClawJoystickHandle.position - (Vector3)joystickCenter).magnitude > 20f)
                    {
                        Vector2 launchDirection = (joystickCenter - (Vector2)miningClawJoystickHandle.position).normalized;
                        LaunchMiningClawEvent?.Invoke(launchDirection);
                    }
                    else
                    {
                        StopMiningClawEvent?.Invoke();
                    }
                }

                // Clean up
                joystickBackgroundImage.enabled = false;
                joystickHandleImage.enabled = false;
                aimMiningClawLR.enabled = false;
                isJoystickActive = false;
                miningClawJoyStickUIHolder.SetActive(false);
                swipeControls.EnableTouchControls(true);
                break;
        }
    }

    private bool IsTouchOnJoystick(Vector2 touchPosition)
    {
        // Check if touch is within joystick activation area
        return Vector2.Distance(touchPosition, joystickCenter) < joystickRadius * 1.5f;
    }

    private void PlayButtonPositive()
    {
        if (SFXManager != null && buttonPositiveSFX.Length > 0)
        {
            SFXManager.PlaySFX(buttonPositiveSFX[UnityEngine.Random.Range(0, buttonPositiveSFX.Length)]);
        }
        else
        {
            Debug.LogWarning("SFXManager or buttonSFX is not set up correctly.");
        }
    }

    private void PlayButtonNegitive()
    {
        if (SFXManager != null && buttonNegitiveSFX != null)
        {
            SFXManager.PlaySFX(buttonNegitiveSFX);
        }
        else
        {
            Debug.LogWarning("SFXManager or buttonSFX is not set up correctly.");
        }
    }


}


