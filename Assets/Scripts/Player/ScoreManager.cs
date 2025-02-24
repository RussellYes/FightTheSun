using System;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    private int totalObstaclesInScene;
    private int killedByPlayerCount;
    private int currentObstaclesInScene;
    public int score = 0;

    // Define events for score changes and obstacles destroyed by the player
    public static event Action<int> OnScoreChanged;
    public static event Action<int> OnObstaclesDestroyedByPlayerChanged;

    private void OnEnable()
    {
        // Subscribe to obstacle events
        Obstacle.ObstacleEntersSceneEvent += OnObstacleEntersScene;
        Obstacle.ObstacleExitsSceneEvent += OnObstacleExitsScene;
    }

    private void OnDisable()
    {
        // Unsubscribe from obstacle events
        Obstacle.ObstacleEntersSceneEvent -= OnObstacleEntersScene;
        Obstacle.ObstacleExitsSceneEvent -= OnObstacleExitsScene;
    }

    private void OnObstacleEntersScene()
    {
        totalObstaclesInScene++;
        currentObstaclesInScene++;
        Debug.Log($"Obstacle entered scene. Total: {totalObstaclesInScene}, Current: {currentObstaclesInScene}");
    }

    private void OnObstacleExitsScene(bool isKilledByPlayer, int pointValue)
    {
        currentObstaclesInScene--;
        Debug.Log($"Obstacle exited scene. Total: {totalObstaclesInScene}, Current: {currentObstaclesInScene}");

        if (isKilledByPlayer)
        {
            killedByPlayerCount++;
            ChangeScore(pointValue);
            Debug.Log($"Score updated: {score}");

            // Trigger the obstacles destroyed by player event
            OnObstaclesDestroyedByPlayerChanged?.Invoke(killedByPlayerCount);
        }
    }

    private void ChangeScore(int points)
    {
        score += points;

        // Trigger the score change event
        OnScoreChanged?.Invoke(score);
    }

    public int GetScore()
    {
        return score;
    }

    public int KilledByPlayerCount()
    {
        return killedByPlayerCount;
    }
}