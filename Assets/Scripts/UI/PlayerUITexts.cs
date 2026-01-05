using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUITexts : MonoBehaviour
{
    [SerializeField] private bool isHealth;
    [SerializeField] private bool isThrust;
    [SerializeField] private bool isCheckpoint;
    [SerializeField] private bool isClawTimer;

    [SerializeField] private TextMeshProUGUI statText;

    private void OnEnable()
    {
        // Subscribe to events
        if (isHealth)
        {
            PlayerStatsManager.OnCurrentHullChanged += HandleOnCurrentHullChanged;
        }
        if (isThrust)
        {
            PlayerStatsManager.OnCurrentThrustChanged += HandleOnCurrentThrustChanged;
        }
        if (isCheckpoint)
        {
            PlayerStatsManager.OnCheckpointProgressChanged += HandleOnCheckpointProgressChanged;
        }
        if (isClawTimer)
        {
            MiningClaw.OnClawTimerChanged += HandleOnClawTimerChanged;
        }
    }

    private void OnDisable()
    {
        // Unsubscribe from events
        if (isHealth)
        {
            PlayerStatsManager.OnCurrentHullChanged -= HandleOnCurrentHullChanged;
        }
        if (isThrust)
        {
            PlayerStatsManager.OnCurrentThrustChanged -= HandleOnCurrentThrustChanged;
        }
        if (isCheckpoint)
        {
            PlayerStatsManager.OnCheckpointProgressChanged -= HandleOnCheckpointProgressChanged;
        }
        if (isClawTimer)
        {
            MiningClaw.OnClawTimerChanged -= HandleOnClawTimerChanged;
        }
    }

    private void HandleOnCurrentHullChanged(object sender, PlayerStatsManager.OnCurrentHullChangedEventArgs e)
    {
        if (isHealth)
        {
           // Debug.Log($"PlayerUITexts HandleOnCurrentHullChanged - saved currentHull {DataPersister.Instance.CurrentGameData.playerCurrentHull}, currentHull: {e.currentHull}, maxHull: {e.maxHull}, progress: {e.progressNormalized}");
            if (e.progressNormalized <= 0)
            {
                statText.text = "";
            }
            else
            {
                statText.text = $"{e.currentHull:F1}";
            }
        }
    }

    private void HandleOnCurrentThrustChanged(object sender, PlayerStatsManager.OnCurrentThrustChangedEventArgs e)
    {
        Debug.Log($"PlayerUITexts HandleOnCurrentThrustChanged - currentThrust: {e.currentThrust}, maxHull: {e.maxThrust}, progress: {e.progressNormalized}");
        if (isThrust)
        {
            if (e.progressNormalized <= 0)
            {
                statText.text = "";
            }
            else
            {
                statText.text = $"{e.currentThrust:F1}";
            }
        }
    }

    private void HandleOnCheckpointProgressChanged(object sender, PlayerStatsManager.OnCheckpointProgressChangedEventArgs e)
    {
        Debug.Log($"PlayerUITexts HandleOnCheckpointProgressChanged");
        if (isCheckpoint)
        {
            if (e.progressNormalized <= 0)
            {
                statText.text = "";
            }
            else
            {
                statText.text = $"{e.progressNormalized * 100:F0}%";
            }

        }
    }

    private void HandleOnClawTimerChanged(object sender, MiningClaw.OnClawTimerChangedEventArgs e)
    {
        Debug.Log($"PlayerUITexts HandleOnClawTimerChanged");
        if (isClawTimer)
        {
            if (e.progressNormalized <= 0)
            {
                statText.text = "";
            }
            else
            {
                statText.text = $"{e.timeRemaining:F1}s";
            }
        }
    }
}

