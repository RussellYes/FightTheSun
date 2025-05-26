using UnityEngine;
using System;
using UnityEngine.EventSystems;

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

    private void Update()
    {
        if (!touchEnabled) return;

        HandleTouchInput();
    }

    private void HandleTouchInput()
    {
        if (Input.touchCount > 0 && EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
        {
            // Ignore touch input if it's over a UI element
            return;
        }
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

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

    public void EnableTouchControls(bool enable)
    {
        touchEnabled = enable;
    }
}