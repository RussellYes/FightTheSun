using System;
using UnityEngine;
using UnityEngine.UI;

// This script updates the UI progress bars.

public class ProgressBarUI : MonoBehaviour
{
    [SerializeField] private bool isHullBar;
    [SerializeField] private bool isThrustBar;
    [SerializeField] private bool isCheckpointBar;
    [SerializeField] private bool isClawTimerBar;

    [SerializeField] private Image barImage;

    private void OnEnable()
    {
        // Subscribe to events
        if (isHullBar)
        {
            PlayerStatsManager.OnCurrentHullChanged += PlayerStatsManager_OnCurrentHullChanged;
        }
        if (isThrustBar)
        {
            PlayerStatsManager.OnCurrentThrustChanged += PlayerStatsManager_OnCurrentThrustChanged;
        }
        if (isCheckpointBar)
        {
            PlayerStatsManager.OnCheckpointProgressChanged += PlayerStatsManager_OnCheckpointProgressChanged;
        }
        if (isClawTimerBar)
        {
            MiningClaw.OnClawTimerChanged += MiningClaw_OnClawTimerChanged;
        }
    }

    private void OnDisable()
    {
        // Unsubscribe from events
        if (isHullBar)
        {
            PlayerStatsManager.OnCurrentHullChanged -= PlayerStatsManager_OnCurrentHullChanged;
        }
        if (isThrustBar)
        {
            PlayerStatsManager.OnCurrentThrustChanged -= PlayerStatsManager_OnCurrentThrustChanged;
        }
        if (isCheckpointBar)
        {
            PlayerStatsManager.OnCheckpointProgressChanged -= PlayerStatsManager_OnCheckpointProgressChanged;
        }
        if (isClawTimerBar)
        {
            MiningClaw.OnClawTimerChanged -= MiningClaw_OnClawTimerChanged;
        }
    }

    private void PlayerStatsManager_OnCurrentHullChanged(object sender, PlayerStatsManager.OnCurrentHullChangedEventArgs e)
    {
        if (isHullBar)
        {
            barImage.fillAmount = e.progressNormalized;
        }
    }

    private void PlayerStatsManager_OnCurrentThrustChanged(object sender, PlayerStatsManager.OnCurrentThrustChangedEventArgs e)
    {
        if (isThrustBar)
        {
            barImage.fillAmount = e.progressNormalized;
        }
    }

    private void PlayerStatsManager_OnCheckpointProgressChanged(object sender, PlayerStatsManager.OnCheckpointProgressChangedEventArgs e)
    {
        Debug.Log("ProgressBarUI_PlayerStatsManager_OnCheckpointProgressChanged");
        if (isCheckpointBar)
        {
            barImage.fillAmount = e.progressNormalized;
        }
    }

    private void MiningClaw_OnClawTimerChanged(object sender, MiningClaw.OnClawTimerChangedEventArgs e)
    {
        if (isClawTimerBar)
        {
            barImage.fillAmount = e.progressNormalized;
        }
    }
}