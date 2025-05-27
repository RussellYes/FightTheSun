using UnityEngine;
using System;
using System.Collections;

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
    private void OnEnable()
    {
        ShipUIManager.FireMissilesEvent += BlockTouchInput;
        ShipUIManager.PauseButtonEvent += BlockTouchInput;
    }
    private void OnDisable()
    {
        ShipUIManager.FireMissilesEvent -= BlockTouchInput;
        ShipUIManager.PauseButtonEvent -= BlockTouchInput;
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
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (isMainMenu)
            {
                switch (touch.phase)
                {
                    case TouchPhase.Began:
                        touchStartPos = touch.position;
                        break;

                    case TouchPhase.Ended:
                        Vector2 touchEndPos = touch.position;
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
                            // Simple tap control
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
                        break;
                }
            }

            if (!isMainMenu)
            {
                switch (touch.phase)
                {
                    case TouchPhase.Began:
                        touchStartPos = touch.position;
                        break;

                    case TouchPhase.Ended:
                        Vector2 touchEndPos = touch.position;
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
                            // Simple tap control
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
                        break;
                }
            }
        }
    }


            


    public void EnableTouchControls(bool enable)
    {
        touchEnabled = enable;
    }
}