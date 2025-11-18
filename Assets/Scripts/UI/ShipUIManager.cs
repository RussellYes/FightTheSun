using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

// This script controls the ship UI.

public class ShipUIManager : MonoBehaviour
{
    public static event Action FireMissilesEvent;
    public static event Action PauseButtonEvent;
    public static event Action<Vector2> LaunchMiningClawEvent;
    public static event Action StopMiningClawEvent;
    public static event Action CreateShieldEvent;

    SFXManager SFXManager => SFXManager.Instance;
    SwipeControls swipeControls;
    private bool wasTouching = false;
    private float swipeControlDelay = 0.5f;

    [Header("Top Screen UI")]
    [SerializeField] private GameObject shipDashboardUIHolder;
    [SerializeField] private GameObject shipTotalTimeUIHolder;
    [SerializeField] private GameObject shipSunCountHolder;
    [SerializeField] private TextMeshProUGUI SunCountText;

    [Header("Missile UI")]
    [SerializeField] private GameObject fireMissileButton;
    [SerializeField] private TextMeshProUGUI missileCountText;
    [SerializeField] private TextMeshProUGUI missileLauncherLevelText;
    [SerializeField] private AudioClip[] buttonPositiveSFX;
    [SerializeField] private AudioClip buttonNegitiveSFX;

    [Header("Shield UI")]
    [SerializeField] private GameObject fireShieldButton;
    [SerializeField] private TextMeshProUGUI shieldCountText;
    [SerializeField] private int shieldCount;

    [Header("Nuke UI")]
    [SerializeField] private GameObject fireNukeButton;
    [SerializeField] private TextMeshProUGUI nukeCountText;
    [SerializeField] private int nukeCount;
    [SerializeField] private GameObject nukePrefab;

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

    private void Start()
    {
        swipeControls = FindFirstObjectByType<SwipeControls>();
        defaultTimeTextColor = totalTimeText.color;

        InitializeMiningClawJoystick();
        LoadData();
        UpdateShieldUI();
        UpdateNukeUI();
    }

    private void OnEnable()
    {
        fireMissileButton.GetComponent<Button>().onClick.AddListener(() => { FireMissileButton(); });
        fireShieldButton.GetComponent<Button>().onClick.AddListener(() => { FireShieldButton(); });
        fireNukeButton.GetComponent<Button>().onClick.AddListener(() => { FireNukeButton(); });
        pauseButton.GetComponent<Button>().onClick.AddListener(() => { PauseButtonClicked(); });
        MiningMissileLauncher.LauncherActiveEvent += UpdateMissileButton;
        DataPersister.InitializationComplete += OnInitializationComplete;
        miningClawButton.GetComponent<Button>().onClick.AddListener(() => { ActivateClawJoyStick(); });
        PlayerStatsManager.PlayerHullPercentEvent += HidePauseButtonOnDeath;
        ObstacleMovement.MissilePickupEvent += HandleMissilePickupEvent;
    }
    private void OnDisable()
    {
        fireMissileButton.GetComponent<Button>().onClick.RemoveListener(() => { FireMissileButton(); });
        fireShieldButton.GetComponent<Button>().onClick.RemoveListener(() => { FireShieldButton(); });
        fireNukeButton.GetComponent<Button>().onClick.RemoveListener(() => { FireNukeButton(); });
        pauseButton.GetComponent<Button>().onClick.RemoveListener(() => { PauseButtonClicked(); });
        MiningMissileLauncher.LauncherActiveEvent -= UpdateMissileButton;
        DataPersister.InitializationComplete -= OnInitializationComplete;
        miningClawButton.GetComponent<Button>().onClick.RemoveListener(() => { ActivateClawJoyStick(); });
        PlayerStatsManager.PlayerHullPercentEvent -= HidePauseButtonOnDeath;
        ObstacleMovement.MissilePickupEvent -= HandleMissilePickupEvent;
    }

    private void PauseButtonClicked()
    {
        PlayButtonPositive();
        PauseButtonEvent?.Invoke();
    }

