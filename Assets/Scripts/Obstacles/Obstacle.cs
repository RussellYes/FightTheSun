using System;
using UnityEngine;

// This script controls attributes of obstacles such as appearance, visual cues, point value.

public class Obstacle : MonoBehaviour
{
     // Define events for obstacle entering and exiting the scene
    public static event Action ObstacleEntersSceneEvent;
    public static event Action BossDefeatedEvent;
    public static event Action<bool, int> ObstacleExitsSceneEvent;

    [SerializeField] private bool isBossObstacle;
    [SerializeField] private bool isFlipped;

    [SerializeField] private Sprite[] obstacleSprites; // Array of sprites for the obstacle
    [SerializeField] private SpriteRenderer obstacleRenderer; // Renderer component for the obstacle
    [SerializeField] private int pointValue; // Points awarded if killed by the player
    [SerializeField] private GameObject visualWarning;
    [SerializeField] private SpriteRenderer warningSpriteRenderer;
    [SerializeField] private Color visualWarningColor;
    [SerializeField] private float visualWarningTimer = 3f;
    
    private bool isWarningActive = true;

    private void Awake()
    {
        if (obstacleRenderer == null)
        {
            obstacleRenderer = GetComponent<SpriteRenderer>();
        }

        if (warningSpriteRenderer == null && visualWarning != null)
        {
            warningSpriteRenderer = visualWarning.GetComponent<SpriteRenderer>();
        }
    }
    private void Start()
    {
        AssignRandomSprite();

        // Set the initial color of the visualWarning
        warningSpriteRenderer.color = visualWarningColor;

        if (isFlipped)
        {
            transform.Rotate(0, 0, 180);
        }

        ObstacleEntersSceneEvent?.Invoke();
    }

    private void Update()
    {
        if (visualWarning != null && warningSpriteRenderer != null)
        {
            if (isWarningActive)
            {
                VisualWarningTimer();
            }
        }

    }

    private void AssignRandomSprite()
    {
        // Check if the obstacleRenderer and obstacleSprites array are valid
        if (obstacleRenderer != null && obstacleSprites.Length > 0)
        {
            // Assign a random sprite from the obstacleSprites array
            obstacleRenderer.sprite = obstacleSprites[UnityEngine.Random.Range(0, obstacleSprites.Length)];
        }
        else
        {
            Debug.LogWarning("Obstacle: No SpriteRenderer or obstacleSprites found.");
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
        Debug.Log("Obstacle Die()");
        // Notify that this obstacle is exiting the scene
        ObstacleExitsSceneEvent?.Invoke(isKilledByPlayer, pointValue);

        if (isBossObstacle)
        {
            BossDefeatedEvent?.Invoke();
            return;
        }

        // Destroy the obstacle GameObject
        Destroy(gameObject);
    }
}