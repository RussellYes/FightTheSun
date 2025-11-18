using UnityEngine;
using System;
using System.Collections;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;

public class SwipeControls : MonoBehaviour
{
    // Events for movement
    public static event Action OnSwipeLeft;
    public static event Action OnSwipeRight;
    public static event Action OnSwipeUp;
    public static event Action OnSwipeDown;

    [SerializeField] private float minSwipeDistance = 50f; // Minimum distance for a swipe to be registered
    private Vector2 touchStartPos;
    private bool touchEnabled = true;
    private bool touchStartedOnUI = false;
    [SerializeField] private bool isMainMenu;
    private float noSwipeBottomScreenArea; // Bottom zone height where swipes are ignored (where buttons are located)
    private bool touchStartedInNoSwipeArea = false;
    private float screenPercentToExclude = 0.125f;

    // New Input System variables
    private Touchscreen touchscreen;
    private InputAction touchPositionAction;
    private InputAction touchPressAction;
    private InputSystemUIInputModule uiInputModule;

    private void Awake()
    {
        // Initialize touchscreen
        uiInputModule = FindFirstObjectByType<InputSystemUIInputModule>();
        touchscreen = Touchscreen.current;

        // Set up input actions
        touchPositionAction = new InputAction(binding: "<Touchscreen>/primaryTouch/position");
        touchPressAction = new InputAction(binding: "<Touchscreen>/primaryTouch/press");

        // Enable actions
        touchPositionAction.Enable();
        touchPressAction.Enable();
    }

    private void OnEnable()
    {
        ShipUIManager.FireMissilesEvent += BlockTouchInput;
        ShipUIManager.PauseButtonEvent += BlockTouchInput;
    }
    private void OnDisable()
    {
        ShipUIManager.FireMissilesEvent -= BlockTouchInput;
        ShipUIManager.PauseButtonEvent -= BlockTouchInput;

        // Clean up input actions
        touchPositionAction.Disable();
        touchPressAction.Disable();
    }

    private void Start()
    {
        // Scale swipe distance based on screen DPI. This is added because swiping works differently on different devices and some swipes were not being registered.
        minSwipeDistance *= Screen.dpi / 160f;

        // Set no swipe area to a percentage of screen height 
        noSwipeBottomScreenArea = Screen.height * screenPercentToExclude;
        Debug.Log($"SwipeControls Start - no swipe area: {noSwipeBottomScreenArea} pixels (25% of {Screen.height})");
    }
    private void Update()
    {
        if (!touchEnabled) return;

        HandleTouchInput();
    }

    private void BlockTouchInput()
    {
        touchEnabled = false;
        StartCoroutine(EnableTouchAfterDelay(0.5f));
    }

    IEnumerator EnableTouchAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        touchEnabled = true;
    }
    private void HandleTouchInput()
    {
        // Check if touch is pressed (equivalent to Input.touchCount > 0)
        if (touchPressAction.ReadValue<float>() > 0.5f)
        {
            //Debug.Log("SwipeControls HandleTouchInput - touchPressAction.ReadValue");
            Vector2 currentTouchPos = touchPositionAction.ReadValue<Vector2>();

            // Simulate TouchPhase.Began
            if (touchPressAction.triggered)
            {
                //Debug.Log("SwipeControls HandleTouchInput - touchPressAction.triggered");
                touchStartPos = currentTouchPos;
                touchStartedOnUI = IsPointerOverUI(currentTouchPos);
                touchStartedInNoSwipeArea = false;

                if (!isMainMenu && currentTouchPos.y <= noSwipeBottomScreenArea)
                {
                    Debug.Log($"SwipeControls HandleTouchInput - Blocking touch: Y={currentTouchPos.y}, Limit={noSwipeBottomScreenArea}, ScreenHeight={Screen.height}");
                    touchStartedInNoSwipeArea = true;
                    return;
                }
            }
        }

        // Check for touch release separately
        if (touchPressAction.WasReleasedThisFrame())
        {
            Debug.Log("SwipeControls HandleTouchInput - touchPressAction.WasReleasedThisFrame");

            if (touchStartedOnUI || touchStartedInNoSwipeArea)
            {
                Debug.Log("SwipeControls HandleTouchInput - Ignoring swipe that started on UI");
                touchStartedOnUI = false;
                touchStartedInNoSwipeArea = false;
                return;
            }

            Vector2 touchEndPos = touchPositionAction.ReadValue<Vector2>();
            Debug.Log($"SwipeControls - Touch ended at {touchEndPos}, started at {touchStartPos}");
            float swipeDistanceX = touchEndPos.x - touchStartPos.x;
            float swipeDistanceY = touchEndPos.y - touchStartPos.y;

            Debug.Log($"SwipeControls HandleTouchInput - Swipe delta: X={swipeDistanceX}, Y={swipeDistanceY}");

            if (Mathf.Abs(swipeDistanceX) > minSwipeDistance)
            {
                if (swipeDistanceX > 0)
                {
                    Debug.Log("SwipeControls HandleTouchInput - Swipe RIGHT detected");
                    OnSwipeRight?.Invoke();
                }
                else
                {
                    Debug.Log("SwipeControls HandleTouchInput - Swipe LEFT detected");
                    OnSwipeLeft?.Invoke();
                }
                return;
            }

            if (Mathf.Abs(swipeDistanceY) > minSwipeDistance)
            {
                if (swipeDistanceY > 0)
                {
                    Debug.Log("SwipeControls HandleTouchInput - Swipe UP detected");
                    OnSwipeUp?.Invoke();
                }
                else
                {
                    Debug.Log("SwipeControls HandleTouchInput - Swipe DOWN detected");
                    OnSwipeDown?.Invoke();
                }
                return;
            }

            // Simple tap control
            Debug.Log("SwipeControls HandleTouchInput - Tap detected");
            float screenCenter = Screen.width / 2f;
            if (touchEndPos.x > screenCenter)
            {
                OnSwipeRight?.Invoke();
            }
            else
            {
                OnSwipeLeft?.Invoke();
            }
        }
    }

    private bool IsPointerOverUI(Vector2 screenPosition)
    {
        return uiInputModule != null && uiInputModule.IsPointerOverGameObject(0);
    }

    public void EnableTouchControls(bool enable)
    {
        touchEnabled = enable;
    }
}