    private void FireMissileButton()
    {
        Debug.Log("ShipUIManager FireMissileButton");

        // Disable swipe controls immediately
        if (swipeControls != null)
            swipeControls.EnableTouchControls(false);

        if (fireMissileButton.GetComponent<Button>().interactable)
        {
            PlayButtonPositive();
            FireMissilesEvent?.Invoke();
        }
        if (!fireMissileButton.GetComponent<Button>().interactable)
        {
            PlayButtonNegative();
            Debug.LogWarning("ShipUIManager FireMissileButton - Button is not interactable, cannot fire missiles.");
        }

        // Re-enable after a short delay
        StartCoroutine(ReenableSwipeControls(swipeControlDelay));
    }
    private void FireShieldButton()
    {
        Debug.Log("ShipUIManager FireShieldButton");

        // Disable swipe controls immediately
        if (swipeControls != null)
            swipeControls.EnableTouchControls(false);

        if (fireShieldButton.GetComponent<Button>().interactable)
        {
            Shield existingShield = FindAnyObjectByType<Shield>();
            if (existingShield == null)
            {
                PlayButtonPositive();
                CreateShieldEvent?.Invoke();
                shieldCount--;
                UpdateShieldUI();
                if (DataPersister.Instance != null && DataPersister.Instance.CurrentGameData != null)
                {
                    DataPersister.Instance.CurrentGameData.shieldCount = shieldCount;
                    DataPersister.Instance.SaveCurrentGame();
                }
            }
            else
            {
                PlayButtonNegative();
                Debug.LogWarning("ShipUIManager FireShieldButton - Shield already exists.");
            }
        }
        if (!fireShieldButton.GetComponent<Button>().interactable)
        {
            PlayButtonNegative();
            Debug.LogWarning("ShipUIManager FireShieldButton - Button is not interactable, cannot fire shield.");
        }

        // Re-enable after a short delay
        StartCoroutine(ReenableSwipeControls(swipeControlDelay));
    }
    private void FireNukeButton()
    {
        Debug.Log("ShipUIManager FireNukeButton");

        // Disable swipe controls immediately
        if (swipeControls != null)
            swipeControls.EnableTouchControls(false);

        if (fireNukeButton.GetComponent<Button>().interactable)
        {
            PlayButtonPositive();
            // Find the center of the screen in world space
            Vector3 screenCenter = new Vector3(Screen.width / 2f, Screen.height / 2f, 10f); // Use camera's far plane or desired Z
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(screenCenter);
            Instantiate(nukePrefab, worldPosition, Quaternion.identity);

            // Subtract nuke count and save
            nukeCount--;
            UpdateNukeUI();
            if (DataPersister.Instance != null && DataPersister.Instance.CurrentGameData != null)
            {
                DataPersister.Instance.CurrentGameData.nukeCount = nukeCount;
                DataPersister.Instance.SaveCurrentGame();
            }
        }
        if (!fireNukeButton.GetComponent<Button>().interactable)
        {
            PlayButtonNegative();
            Debug.LogWarning("ShipUIManager FireNukeButton - Button is not interactable, cannot fire nuke.");
        }

        // Re-enable after a short delay
        StartCoroutine(ReenableSwipeControls(swipeControlDelay));
    }

