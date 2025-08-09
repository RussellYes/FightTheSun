using UnityEngine;
using System;
using System.Collections;
using UnityEngine.InputSystem;

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
    [SerializeField] private bool isMainMenu;

    // New Input System variables
    private Touchscreen touchscreen;
    private InputAction touchPositionAction;
    private InputAction touchPressAction;


    private void Awake()
    {
        // Initialize touchscreen
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
            Vector2 currentTouchPos = touchPositionAction.ReadValue<Vector2>();

            // Simulate TouchPhase.Began
            if (touchPressAction.triggered)
            {
                touchStartPos = currentTouchPos;
            }

            // Simulate TouchPhase.Ended
            if (touchPressAction.WasReleasedThisFrame())
            {
                Vector2 touchEndPos = currentTouchPos;
                float swipeDistanceX = touchEndPos.x - touchStartPos.x;
                float swipeDistanceY = touchEndPos.y - touchStartPos.y;

                if (Mathf.Abs(swipeDistanceX) > minSwipeDistance)
                {
                    if (swipeDistanceX > 0)
                    {
                        OnSwipeRight?.Invoke();
                    }
                    else
                    {
                        OnSwipeLeft?.Invoke();
                    }
                }
                else if (Mathf.Abs(swipeDistanceY) > minSwipeDistance)
                {
                    if (swipeDistanceY > 0)
                    {
                        OnSwipeUp?.Invoke();
                    }
                    else
                    {
                        OnSwipeDown?.Invoke();
                    }
                }
                else
                {
                    // Simple tap control (unchanged)
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
        }
    }





    public void EnableTouchControls(bool enable)
    {
        touchEnabled = enable;
    }
}