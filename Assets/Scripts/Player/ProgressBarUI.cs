using System;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBarUI : MonoBehaviour
{
    [SerializeField] private bool isHullBar;
    [SerializeField] private bool isThrustBar;
    [SerializeField] private bool isCheckpointBar;

    [SerializeField] private Image barImage;

    private void Start()
    {

    }

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
}