    private IEnumerator ReenableSwipeControls(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (swipeControls != null)
            swipeControls.EnableTouchControls(true);
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
        UpdateMissileButton(DataPersister.Instance.CurrentGameData.missileCount);
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
        fireMissileButton.SetActive(true);
        fireShieldButton.SetActive(true);
        fireNukeButton.SetActive(true);
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
        fireMissileButton.SetActive(false);
        fireShieldButton.SetActive(false);
        fireNukeButton.SetActive(false);
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
        int launcherLevel = DataPersister.Instance.CurrentGameData.launcherLevel;
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
        Debug.Log("ShipUIManager ActivateClawJoyStick");
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

        // After delay, check for current touch and use that position if available
        var touchscreen = UnityEngine.InputSystem.Touchscreen.current;
        if (touchscreen != null && touchscreen.primaryTouch.press.ReadValue() > 0.5f)
        {
            Vector2 touchPosition = touchscreen.primaryTouch.position.ReadValue();
            joystickCenter = touchPosition;
            miningClawJoystickBackground.position = joystickCenter;
            miningClawJoystickHandle.position = joystickCenter;
        }
        // Ready for joystick input
        isJoystickActive = true;
    }
    private void HandleMiningClawJoystick()
    {
        Debug.Log($"ShipUIManager HandleMiningClawJoystick - Joystick active: {isJoystickActive}");

        var touchscreen = UnityEngine.InputSystem.Touchscreen.current;
        if (touchscreen == null) return;

        // Get touch info
        Vector2 touchPosition = touchscreen.primaryTouch.position.ReadValue();
        float pressValue = touchscreen.primaryTouch.press.ReadValue();
        bool isTouching = pressValue > 0.5f;

        // Handle touch phases manually
        if (isTouching && !wasTouching)
        {
            // Touch began - center joystick on touch
            Debug.Log("Mining claw - Touch began");
            joystickCenter = touchPosition;
            miningClawJoystickBackground.position = joystickCenter;
            miningClawJoystickHandle.position = joystickCenter;
            aimMiningClawLR.enabled = true;
            wasTouching = true;
        }
        else if (isTouching && wasTouching)
        {
            // Touch moved - update joystick
            Debug.Log("Mining claw - Touch moved");
            if (!aimMiningClawLR.enabled) return;

            Vector2 touchDelta = touchPosition - joystickCenter;
            float distance = Mathf.Clamp(touchDelta.magnitude, 0, joystickRadius);
            Vector2 direction = touchDelta.normalized;

            // Update joystick handle position
            miningClawJoystickHandle.position = joystickCenter + (direction * distance);

            // Calculate INVERSE direction for line renderer
            Vector2 inverseDirection = -direction;
            Vector2 worldInverseDirection = Camera.main.ScreenToWorldPoint(inverseDirection) -
                                         Camera.main.ScreenToWorldPoint(Vector2.zero);

            // Update line renderer
            aimMiningClawLR.SetPosition(0, miningClawStartPosition);
            aimMiningClawLR.SetPosition(1, miningClawStartPosition + (worldInverseDirection.normalized * 3f));
        }
        else if (!isTouching && wasTouching)
        {
            // Touch ended
            Debug.Log("Mining claw - Touch ended");
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
            wasTouching = false;
            StartCoroutine(DelayTouchControlsDuringMiningLaunch());
        }
    }

    IEnumerator DelayTouchControlsDuringMiningLaunch()
    {
        float controlDelay = 0.5f;
        yield return new WaitForSeconds(controlDelay);
        swipeControls.EnableTouchControls(true);
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

    private void PlayButtonNegative()
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

    private void HidePauseButtonOnDeath(int hull)
    {
        if (hull <= 0)
        {
            pauseButton.SetActive(false);
        }
        else
        {
            pauseButton.SetActive(true);
        }
    }

    private void HandleMissilePickupEvent(int amt, string type)
    {
        if (type == "Shield")
        {
            shieldCount += amt;
            UpdateShieldUI();
            if (DataPersister.Instance != null && DataPersister.Instance.CurrentGameData != null)
            {
                DataPersister.Instance.CurrentGameData.shieldCount = shieldCount;
            }
        }
        else if (type == "Nuke")
        {
            nukeCount += amt;
            UpdateNukeUI();
            if (DataPersister.Instance != null && DataPersister.Instance.CurrentGameData != null)
            {
                DataPersister.Instance.CurrentGameData.nukeCount = nukeCount;
            }
        }
    }

    private void UpdateShieldUI()
    {
        if (shieldCount > 0)
        {
            fireShieldButton.GetComponent<Button>().interactable = true;
            shieldCountText.text = shieldCount.ToString();
        }
        else if (shieldCount <= 0)
        {
            fireShieldButton.GetComponent<Button>().interactable = false;
            shieldCountText.text = "0";
        }
    }
    private void UpdateNukeUI()
    {
        if (nukeCount > 0)
        {
            fireNukeButton.GetComponent<Button>().interactable = true;
            nukeCountText.text = nukeCount.ToString();
        }
        else if (nukeCount <= 0)
        {
            fireNukeButton.GetComponent<Button>().interactable = false;
            nukeCountText.text = "0";
        }
    }


    private void LoadData()
    {
        if (DataPersister.Instance != null && DataPersister.Instance.CurrentGameData != null)
        {
            shieldCount = DataPersister.Instance.CurrentGameData.shieldCount;
            nukeCount = DataPersister.Instance.CurrentGameData.nukeCount;
        }
    }

}


