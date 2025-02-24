using System;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    // Define events for obstacle entering and exiting the scene
    public static event Action ObstacleEntersSceneEvent;
    public static event Action<bool, int> ObstacleExitsSceneEvent;

    [SerializeField] private int pointValue; // Points awarded if killed by the player

    private void Start()
    {
        // Notify that this obstacle has entered the scene
        ObstacleEntersSceneEvent?.Invoke();
    }

    public void Die(bool isKilledByPlayer)
    {
        // Notify that this obstacle is exiting the scene
        ObstacleExitsSceneEvent?.Invoke(isKilledByPlayer, pointValue);

        // Destroy the obstacle GameObject
        Destroy(gameObject);
    }
}