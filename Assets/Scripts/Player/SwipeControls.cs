using UnityEngine;
using System;

public class SwipeControls : MonoBehaviour
{
    // Events for movement
    public static event Action OnMoveLeft;
    public static event Action OnMoveRight;

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
                    float swipeDistance = touchEndPos.x - touchStartPos.x;

                    if (Mathf.Abs(swipeDistance) > minSwipeDistance)
                    {
                        if (swipeDistance > 0)
                        {
                            OnMoveRight?.Invoke();
                        }
                        else
                        {
                            OnMoveLeft?.Invoke();
                        }
                    }
                    else
                    {
                        // Simple tap control
                        float screenCenter = Screen.width / 2f;
                        if (touchEndPos.x > screenCenter)
                        {
                            OnMoveRight?.Invoke();
                        }
                        else
                        {
                            OnMoveLeft?.Invoke();
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