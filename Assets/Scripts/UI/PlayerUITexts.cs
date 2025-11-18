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
            PlayerStatsManager.OnCurrentHullChanged += PlayerStatsManager_OnCurrentHullChanged;
        }
        if (isThrust)
        {
            PlayerStatsManager.OnCurrentThrustChanged += PlayerStatsManager_OnCurrentThrustChanged;
        }
        if (isCheckpoint)
        {
            PlayerStatsManager.OnCheckpointProgressChanged += PlayerStatsManager_OnCheckpointProgressChanged;
        }
        if (isClawTimer)
        {
            MiningClaw.OnClawTimerChanged += MiningClaw_OnClawTimerChanged;
        }
    }

    private void OnDisable()
    {
        // Unsubscribe from events
        if (isHealth)
        {
            PlayerStatsManager.OnCurrentHullChanged -= PlayerStatsManager_OnCurrentHullChanged;
        }
        if (isThrust)
        {
            PlayerStatsManager.OnCurrentThrustChanged -= PlayerStatsManager_OnCurrentThrustChanged;
        }
        if (isCheckpoint)
        {
            PlayerStatsManager.OnCheckpointProgressChanged -= PlayerStatsManager_OnCheckpointProgressChanged;
        }
        if (isClawTimer)
        {
            MiningClaw.OnClawTimerChanged -= MiningClaw_OnClawTimerChanged;
        }
    }

    private void PlayerStatsManager_OnCurrentHullChanged(object sender, PlayerStatsManager.OnCurrentHullChangedEventArgs e)
    {
        if (isHealth)
        {
            if (e.progressNormalized <= 0)
            {
                statText.text = "";
            }
            else
            {
                statText.text = $"{e.currentHull:F0}";
            }
        }
    }

    private void PlayerStatsManager_OnCurrentThrustChanged(object sender, PlayerStatsManager.OnCurrentThrustChangedEventArgs e)
    {
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

    private void PlayerStatsManager_OnCheckpointProgressChanged(object sender, PlayerStatsManager.OnCheckpointProgressChangedEventArgs e)
    {
        Debug.Log("ProgressBarUI_PlayerStatsManager_OnCheckpointProgressChanged");
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

    private void MiningClaw_OnClawTimerChanged(object sender, MiningClaw.OnClawTimerChangedEventArgs e)
    {
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

