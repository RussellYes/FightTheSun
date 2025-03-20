using System;
using UnityEngine;
using UnityEngine.VFX;
using static GameManager;

public class Obstacle : MonoBehaviour
{
     // Define events for obstacle entering and exiting the scene
    public static event Action ObstacleEntersSceneEvent;
    public static event Action BossDefeatedEvent;
    public static event Action<bool, int> ObstacleExitsSceneEvent;

    [SerializeField] private bool isBossObstacle;

    [SerializeField] private int pointValue; // Points awarded if killed by the player
    [SerializeField] private GameObject visualWarning;
    [SerializeField] private SpriteRenderer warningSpriteRenderer;
    [SerializeField] private Color visualWarningColor;
    [SerializeField] private float visualWarningTimer = 3f;
    private bool isWarningActive = true;


    private void Start()
    {
        // Set the initial color of the visualWarning
        warningSpriteRenderer.color = visualWarningColor;
    }

    private void Update()
    {
        if (visualWarning != null)
        {
            if (isWarningActive)
            {
                VisualWarningTimer();
            }
        }

    }

    private void VisualWarningTimer()
    {
        visualWarningTimer -= Time.deltaTime;

        if (visualWarningTimer <= 0)
        {
            isWarningActive = false;
            Debug.Log("visualWarning.SetActive(false)");
            visualWarning.SetActive(false);
        }
    }

    public void Die(bool isKilledByPlayer)
    {
        // Notify that this obstacle is exiting the scene
        ObstacleExitsSceneEvent?.Invoke(isKilledByPlayer, pointValue);

        if (isBossObstacle)
        {
            BossDefeatedEvent?.Invoke();
        }

        // Destroy the obstacle GameObject
        Destroy(gameObject);
    }
}