using UnityEngine;
using System;

public class SwipeControls : MonoBehaviour
{
    public static SwipeControls Instance { get; private set; }

    // Events for movement
    public event Action OnMoveLeft;
    public event Action OnMoveRight;

    [SerializeField] private float minSwipeDistance = 50f; // Minimum distance for a swipe to be registered
    private Vector2 touchStartPos;
    private bool touchEnabled = true;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if (!touchEnabled) return;

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

                    // Check if it's a valid swipe
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
                        // Handle tap
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